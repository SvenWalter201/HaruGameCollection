using System.Collections;
using System;
using UnityEngine;

public abstract class Game : MonoBehaviour
{
    [SerializeField]
    protected bool useAppConfig = true;

    protected bool isExecuting = false;

    public bool IsExecuting => isExecuting;

    bool paused = false;

    public void PlayGame()
    {
        StartCoroutine(Play());
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            Finish();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = !paused;
            Time.timeScale = (paused) ? 0f : 1f;
        }
    }

    IEnumerator Play()
    {
        //wait for kinect to be setup
        //yield return new WaitForSeconds(0.01f);
        yield return new WaitForEndOfFrame();

        if (useAppConfig)
            ConfigSetup();

        yield return Init();

        yield return Execute();

        Finish();
    }

    protected virtual void ConfigSetup()
    {

    }

    protected virtual IEnumerator Init()
    {
        yield break;
    }

    protected virtual IEnumerator Execute()
    {
        yield break;
    }

    public void Finish()
    {
        OnGameFinished?.Invoke(this, null);
    }

    public event EventHandler<EventArgs> OnGameFinished;

}
