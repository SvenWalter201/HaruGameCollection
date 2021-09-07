using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.VFX;

public class MotionMemoryHouse : Game
{
    [SerializeField]
    WindowController wc2x2, wc3x3;

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

    const float WINDOW_ANIM_TIME = 1f;

    [Header("GAME CONFIG")]
    [SerializeField]
    MotionMemoryConfiguration conf;

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
    GameObject sceneryGO;

    WindowController windowController;

    [SerializeField]
    Camera gameCamera;

    [SerializeField]
    Light mainLight;

    List<MemoryCardHouse> unsolved, solved, tempStack;
    readonly CoroutineTimer timer = new CoroutineTimer();
    readonly string[] motions = new string[] { "egyptianLeft", "m", "c", "a", "n", "chicken" };
    MemoryCardHouse[] cards;

    readonly Vector3 cameraPositionOffset = new Vector3(0.3f, 8.8f, 13f);
    readonly Quaternion cameraRotation = Quaternion.Euler(17f, 180f, 0f);
    
    void Start()
    {
        PlayGame();
    }

    protected override IEnumerator Init()
    {
        //choose Window Controller
        switch (conf.houseSize)
        {
            case HouseSize.SIZE_2X2:
                {
                    windowController = wc2x2;
                    wc2x2.gameObject.SetActive(true);
                    wc3x3.gameObject.SetActive(false);
                    break;
                }
            case HouseSize.SIZE_3X3:
                {
                    windowController = wc3x3;
                    wc2x2.gameObject.SetActive(false);
                    wc3x3.gameObject.SetActive(true);
                    break;
                }
        }

        //initialize UI-components
        progressBar.enabled = false;
        taskText.text = "";
        remainingTimeText.text = "";
        comparePercentage.text = "";

        if (AppManager.useVirtualWorld)
        {
            //Debug.Log("?");
            Camera virtualWorldCam = GameController.Instance.mainSceneCamera;
            mainLight.enabled = false;
            gameCamera.enabled = false;

            windowController.gameObject.SetActive(false);
            sceneryGO.SetActive(false);
            windowController = VirtualWorldController.Instance.windowController;
            Transform house = windowController.transform;

            Vector3 positionOffset = house.right * cameraPositionOffset.x + house.up * cameraPositionOffset.y + house.forward * cameraPositionOffset.z;
            yield return StartCoroutine(Tween.TweenPositionAndRotation(virtualWorldCam.transform, house.position + positionOffset, house.rotation * cameraRotation, 3f));
            //cam.transform.position = house.position + house.forward * 13f + house.up * 8.8f + house.right * 0.3f;
            //cam.transform.rotation = VirtualWorldController.Instance.house.rotation * Quaternion.Euler(17f, 180f, 0f);
        }

        List<Motion> poses = GetRandomSetOfPoses();
        cards = ConstructUI(poses);
        BodyDisplay.Instance.display = DisplayOption.IGNORE;

        yield break;
    }

    protected override void ConfigSetup()
    {
        conf = AppManager.AppConfig.MotionMemoryConfiguration.Clone() as MotionMemoryConfiguration;
    }

