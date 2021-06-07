using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : Singleton<UIController>
{
    [Header("ImageSection")]
    [SerializeField] 
    Button imageTracking, imageDisplay;
    [Space]
    [Header("BTSection")]
    [SerializeField] 
    Button bodyTracking, record, bodyCompare;
    [Space]
    [Header("FileManagementSection")]
    [SerializeField] 
    InputField fileNameField;
    [SerializeField] 
    Button loadMotion, saveMotion, savePose;
    [Space]
    [Header("ReplaySection")]
    [SerializeField] 
    public Slider frameSlider;

    [SerializeField] 
    Button playPauseButton;
    [SerializeField] 
    Dropdown display;
    [Space]
    [Header("3D Viewport")]
    [SerializeField] 
    TextMeshProUGUI compareAccuracy;
    [SerializeField] 
    TMP_InputField smoothingFrames;
    [Header("ImageDisplayPanel")]
    [SerializeField] 
    GameObject imageDisplayPanel;

    GameObject savePoseGO, bodyTrackingGO, displayGO, recordGO, saveMotionGO, frameSliderGO, playPauseButtonGO, imageDisplayGO, bodyCompareGO;

    public readonly ColorBlock regularButtonColors = new ColorBlock
    {
        normalColor = Color.grey,
        highlightedColor = Color.grey,
        pressedColor = Color.grey,
        selectedColor = Color.grey,
        colorMultiplier = 1
    };

    public readonly ColorBlock offStateColors = new ColorBlock {
        normalColor = Color.grey,
        highlightedColor = Color.grey,
        pressedColor = Color.grey,
        selectedColor = Color.grey,
        colorMultiplier = 1
    };

    public readonly ColorBlock onStateColors = new ColorBlock {
        normalColor = Color.white,
        highlightedColor = Color.white,
        pressedColor = Color.white,
        selectedColor = Color.white,
        colorMultiplier = 1
    };

    void Start()
    {
        bodyTrackingGO = bodyTracking.gameObject;
        displayGO = display.gameObject;
        recordGO = record.gameObject;
        saveMotionGO = saveMotion.gameObject;
        frameSliderGO = frameSlider.gameObject;
        playPauseButtonGO = playPauseButton.gameObject;
        savePoseGO = savePose.gameObject;
        imageDisplayGO = imageDisplay.gameObject;
        bodyCompareGO = bodyCompare.gameObject;

        imageTracking.onClick.AddListener(TrackImageData);
        imageTracking.colors = offStateColors;

        imageDisplay.onClick.AddListener(DisplayImageData);

        bodyTracking.onClick.AddListener (TrackBodyData);
        bodyTracking.colors = offStateColors;

        record.onClick.AddListener(RecordCapture);
        record.colors = offStateColors;

        //display.onValueChanged.AddListener(DisplayCurrentTracking);
        saveMotion.onClick.AddListener(StoreMotionFile);
        loadMotion.onClick.AddListener(LoadMotion);
        savePose.onClick.AddListener(SavePose);

        bodyCompare.onClick.AddListener(RunBodyCompare);
        bodyCompare.colors = offStateColors;

        imageDisplayGO.SetActive(false);

        recordGO.SetActive(false);
        frameSliderGO.SetActive(false);
        playPauseButtonGO.SetActive(false);
        saveMotionGO.SetActive(false);
        savePoseGO.SetActive(false);
        bodyCompareGO.SetActive(false);

        SkeletonDisplay.Instance.InitUIComponents(frameSlider, playPauseButton, compareAccuracy, smoothingFrames);

    }

    public void TrackImageData()
    {
        if (AppState.imageTrackingRunning)
        {
            AppState.imageTrackingRunning = false;
            imageTracking.colors = offStateColors;
            imageDisplayGO.SetActive(false);
        }
        else
        {
            if (KinectDeviceManager.Instance.BeginImageTracking())
            {
                imageTracking.colors = onStateColors;

                //enable UI elements
                imageDisplayGO.SetActive(true);
            }
        }
    }

    public void DisplayImageData()
    {
        if (AppState.imageDisplayRunning)
        {
            AppState.imageDisplayRunning = false;
            imageDisplay.colors = offStateColors;
            imageDisplayPanel.SetActive(false);
        }
        else
        {
            imageDisplayPanel.SetActive(true);
            AppState.imageDisplayRunning = true;
            imageDisplay.colors = onStateColors;
        }
    }

    public void RunBodyCompare()
    {
        if (AppState.bodyTrackingRunning)
        {
            if (AppState.bodyCompareRunning)
            {
                AppState.bodyCompareRunning = false;
            }
            else
            {
                AppState.bodyCompareRunning = true;
                StartCoroutine(SkeletonDisplay.Instance.BodyCompareCoroutine());
            }
        }
    }

    public void TrackBodyData()
    {
        if (AppState.bodyTrackingRunning)
        {
            AppState.bodyTrackingRunning = false;
            bodyTracking.colors = offStateColors;

            recordGO.SetActive(false);
            AppState.bodyCompareRunning = false;
            bodyCompareGO.SetActive(false);
        }
        else
        {
            if (KinectDeviceManager.Instance.BeginBodyTracking())
            {
                bodyTracking.colors = onStateColors;

                //enable UI elements
                recordGO.SetActive(true);
                if (AppState.motionLoaded)
                {
                    bodyCompareGO.SetActive(true);
                    //compare enable
                }
            }
        }
    }

    public void LoadMotion()
    {
        string fileName = fileNameField.text;

        Motion motion = MotionManager.Instance.Load(fileName);
        CheckMotionLoaded(motion);
    }

    void CheckMotionLoaded(Motion motion)
    {
        if (!AppState.motionLoaded)
        {
            bodyCompareGO.SetActive(false);
            Debug.Log("couldn't load motion. File either doesn't exist or is broken");
            return;
        }
        else if(motion.motion.Count > 1){
            frameSliderGO.SetActive(true);
            playPauseButtonGO.SetActive(true);
            savePoseGO.SetActive(true);
            bodyCompareGO.SetActive(false);
        }
        else
        {
            frameSliderGO.SetActive(false);
            playPauseButtonGO.SetActive(false);
            savePoseGO.SetActive(false);
        }
        saveMotionGO.SetActive(true);
        if (AppState.bodyTrackingRunning)
        {
            bodyCompareGO.SetActive(true);
        }
    }

    public void RecordCapture()
    {
        if (AppState.recording)
        {
            AppState.recording = false;
            record.colors = offStateColors;
            CheckMotionLoaded(MotionManager.Instance.StoreMotion());
        }
        else
        {
            AppState.recording = true;
            MotionManager.Instance.BeginMotionCapture();
            record.colors = onStateColors;
        }
    }

    public void StoreMotionFile()
    {
        string fileName = fileNameField.text;
        MotionManager.Instance.SaveMotion(fileName);
    }

    public void SavePose()
    {
        if(AppState.motionLoaded)
        {
            Motion loaded = MotionManager.Instance.loadedMotion;
            int frame = Mathf.RoundToInt(frameSlider.value * loaded.motion.Count);
            if (frame >= loaded.motion.Count)
            {
                frame = loaded.motion.Count - 1;
            }
            MotionManager.Instance.SavePose(fileNameField.text, loaded.motion[frame]);
        }
        else
        {
            Debug.Log("No motion to save");
        }

    }
}
