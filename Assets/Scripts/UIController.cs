using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIController : Singleton<UIController>
{
    [Header("ImageSection")]
    [SerializeField] private Button imageTracking;
    [SerializeField] private Button imageDisplay;
    [Space]
    [Header("BTSection")]
    [SerializeField] private Button bodyTracking;
    [SerializeField] private Button record;
    [SerializeField] private Button bodyCompare;
    [Space]
    [Header("FileManagementSection")]
    [SerializeField] private InputField fileNameField;
    [SerializeField] private Button loadMotion;
    [SerializeField] private Button saveMotion;
    [SerializeField] private Button savePose;
    [Space]
    [Header("ReplaySection")]
    [SerializeField] public Slider frameSlider;
    [SerializeField] private Button playPauseButton;
    [SerializeField] private Dropdown display;
    [Space]
    [Header("3D Viewport")]
    [SerializeField] private TextMeshProUGUI compareAccuracy;
    [Header("ImageDisplayPanel")]
    [SerializeField] private GameObject imageDisplayPanel;

    private GameObject savePoseGO;
    private GameObject bodyTrackingGO;
    private GameObject displayGO;
    private GameObject recordGO;
    private GameObject saveMotionGO;
    private GameObject frameSliderGO;
    private GameObject playPauseButtonGO;
    private GameObject imageDisplayGO;
    private GameObject bodyCompareGO;

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

    public readonly ColorBlock onStateColors = new ColorBlock
    {
        normalColor = Color.white,
        highlightedColor = Color.white,
        pressedColor = Color.white,
        selectedColor = Color.white,
        colorMultiplier = 1
    };

    private void Start()
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

        display.onValueChanged.AddListener(DisplayCurrentTracking);
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

        SkeletonDisplay.Instance.InitUIComponents(frameSlider, playPauseButton, compareAccuracy);

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

    public void DisplayCurrentTracking(int option)
    {
        SkeletonDisplay.Instance.SwitchDisplayType(option);
    }

    public void LoadMotion()
    {
        string fileName = fileNameField.text;

        Motion motion = SkeletonTracker.Instance.Load(fileName);
        CheckMotionLoaded(motion);
    }

    private void CheckMotionLoaded(Motion motion)
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
            CheckMotionLoaded(SkeletonTracker.Instance.StoreMotion());
        }
        else
        {
            AppState.recording = true;
            SkeletonTracker.Instance.BeginMotionCapture();
            record.colors = onStateColors;
        }
    }

    public void StoreMotionFile()
    {
        string fileName = fileNameField.text;
        SkeletonTracker.Instance.SaveMotion(fileName);
    }

    public void SavePose()
    {
        if(AppState.motionLoaded)
        {
            Motion loaded = SkeletonTracker.Instance.loadedMotion;
            int frame = Mathf.RoundToInt(frameSlider.value * loaded.motion.Count);
            if (frame >= loaded.motion.Count)
            {
                frame = loaded.motion.Count - 1;
            }
            SkeletonTracker.Instance.SavePose(fileNameField.text, loaded.motion[frame]);
        }
        else
        {
            Debug.Log("No motion to save");
        }

    }
}
