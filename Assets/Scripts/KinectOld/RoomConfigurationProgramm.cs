using System.Collections;
using UnityEngine;

class RoomConfigurationProgram : Game
{
    //bool useHue = false;
    //CoroutineTimer timer = new CoroutineTimer();

    protected override IEnumerator Init()
    {
        //check, if there already exists a room calibration 
        return base.Init();
    }

    protected override IEnumerator Execute()
    {
        int cornersToConfigure = 0;
        Vector3[] corners = new Vector3[cornersToConfigure];

        for (int i = 0; i < cornersToConfigure; i++)
        {
            //glow a lamp
            //show UI Text
            //leave some time for the person to raise their hand
            //save the position in the corners[]
        }

        RoomManager.Instance.CreateRoom(corners);
        yield break;
    }
}


