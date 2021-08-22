using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;

[System.Serializable]
public class AppConfig
{
    public static AppConfig DefaultConfig() => 
            new AppConfig
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
                MotionMemoryConfiguration = MotionMemoryConfiguration.DefaultConfig(),
                TriviaQuizConfiguration = TriviaQuizConfiguration.DefaultConfiguration(),
                DuplikConfiguration = DuplikConfiguration.DefaultConfiguration()
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
    public TriviaQuizConfiguration TriviaQuizConfiguration { get; set; }
    public DuplikConfiguration DuplikConfiguration { get; set; }
}

[System.Serializable]
public class TriviaQuizConfiguration : System.ICloneable
{
    public int questionAmount = 10;
    public TriviaQuizLeftorRight.AnsweringMode answeringMode = TriviaQuizLeftorRight.AnsweringMode.MOVE_X_AXIS;
    public bool flipX = false;

    public float 
        answerTime = 15f,  
        questionTime = 10f, 
        showCorrectAnswerTime = 5f;

    public static TriviaQuizConfiguration DefaultConfiguration() => 
        new TriviaQuizConfiguration 
        { 
            questionAmount = 10,
            answeringMode = TriviaQuizLeftorRight.AnsweringMode.MOVE_X_AXIS,
            flipX = false,
            answerTime = 15f,
            questionTime = 10f,
            showCorrectAnswerTime = 5f
        };

    public object Clone()
    {
        return new TriviaQuizConfiguration
        {
            questionAmount = questionAmount,
            answeringMode = answeringMode,
            flipX = flipX,
            answerTime = answerTime,
            questionTime = questionTime,
            showCorrectAnswerTime = showCorrectAnswerTime
        };
    }
}

[System.Serializable]
public class DuplikConfiguration : System.ICloneable
{
    public int 
        sentenceAmount = 5, 
        rounds = 1;
    public float 
        drawingTime = 45f, 
        showingTime = 60f;

    public static DuplikConfiguration DefaultConfiguration() =>
        new DuplikConfiguration
        {
            rounds = 1,
            sentenceAmount = 5,
            drawingTime = 45f,
            showingTime = 60f
        };

    public object Clone()
    {
        return new DuplikConfiguration
        {
            rounds = rounds,
            sentenceAmount = sentenceAmount,
            drawingTime = drawingTime,
            showingTime = showingTime
        };
    }
}

[System.Serializable]
public class MotionMemoryConfiguration : System.ICloneable
{
    public MotionMemoryHouse.HouseSize houseSize;

    public int 
        amount = 4, 
        solvePercentage = 85, 
        maxGroupSize = 2, 
        maxRounds = 2;

    public float 
        cardShowingTime = 5f, 
        motionGuessingTime = 15f, 
        timeBeforeMotionTracking = 1f, 
        timeBetweenRounds = 3f, 
        timeBetweenShowingAndGuessing = 3f, 
        timeBetweenCardsShowing = 3f, 
        timeBetweenCardsGuessing = 3f;

    public static MotionMemoryConfiguration DefaultConfig() =>
        new MotionMemoryConfiguration
        {
            houseSize = MotionMemoryHouse.HouseSize.SIZE_2X2,
            amount = 4,
            solvePercentage = 85,
            maxGroupSize = 2,
            maxRounds = 2,
            cardShowingTime = 5f,
            motionGuessingTime = 15f,
            timeBeforeMotionTracking = 1f,
            timeBetweenRounds = 3f,
            timeBetweenShowingAndGuessing = 3f,
            timeBetweenCardsGuessing = 3f,
            timeBetweenCardsShowing = 3f
        };

    public object Clone()
    {
        return new MotionMemoryConfiguration
        {
            houseSize = houseSize,
            amount = amount,
            solvePercentage = solvePercentage,
            maxGroupSize = maxGroupSize,
            maxRounds = maxRounds,
            cardShowingTime = cardShowingTime,
            motionGuessingTime = motionGuessingTime,
            timeBeforeMotionTracking = timeBeforeMotionTracking,
            timeBetweenRounds = timeBetweenRounds,
            timeBetweenShowingAndGuessing = timeBetweenShowingAndGuessing,
            timeBetweenCardsGuessing = timeBetweenCardsGuessing,
            timeBetweenCardsShowing = timeBetweenCardsShowing
        };
    }
}
