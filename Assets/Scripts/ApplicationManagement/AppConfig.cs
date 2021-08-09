using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;

[System.Serializable]
public class AppConfig
{
    public Lang Language { get; set; }
    public ColorResolution ColorResolution { get; set; }
    public ImageFormat ImageFormat { get; set; }
    public FPS Fps { get; set; }
    public DepthMode DepthMode { get; set; }
    public bool SyncronizedImagesOnly { get; set; }
    public TrackerProcessingMode ProcessingMode { get; set; }
    public bool UseVirtualWorld { get; set; }
    public Limb[] LimbConstraints { get; set; }
}
