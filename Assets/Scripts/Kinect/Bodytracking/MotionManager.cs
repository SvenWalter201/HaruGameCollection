using System.Collections.Generic;
using UnityEngine;

public class MotionManager : Singleton<MotionManager>
{
    List<UJoint[]> currentMotion;
    public Motion loadedMotion;

    public void BeginMotionCapture() => 
        currentMotion = new List<UJoint[]>();

    public void StoreFrame(UJoint[] joints) => 
        currentMotion.Add(joints);

    public Motion StoreMotion()
    {
        if(currentMotion != null)
        {
            loadedMotion = new Motion
            {
                fps = 30,
                motion = currentMotion
            };
            AppState.motionLoaded = true;
            return loadedMotion;
        }
        else
        {
            AppState.motionLoaded = false;
            return null;
        }
    }

    public void SaveMotion(string fileName)
    {
        Motion motion = new Motion
        {
            fps = 30,
            motion = currentMotion
        };

        string path = Application.persistentDataPath + "/" + fileName + ".json";

        JsonFileManager.Save(path, motion);
    }

    public Motion Load(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".json";

        if (JsonFileManager.Load(path, out Motion motion))
        {
            loadedMotion = motion;
            AppState.motionLoaded = true;
            return motion;
        }
        AppState.motionLoaded = false;
        return null;
    }

    public void SavePose(string fileName, UJoint[] poseJoints)
    {
        Motion motion = new Motion
        {
            fps = 30,
            motion = new List<UJoint[]> { poseJoints }
        };

        string path = Application.persistentDataPath + "/" + fileName + ".json";
        JsonFileManager.Save(path, motion);
    }
}
