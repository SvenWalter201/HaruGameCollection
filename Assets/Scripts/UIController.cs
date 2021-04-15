using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    [SerializeField] private Button bodyTracking;
    [SerializeField] private Button display;
    [SerializeField] private Button record;
    [SerializeField] private Button saveMotion;
    [SerializeField] private InputField fileNameField;
    [SerializeField] private Slider frameSlider;
    [SerializeField] private Button playPauseButton;
    private GameObject bodyTrackingGO;
    private GameObject displayGO;
    private GameObject recordGO;
    private GameObject saveMotionGO;
    private GameObject frameSliderGO;
    private GameObject playPauseButtonGO;

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

        bodyTracking.onClick.AddListener (TrackBodyData);
        bodyTracking.colors = offStateColors;

        record.onClick.AddListener(RecordCapture);
        record.colors = offStateColors;

        SkeletonDisplay.Instance.display = true;
        display.onClick.AddListener(DisplayCurrentTracking);
        display.colors = onStateColors;

        saveMotion.onClick.AddListener(StoreMotionFile);

        recordGO.SetActive(false);
        frameSliderGO.SetActive(false);
        playPauseButtonGO.SetActive(false);

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
            KinectDeviceManager.Instance.BeginBodyTracking();
            bodyTracking.colors = onStateColors;

            //enable UI elements
            recordGO.SetActive(true);
        }
    }

    public void DisplayCurrentTracking()
    {
        if (SkeletonDisplay.Instance.display) {
            SkeletonDisplay.Instance.display = false;
            display.colors = offStateColors;
        } 
        else
        {
            SkeletonDisplay.Instance.display = true;
            display.colors = onStateColors;
        }
    }

    public void ReplayCapture()
    {
        if (KinectDeviceManager.Instance.bodyTracking)
        {
            TrackBodyData();
        }
        string fileName = fileNameField.text;

        Motion motion = SkeletonTracker.Instance.Load<Motion>(fileName);
        if(motion == null)
        {
            Debug.Log("couldn't load motion. Motion was null");
            return;
        }
        frameSliderGO.SetActive(true);
        playPauseButtonGO.SetActive(true);
        StartCoroutine(SkeletonDisplay.Instance.Replay(motion, frameSlider, playPauseButton));
    }

    public void RecordCapture()
    {
        if (SkeletonDisplay.Instance.record)
        {
            SkeletonDisplay.Instance.record = false;
            record.colors = offStateColors;
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
}
