using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Microsoft.Azure.Kinect.BodyTracking;


class RoomManager
{
    public Room m_Room { get; set; }
    static float m_toleranceXZ = 300f;

    private static readonly RoomManager instance = new RoomManager();

    static RoomManager()
    {
    }
    private RoomManager()
    {
        //get Room Singleton
    }
    public static RoomManager Instance
    {
        get
        {
            return instance;
        }
    }

    public bool CheckIfPersonIsInCorner(Corner corner, Vector3 personPosition)
    {
        Vector3 cornerPosition = m_Room.m_Corners[corner];
        //Console.WriteLine(corner + ": " + cornerPosition);
        if (Math.Abs(personPosition.X - cornerPosition.X) <= m_toleranceXZ &&
            Math.Abs(personPosition.Z - cornerPosition.Z) <= m_toleranceXZ)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void ShowRoomCorner(Body body)
    {
        Vector3 position = body.Skeleton.GetJoint(JointId.Head).Position;

        if (CheckIfPersonIsInCorner(Corner.LEFT_UP,position))
        {
            
            Console.WriteLine("Corner A");
        }
        else if (CheckIfPersonIsInCorner(Corner.RIGHT_UP, position))
        {
            Console.WriteLine("Corner B");
        }
        else if (CheckIfPersonIsInCorner(Corner.LEFT_DOWN, position))
        {
            Console.WriteLine("Corner C");
        }
        else if (CheckIfPersonIsInCorner(Corner.RIGHT_DOWN, position))
        {
            Console.WriteLine("Corner D");
        }
        else
        {
            Console.WriteLine("In the middle");
        }

    }

    public void CheckIfPersonIsInCorrectCorner(long bodyHandle, Corner cor, long timeInterval)
    {
        long toTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        long fromTime = toTime - timeInterval;

        Vector3 averageBodyPosition = TempDataManager.Instance.GetAverageBetweenFromCollection(bodyHandle, fromTime, toTime);
        Console.WriteLine("Pos: " + averageBodyPosition);
        Console.WriteLine("LEFT_UP: " + CheckIfPersonIsInCorner(Corner.LEFT_UP, averageBodyPosition));
        Console.WriteLine("RIGHT_UP: " + CheckIfPersonIsInCorner(Corner.RIGHT_UP, averageBodyPosition));
        Console.WriteLine("LEFT_DOWN: " + CheckIfPersonIsInCorner(Corner.LEFT_DOWN, averageBodyPosition));
        Console.WriteLine("RIGHT_DOWN: " + CheckIfPersonIsInCorner(Corner.RIGHT_DOWN, averageBodyPosition));
        /*
        if (CheckIfPersonIsInCorner(Corner.LEFT_UP, averageBodyPosition))
        {
            Console.WriteLine("Corner A");
        }
        else if (CheckIfPersonIsInCorner(Corner.RIGHT_UP, averageBodyPosition))
        {
            Console.WriteLine("Corner B");
        }
        else if (CheckIfPersonIsInCorner(Corner.LEFT_DOWN, averageBodyPosition))
        {
            Console.WriteLine("Corner C");
        }
        else if (CheckIfPersonIsInCorner(Corner.RIGHT_DOWN, averageBodyPosition))
        {
            Console.WriteLine("Corner D");
        }
        else
        {
            Console.WriteLine("In the middle");
        }
        */
    }

}

