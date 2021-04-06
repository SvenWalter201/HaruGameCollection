using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Azure.Kinect.BodyTracking;
using System.Numerics;

//a testprogram, that checks, if a person has his hands raised, opened or closed
class GestureRecognitionProgram : IKinectProgram
{
    Frame bodyFrame;
    private const long TIME_INTERVAL = 1000;
    private const long MAXIMUM_LIFETIME = 10000;
    private long lHandHandle, rHandHandle, rThumbHandle, lThumbHandle, rHandtipHandle, lHandtipHandle, headHandle;

    public void Execute()
    {
        //create new collections
        rHandHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
        lHandHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
        rHandtipHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
        lHandtipHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
        rThumbHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
        lThumbHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
        headHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);

        TrackingManager.Instance.FrameArrivedEvent += GestureRecognition_OnBodyFrameArrived;
        Console.WriteLine("Gesture Program started");


        while (true)
        {
            Thread.Sleep(1000);
            BodyParser.Instance.CheckIfHandIsRaised(lHandHandle, headHandle, TIME_INTERVAL);
            BodyParser.Instance.CheckIfHandIsRaised(rHandHandle, headHandle, TIME_INTERVAL);
            BodyParser.Instance.GetHandClosed(rThumbHandle, rHandtipHandle, TIME_INTERVAL);
            BodyParser.Instance.GetHandClosed(lThumbHandle, lHandtipHandle, TIME_INTERVAL);
        }

        /*
        while (true)
        {
            Console.WriteLine("[1] GetHandState");
            Console.WriteLine("[2] Exit");
            string userInput = Console.ReadLine();

            switch (userInput)
            {
                case "1":
                    {
                        if (bodyFrame != null)
                        {
                            BodyParser.Instance.CheckIfHandIsRaised(lHandHandle, headHandle, TIME_INTERVAL);
                        }
                        break;

                    }
                case "2":
                    {
                        TrackingManager.Instance.FrameArrivedEvent -= GestureRecognition_OnBodyFrameArrived;
                        //Dispose of collections
                        TempDataManager.Instance.DeleteDataCollection(rHandHandle);
                        TempDataManager.Instance.DeleteDataCollection(lHandHandle);
                        TempDataManager.Instance.DeleteDataCollection(rHandtipHandle);
                        TempDataManager.Instance.DeleteDataCollection(lHandtipHandle);
                        TempDataManager.Instance.DeleteDataCollection(rThumbHandle);
                        TempDataManager.Instance.DeleteDataCollection(lThumbHandle);
                        TempDataManager.Instance.DeleteDataCollection(headHandle);
                        Console.WriteLine("now exiting program");
                        return;
                    }
                default:
                    {
                        Console.WriteLine("please select a valid option");

                        break;
                    }
            }
        }*/
    }

    public void GestureRecognition_OnBodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
    {
        bodyFrame = e.bodyFrame;

        Vector3 rHand = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.HandRight).Position;
        if(rHand != null)
        {
            Vector3 pos = new Vector3(rHand.X, rHand.Y, rHand.Z);
            TempDataManager.Instance.AddDataToCollection(rHandHandle, pos);
        }

        Vector3 lHand = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.HandLeft).Position;
        if (lHand != null)
        {
            Vector3 pos = new Vector3(lHand.X, lHand.Y, lHand.Z);
            TempDataManager.Instance.AddDataToCollection(lHandHandle, pos);
        }

        Vector3 rHandtip = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.HandTipRight).Position;
        if (rHandtip != null)
        {
            Vector3 pos = new Vector3(rHandtip.X, rHandtip.Y, rHandtip.Z);
            TempDataManager.Instance.AddDataToCollection(rHandtipHandle, pos);
        }

        Vector3 lHandtip = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.HandTipLeft).Position;
        if (lHandtip != null)
        {
            Vector3 pos = new Vector3(lHandtip.X, lHandtip.Y, lHandtip.Z);
            TempDataManager.Instance.AddDataToCollection(lHandtipHandle, pos);
        }

        Vector3 rThumb = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.ThumbRight).Position;
        if (rThumb != null)
        {
            Vector3 pos = new Vector3(rThumb.X, rThumb.Y, rThumb.Z);
            TempDataManager.Instance.AddDataToCollection(rThumbHandle, pos);
        }

        Vector3 lThumb = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.ThumbLeft).Position;
        if (lThumb != null)
        {
            Vector3 pos = new Vector3(lThumb.X, lThumb.Y, lThumb.Z);
            TempDataManager.Instance.AddDataToCollection(lThumbHandle, pos);
        }

        Vector3 head = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.Head).Position;
        if (head != null)
        {
            Vector3 headPosition = new Vector3(head.X, head.Y, head.Z);
            TempDataManager.Instance.AddDataToCollection(headHandle, headPosition);
        }
    }
}

