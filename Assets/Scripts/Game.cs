using System.Collections;
using System;
using UnityEngine;

public abstract class Game : MonoBehaviour
{
    protected bool isExecuting = false;

    public bool IsExecuting => isExecuting;

    public void PlayGame()
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        //wait for kinect to be setup
        yield return new WaitForSeconds(0.5f);

        yield return Init();

        yield return Execute();

        Finish();
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
