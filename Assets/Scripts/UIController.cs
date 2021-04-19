using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIController : Singleton<UIController>
{
    [SerializeField] private Button bodyTracking;
    [SerializeField] private Dropdown display;
    [SerializeField] private Button record;
    [SerializeField] private Button loadMotion;
    [SerializeField] private Button saveMotion;
    [SerializeField] private InputField fileNameField;
    [SerializeField] private Button savePose;
    [SerializeField] public Slider frameSlider;
    [SerializeField] private Button playPauseButton;
    [SerializeField] private Button imageTracking;
    [SerializeField] private Button imageDisplay;

    private GameObject savePoseGO;
    private GameObject bodyTrackingGO;
    private GameObject displayGO;
    private GameObject recordGO;
    private GameObject saveMotionGO;
    private GameObject frameSliderGO;
    private GameObject playPauseButtonGO;
    private GameObject imageDisplayGO;

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

        imageDisplayGO.SetActive(false);

        recordGO.SetActive(false);
        frameSliderGO.SetActive(false);
        playPauseButtonGO.SetActive(false);
        saveMotionGO.SetActive(false);
        savePoseGO.SetActive(false);

        SkeletonDisplay.Instance.InitUIComponents(frameSlider, playPauseButton);

    }

    public void TrackImageData()
    {
        if (KinectDeviceManager.Instance.imageTracking)
        {
            KinectDeviceManager.Instance.imageTracking = false;
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
        if (KinectDeviceManager.Instance.imageDisplay)
        {
            KinectDeviceManager.Instance.imageDisplay = false;
            imageDisplay.colors = offStateColors;
        }
        else
        {
            KinectDeviceManager.Instance.imageDisplay = true;
            imageDisplay.colors = onStateColors;
        }
    }

    public void TrackBodyData()
    {
        if (KinectDeviceManager.Instance.bodyTracking)
        {
            KinectDeviceManager.Instance.bodyTracking = false;
            bodyTracking.colors = offStateColors;

            recordGO.SetActive(false);
        }
        else
        {
            if (KinectDeviceManager.Instance.BeginBodyTracking())
            {
                bodyTracking.colors = onStateColors;

                //enable UI elements
                recordGO.SetActive(true);
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
        if (motion == null)
        {
            Debug.Log("couldn't load motion. File either doesn't exist or is broken");
            return;
        }
        frameSliderGO.SetActive(true);
        playPauseButtonGO.SetActive(true);
        saveMotionGO.SetActive(true);
        savePoseGO.SetActive(true);
    }

    public void RecordCapture()
    {
        if (SkeletonDisplay.Instance.record)
        {
            SkeletonDisplay.Instance.record = false;
            record.colors = offStateColors;
            CheckMotionLoaded(SkeletonTracker.Instance.StoreMotion());
        }
        else
        {
            SkeletonDisplay.Instance.record = true;
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
        Motion loaded = SkeletonTracker.Instance.loadedMotion;
        if(loaded != null)
        {
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
