using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Azure.Kinect.BodyTracking;
using System.Numerics;
using System.Linq;

class GestureSavingTest
{
    Frame bodyFrame;
    private const long TIME_INTERVAL = 1000;
    private const long MAXIMUM_LIFETIME = 10000;
    private static int poseType;
    private long lHandHandle, rHandHandle, lElbowHandle, rElbowHandle, lKneeHandle, rKneeHandle, lAnkleHandle,
        rAnkleHandle, chestHandle, headHandle, hipHandle;

    public void Execute()
    {


        Console.WriteLine("Gesture Program started");

        Console.WriteLine("What kind of pose do you wish to record?");
        Console.WriteLine("[1] Full body pose");
        Console.WriteLine("[2] Upper Body Pose");
        poseType = int.Parse(Console.ReadLine());

        //create new collections
        rHandHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
        lHandHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
        rElbowHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
        lElbowHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
        chestHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
        headHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);

        if (poseType == 1)
        {
            //create new collections
            lKneeHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
            rKneeHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
            lAnkleHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
            rAnkleHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
            hipHandle = TempDataManager.Instance.CreateNewDataCollection(MAXIMUM_LIFETIME);
        }

        TrackingManager.Instance.FrameArrivedEvent += GestureRecognition_OnBodyFrameArrived;
        TrackingManager.Instance.FrameArrivedEvent -= GestureRecognition_OnBodyFrameArrived;


    }





    public void GestureRecognition_OnBodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
    {
        bodyFrame = e.bodyFrame;


        if (poseType == 1)
        {
            Vector3 lKnee = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.KneeLeft).Position;
            if (lKnee != null)
            {
                TempDataManager.Instance.AddDataToCollection(lKneeHandle, lKnee);
            }

            Vector3 rKnee = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.KneeRight).Position;
            if (rKnee != null)
            {
                TempDataManager.Instance.AddDataToCollection(rKneeHandle, rKnee);
            }

            Vector3 lAnkle = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.AnkleLeft).Position;
            if (lAnkle != null)
            {
                TempDataManager.Instance.AddDataToCollection(lAnkleHandle, lAnkle);
            }

            Vector3 rAnkle = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.AnkleRight).Position;
            if (rAnkle != null)
            {
                TempDataManager.Instance.AddDataToCollection(rAnkleHandle, rAnkle);
            }

            Vector3 hipR = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.HipRight).Position;
            Vector3 hipL = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.HipLeft).Position;
            if (hipR != null && hipL != null)
            {
                Vector3 hip = (hipL + hipR) / 2f;
                TempDataManager.Instance.AddDataToCollection(hipHandle, hip);
            }
        }

        Vector3 rHand = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.HandRight).Position;
        if (rHand != null)
        {
            TempDataManager.Instance.AddDataToCollection(rHandHandle, rHand);
        }

        Vector3 lHand = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.HandLeft).Position;
        if (lHand != null)
        {
            TempDataManager.Instance.AddDataToCollection(lHandHandle, lHand);
        }

        Vector3 head = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.Head).Position;
        if (head != null)
        {
            TempDataManager.Instance.AddDataToCollection(headHandle, head);
        }

        Vector3 lElbow = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.ElbowLeft).Position;
        if (lElbow != null)
        {
            TempDataManager.Instance.AddDataToCollection(lElbowHandle, lElbow);
        }

        Vector3 elbowRight = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.ElbowRight).Position;
        if (elbowRight != null)
        {
            TempDataManager.Instance.AddDataToCollection(rElbowHandle, elbowRight);
        }

        Vector3 chest = e.bodyFrame.GetBodySkeleton(0).GetJoint(JointId.SpineChest).Position;
        if (chest != null)
        {
            TempDataManager.Instance.AddDataToCollection(chestHandle, chest);
        }
    }
}


public class BodyPose
{
    public Dictionary<JointId, Vector3> jointPositions;

    public void MakePositionsRelativeToHip(Vector3 hipPosition)
    {
        for (int i = 0; i < jointPositions.Count; i++)
        {
            //Vector3 id = jointPositions.Values.ElementAt<Vector3>(i);
            //jointPositions(i) -= hipPosition;
        }
    }
    /*
    public string getCard(int random)
    {
        return Karta._dict.ElementAt(random).Key;
    }*/

}