using System.Collections.Generic;
using UnityEngine;

public class VirtualWorldController : Singleton<VirtualWorldController>
{
    public Transform house; 

    List<InputSource> mainSceneInputs = new List<InputSource>();
    bool inputState = true;

    public WindowController windowController;

    public void AddInputSource(InputSource source) => mainSceneInputs.Add(source);

    public void FlipMainSceneControl()
    {
        foreach(var i in mainSceneInputs)
            i.active = !i.active;

        inputState = !inputState;
    }
}
