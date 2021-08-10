using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.VFX;

public class MotionMemoryHouse : Game
{
    const int amount = 4;

    [SerializeField]
    bool standaloneScene;
    
    [SerializeField]
    GameObject startScreen = default, memoryCanvas = default, memoryCardRow = default;

    [SerializeField]
    Material mat;
    [SerializeField]
    RenderTexture cameraRenderTexture = default;
    [Space]
    [Header("Timing")]
    //how many cards get turned around after one another
    [SerializeField]
    float timeBeforeStart = 2;
    [SerializeField]
    int maxGroupSize = 2, maximumRounds = 3;

    [SerializeField]
    float
        cardShowingTime = 3,
        motionGuessingTime = 2,
        timeBeforeMotionTracking = 1f,
        timeBetweenRounds = 2f,
        timeBetweenShowingAndGuessing = 2f,
        timeBetweenCardsShowing = 2f,
        timeBetweenCardsGuessing = 1f,
        windowOpenCloseTime = 1f;

    [Space]
    [Header("UIElements")]

    [SerializeField]
    TextMeshProUGUI taskText;
    [SerializeField]
    TextMeshProUGUI remainingTimeText;

    [SerializeField]
    Image progressBar, progressBarMask;

    [SerializeField]
    TextMeshProUGUI startScreenCountdown, startScreenText, comparePercentage;

    [SerializeField]
    VisualEffect e;

    [SerializeField]
    Image semitransparentPanel;

    [SerializeField]
    MeshRenderer house;

    [SerializeField]
    GameObject houseGO;

    [SerializeField]
    GameObject sceneryGO;

    [SerializeField]
    WindowController windowController;

    [SerializeField]
    Camera cam;

    [SerializeField]
    Light mainLight;

    List<MemoryCardHouse> unsolved, solved, tempStack;
    readonly CoroutineTimer timer = new CoroutineTimer();
    readonly string[] motions = new string[] { "w", "m", "c", "a" };
    MemoryCardHouse[] cards;

    readonly Vector3 cameraPositionOffset = new Vector3(0.3f, 8.8f, 13f);
    readonly Quaternion cameraRotation = Quaternion.Euler(17f, 180f, 0f);
    
    void Start()
    {
        PlayGame();
    }

    protected override IEnumerator Init()
    {
        if (AppManager.useVirtualWorld)
        {
            cam.transform.position = GameController.Instance.mainSceneCamera.transform.position;
            cam.transform.rotation = GameController.Instance.mainSceneCamera.transform.rotation;
            mainLight.enabled = false;
            houseGO.SetActive(false);
            sceneryGO.SetActive(false);
            windowController = VirtualWorldController.Instance.windowController;
            Transform house = windowController.transform;

            Vector3 positionOffset = house.right * cameraPositionOffset.x + house.up * cameraPositionOffset.y + house.forward * cameraPositionOffset.z;
            yield return StartCoroutine(Tween.TweenPositionAndRotation(cam.transform, house.position + positionOffset, house.rotation * cameraRotation, 3f));
            //cam.transform.position = house.position + house.forward * 13f + house.up * 8.8f + house.right * 0.3f;
            //cam.transform.rotation = VirtualWorldController.Instance.house.rotation * Quaternion.Euler(17f, 180f, 0f);
        }

        //initialize UI-components
        progressBar.enabled = false;
        taskText.text = "";
        remainingTimeText.text = "";

        List<Motion> poses = GetRandomSetOfPoses();
        cards = ConstructUI(poses);
        BodyDisplay.Instance.display = DisplayOption.IGNORE;

        yield break;
    }

    protected override IEnumerator Execute()
    {
        KinectDeviceManager.Instance.BeginBodyTracking();

        memoryCanvas.SetActive(false);
        startScreen.SetActive(true);
        startScreenText.text = StringRes.Get("_GetReady");

        semitransparentPanel.enabled = true;
        progressBar.enabled = true;
        yield return timer.UITimer(timeBeforeStart, progressBarMask, startScreenCountdown);
        progressBar.enabled = false;
        semitransparentPanel.enabled = false;

        startScreenText.text = "";
        startScreen.SetActive(false);
        memoryCanvas.SetActive(true);


        BodyDisplay.Instance.OnBeginDisplay();

        int remainingRounds = maximumRounds;

        unsolved = new List<MemoryCardHouse>(cards);
        tempStack = new List<MemoryCardHouse>();
        solved = new List<MemoryCardHouse>();

        while (unsolved.Count > 0)
        {
            remainingRounds--;

            yield return CardShowingStage();

            taskText.text = StringRes.Get("_MotionGuess");

            yield return timer.SimpleTimer(timeBetweenShowingAndGuessing);

            yield return CardGuessingPhase();

            if (remainingRounds == 0)
                break;

            yield return timer.SimpleTimer(timeBetweenRounds);
        }

        //Debug.Log("???");
        BodyDisplay.Instance.OnStopDisplay();
        taskText.text = "";

        memoryCanvas.SetActive(false);
        startScreen.SetActive(true);

        if (unsolved.Count <= 0)
        {
            ConfettiBurst();
        }
        startScreenText.text = (unsolved.Count > 0) ? StringRes.Get("_NoRoundsRemaining") : StringRes.Get("_Win");

        AppManager.bodyTrackingRunning = false;
    }

