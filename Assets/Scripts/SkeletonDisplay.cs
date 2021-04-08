using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;
using Vector3 = UnityEngine.Vector3;

public class SkeletonDisplay : Singleton<SkeletonDisplay>
{
    private void OnDrawGizmos()
    {
        List<JointConnection> jointConnections = ResolveSkeleton();
        for (int i = 0; i < jointConnections.Count; i++)
        {
            JointConnection jC = jointConnections[i];
            if (jC.posA != null && jC.posB != null)
            {
                if (jC.posA != Vector3.zero || jC.posB != Vector3.zero)
                {
                    jC.posA.y = -jC.posA.y; 
                    jC.posB.y = -jC.posB.y; 
                    Gizmos.DrawLine(jC.posA/100f, jC.posB/100f);
                }
            }
        }

    }

    public Skeleton skeleton;
    public List<JointConnection> ResolveSkeleton()
    {
        List<JointConnection> jointConnections = new List<JointConnection>();

        //get all joint positions
        Joint[] joints = new Joint[27];


        for (int i = 0; i < 27; i++)
        {
            //Joint joint = skeleton.GetJoint(i);
            joints[i] = skeleton.GetJoint(i);
        }

        for (int i = 0; i < 3; i++)
        {
            ConnectJoints(joints[i], joints[i + 1]);
        }
        ConnectJoints(joints[3], joints[26]);
        ConnectJoints(joints[2], joints[4]);

        //connect left arm joints
        for (int i = 4; i < 9; i++)
        {
            ConnectJoints(joints[i], joints[i + 1]);
        }
        ConnectJoints(joints[8], joints[10]);
        ConnectJoints(joints[2], joints[11]);

        //connect right arm joints
        for (int i = 11; i < 16; i++)
        {
            ConnectJoints(joints[i], joints[i + 1]);
        }
        ConnectJoints(joints[15], joints[17]);

        //connect left leg joints
        for (int i = 18; i < 21; i++)
        {
            ConnectJoints(joints[i], joints[i + 1]);
        }

        //connect right leg joints
        for (int i = 22; i < 25; i++)
        {
            ConnectJoints(joints[i], joints[i + 1]);
        }

        ConnectJoints(joints[0], joints[18]);
        ConnectJoints(joints[0], joints[22]);

        return jointConnections;

        void ConnectJoints(Joint jointA, Joint jointB)
        {
            jointConnections.Add(new JointConnection { posA = jointA.Position.ToUnityVector3(), posB = jointB.Position.ToUnityVector3() });
        }
    }



    public struct JointConnection
    {
        public Vector3 posA;
        public Vector3 posB;
    }

}




//0-1, 1-2, 2-3, 3-26 -> Spine + Head

//2-4,4-5,5-6,6-7,7-8,8-9,8-10   --> LeftArm
//2-11,11-12,12-13,13-14,14-15,15-16,15-17  --> RightArm
//0-18,18-19,19-20,20-21 --> LeftFoot
//0-22,22-23,23-24,24-25 --> RightFoot

//0 pelvis
//1 spinenavel
//2 spinechest
//3 neck

//4 clavicleleft
//5 shoulderleft
//6 elbowleft
//7 wristleft
//8 handleft
//9 handtipleft
//10 thumbleft

//11 clavicleright
//12 shoulderright
//13 elbowright
//14 wristright
//15 handright
//16 handtipright
//17 thumbright


//18 hipleft
//19 kneeleft
//20 ankleleft
//21 footleft

//22 hipright
//23 kneeright
//24 ankleright
//25 footright

//26 head