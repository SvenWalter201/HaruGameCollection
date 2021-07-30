using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
using System.Linq;

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
    TextMeshProUGUI compareAccuracy, bodyPosText;
    [SerializeField] 
    TMP_InputField smoothingFrames;
    [Header("ImageDisplayPanel")]
    [SerializeField]
    GameObject imageDisplayPanel;

    [SerializeField]
    TMP_Dropdown addConstraintOptions;
    [SerializeField]
    RectTransform scrollView;
    [SerializeField]
    Button addLimbConstraintBtn;
    [SerializeField]
    GameObject limbConstraintPrefab;

    List<Limb> constraintOptions = new List<Limb>();

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

        addLimbConstraintBtn.onClick.AddListener(AddConstraint);

        //imageDisplayGO.SetActive(false);
        imageDisplay.gameObject.SetActive(false);
        record.gameObject.SetActive(false);
        frameSlider.gameObject.SetActive(false);
        playPauseButton.gameObject.SetActive(false);
        saveMotion.gameObject.SetActive(false);
        savePose.gameObject.SetActive(false);
        bodyCompare.gameObject.SetActive(false);
        

        BodyDisplay.Instance.InitUIComponents(frameSlider, playPauseButton, compareAccuracy, smoothingFrames, bodyPosText);

    }

    public void RemoveLimbConstraint(Limb limbConstraint)
    {
        MotionManager.Instance.loadedMotion.notInvolvedLimbs.Remove(limbConstraint);
        ReloadConstraintWindow();
    }

    public void ReloadConstraintWindow()
    {
        ClearConstraintDisplay();

        List<Limb> constraints = MotionManager.Instance.loadedMotion.notInvolvedLimbs;
        if (constraints == null)
            constraints = new List<Limb>();

        for (int i = 0; i < constraints.Count; i++)
        {
            GameObject instance = Instantiate(limbConstraintPrefab, scrollView);
            instance.GetComponent<LimbConstraint>().Init(constraints[i]);
        }

        List<string> possibleConstraints = new List<string>();
        var values = EnumUtil.GetValues<Limb>();
        foreach(var v in values)
        {
            if (!constraints.Contains(v))
            {
                possibleConstraints.Add(v.ToString());
                constraintOptions.Add(v);

            }
        }
        addConstraintOptions.AddOptions(possibleConstraints);

    }

    public void AddConstraint()
    {
        if (constraintOptions.Count == 0)
            return;

        int index = addConstraintOptions.value;
        List<Limb> constraints = MotionManager.Instance.loadedMotion.notInvolvedLimbs;
        if (constraints == null)
            constraints = new List<Limb>();

        constraints.Add(constraintOptions[index]);
        MotionManager.Instance.loadedMotion.notInvolvedLimbs = constraints;
        ReloadConstraintWindow();
    }

    void ClearConstraintDisplay()
    {
        constraintOptions.Clear();
        addConstraintOptions.ClearOptions();

        int count = scrollView.childCount;
        for (int i = 0; i < count; i++)
            Destroy(scrollView.GetChild(i).gameObject);
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
                StartCoroutine(BodyDisplay.Instance.PelvisTrackRoutine());

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
        else if(motion.motion.Count > 1)
        {
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

        ReloadConstraintWindow();

        if (AppManager.bodyTrackingRunning)
            bodyCompare.gameObject.SetActive(true);
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
                frame = loaded.motion.Count - 1;

            MotionManager.Instance.SavePose(fileNameField.text, loaded.motion[frame]);
        }
        else
            Debug.Log("No motion to save");

    }
}

public static class EnumUtil
{
    public static IEnumerable<T> GetValues<T>() =>
        Enum.GetValues(typeof(T)).Cast<T>();
}