    protected override IEnumerator Execute()
    {
        KinectDeviceManager.Instance.BeginBodyTracking(true);

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

        int remainingRounds = conf.maxRounds;

        unsolved = new List<MemoryCardHouse>(cards);
        tempStack = new List<MemoryCardHouse>();
        solved = new List<MemoryCardHouse>();

        while (unsolved.Count > 0)
        {
            remainingRounds--;

            yield return CardShowingStage();

            taskText.text = StringRes.Get("_MotionGuessHouse");

            yield return timer.SimpleTimer(conf.timeBetweenShowingAndGuessing);

            yield return CardGuessingPhase();

            if (remainingRounds == 0)
                break;

            yield return timer.SimpleTimer(conf.timeBetweenRounds);
        }

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
        if (conf.amount > motions.Length)
        {
            Debug.LogWarning("there are more windows than there are available poses");
            conf.amount = motions.Length;
        }

        if (conf.amount > windowController.WindowAmount)
            conf.amount = windowController.WindowAmount;

        List<string> motionNames = new List<string>();
        motionNames.AddRange((string[])motions.Clone());

        List<Motion> poses = new List<Motion>();
        for (int i = 0; i < conf.amount; i++)
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
        taskText.text = StringRes.Get("_MotionMemorizeHouse");

        yield return timer.SimpleTimer(3f);

        yield return timer.SimpleTimer(conf.timeBetweenCardsShowing);

        int remainingGroupSize = unsolved.Count < conf.maxGroupSize ? unsolved.Count : conf.maxGroupSize;
        for (int i = 0; i < remainingGroupSize; i++)
        {
            MemoryCardHouse card = unsolved[GetRandom(unsolved.Count)];
            windowController.BeginOutline(card);
            windowController.BeginShowPose(card);

            yield return windowController.OpenWindows(card.index, WINDOW_ANIM_TIME);
            //wait for minimum accuracy or minimum time to pass
            
            progressBar.enabled = true;
            yield return timer.UITimer(conf.cardShowingTime, progressBarMask, remainingTimeText);
            progressBar.enabled = false;
            yield return windowController.CloseWindows(card.index, WINDOW_ANIM_TIME);
            windowController.StopShowPose(card);
            windowController.StopOutline(card);

            tempStack.Add(card);
            unsolved.Remove(card);
            if (i < remainingGroupSize)
            {
                yield return timer.SimpleTimer(conf.timeBetweenCardsShowing);
            }
        }
    }

    int solvePercentage = 90;

    IEnumerator CardGuessingPhase()
    {
        yield return timer.SimpleTimer(2f);

        int tempStackSize = tempStack.Count;
        for (int i = 0; i < tempStackSize; i++)
        {
            taskText.text = StringRes.Get("_MotionGuessHouse");

            MemoryCardHouse card = tempStack[GetRandom(tempStack.Count)];
            int maxAccuracy = 0;
            windowController.BeginOutline(card);

            float remainingTime = conf.motionGuessingTime;
            comparePercentage.text = "";
            BodyDisplay.Instance.comparePercentage = 0;

            yield return timer.SimpleTimer(conf.timeBeforeMotionTracking);

            Coroutine cR = StartCoroutine(BodyDisplay.Instance.BodyCompareCoroutine(card.pose.motion[0], remainingTime));

            progressBar.enabled = true;
            while (remainingTime > 0f)
            {
                int currentAccuracy = BodyDisplay.Instance.comparePercentage;

                if (currentAccuracy > maxAccuracy)
                    maxAccuracy = currentAccuracy;

                if (maxAccuracy > solvePercentage)
                {
                    solved.Add(card);
                    StopCoroutine(cR);
                    break;
                }
                comparePercentage.text = StringRes.Get("_Accuracy") + ": " + maxAccuracy.ToString() + "%";

                remainingTime -= Time.deltaTime;
                progressBarMask.fillAmount = 1 - remainingTime / conf.motionGuessingTime;
                remainingTimeText.text = Mathf.RoundToInt(remainingTime).ToString();
                yield return null;
            }

            if (maxAccuracy > solvePercentage)
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

            yield return windowController.OpenWindows(card.index, WINDOW_ANIM_TIME);

            yield return timer.SimpleTimer(conf.cardShowingTime);

            yield return windowController.CloseWindows(card.index, WINDOW_ANIM_TIME);

            windowController.StopShowPose(card);
            windowController.StopOutline(card);


            //Debug.Log("Acc: "+ maxAccuracy);
            if (maxAccuracy <= solvePercentage)
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
                yield return timer.SimpleTimer(conf.timeBetweenCardsGuessing);
        }
    }

    MemoryCardHouse[] ConstructUI(List<Motion> j)
    {

        MemoryCardHouse[] memory = new MemoryCardHouse[j.Count];
        Material[] mats = windowController.MR.materials;
        for (int i = 0; i < conf.amount; i++)
        {
            MemoryCardHouse card = new MemoryCardHouse { index = i, pose = j[i], window = mat };
            memory[i] = card;
            mats[i + 1] = Instantiate(mat);
            mats[card.index + 1].SetTexture("_MainTex", cameraRenderTexture);
            mats[card.index + 1].SetInt("_ShowTex", 0);
        }
        windowController.MR.materials = mats;
        return memory;
    }

    int GetRandom(int length) =>
        Random.Range(0, length);

    void ConfettiBurst() =>
        e.SendEvent("TriggerBurst");

    public enum HouseSize
    {
        SIZE_2X2,
        SIZE_3X3
    }
}


