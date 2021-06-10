using UnityEngine;
using UnityEngine.SceneManagement;
public class AppInit : MonoBehaviour
{
    [SerializeField]
    bool forwardToMainScene;

    [SerializeField]
    int mainSceneBuildIndex;

    void Awake() 
    {
        if (AppManager.applicationInitialized)
            return;

        if(!AppManager.Initialize()){
            Debug.LogWarning("App couldn't be initialized and will now shut down");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        if (forwardToMainScene)
            SceneManager.LoadScene(mainSceneBuildIndex, LoadSceneMode.Single);
    }
}