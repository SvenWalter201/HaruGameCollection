using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoCapStudio : Game
{
    [SerializeField]
    Button returnButton;

    void Start()
    {
        PlayGame();
    }

    protected override IEnumerator Init()
    {
        return base.Init();
    }

    protected override IEnumerator Execute()
    {
        yield return new WaitForUIButtons(new Button[] { returnButton});
    }
}
