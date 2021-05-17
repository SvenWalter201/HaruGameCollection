using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;
using System.IO;

public class SkeletonTracker : Singleton<SkeletonTracker>
{
    List<Joint[]> currentMotion;
    public Motion loadedMotion;

    public void BeginMotionCapture() => currentMotion = new List<Joint[]>();

    public void StoreFrame(Joint[] joints) => currentMotion.Add(joints);

    public Motion StoreMotion()
    {
        if(currentMotion != null)
        {
            loadedMotion = new Motion
            {
                fps = 30,
                motion = currentMotion
            };
        }
        return loadedMotion;
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

    public Motion Load(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        if (File.Exists(path))
        {
            string jsonString = File.ReadAllText(path);
            Motion motion = JsonConvert.DeserializeObject<Motion>(jsonString);
            loadedMotion = motion;
            AppState.motionLoaded = true;
            return motion;
        }
        AppState.motionLoaded = false;
        return null;
    }

    public void SavePose(string fileName, Joint[] poseJoints)
    {
        Motion motion = new Motion
        {
            fps = 30,
            motion = new List<Joint[]> { poseJoints }
        };
        string jsonString = JsonConvert.SerializeObject(motion);
        File.WriteAllText(Application.persistentDataPath + "/" + fileName + ".json", jsonString);
    }
}