    public List<Motion> GetRandomSetOfPoses()
    {
        if (amount - 1 > motions.Length)
        {
            Debug.LogWarning("there are more windows than there are available poses");
        }

        List<string> motionNames = new List<string>();
        motionNames.AddRange((string[])motions.Clone());

        List<Motion> poses = new List<Motion>();
        for (int i = 0; i < amount; i++)
        {
            string motionName = motionNames[GetRandom(motionNames.Count)];
            motionNames.Remove(motionName);
            Motion motion = MotionManager.Instance.Load(motionName);
            if (motion != null)
            {
                poses.Add(motion);
                //Debug.Log("Added motion");
            }
            if (motionNames.Count == 0)
            {
                break;
            }
        }
        return poses;
    }

    IEnumerator CardShowingStage()
    {
        taskText.text = StringRes.Get("_MotionMemorize");

        yield return timer.SimpleTimer(3f);

        yield return timer.SimpleTimer(timeBetweenCardsShowing);

        int remainingGroupSize = unsolved.Count < maxGroupSize ? unsolved.Count : maxGroupSize;
        for (int i = 0; i < remainingGroupSize; i++)
        {
            MemoryCardHouse card = unsolved[GetRandom(unsolved.Count)];
            windowController.BeginOutline(card);
            windowController.BeginShowPose(card);

            yield return windowController.OpenWindows(card.index, windowOpenCloseTime);
            //wait for minimum accuracy or minimum time to pass
            
            progressBar.enabled = true;
            yield return timer.UITimer(cardShowingTime, progressBarMask, remainingTimeText);
            progressBar.enabled = false;
            yield return windowController.CloseWindows(card.index, windowOpenCloseTime);
            windowController.StopShowPose(card);
            windowController.StopOutline(card);

            tempStack.Add(card);
            unsolved.Remove(card);
            if (i < remainingGroupSize)
            {
                yield return timer.SimpleTimer(timeBetweenCardsShowing);
            }
        }
    }

    IEnumerator CardGuessingPhase()
    {
        yield return timer.SimpleTimer(2f);

        int tempStackSize = tempStack.Count;
        for (int i = 0; i < tempStackSize; i++)
        {
            taskText.text = StringRes.Get("_MotionGuess");

            MemoryCardHouse card = tempStack[GetRandom(tempStack.Count)];
            int maxAccuracy = 0;
            windowController.BeginOutline(card);

            float remainingTime = motionGuessingTime - timeBeforeMotionTracking;
            comparePercentage.text = "";
            BodyDisplay.Instance.comparePercentage = 0;

            yield return timer.SimpleTimer(timeBeforeMotionTracking);

            Coroutine cR = StartCoroutine(BodyDisplay.Instance.BodyCompareCoroutine(card.pose.motion[0], remainingTime));

            progressBar.enabled = true;
            while (remainingTime > 0f)
            {
                int currentAccuracy = BodyDisplay.Instance.comparePercentage;

                if (currentAccuracy > maxAccuracy)
                    maxAccuracy = currentAccuracy;

                if (maxAccuracy > 90)
                {
                    solved.Add(card);
                    StopCoroutine(cR);
                    break;
                }
                comparePercentage.text = maxAccuracy.ToString();

                remainingTime -= Time.deltaTime;
                progressBarMask.fillAmount = 1 - remainingTime / motionGuessingTime;
                remainingTimeText.text = Mathf.RoundToInt(remainingTime).ToString();
                yield return null;
            }

            if (maxAccuracy > 95)
            {
                ConfettiBurst();
                taskText.text = StringRes.Get("_RightAnswer");
            }
            else
                taskText.text = StringRes.Get("_WrongAnswer");


            comparePercentage.text = "";
            remainingTimeText.text = "";
            progressBarMask.fillAmount = 0f;
            progressBar.enabled = false;

            windowController.BeginShowPose(card);

            yield return windowController.OpenWindows(card.index, windowOpenCloseTime);

            yield return timer.SimpleTimer(cardShowingTime);

            yield return windowController.CloseWindows(card.index, windowOpenCloseTime);

            windowController.StopShowPose(card);
            windowController.StopOutline(card);


            //Debug.Log("Acc: "+ maxAccuracy);
            if (maxAccuracy <= 95)
            {
                unsolved.Add(card);
            }
            else
            {
                //if the card was solved, hide it
                //card.uiElement.GetComponent<RawImage>().enabled = false;
            }

            tempStack.Remove(card);

            if (i < tempStackSize)
                yield return timer.SimpleTimer(timeBetweenCardsGuessing);
        }
    }

    MemoryCardHouse[] ConstructUI(List<Motion> j)
    {

        MemoryCardHouse[] memory = new MemoryCardHouse[j.Count];
        Material[] mats = house.materials;
        for (int i = 0; i < amount; i++)
        {
            MemoryCardHouse card = new MemoryCardHouse { index = i, pose = j[i], window = mat };
            memory[i] = card;
            mats[i + 1] = Instantiate(mat);
            mats[card.index + 1].SetTexture("_MainTex", cameraRenderTexture);
            mats[card.index + 1].SetInt("_ShowTex", 0);
        }
        house.materials = mats;
        return memory;
    }

    int GetRandom(int length) =>
        Random.Range(0, length);

    void ConfettiBurst() =>
        e.SendEvent("TriggerBurst");

    [System.Serializable]
    public class MotionMemoryConfiguration
    {
        public WindowController windowController;

    }
}


