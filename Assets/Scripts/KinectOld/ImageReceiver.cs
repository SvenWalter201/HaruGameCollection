using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading;
using UnityEngine;

class ImageReceiver : Singleton<ImageReceiver>
{
    public event EventHandler<ColorFrameArrivedEventArgs> FrameArrivedEvent;
    private static bool finish;
    private static void ReceiveImages()
    {
        Image depthImage = null;
        Image colorImage = null;
        while (!finish)
        {
            if (depthImage != null)
            {
                depthImage.Dispose();
            }
            if (colorImage != null)
            {
                colorImage.Dispose();
            }

            Capture sensorCapture = KinectDeviceManager.Instance.device.GetCapture(TimeSpan.MaxValue);

            if (sensorCapture != null)
            { 
                depthImage = sensorCapture.Depth;
                colorImage = sensorCapture.Color;

                ColorFrameArrivedEventArgs e = new ColorFrameArrivedEventArgs();

                if(depthImage != null)
                {
                    e.depthImage = depthImage;
                }
                if (colorImage != null)
                {
                    e.colorImage = colorImage;
                }
                Instance.FrameArrivedEvent?.Invoke(Instance, e);
            }

            sensorCapture.Dispose();
        }

        if (depthImage != null)
        {
            depthImage.Dispose();
        }
        if(colorImage != null)
        {
            colorImage.Dispose();
        }
    }


    private Thread imageReceiverThread = new Thread(new ThreadStart(ReceiveImages));

    public void StartTracking()
    {
        finish = false;
        imageReceiverThread = new Thread(new ThreadStart(ReceiveImages));
        imageReceiverThread.Start();

        /*
        if (!imageReceiverThread.IsAlive)
        {
            imageReceiverThread.Start();
            Debug.Log("Started Image tracking");

        }
        else
        {
            imageReceiverThread.Start();
            Debug.Log("Continued Image tracking");
        }*/
    }

    public void StopTracking()
    {
        //imageReceiverThread.Interrupt();
        finish = true;
        Debug.Log("Stopped Image tracking");
    }
}
public class ColorFrameArrivedEventArgs : EventArgs
{
    public Image depthImage;
    public Image colorImage;
}