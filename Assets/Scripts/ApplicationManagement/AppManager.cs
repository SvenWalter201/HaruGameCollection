using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public static class AppManager
{
    public static bool
        applicationInitialized = false,
        applicationRunning = false,
        kinectAvailable = false,
        bodyTrackingRunning = false,
        computeShadersAvailable = false,
        imageTrackingRunning = false,
        imageDisplayRunning = false,
        motionLoaded = false,
        bodyCompareRunning = false,
        recording = false,
        vfxGraphSupported = false;

    public static Lang language = Lang.DE;

    public static JointId[] jointConstraints;

    static AppConfig appConfig;
    static string appConfigPath = "/Resources/AppConfig.json";
    /// <summary>
    /// TODO: 
    /// </summary>
    public static bool LoadConfig()
    {
        appConfigPath = Application.dataPath + appConfigPath;
        if (JsonFileManager.Load(appConfigPath, out AppConfig appConfig))
        {
            AppManager.appConfig = appConfig;
        }
        else
        {
            Debug.LogWarning("App Config could not be loaded.");
            return false;
        }

        language = appConfig.Language;

        return true;
    }

    public static void ResolveLimbConstraints(Limbs[] limbConstraints)
    {
        jointConstraints = new JointId[];

        foreach (var constraint in limbConstraints)
        {
            int upper, lower;
            switch (constraint)
            {
                case Limbs.UPPER_BODY:
                    lower = (int)Limbs.UPPER_BODY;
                    upper = lower + 17;
                    break;
                case Limbs.LOWER_BODY:
                    lower = (int)Limbs.LOWER_BODY;
                    upper = lower + 7;
                    break;
                case Limbs.LEFT_ARM_UPPER:
                    lower = (int)Limbs.LEFT_ARM_UPPER;
                    upper = lower + 5;
                    break;
                case Limbs.LEFT_ARM_LOWER:
                    lower = (int)Limbs.LEFT_ARM_LOWER;
                    upper = lower + 4;
                    break;
                case Limbs.RIGHT_ARM_UPPER:
                    lower = (int)Limbs.RIGHT_ARM_UPPER;
                    upper = lower + 5;
                    break;
                case Limbs.RIGHT_ARM_LOWER:
                    lower = (int)Limbs.RIGHT_ARM_LOWER;
                    upper = lower + 4;
                    break;
                case Limbs.LEFT_LEG_UPPER:
                    lower = (int)Limbs.LEFT_LEG_UPPER + 1;
                    upper = lower + 3;
                    break;
                case Limbs.LEFT_LEG_LOWER:
                    lower = (int)Limbs.LEFT_LEG_LOWER;
                    upper = lower + 2;
                    break;
                case Limbs.RIGHT_LEG_UPPER:
                    lower = (int)Limbs.RIGHT_LEG_UPPER;
                    upper = lower + 3;
                    break;
                case Limbs.RIGHT_LEG_LOWER:
                    lower = (int)Limbs.RIGHT_LEG_LOWER;
                    upper = lower + 2;
                    break;
            }
        
            
        }
    }

    public static bool Initialize()
    {
        if (!LoadConfig())
            return false;
        
        if (!StringRes.LoadStringResources())
            return false;

        ResolveLimbConstraints(appConfig.LimbConstraints);

        vfxGraphSupported = SystemInfo.supportsComputeShaders && SystemInfo.maxComputeBufferInputsVertex != 0;

        applicationInitialized = true;
        return true;
    }


}

/// <summary>
/// Limbs are mapped to the beginningIndex
/// </summary>
public enum Limbs
{
    UPPER_BODY = 0, //+17
    LOWER_BODY = 18, //+7
    LEFT_ARM_UPPER = 5, //+5
    LEFT_ARM_LOWER = 6, //+4
    RIGHT_ARM_UPPER = 12, //+5
    RIGHT_ARM_LOWER = 13, //+4
    LEFT_LEG_UPPER = 17, //index +1 +3
    LEFT_LEG_LOWER = 19, //+2
    RIGHT_LEG_UPPER = 22, //+3
    RIGHT_LEG_LOWER = 23 //+2
}

public enum Lang
{
    ENG,
    DE
}
