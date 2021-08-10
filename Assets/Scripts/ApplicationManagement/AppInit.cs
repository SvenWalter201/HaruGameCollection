using UnityEngine;
using UnityEngine.SceneManagement;

public class AppInit : MonoBehaviour
{
    [SerializeField]
    bool forwardToMainScene;

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
        {
            bool useVirtualWorld = false;//AppManager.useVirtualWorld;
            if(useVirtualWorld)
                SceneManager.LoadScene(AppManager.virtualWorldBuildIndex, LoadSceneMode.Single);
            else
                SceneManager.LoadScene(AppManager.mainMenuBuildIndex, LoadSceneMode.Single);
        }
    }
}