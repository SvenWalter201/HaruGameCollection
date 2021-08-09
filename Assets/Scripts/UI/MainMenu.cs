using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    Text header, triviaQuizBtn, motionMemoryBtn, duplikBtn, virtualWorldBtn;

    void Start()
    {
        //change the language on the buttons

        header.text = StringRes.Get("_MainMenuHeader");
        triviaQuizBtn.text = StringRes.Get("_TriviaQuizName");
        motionMemoryBtn.text = StringRes.Get("_MotionMemoryName");
        duplikBtn.text = StringRes.Get("_DuplikName");
        virtualWorldBtn.text = StringRes.Get("_EnterVirtualWorld");
    }

    public void OpenOptionsMenu()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
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
        AppManager.useVirtualWorld = false;
        Instantiate(triviaQuizModeMenu);
    }

    public void EnterDuplik()
    {
        AppManager.useVirtualWorld = false;
        Instantiate(duplikModeMenu);
    }
}
