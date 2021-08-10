using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using TMPro;

public class MainMenu : MonoBehaviour
{

    [SerializeField]
    GameObject 
        motionMemoryModeMenu, 
        triviaQuizModeMenu, 
        duplikModeMenu,
        mainPanel,
        optionsPanel;

    [SerializeField]
    Text header, triviaQuizBtn, motionMemoryBtn, duplikBtn, virtualWorldBtn, moCapStudioBtn, optionsBtn, quitBtn, saveExitBtn;

    [SerializeField]
    TMP_Dropdown languageDD, colorResolutionDD, imageFormatDD, fpsDD, depthModeDD, processingModeDD;

    void DisplayLanguageText()
    {
        header.text = StringRes.Get("_MainMenuHeader");
        triviaQuizBtn.text = StringRes.Get("_TriviaQuizName");
        motionMemoryBtn.text = StringRes.Get("_MotionMemoryName");
        duplikBtn.text = StringRes.Get("_DuplikName");
        virtualWorldBtn.text = StringRes.Get("_EnterVirtualWorld");
        moCapStudioBtn.text = StringRes.Get("_MoCapStudio");
        optionsBtn.text = StringRes.Get("_Options");
        quitBtn.text = StringRes.Get("_QuitApp");
        saveExitBtn.text = StringRes.Get("_SaveAndReturn");
    }

    void Start()
    {
        //change the language on the buttons
        DisplayLanguageText();


        AppConfig conf = AppManager.AppConfig;

        languageDD.onValueChanged.AddListener(OnLanguageChanged);
        languageDD.value = (int)conf.Language;

        colorResolutionDD.onValueChanged.AddListener(OnColorResolutionChanged);
        colorResolutionDD.value = (int)conf.ColorResolution;

        imageFormatDD.onValueChanged.AddListener(OnImageFormatChanged);
        imageFormatDD.value = (int)conf.ImageFormat;

        fpsDD.onValueChanged.AddListener(OnFPSChanged);
        fpsDD.value = (int)conf.Fps;

        depthModeDD.onValueChanged.AddListener(OnDepthModeChanged);
        depthModeDD.value = (int)conf.DepthMode;

        processingModeDD.onValueChanged.AddListener(OnProcessingModeChanged);
        processingModeDD.value = (int)conf.ProcessingMode;
    }

    public void OpenOptionsMenu()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        AppManager.SaveConfig();
        mainPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void EnterVirtualWorld()
    {
        AppManager.useVirtualWorld = true;
        SceneManager.LoadScene(AppManager.virtualWorldBuildIndex, LoadSceneMode.Single);
    }

    public void EnterMotionMemory()
    {
        AppManager.useVirtualWorld = false;
        Instantiate(motionMemoryModeMenu);
    }

    public void EnterTriviaQuiz()
    {
        GameController.Instance.StartGame(0);

    }

    public void EnterDuplik()
    {
        AppManager.useVirtualWorld = false;
        Instantiate(duplikModeMenu);
    }

    public void EnterMoCapStudio()
    {
        GameController.Instance.StartGame(1);
    }

    public void OnColorResolutionChanged(int value) => 
        AppManager.AppConfig.ColorResolution = (ColorResolution)value;

    public void OnImageFormatChanged(int value) => 
        AppManager.AppConfig.ImageFormat = (ImageFormat)value;

    public void OnFPSChanged(int value) => 
        AppManager.AppConfig.Fps = (FPS)value;

    public void OnDepthModeChanged(int value) => 
        AppManager.AppConfig.DepthMode = (DepthMode)value;

    public void OnProcessingModeChanged(int value) => 
        AppManager.AppConfig.ProcessingMode = (TrackerProcessingMode)value;

    //public void OnSyncedImagesOnlyChanged(int value) =>  AppManager.AppConfig.ProcessingMode = (TrackerProcessingMode)value;

    public void OnLanguageChanged(int value)
    {
        AppManager.AppConfig.Language = (Lang)value;
        DisplayLanguageText();
        //reload UI?
    }
}
