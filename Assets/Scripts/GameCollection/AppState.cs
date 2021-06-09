using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public static class AppState
{
    public static bool
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
    /// <summary>
    /// TODO: 
    /// </summary>
    public static void LoadConfig()
    {

    }

    public static void ResolveLimbConstraints(Limbs[] limbConstraints)
    {

    }

    public static void Initialize()
    {
        vfxGraphSupported = SystemInfo.supportsComputeShaders && SystemInfo.maxComputeBufferInputsVertex != 0;
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
        LEFT_LEG_UPPER = 18, //+3
        LEFT_LEG_LOWER = 19, //+2
        RIGHT_LEG_UPPER = 22, //+3
        RIGHT_LEG_LOWER = 23 //+2
    }
}

public enum Lang
{
    ENG,
    DE
}
