using UnityEngine;
using UnityEngine.SceneManagement;

public class AppInit : MonoBehaviour
{
    [SerializeField]
    bool forwardToMainScene;

    const int mainMenuBuildIndex = 0, virtualWorldBuildIndex = 1;

    void Awake() 
    {
        if (AppManager.applicationInitialized)
            return;

        if(!AppManager.Initialize()){
            Debug.LogWarning("App couldn't be initialized and will now shut down");
            /*
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            */
        }
        if (forwardToMainScene)
        {
            bool useVirtualWorld = AppManager.useVirtualWorld;
            if(useVirtualWorld)
                SceneManager.LoadScene(virtualWorldBuildIndex, LoadSceneMode.Single);
            else
                SceneManager.LoadScene(mainMenuBuildIndex, LoadSceneMode.Single);
        }
    }
}