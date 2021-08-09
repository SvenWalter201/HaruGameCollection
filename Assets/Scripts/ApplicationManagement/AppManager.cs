using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

public static class AppManager
{
    public static readonly int
        virtualWorldBuildIndex = 0,
        mainMenuBuildIndex = 1,
        initSceneBuildIndex = 2,
        motionMemoryBuildIndex = 3,
        motionMemoryHouseBuildIndex = 4,
        duplikBuildIndex = 5,
        triviaQuizBuildIndex = 6;

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
        vfxGraphSupported = false,
        useVirtualWorld = false;

    public static ColorResolution colorResolution;
    public static ImageFormat colorFormat;
    public static FPS fps;
    public static DepthMode depthMode;
    public static bool syncronizedImagesOnly;
    public static TrackerProcessingMode processingMode = TrackerProcessingMode.Cpu;

    public static Lang language = Lang.DE;
    public static JointId[] jointConstraints;

    static AppConfig appConfig;
    static string appConfigPath = "AppConfig";

    /// <summary>
    /// TODO: 
    /// </summary>
    public static bool LoadConfig()
    {
        if (FileManager.LoadJSONFromResources(appConfigPath, out AppConfig appConfig))
        {
            AppManager.appConfig = appConfig;
        }
        else
        {
            Debug.LogWarning("App Config could not be loaded.");
            return false;
        }

        language = appConfig.Language;
        colorResolution = appConfig.ColorResolution;
        colorFormat = appConfig.ColorFormat;
        fps = appConfig.Fps;
        depthMode = appConfig.DepthMode;
        syncronizedImagesOnly = appConfig.SyncronizedImagesOnly;
        processingMode = appConfig.ProcessingMode;
        useVirtualWorld = appConfig.UseVirtualWorld;

        return true;
    }

    public static void SaveConfig()
    {
        FileManager
    }

    public static void ResolveLimbConstraints(Limb[] limbConstraints)
    {
        jointConstraints = new JointId[0];

        foreach (var constraint in limbConstraints)
        {
            int upper, lower;
            switch (constraint)
            {
                case Limb.UPPER_BODY:
                    lower = (int)Limb.UPPER_BODY;
                    upper = lower + 17;
                    break;
                case Limb.LOWER_BODY:
                    lower = (int)Limb.LOWER_BODY;
                    upper = lower + 7;
                    break;
                case Limb.LEFT_ARM_UPPER:
                    lower = (int)Limb.LEFT_ARM_UPPER;
                    upper = lower + 5;
                    break;
                case Limb.LEFT_ARM_LOWER:
                    lower = (int)Limb.LEFT_ARM_LOWER;
                    upper = lower + 4;
                    break;
                case Limb.RIGHT_ARM_UPPER:
                    lower = (int)Limb.RIGHT_ARM_UPPER;
                    upper = lower + 5;
                    break;
                case Limb.RIGHT_ARM_LOWER:
                    lower = (int)Limb.RIGHT_ARM_LOWER;
                    upper = lower + 4;
                    break;
                case Limb.LEFT_LEG_UPPER:
                    lower = (int)Limb.LEFT_LEG_UPPER + 1;
                    upper = lower + 3;
                    break;
                case Limb.LEFT_LEG_LOWER:
                    lower = (int)Limb.LEFT_LEG_LOWER;
                    upper = lower + 2;
                    break;
                case Limb.RIGHT_LEG_UPPER:
                    lower = (int)Limb.RIGHT_LEG_UPPER;
                    upper = lower + 3;
                    break;
                case Limb.RIGHT_LEG_LOWER:
                    lower = (int)Limb.RIGHT_LEG_LOWER;
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
public enum Limb
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
