public static class AppState
{
    public static bool applicationRunning = false;
    public static bool kinectAvailable = false;
    public static bool bodyTrackingRunning = false;
    public static bool computeShadersAvailable = false;
    public static bool imageTrackingRunning = false;
    public static bool imageDisplayRunning = false;
    public static bool motionLoaded = false;
    public static bool bodyCompareRunning = false;
    public static bool recording = false;
    /*
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        Debug.Log("Before scene loaded");
    }*/
}
