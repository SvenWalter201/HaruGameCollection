using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;
public class MotionMemory : MonoBehaviour
{
    [SerializeField] private int width = 2;
    [SerializeField] private int height = 2;
    [Space]
    [SerializeField] private GameObject startScreen = default;
    [SerializeField] private GameObject memoryCanvas = default;
    [SerializeField] private GameObject memoryCardRow = default;
    [SerializeField] private GameObject memoryCardPrefab = default;
    [SerializeField] private RenderTexture cameraRenderTexture = default;
    [SerializeField] private Texture2D backsideTexture = default;
    [Space]
    [Header("Timing")]
    //how many cards get turned around after one another
    [SerializeField] private float timeBeforeStart = 2;
    [SerializeField] private int maxGroupSize = 2;
    [SerializeField] private float cardShowingTime = 3;
    [SerializeField] private float motionGuessingTime = 2;
    [SerializeField] private int maximumRounds = 3;
    [SerializeField] private float timeBetweenRounds = 2f;
    [SerializeField] private float timeBetweenShowingAndGuessing = 2f;
    [SerializeField] private float timeBetweenCardsShowing = 2f;
    [SerializeField] private float timeBetweenCardsGuessing = 1f;
    [Space]
    [Header("UIElements")]
    [SerializeField] private TextMeshProUGUI taskText;
    [SerializeField] private TextMeshProUGUI remainingTimeText;
    [SerializeField] private Image progressBar;
    [SerializeField] private Image progressBarMask;
    [SerializeField] private TextMeshProUGUI startScreenCountdown;
    [SerializeField] private TextMeshProUGUI startScreenText;

    private List<MemoryCard> unsolved;
    private List<MemoryCard> solved;
    private List<MemoryCard> tempStack;

    private readonly string[] motions = new string[]{ "w", "m", "c", "a"};

    private void Start()
    {
        //initialize UI-components
        progressBar.enabled = false;
        taskText.text = "";
        remainingTimeText.text = "";

        List<Motion> poses = GetRandomSetOfPoses();
        MemoryCard[] cards = ConstructUI(poses);
        StartCoroutine(Memory(cards));
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
                Debug.Log("Added motion");
            }
            if(motionNames.Count == 0)
            {
                break;
            }
        }
        return poses;
    }

    public IEnumerator Memory(MemoryCard[] cards)
    {
        memoryCanvas.SetActive(false);
        startScreen.SetActive(true);
        startScreenText.text = "Get Ready!";

        yield return Timer(timeBeforeStart, startScreenCountdown);

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

            taskText.text = "Guess which motion was behind a card";

            yield return Timer(timeBetweenShowingAndGuessing);

            yield return CardGuessingPhase();

            if(remainingRounds == 0)
            {
                break;
            }
            yield return Timer(timeBetweenRounds);   
        }

        SkeletonDisplay.Instance.OnStopDisplay();
        taskText.text = "";

        memoryCanvas.SetActive(false);
        startScreen.SetActive(true);

        //check if the rounds timed out or if the player won
        if (unsolved.Count > 0)
        {
            startScreenText.text = "No rounds remaining";
        }
        else
        {
            startScreenText.text = "You win!";
        }
    }

    private IEnumerator CardShowingStage()
    {
        taskText.text = "Memorize the shown poses";
        int remainingGroupSize = unsolved.Count < maxGroupSize ? unsolved.Count : maxGroupSize;
        for (int i = 0; i < remainingGroupSize; i++)
        {
            MemoryCard pose = unsolved[GetRandom(unsolved.Count)];
            BeginShowPose(pose);
            //wait for minimum accuracy or minimum time to pass
            yield return Timer(cardShowingTime, remainingTimeText);
            StopShowPose(pose);

            tempStack.Add(pose);
            unsolved.Remove(pose);
            if (i < remainingGroupSize)
            {
                yield return Timer(timeBetweenCardsShowing);
            }
        }
    }

    private IEnumerator CardGuessingPhase()
    {
        KinectDeviceManager.Instance.BeginBodyTracking();
        int tempStackSize = tempStack.Count;
        for (int i = 0; i < tempStackSize; i++)
        {
            MemoryCard pose = tempStack[GetRandom(tempStack.Count)];
            int maxAccuracy = 0;
            BeginOutline(pose);
            float remainingTime = motionGuessingTime;
            if (AppState.bodyTrackingRunning)
            {
                StartCoroutine(SkeletonDisplay.Instance.BodyCompareCoroutine(pose.pose.motion[0], motionGuessingTime));
            }

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
                    solved.Add(pose);
                    break;
                }
                remainingTime -= Time.deltaTime;
                progressBarMask.fillAmount = 1 - remainingTime / motionGuessingTime;
                remainingTimeText.text = Mathf.RoundToInt(remainingTime).ToString();
                yield return null;
            }

            remainingTimeText.text = "";
            progressBarMask.fillAmount = 0f;
            progressBar.enabled = false;

            StopOutline(pose);
            if (maxAccuracy <= 80)
            {
                unsolved.Add(pose);
            }

            tempStack.Remove(pose);

            BeginShowPose(pose);
            yield return Timer(cardShowingTime);
            StopShowPose(pose);


            if (i < tempStackSize)
            {
                yield return Timer(timeBetweenCardsGuessing);
            }
        }
    }

    /// <summary>
    /// Simple timer
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator Timer(float time)
    {
        float remainingTime = time;
        while (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// A timer with UI components  (progressBar and countDown Text)
    /// </summary>
    /// <param name="time"></param>
    /// <param name="progressBar"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    private IEnumerator Timer(float time, TextMeshProUGUI text)
    {
        float remainingTime = time;
        progressBar.enabled = true;
        while (remainingTime > 0f)
        {
            progressBarMask.fillAmount = 1 - remainingTime / time;
            text.text = Mathf.CeilToInt(remainingTime).ToString();
            

            remainingTime -= Time.deltaTime;
            yield return null;
        }
        text.text = "";
        progressBarMask.fillAmount = 0f;
        progressBar.enabled = false;
    }

    private MemoryCard[] ConstructUI(List<Motion> j)
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

    private void BeginShowPose(MemoryCard card)
    {
        Vector3[] vecs = SkeletonDisplay.Instance.GetVectors(card.pose.motion[0]);
        SkeletonDisplay.Instance.Display(vecs);
        card.uiElement.GetComponent<RawImage>().texture = cameraRenderTexture;
    }

    private void StopShowPose(MemoryCard card)
    {
        card.uiElement.GetComponent<RawImage>().texture = backsideTexture;
    }

    private void BeginOutline(MemoryCard card)
    {
        card.uiElement.GetComponent<Outline>().enabled = true;
    }

    private void StopOutline(MemoryCard card)
    {
        card.uiElement.GetComponent<Outline>().enabled = false;
    }

    private int GetRandom(int length)
    {
        return UnityEngine.Random.Range(0, length);
    }
}
