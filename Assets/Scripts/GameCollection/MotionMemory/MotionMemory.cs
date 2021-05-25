using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MotionMemory : Game
{
    [SerializeField] int width = 2, height = 2;
    [Space]
    [SerializeField] 
    GameObject startScreen = default, memoryCanvas = default, memoryCardRow = default, memoryCardPrefab = default;

    [SerializeField] 
    RenderTexture cameraRenderTexture = default;
    [SerializeField] 
    Texture2D backsideTexture = default;
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
        timeBetweenCardsGuessing = 1f;

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

    List<MemoryCard> unsolved, solved, tempStack;
    readonly CoroutineTimer timer = new CoroutineTimer();
    readonly string[] motions = new string[]{ "w", "m", "c", "a"};
    MemoryCard[] cards;

    void Start()
    {
        PlayGame();
    }

    protected override IEnumerator Init()
    {
        //initialize UI-components
        progressBar.enabled = false;
        taskText.text = "";
        remainingTimeText.text = "";

        List<Motion> poses = GetRandomSetOfPoses();
        cards = ConstructUI(poses);
        SkeletonDisplay.Instance.display = DisplayOption.NONE;

        yield break;
    }

    protected override IEnumerator Execute()
    {
        KinectDeviceManager.Instance.BeginBodyTracking();

        memoryCanvas.SetActive(false);
        startScreen.SetActive(true);
        startScreenText.text = "Get Ready!";

        progressBar.enabled = true;
        yield return timer.UITimer(timeBeforeStart, progressBarMask, startScreenCountdown);
        progressBar.enabled = false;

        startScreenText.text = "";
        startScreen.SetActive(false);
        memoryCanvas.SetActive(true);


        SkeletonDisplay.Instance.OnBeginDisplay();

        int remainingRounds = maximumRounds;

        unsolved = new List<MemoryCard>(cards);
        tempStack = new List<MemoryCard>();
        solved = new List<MemoryCard>();

        while (unsolved.Count > 0)
        {
            remainingRounds--;

            yield return CardShowingStage();

            taskText.text = "Guess which motion was behind which card";

            yield return timer.SimpleTimer(timeBetweenShowingAndGuessing);

            yield return CardGuessingPhase();

            if (remainingRounds == 0)
                break;

            yield return timer.SimpleTimer(timeBetweenRounds);
        }

        SkeletonDisplay.Instance.OnStopDisplay();
        taskText.text = "";

        memoryCanvas.SetActive(false);
        startScreen.SetActive(true);

        startScreenText.text = (unsolved.Count > 0) ? "No rounds remaining" : "You win!"; 

        AppState.bodyTrackingRunning = false;
    }

    public List<Motion> GetRandomSetOfPoses()
    {
        if((width * height) - 1 > motions.Length)
        {
            Debug.LogWarning("memory width * height is bigger than the available poses");
        }

        List<string> motionNames = new List<string>();
        motionNames.AddRange((string[])motions.Clone());

        List<Motion> poses = new List<Motion>();
        for (int i = 0; i < (width * height); i++)
        {
            string motionName = motionNames[GetRandom(motionNames.Count)];
            motionNames.Remove(motionName);
            Motion motion = SkeletonTracker.Instance.Load(motionName);
            if (motion != null)
            {
                poses.Add(motion);
                //Debug.Log("Added motion");
            }
            if(motionNames.Count == 0)
            {
                break;
            }
        }
        return poses;
    }

    IEnumerator CardShowingStage()
    {
        taskText.text = "Memorize the shown poses";

        yield return timer.SimpleTimer(timeBetweenCardsShowing);

        int remainingGroupSize = unsolved.Count < maxGroupSize ? unsolved.Count : maxGroupSize;
        for (int i = 0; i < remainingGroupSize; i++)
        {
            MemoryCard card = unsolved[GetRandom(unsolved.Count)];
            BeginShowPose(card);
            //wait for minimum accuracy or minimum time to pass
            progressBar.enabled = true;
            yield return timer.UITimer(cardShowingTime, progressBarMask, remainingTimeText);
            progressBar.enabled = false;
            StopShowPose(card);

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
            MemoryCard card = tempStack[GetRandom(tempStack.Count)];
            int maxAccuracy = 0;
            BeginOutline(card);
            float remainingTime = motionGuessingTime- timeBeforeMotionTracking;
            comparePercentage.text = "";
            SkeletonDisplay.Instance.comparePercentage = 0;

            yield return timer.SimpleTimer(timeBeforeMotionTracking);
            Coroutine cR = StartCoroutine(SkeletonDisplay.Instance.BodyCompareCoroutine(card.pose.motion[0], remainingTime));
            
            progressBar.enabled = true;
            while (remainingTime > 0f)
            {
                int currentAccuracy = SkeletonDisplay.Instance.comparePercentage;
                if(currentAccuracy > maxAccuracy)
                {
                    maxAccuracy = currentAccuracy;
                }
                if (maxAccuracy > 80)
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
            comparePercentage.text = "";
            remainingTimeText.text = "";
            progressBarMask.fillAmount = 0f;
            progressBar.enabled = false;

            StopOutline(card);
            //Debug.Log("Acc: "+ maxAccuracy);
            if (maxAccuracy <= 80)
            {
                unsolved.Add(card);
            }

            tempStack.Remove(card);

            BeginShowPose(card);
            yield return timer.SimpleTimer(cardShowingTime);
            StopShowPose(card);


            if (i < tempStackSize)
            {
                yield return timer.SimpleTimer(timeBetweenCardsGuessing);
            }
        }
    }

    MemoryCard[] ConstructUI(List<Motion> j)
    {
        MemoryCard[] memory = new MemoryCard[j.Count];
        for (int y = 0; y < height; y++)
        {
            GameObject currentRow = Instantiate(memoryCardRow, transform.position, transform.rotation);
            currentRow.transform.SetParent(memoryCanvas.transform, false);
            for (int x = 0; x < width; x++)
            {
                if(y * width + x >= j.Count)
                {
                    break;
                }
                GameObject currentCard = Instantiate(memoryCardPrefab, transform.position, transform.rotation);
                MemoryCard card = new MemoryCard { x = x, y = y, pose = j[y * width + x], uiElement = currentCard };
                currentCard.transform.SetParent(currentRow.transform, false);
                memory[y * width + x] = card;
            }
        }
        return memory;
    }

    void BeginShowPose(MemoryCard card)
    {
        Vector3[] vecs = SkeletonDisplay.Instance.GetVectors(card.pose.motion[0]);
        SkeletonDisplay.Instance.Display(vecs);
        card.uiElement.GetComponent<RawImage>().texture = cameraRenderTexture;
    }

    void StopShowPose(MemoryCard card) => 
        card.uiElement.GetComponent<RawImage>().texture = backsideTexture;

    void BeginOutline(MemoryCard card) => card.uiElement.GetComponent<Outline>().enabled = true;

    void StopOutline(MemoryCard card) => card.uiElement.GetComponent<Outline>().enabled = false;

    int GetRandom(int length) => Random.Range(0, length);
}
