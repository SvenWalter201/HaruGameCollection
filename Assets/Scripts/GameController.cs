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

    Game currentGame;

    int loadedLevelBuildIndex = 0;

    public void StartGame(int index)
    {
        mainsceneCanvas.enabled = false;
        mainSceneAudioListener.enabled = false;
        mainSceneEventSystem.enabled = false;

        StartCoroutine(LoadLevel(index));
    }

    public void OnCurrentGameFinished(object sender, EventArgs e) => StartCoroutine(GameFinished());

    IEnumerator LoadLevel(int levelBuildIndex)
    {
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));

        loadedLevelBuildIndex = levelBuildIndex;

        currentGame = FindObjectOfType<Game>();
        currentGame.OnGameFinished += OnCurrentGameFinished;
    }

    IEnumerator GameFinished()
    {
        yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);

        mainsceneCanvas.enabled = true;
        mainSceneAudioListener.enabled = true;
        mainSceneEventSystem.enabled = true;

        loadedLevelBuildIndex = 0;
    }
}
