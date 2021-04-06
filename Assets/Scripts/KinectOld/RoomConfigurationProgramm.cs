using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Numerics;
using Microsoft.Azure.Kinect.BodyTracking;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

class RoomConfigurationProgram : IKinectProgram
{
    private string path = "Appsettings.json";
    private const long TIME_INTERVAL = 500L;
    private long bodyHandle,lHandHandle,rHandHandle,headHandle;

    public static bool inited = false;

    public void Execute()
    {
        LightManager lightManager = LightManager.Instance;

        lightManager.Init();
        while (!inited)
        {

        }
        lightManager.TurnOffLights();
        int availableLights = lightManager.GetLamps();
        string appSettings = File.ReadAllText(@path);
        List<CornerValues> cv = new List<CornerValues>();
        
        //subcribe to frame arrived
        TrackingManager.Instance.FrameArrivedEvent += RoomConfiguration_OnFrameArrived;
        //create new collections
        bodyHandle = TempDataManager.Instance.CreateNewDataCollection(TIME_INTERVAL);
        lHandHandle = TempDataManager.Instance.CreateNewDataCollection(TIME_INTERVAL);
        rHandHandle = TempDataManager.Instance.CreateNewDataCollection(TIME_INTERVAL);
        headHandle = TempDataManager.Instance.CreateNewDataCollection(TIME_INTERVAL);

        Console.WriteLine("How many corners do you want to configure? There are " + availableLights + " Lights available.");
        
        while (true)
        {
            string input = Console.ReadLine();
            try
            {
                int amount = int.Parse(input);
                if (amount > availableLights)
                {
                    Console.WriteLine("You cant configure more corners, than there are available lights.");
                    break;
                }
                KinectDeviceManager.Instance.corners = int.Parse(input);
                break;
            }
            catch (Exception)
            {
                Console.WriteLine("Please type an Integer number");
            }
        }

        Color color = Color.white;
        lightManager.SetLights(Command.BRIGHTEN, color);
        lightManager.TurnOffLights();

        for (int i = 0; i < KinectDeviceManager.Instance.corners; i++)
        {

            lightManager.SetLights(Command.ON, new List<string> { lightManager.availableIndizes[i] }, color);
            Console.WriteLine("configure Corner " + i + ", Please go to the glowing lamp and raise your hand to set it");
            Console.WriteLine("Waiting...");

            while (true)
            {
                Thread.Sleep(50);
                if (BodyParser.Instance.CheckIfHandIsRaised(lHandHandle, headHandle, TIME_INTERVAL) || BodyParser.Instance.CheckIfHandIsRaised(rHandHandle, headHandle, TIME_INTERVAL))
                {
                    Vector3 currentPosition = GetCurrentPosition();
                    
                    //Console.WriteLine(V3ToString(currentPosition));
                    cv.Add(new CornerValues(i, V3ToString(currentPosition)));

                    TempDataManager.Instance.ClearCollection(lHandHandle);                    
                    TempDataManager.Instance.ClearCollection(rHandHandle);                    
                    TempDataManager.Instance.ClearCollection(headHandle);                    
                    break;
                }
            }
            lightManager.TurnOffLights(new List<string> { lightManager.availableIndizes[i] });
            Console.ReadLine();

            //wait for a short period of time to allow new data to be collected. This is required, since the
            //data gets averaged over a short period of time. Directly taking values from a near empty collection may lead
            //to inaccurate results, due to inaccuracies in the kinect body-tracking.
            Thread.Sleep(500);
        }

        string jsonString = JsonConvert.SerializeObject(cv);
        File.WriteAllText(@path, jsonString);

        TrackingManager.Instance.FrameArrivedEvent -= RoomConfiguration_OnFrameArrived;
        TempDataManager.Instance.DeleteDataCollection(lHandHandle);
        TempDataManager.Instance.DeleteDataCollection(rHandHandle);
        TempDataManager.Instance.DeleteDataCollection(headHandle);
        TempDataManager.Instance.DeleteDataCollection(bodyHandle);

        Console.WriteLine("All corners have been successfully configured!");
    }

    private Vector3 GetCurrentPosition()
    {
        long toTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        long fromTime = toTime - TIME_INTERVAL;

        return TempDataManager.Instance.GetAverageBetweenFromCollection(bodyHandle, fromTime, toTime);
    }

    private string V3ToString(Vector3 v3)
    {
        return v3.X+";"+v3.Y+";"+v3.Z;
    }


    private void RoomConfiguration_OnFrameArrived(object sender, BodyFrameArrivedEventArgs e)
    {
        Vector3 body = e.bodyFrame.GetBody(0).Skeleton.GetJoint(JointId.SpineNavel).Position;
        if (body != null)
        {
            Vector3 pos = new Vector3(body.X, body.Y, body.Z);
            TempDataManager.Instance.AddDataToCollection(bodyHandle, pos);
        }

        Vector3 rHand = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.HandRight).Position;
        if (rHand != null)
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

        Vector3 head = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.Head).Position;
        if (head != null)
        {
            Vector3 headPosition = new Vector3(head.X, head.Y, head.Z);
            TempDataManager.Instance.AddDataToCollection(headHandle, headPosition);
        }

    }
      
}


