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
            AppManager.motionLoaded = true;
            return loadedMotion;
        }
        else
        {
            AppManager.motionLoaded = false;
            return null;
        }
    }

    public void SaveMotion(string fileName)
    {
        if(FileManager.LoadJSONFromResources("DefaultPoses/" + fileName, out Motion m))
        {
            Debug.LogWarning("The motion/pose you want to save has the same name as one of the default poses. Therefore it was not saved. Please retry with a different name!");
            return;
        }

        string path = Application.persistentDataPath + "/" + fileName + ".json";
        FileManager.SaveJSON(path, loadedMotion);
    }

    public Motion Load(string fileName)
    {
        Motion motion;
        if (FileManager.LoadJSONFromResources("DefaultPoses/" + fileName, out motion))
        {
            loadedMotion = motion;
            AppManager.motionLoaded = true;
            return motion;
        }


        string path = Application.persistentDataPath + "/" + fileName + ".json";

        if (FileManager.LoadJSON(path, out motion))
        {
            loadedMotion = motion;
            AppManager.motionLoaded = true;
            return motion;
        }

        AppManager.motionLoaded = false;
        return null;
    }

    public void SavePose(string fileName, UJoint[] poseJoints)
    {
        Motion motion = new Motion
        {
            fps = 30,
            motion = new List<UJoint[]> { poseJoints },
            notInvolvedLimbs = loadedMotion.notInvolvedLimbs
        };
        

        string path = Application.persistentDataPath + "/" + fileName + ".json";
        FileManager.SaveJSON(path, motion);
    }
}
