using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

class TrackingManager : Singleton<TrackingManager>
{

    public event EventHandler<BodyFrameArrivedEventArgs> FrameArrivedEvent;
    private static bool finish;
    private static void ThreadTrack()
    {
        Tracker tracker = Tracker.Create(KinectDeviceManager.Instance.calibration, TrackerConfiguration.Default);
        Frame bodyFrame = null;
        while (!finish)
        {
            if (bodyFrame != null)
            {
                bodyFrame.Dispose();
            }

            Capture sensorCapture = KinectDeviceManager.Instance.device.GetCapture(TimeSpan.MaxValue);
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
                        Instance.FrameArrivedEvent?.Invoke(Instance, new BodyFrameArrivedEventArgs { bodyFrame = bodyFrame });
                    }
                    //bodyFrame.Dispose();
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


    private readonly Thread bodyTrackingThread = new Thread(new ThreadStart(ThreadTrack));
     
    public void StartTracking()
    {
        finish = false;
        if(KinectDeviceManager.Instance.device != null)
        {
            Debug.Log(KinectDeviceManager.Instance.device);
            bodyTrackingThread.Start();
        }
        else
        {
            Debug.Log("No Kinect Device found");
        }
    }

    public void StopTracking()
    {
        finish = true;
    }
}
public class BodyFrameArrivedEventArgs : EventArgs
{
    public Frame bodyFrame;
}

