using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

public class KinectDeviceManager : Singleton<KinectDeviceManager>
{
    [SerializeField] private UnityEngine.UI.RawImage colourImage;
    [SerializeField] private UnityEngine.UI.RawImage depthImage;
    [SerializeField] private UnityEngine.UI.RawImage irImage;

    public bool bodyTracking = false;
    public bool imageTracking = false;
    public bool imageDisplay = false;

    public SkeletonDisplay skeletonDisplay;

    [Header("Configuration Parameters. Cant be changed at runtime")]
    public ColorResolution colorResolution;
    public ImageFormat colorFormat;
    public FPS fps;
    public DepthMode depthMode;
    public bool syncronizedImagesOnly;

    public Device device;
    public Calibration calibration;
    public int corners;
    [Space]

    //IMU Data
    public string accelerometerTimestamp;
    public Vector3 accel;
    [Space]
    public string gyroTimeStamp;
    public Vector3 gyro;
    [Space]
    public float temperature;

    private Transformation transformation;
    private int colourWidth;
    private int colourHeight;
    private const int irWidth = 640;
    private const int irHeight = 576;

    private float syncedUpdateTimer = 0f;

    private BGRA[] colourPixels;
    private ushort[] depthPixels;
    private ushort[] irPixels;

    private bool applicationRunning = false;

    static int colourKernelId, irKernelId, depthKernelId;
    private void Awake()
    {
        colourKernelId = computeShader.FindKernel("ColourTex");
        irKernelId = computeShader.FindKernel("IRTex");
        depthKernelId = computeShader.FindKernel("DepthTex");

        switch (colorResolution)
        {
            case ColorResolution.R720p:
                {
                    colourWidth = 1280;
                    colourHeight = 720;
                    break;
                }
            case ColorResolution.R1080p:
                {
                    colourWidth = 1920;
                    colourHeight = 1080;
                    break;
                }
            default:
                {
                    colorResolution = ColorResolution.R1080p;
                    colourWidth = 1920;
                    colourHeight = 1080;
                    break;
                }
        }

        colourTex = new RenderTexture(colourWidth, colourHeight, 24)
        {
            enableRandomWrite = true
        };
        colourTex.Create();
        colourImage.texture = colourTex;

        irTex = new RenderTexture(irWidth, irHeight, 24)
        {
            enableRandomWrite = true
        };
        irTex.Create();
        irImage.texture = irTex;

        depthTex = new RenderTexture(colourWidth, colourHeight, 24)
        {
            enableRandomWrite = true
        };
        depthTex.Create();
        depthImage.texture = depthTex;

    }

    private void Start()
    {
        Init();
    }

    private void OnApplicationQuit()
    {
        Close();
    }


    private void Update()
    {
        if(syncedUpdateTimer <= 0f)
        {
            syncedUpdateTimer = 0.033f;
            if (imageDisplay)
            {
                ShowImage();
            }
        }
        else
        {
            syncedUpdateTimer -= Time.deltaTime;
        }

    }

    public void Init()
    {
        skeletonDisplay = SkeletonDisplay.Instance;

        int count = Device.GetInstalledCount();
        if (count == 0)
        {
            Debug.Log("No k4a devices attached!\n");
            return;
        }

        // Open the first plugged in Kinect device
        device = Device.Open();
        if (device == null)
        {
            Debug.Log("Failed to open k4a device!\n");
            return;
        }

        string serial = device.SerialNum;
        Debug.Log("Opened device:" + serial);

        var config = new DeviceConfiguration
        {
            ColorResolution = colorResolution,
            ColorFormat = colorFormat,
            DepthMode = depthMode,
            CameraFPS = fps,
            SynchronizedImagesOnly = syncronizedImagesOnly

        };
        device.StartCameras(config);
        device.StartImu();
        calibration = device.GetCalibration(depthMode, colorResolution);
        transformation = calibration.CreateTransformation();
        colourWidth = calibration.ColorCameraCalibration.ResolutionWidth;
        colourHeight = calibration.ColorCameraCalibration.ResolutionHeight;

        Debug.Log("Kinect started successfully");
        //Task.Run(() => ImuCapture());

        applicationRunning = true;
    }

