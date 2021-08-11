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
                UseVirtualWorld = false,
                LimbConstraints = new Limb[0],
                MotionMemoryConfiguration = MotionMemoryConfiguration.DefaultConfig()
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
    public MotionMemoryConfiguration MotionMemoryConfiguration { get; set; }
}

[System.Serializable]
public class MotionMemoryConfiguration
{
    public MotionMemoryHouse.HouseSize HouseSize { get; set; }
    public int Amount { get; set; }
    public int SolvePercentage { get; set; }
    public int MaxGroupSize  {get;set;}
    public int MaximumRounds { get; set; }

    public float CardShowingTime { get; set; }
    public float MotionGuessingTime {get;set;}
    public float TimeBeforeMotionTracking {get;set;}
    public float TimeBetweenRounds {get;set;}
    public float TimeBetweenShowingAndGuessing {get;set;}
    public float TimeBetweenCardsShowing {get;set;}
    public float TimeBetweenCardsGuessing {get;set;}

    public static MotionMemoryConfiguration DefaultConfig() =>
        new MotionMemoryConfiguration
        {
            HouseSize = MotionMemoryHouse.HouseSize.SIZE_2X2,
            Amount = 4,
            SolvePercentage = 85,
            MaxGroupSize = 2,
            MaximumRounds = 2,
            CardShowingTime = 5f,
            MotionGuessingTime = 15f,
            TimeBeforeMotionTracking = 1f,
            TimeBetweenRounds = 3f,
            TimeBetweenShowingAndGuessing = 3f,
            TimeBetweenCardsGuessing = 3f,
            TimeBetweenCardsShowing = 3f
        };

}
