using Microsoft.Azure.Kinect.BodyTracking;
using System;
using System.Numerics;

class BodyParser
{
    private const long timeAlive = 2000L;
    private TempDataManager m_leftHandPositions;
    private TempDataManager m_rightHandPositions;
    private const float handThreshhold = 50f;

    private static readonly BodyParser instance = new BodyParser();

    static BodyParser()
    {
    }
    private BodyParser()
    {

    }
    public static BodyParser Instance
    {
        get
        {
            return instance;
        }
    }

    //the maximum heightdifference in meters between head and hand for the hand to be counted as raised
    private float minDiffHandHead = +100f;

    public Vector3 PositionalDifference(JointId jointA, JointId jointB, Body body)
    {
        Vector3 positionA = body.Skeleton.GetJoint(jointA).Position; 
        Vector3 positionB = body.Skeleton.GetJoint(jointB).Position;

        float distance = Vector3.Distance(positionA, positionB);

        Console.WriteLine(distance);
        return new Vector3(positionA.X - positionB.X, positionA.Y - positionB.Y, positionA.Z - positionB.Z);
    }

    public bool GetHandClosed(long thumbHandle, long tipHandle, long timeInterval)
    {
        long toTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        long fromTime = toTime - timeInterval;

        Vector3 averageTipPosition = TempDataManager.Instance.GetAverageBetweenFromCollection(tipHandle, fromTime, toTime);
        Vector3 averageThumbPosition = TempDataManager.Instance.GetAverageBetweenFromCollection(thumbHandle, fromTime, toTime);

        float distance = Vector3.Distance(averageThumbPosition, averageTipPosition);

        Console.WriteLine(distance);
        if (distance<handThreshhold)
        {
            Console.WriteLine("Hand Closed");
            return true;
        }
        Console.WriteLine("HandOpen");
        return false;
    }

    public bool CheckIfHandIsRaised(long handHandle, long headHandle, long timeInterval)
    {
        long toTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        long fromTime = toTime - timeInterval;

        Vector3 averageHandPosition = TempDataManager.Instance.GetAverageBetweenFromCollection(handHandle, fromTime, toTime);
        Vector3 averageHeadPosition = TempDataManager.Instance.GetAverageBetweenFromCollection(headHandle, fromTime, toTime);

        float yDiff = averageHandPosition.Y - averageHeadPosition.Y;


        if (yDiff < minDiffHandHead)
        {
            Console.WriteLine("Hand raised");
            return true;
        }
        Console.WriteLine("Hand not raised");

        return false;
    }

    public bool CheckIfHandIsRaised(bool hand /*false = left Hand | true = right Hand*/, Body body)
    {
        Vector3 handPosition;

        if (hand)
        {
            handPosition = body.Skeleton.GetJoint(JointId.HandRight).Position;
        }
        else
        {
            handPosition = body.Skeleton.GetJoint(JointId.HandLeft).Position;
        }

        Vector3 headPosition = body.Skeleton.GetJoint(JointId.Head).Position;

        float yDiff = handPosition.Y - headPosition.Y;


        if (yDiff < minDiffHandHead)
        {
            Console.WriteLine("Hand raised");
            return true;
        }
        Console.WriteLine("Hand not raised");

        return false;
    }
}

public enum Hand {
    LEFT,
    RIGHT
}