    public bool BeginImageTracking()
    {
        if (device != null)
        {
            imageTracking = true;
            Task.Run(()=> CameraCapture());
            return true;
        }
        else
        {
            Debug.Log("Can't start imagetracking, because no k4a-device is attached");
            return false;
        }
    }

    public bool BeginBodyTracking()
    {
        if(device != null)
        {
            try
            {
                Tracker tracker = Tracker.Create(calibration, TrackerConfiguration.Default);
                Task.Run(() => BodyCapture(tracker));
            }
            catch (Exception e)
            {
                Debug.Log("An error occured: " + e.Message);
                bodyTracking = false;
                return false;
            }
            bodyTracking = true;
            return true;
        }
        else
        {
            Debug.Log("Can't start bodytracking, because no k4a-device is attached");
            return false;
        }
    }

    private void BodyCapture(Tracker tracker)
    {
        Frame bodyFrame = null;
        int btFrame = 0;
        
        while (bodyTracking && applicationRunning)
        {
            if(syncedUpdateTimer <= 0f)
            {
                try
                {
                    if (bodyFrame != null)
                    {
                        bodyFrame.Dispose();
                    }

                    Capture sensorCapture = device.GetCapture();
                    if (sensorCapture != null)
                    {
                        tracker.EnqueueCapture(sensorCapture, TimeSpan.MaxValue);
                        sensorCapture.Dispose();

                        bodyFrame = tracker.PopResult(TimeSpan.MaxValue);
                        if (bodyFrame != null)
                        {
                            // Successfully popped the body tracking result. Start your processing
                            uint numBodies = bodyFrame.NumberOfBodies;
                            if (numBodies > 0)
                            {

                                skeletonDisplay.trackedBody = bodyFrame.GetBody(0).Skeleton;
                                skeletonDisplay.frame = btFrame;
                                //Debug.Log("Tracking " + numBodies + " Bodies");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    bodyTracking = false;
                    Debug.Log("An error occured: " + e.Message);
                    if (bodyFrame != null)
                    {
                        bodyFrame.Dispose();
                    }
                    tracker.Shutdown();
                    tracker.Dispose();
                }
            }
            
        }
        if (bodyFrame != null)
        {
            bodyFrame.Dispose();
        }
        tracker.Shutdown();
        tracker.Dispose();
    
}

    private void CameraCapture()
    {
        while (imageTracking && applicationRunning)
        {
            if (syncedUpdateTimer <= 0f)
            {
                try
                {
                    Capture capture = device.GetCapture();
                    
                    BuildColourImageSource(capture);
                    BuildDepthImageSource(capture);
                    BuildIRImageSource(capture);

                    capture.Dispose();
                }
                catch (Exception e)
                {
                    imageTracking = false;
                    Debug.Log("An error occured: " + e.Message);
                }
            }
        }
    }

    private void BuildColourImageSource(Capture capture)
    {
        Image img = capture.Color;
        if (img == null)
        {
            return;
        }

        colourPixels = img.GetPixels<BGRA>().ToArray();

    }


    private Image transformedDepth;
    private int DepthWidth;
    private int DepthHeight;
    private void BuildDepthImageSource(Capture capture)
    {
        if (capture.Color == null || capture.Depth == null)
        {
            return;
        }

        transformedDepth = new Image(ImageFormat.Depth16, colourWidth, colourHeight, colourWidth * sizeof(ushort));

        transformation.DepthImageToColorCamera(capture, transformedDepth);
        depthPixels = transformedDepth.GetPixels<ushort>().ToArray();
        //depthPixels = capture.CreateDepthImage(transformation);
        DepthWidth = transformedDepth.WidthPixels;
        DepthHeight = transformedDepth.HeightPixels;

        transformedDepth.Dispose();
    }

    private void BuildIRImageSource(Capture capture)
    {
        Image img = capture.IR;
        if (img == null)
        {
            return;
        }

        irPixels = img.GetPixels<ushort>().ToArray();
    }



    private void ImuCapture()
    {
        while (applicationRunning)
        {
            try
            {
                var imu = device.GetImuSample();

                accelerometerTimestamp = imu.AccelerometerTimestamp.ToString(@"hh\:mm\:ss");
                System.Numerics.Vector3 gyroSample = imu.GyroSample;
                gyro = new Vector3(gyroSample.X, gyroSample.Y, gyroSample.Z);
                temperature = imu.Temperature;
                System.Numerics.Vector3 accelerometerSample = imu.AccelerometerSample;
                accel = new Vector3(accelerometerSample.X, accelerometerSample.Y, accelerometerSample.Z);
                gyroTimeStamp = imu.GyroTimestamp.ToString(@"hh\:mm\:ss");
                AddOrUpdateDeviceData("Accelerometer timestamp: ", imu.AccelerometerTimestamp.ToString(@"hh\:mm\:ss"));
            }
            catch (Exception e)
            {
                applicationRunning = false;
                Debug.Log("An error occured: " + e.Message);
            }
        }
    }

    private void AddOrUpdateDeviceData(string key, string value)
    {
        //find out if the value already exists
    }

    public void Close()
    {
        bodyTracking = false;
        applicationRunning = false;
        Task.WaitAny(Task.Delay(1000));
        if (device == null)
        {
            return;
        }

        device.StopCameras();
        device.StopImu();
        device.Dispose();
        Debug.Log("Kinect closed successfully");
    }

    public ComputeShader computeShader;
    private RenderTexture colourTex;
    private RenderTexture irTex;
    private RenderTexture depthTex;

    static readonly int
    colourPixelsInID = Shader.PropertyToID("_ColourPixelsIn"),
    colourPixelsOutID = Shader.PropertyToID("_ColourPixelsOut"),
    colourWidthID = Shader.PropertyToID("_Width"),
    irWidthID = Shader.PropertyToID("_IRWidth"),
    irPixelsInID = Shader.PropertyToID("_IRPixelsIn"),
    irPixelsOutID = Shader.PropertyToID("_IRPixelsOut"),
    depthWidthID = Shader.PropertyToID("_DepthWidth"),
    depthPixelsInID = Shader.PropertyToID("_DepthPixelsIn"),
    depthPixelsOutID = Shader.PropertyToID("_DepthPixelsOut");

    private ComputeBuffer colourBuffer;
    private ComputeBuffer irBuffer;
    private ComputeBuffer depthBuffer;
    public void ShowImage()
    {
        if(colourPixels != null)
        {
            colourBuffer = new ComputeBuffer(colourPixels.Length, sizeof(int));
            colourBuffer.SetData(colourPixels);
            computeShader.SetBuffer(colourKernelId, colourPixelsInID, colourBuffer);
            computeShader.SetInt(colourWidthID, colourWidth);
            computeShader.SetTexture(colourKernelId, colourPixelsOutID, colourTex);
            computeShader.Dispatch(colourKernelId, colourWidth/8,colourHeight/8,1);
            colourBuffer.Dispose();
        }
        if (depthPixels != null)
        {
            depthBuffer = new ComputeBuffer(depthPixels.Length/2, sizeof(int));
            depthBuffer.SetData(depthPixels);
            computeShader.SetBuffer(depthKernelId, depthPixelsInID, depthBuffer);
            computeShader.SetInt(depthWidthID, DepthWidth);
            computeShader.SetTexture(depthKernelId, depthPixelsOutID, depthTex);
            computeShader.Dispatch(depthKernelId, DepthWidth / 8, DepthHeight / 8, 1);
            depthBuffer.Dispose();
        }
        if (irPixels != null)
        {
            irBuffer = new ComputeBuffer(irPixels.Length/2, sizeof(int));
            irBuffer.SetData(irPixels);
            computeShader.SetBuffer(irKernelId, irPixelsInID, irBuffer);
            computeShader.SetInt(irWidthID, irWidth);
            computeShader.SetTexture(irKernelId, irPixelsOutID, irTex);
            computeShader.Dispatch(irKernelId, irWidth / 8, irHeight / 8, 1);
            irBuffer.Dispose();            
            //irImage.sprite = Sprite.Create(TextureGenerator.TextureFromColourMap(irPixels, irWidth, irHeight), new Rect(0, 0, irWidth, irHeight), Vector2.zero);
        }
    }
}