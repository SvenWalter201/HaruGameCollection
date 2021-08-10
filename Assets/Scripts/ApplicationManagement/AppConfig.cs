using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;

[System.Serializable]
public class AppConfig
{
    public static AppConfig DefaultConfig()
=> new AppConfig
            {
                Language = Lang.DE,
                ColorResolution = ColorResolution.R1080p,
                ImageFormat = ImageFormat.ColorBGRA32,
                Fps = FPS.FPS30,
                DepthMode = DepthMode.NFOV_Unbinned,
                SyncronizedImagesOnly = true,
                ProcessingMode = TrackerProcessingMode.Cuda,
                UseVirtualWorld = true,
                LimbConstraints = new Limb[0]
            };
    

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
