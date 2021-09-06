using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScene : InputSource
{
    [SerializeField]
    int buildIndex;

    public override void TestForInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
        }
    }
}