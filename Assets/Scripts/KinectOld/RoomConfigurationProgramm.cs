using UnityEngine;

class RoomConfigurationProgram : Singleton<RoomConfigurationProgram>
{
    public static bool inited = false;
    //bool useHue = false;
    //CoroutineTimer timer = new CoroutineTimer();

    public void Execute()
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
    }  
}


