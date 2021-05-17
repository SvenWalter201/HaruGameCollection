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

    public virtual void Terminate()
    {

    }

    public void Finish()
    {
        OnGameFinished?.Invoke(this, null);
    }

    public event EventHandler<EventArgs> OnGameFinished;

}
