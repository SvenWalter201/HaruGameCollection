using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.Kinect.BodyTracking;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;
using Vector3 = UnityEngine.Vector3;
using Quaternion = System.Numerics.Quaternion;

public class SkeletonDisplay : Singleton<SkeletonDisplay>
{
    public bool record = false;
    public bool display = false;
    public bool replaying = false;

    [SerializeField] private LineRenderer body = default;
    [SerializeField] private LineRenderer leftLeg = default;
    [SerializeField] private LineRenderer rightLeg = default;
    [SerializeField] private LineRenderer leftArm = default;
    [SerializeField] private LineRenderer rightArm = default;

    public Skeleton skeleton;
    public int frame = 0;
    private const float FPS_30_DELTA = 0.033f;
    private float currentTimeStep;

    private void Start()
    {
        currentTimeStep = FPS_30_DELTA;
    }

    private void Update()
    {
        if (replaying)
        {
            return;
        }
        //resolve skeleton at framerate = 30;
        if(currentTimeStep <= 0f)
        {
            ResolveSkeleton();
            currentTimeStep = FPS_30_DELTA;
        }
        else
        {
            currentTimeStep -= Time.deltaTime;
        }
    }

    public IEnumerator Replay(Motion motion, Slider frameSlider, Button playPauseButton)
    {
        replaying = true;
        int currentFrame = 0;
        int totalFrames = motion.motion.Count;
        float timer = 0f;
        bool playing = true;
        playPauseButton.onClick.AddListener(() => playing = !playing);

        while(currentFrame < motion.motion.Count)
        {
            if(timer <= 0f)
            {
                frameSlider.value += 1/(float)totalFrames;
                currentFrame = Mathf.RoundToInt(frameSlider.value * (totalFrames-1));
                timer = 1/(float)motion.fps;
            }
            else
            {
                if (playing)
                {
                    timer -= Time.deltaTime;
                }
            }
            currentFrame = Mathf.RoundToInt(frameSlider.value * (totalFrames - 1));
            Display(motion.motion[currentFrame]);
            yield return null;
        }

        frameSlider.gameObject.SetActive(false);
        playPauseButton.gameObject.SetActive(false);
        replaying = false;
    }

    public void OnBeginDisplay()
    {
        body.enabled = true;
        leftLeg.enabled = true;
        rightLeg.enabled = true;
        leftArm.enabled = true;
        rightArm.enabled = true;
    }

    public void OnStopDisplay()
    {
        body.enabled = false;
        leftLeg.enabled = false;
        rightLeg.enabled = false;
        leftArm.enabled = false;
        rightArm.enabled = false;
    }

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

        if (record)
        {
            SkeletonTracker.Instance.StoreFrame(joints);
        }

        if (display)
        {
            if(body.enabled == false)
            {
                OnBeginDisplay();
            }

            Display(joints);
        }
        else
        {
            if(body.enabled == true)
            {
                OnStopDisplay();
            }
        }
        

        /**
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
        ConnectJoints(joints[0], joints[22]);*/

        return jointConnections;

        void ConnectJoints(Joint jointA, Joint jointB)
        {
            jointConnections.Add(new JointConnection { posA = jointA.Position.ToUnityVector3(), posB = jointB.Position.ToUnityVector3() });
        }
    }

    private void Display(Joint[] joints)
    {
        //body
        body.Assign(0, joints[(int)JointId.Pelvis]);
        body.Assign(1, joints[(int)JointId.SpineNavel]);
        body.Assign(2, joints[(int)JointId.SpineChest]);
        body.Assign(3, joints[(int)JointId.Neck]);
        body.Assign(4, joints[(int)JointId.Head]);

        //left arm
        leftArm.Assign(0, joints[(int)JointId.SpineChest]);
        leftArm.Assign(1, joints[(int)JointId.ClavicleLeft]);
        leftArm.Assign(2, joints[(int)JointId.ShoulderLeft]);
        leftArm.Assign(3, joints[(int)JointId.ElbowLeft]);
        leftArm.Assign(4, joints[(int)JointId.WristLeft]);
        leftArm.Assign(5, joints[(int)JointId.HandLeft]);
        leftArm.Assign(6, joints[(int)JointId.HandTipLeft]);

        //left arm
        rightArm.Assign(0, joints[(int)JointId.SpineChest]);
        rightArm.Assign(1, joints[(int)JointId.ClavicleRight]);
        rightArm.Assign(2, joints[(int)JointId.ShoulderRight]);
        rightArm.Assign(3, joints[(int)JointId.ElbowRight]);
        rightArm.Assign(4, joints[(int)JointId.WristRight]);
        rightArm.Assign(5, joints[(int)JointId.HandRight]);
        rightArm.Assign(6, joints[(int)JointId.HandTipRight]);

        leftLeg.Assign(0, joints[(int)JointId.Pelvis]);
        leftLeg.Assign(1, joints[(int)JointId.HipLeft]);
        leftLeg.Assign(2, joints[(int)JointId.KneeLeft]);
        leftLeg.Assign(3, joints[(int)JointId.AnkleLeft]);
        leftLeg.Assign(4, joints[(int)JointId.FootLeft]);

        rightLeg.Assign(0, joints[(int)JointId.Pelvis]);
        rightLeg.Assign(1, joints[(int)JointId.HipRight]);
        rightLeg.Assign(2, joints[(int)JointId.KneeRight]);
        rightLeg.Assign(3, joints[(int)JointId.AnkleRight]);
        rightLeg.Assign(4, joints[(int)JointId.FootRight]);
    }

    public void ComparePoses(Joint[] originalPose, Joint[] comparePose)
    {
        Vector3 posDifferenceSum = Vector3.zero;

        Vector3 originalPelvisPosition = originalPose[(int)JointId.Pelvis].Position.ToUnityVector3();
        Vector3 comparePelvisPosition = comparePose[(int)JointId.Pelvis].Position.ToUnityVector3();

        //handle unequal length arrays
        if (originalPose.Length != comparePose.Length)
        {
            Debug.Log("poses had different amount of joints");
            return;
        }

        int comparedJointsCount = 0;

        for (int i = 1; i < originalPose.Length; i++)
        {
            Joint pI = originalPose[i];

            //don't compare obstructed or out of range joints
            if(pI.ConfidenceLevel == JointConfidenceLevel.Low ||pI.ConfidenceLevel == JointConfidenceLevel.None)
            {
                continue;
            }

            //get the position of the joint in relation to the pelvis
            Vector3 relativeOriginalPosition = originalPose[i].Position.ToUnityVector3() - originalPelvisPosition;
            Vector3 relativeComparePosition = comparePose[i].Position.ToUnityVector3() - comparePelvisPosition;

            posDifferenceSum += relativeComparePosition - relativeOriginalPosition;
            comparedJointsCount++;
        }

        //average positional difference in mm
        Vector3 posDifference = posDifferenceSum /= comparedJointsCount;
        Debug.Log("Positional difference in mm: " + posDifference);
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