using System.Collections;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController>
{
    [SerializeField]
    Canvas mainsceneCanvas;
    [SerializeField]
    AudioListener mainSceneAudioListener;
    [SerializeField]
    EventSystem mainSceneEventSystem;
    [SerializeField]
    Light mainSceneLight;
    [SerializeField]
    Camera mainSceneCamera;

    Game currentGame;

    int loadedLevelBuildIndex = 0;

    public void StartGame(int index)
    {


        StartCoroutine(LoadLevel(index));
    }


    IEnumerator LoadLevel(int levelBuildIndex)
    {


        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);

        FlipMainScene();

        if (AppManager.useVirtualWorld)
            VirtualWorldController.Instance.FlipMainSceneControl();

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));

        loadedLevelBuildIndex = levelBuildIndex;

        currentGame = FindObjectOfType<Game>();
        currentGame.OnGameFinished += OnCurrentGameFinished;
    }

    public void OnCurrentGameFinished(object sender, EventArgs e) => StartCoroutine(GameFinished());

    IEnumerator GameFinished()
    {
        yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);

        FlipMainScene();

        if (AppManager.useVirtualWorld)
            VirtualWorldController.Instance.FlipMainSceneControl();

        loadedLevelBuildIndex = 0;
    }

    void FlipMainScene()
    {
        mainsceneCanvas.enabled = !mainsceneCanvas.enabled;
        mainSceneAudioListener.enabled = !mainSceneAudioListener.enabled;
        mainSceneEventSystem.enabled = !mainSceneEventSystem.enabled;
        //mainSceneLight.enabled = !mainSceneLight.enabled;
        mainSceneCamera.enabled = !mainSceneCamera.enabled;
    }
}
