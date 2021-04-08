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

    public ImageType imageType;
    public SkeletonDisplay skeletonDisplay;

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

    public int width;
    public int height;
    private Color[] pixels;

    private bool applicationRunning = false;

    private void Start()
    {
        Init();
    }

    private void OnApplicationQuit()
    {
        Close();
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

        applicationRunning = true;

        Task.Run(()=>CameraCapture());
        Task.Run(()=> ImuCapture());
        Task.Run(()=> BodyCapture());
        
    }

    private void BodyCapture()
    {
        Tracker tracker = null; 
        Frame bodyFrame = null;

        try
        {
            tracker = Tracker.Create(calibration, TrackerConfiguration.Default);
        }
        catch (Exception e)
        {
            applicationRunning = false;
            Debug.Log("An error occured: " + e.Message);
        }
        while (applicationRunning)
        {
            try
            {
                if (bodyFrame != null)
                {
                    bodyFrame.Dispose();
                }

                Capture sensorCapture = device.GetCapture(TimeSpan.MaxValue);
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
                            skeletonDisplay.skeleton = bodyFrame.GetBody(0).Skeleton;
                            //Debug.Log("Tracking " + numBodies + " Bodies");
                        }
                    }

                }
            }
            catch (Exception e)
            {
                applicationRunning = false;
                Debug.Log("An error occured: " + e.Message);
                if (bodyFrame != null)
                {
                    bodyFrame.Dispose();
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
        while (applicationRunning)
        {
            try
            {
                using (var  capture = device.GetCapture())
                {
                    switch (imageType)
                    {
                        case ImageType.Colour: 
                            {
                                BuildColourImageSource(capture);
                                break;
                            }
                        case ImageType.Depth:
                            {
                                BuildDepthImageSource(capture);
                                break;
                            }
                        case ImageType.IR:
                            {
                                BuildIRImageSource(capture);
                                break;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                applicationRunning = false;
                Debug.Log("An error occured: " + e.Message);
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
        width = img.WidthPixels;
        height = img.HeightPixels;

        pixels = img.CreateColourMap();

    }

    private void BuildDepthImageSource(Capture capture)
    {
        Image img = capture.Color;
        if (img == null)
        {
            return;
        }
        width = img.WidthPixels;
        height = img.HeightPixels;

        using (Image transformedDepth = new Image(ImageFormat.Depth16, width, height, width * sizeof(UInt16)))
        {
            // Transform the depth image to the colour capera perspective.
            transformation.DepthImageToColorCamera(capture, transformedDepth);

            // Get the transformed pixels (colour camera perspective but depth pixels).
            Span<ushort> depthBuffer = transformedDepth.GetPixels<ushort>().Span;

            pixels = img.CreateColourMap();

            // Create a new image with data from the depth and colour image.
            for (int i = 0; i < pixels.Length; i++)
            {
                // We'll use the colour image if the depth is less than 1 metre. 
                var depth = depthBuffer[i];

                if (depth == 0 || depth >= 2000) // No depth image.
                {
                    pixels[i].r = 0;
                    pixels[i].g = 0;
                    pixels[i].b = 0;
                }

                if (depth >= 1000 && depth < 1200) // More than a meter away.
                {
                    pixels[i].r = Convert.ToByte(255 - (255 / (depth - 999)));
                }

                if (depth >= 1200 && depth < 1500)
                {
                    pixels[i].g = Convert.ToByte(255 - (255 / (depth - 1199)));
                }

                if (depth >= 1500 && depth < 2000)
                {
                    pixels[i].b = Convert.ToByte(255 - (255 / (depth - 1499)));
                }
            }
        }

        pixels = img.CreateColourMap();   
    }

    private void BuildIRImageSource(Capture capture)
    {
        Image img = capture.IR;
        if (img == null)
        {
            return;
        }
        width = img.WidthPixels;
        height = img.HeightPixels;

        pixels = img.CreateIRMap();
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


    public void ShowImage()
    {
        if(pixels == null)
        {
            return;
        }
        Texture2D tex = TextureGenerator.TextureFromColourMap(pixels, width, height);
        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = tex;
    }
}

public enum ImageType
{
    Colour,
    Depth,
    IR
}