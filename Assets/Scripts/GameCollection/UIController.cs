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
    Button loadMotion, saveMotion, savePose, addLimbConstraint;
    TMP_Dropdown limbConstraints;
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

    //GameObject savePoseGO, bodyTrackingGO, displayGO, recordGO, saveMotionGO, frameSliderGO, playPauseButtonGO, imageDisplayGO, bodyCompareGO;

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
        /*
        bodyTrackingGO = bodyTracking.gameObject;
        displayGO = display.gameObject;
        recordGO = record.gameObject;
        saveMotionGO = saveMotion.gameObject;
        frameSliderGO = frameSlider.gameObject;
        playPauseButtonGO = playPauseButton.gameObject;
        savePoseGO = savePose.gameObject;
        imageDisplayGO = imageDisplay.gameObject;
        bodyCompareGO = bodyCompare.gameObject;
        */
        imageTracking.onClick.AddListener(TrackImageData);
        imageTracking.colors = offStateColors;

        imageDisplay.onClick.AddListener(DisplayImageData);

        bodyTracking.onClick.AddListener (TrackBodyData);
        bodyTracking.colors = offStateColors;

        record.onClick.AddListener(RecordCapture);
        record.colors = offStateColors;

        display.onValueChanged.AddListener(DisplayCurrentTracking);
        saveMotion.onClick.AddListener(StoreMotionFile);
        loadMotion.onClick.AddListener(LoadMotion);
        savePose.onClick.AddListener(SavePose);

        bodyCompare.onClick.AddListener(RunBodyCompare);
        bodyCompare.colors = offStateColors;

        //imageDisplayGO.SetActive(false);
        imageDisplay.gameObject.SetActive(false);
        record.gameObject.SetActive(false);
        frameSlider.gameObject.SetActive(false);
        playPauseButton.gameObject.SetActive(false);
        saveMotion.gameObject.SetActive(false);
        savePose.gameObject.SetActive(false);
        bodyCompare.gameObject.SetActive(false);
        

        BodyDisplay.Instance.InitUIComponents(frameSlider, playPauseButton, compareAccuracy, smoothingFrames);

    }

    public void TrackImageData()
    {
        if (AppManager.imageTrackingRunning)
        {
            AppManager.imageTrackingRunning = false;
            imageTracking.colors = offStateColors;
            imageDisplay.gameObject.SetActive(false);
        }
        else
        {
            if (KinectDeviceManager.Instance.BeginImageTracking())
            {
                imageTracking.colors = onStateColors;

                //enable UI elements
                imageDisplay.gameObject.SetActive(true);
            }
        }
    }

    public void DisplayCurrentTracking(int index) => 
        BodyDisplay.Instance.SwitchDisplayType(index);

    public void DisplayImageData()
    {
        if (AppManager.imageDisplayRunning)
        {
            AppManager.imageDisplayRunning = false;
            imageDisplay.colors = offStateColors;
            imageDisplayPanel.SetActive(false);
        }
        else
        {
            imageDisplayPanel.SetActive(true);
            AppManager.imageDisplayRunning = true;
            imageDisplay.colors = onStateColors;
        }
    }

    public void RunBodyCompare()
    {
        if (AppManager.bodyTrackingRunning)
        {
            if (AppManager.bodyCompareRunning)
            {
                AppManager.bodyCompareRunning = false;
            }
            else
            {
                AppManager.bodyCompareRunning = true;
                StartCoroutine(BodyDisplay.Instance.BodyCompareCoroutine());
            }
        }
    }

    public void TrackBodyData()
    {
        if (AppManager.bodyTrackingRunning)
        {
            AppManager.bodyTrackingRunning = false;
            bodyTracking.colors = offStateColors;

            record.gameObject.SetActive(false);
            AppManager.bodyCompareRunning = false;
            bodyCompare.gameObject.SetActive(false);
        }
        else
        {
            if (KinectDeviceManager.Instance.BeginBodyTracking())
            {
                bodyTracking.colors = onStateColors;

                //enable UI elements
                record.gameObject.SetActive(true);
                if (AppManager.motionLoaded)
                {
                    bodyCompare.gameObject.SetActive(true);
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
        if (!AppManager.motionLoaded)
        {
            bodyCompare.gameObject.SetActive(false);
            Debug.Log("couldn't load motion. File either doesn't exist or is broken");
            return;
        }
        else if(motion.motion.Count > 1){
            frameSlider.gameObject.SetActive(true);
            playPauseButton.gameObject.SetActive(true);
            savePose.gameObject.SetActive(true);
            bodyCompare.gameObject.SetActive(false);
        }
        else
        {
            frameSlider.gameObject.SetActive(false);
            playPauseButton.gameObject.SetActive(false);
            savePose.gameObject.SetActive(false);
        }
        saveMotion.gameObject.SetActive(true);
        if (AppManager.bodyTrackingRunning)
        {
            bodyCompare.gameObject.SetActive(true);
        }
    }

    public void RecordCapture()
    {
        if (AppManager.recording)
        {
            AppManager.recording = false;
            record.colors = offStateColors;
            CheckMotionLoaded(MotionManager.Instance.StoreMotion());
        }
        else
        {
            AppManager.recording = true;
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
        if(AppManager.motionLoaded)
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
