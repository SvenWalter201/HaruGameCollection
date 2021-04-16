using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Newtonsoft.Json;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;
using System.IO;

public class SkeletonTracker : Singleton<SkeletonTracker>
{
    private List<Joint[]> currentMotion;

    public void BeginMotionCapture()
    {
        currentMotion = new List<Joint[]>();
    }

    public void StoreFrame(Joint[] joints)
    {
        currentMotion.Add(joints);
    }

    public void SaveMotion(string fileName)
    {
        Motion motion = new Motion
        {
            fps = 30,
            motion = currentMotion
        };
        string jsonString = JsonConvert.SerializeObject(motion);
        File.WriteAllText(Application.persistentDataPath + "/" + fileName + ".json", jsonString);
    }

    public T Load<T>(string fileName)
    {
        string jsonString = File.ReadAllText(Application.persistentDataPath +  "/" + fileName + ".json"); 
        T motion = JsonConvert.DeserializeObject<T>(jsonString);
        return motion;
    }

    public void SavePose()
    {

    }
}
