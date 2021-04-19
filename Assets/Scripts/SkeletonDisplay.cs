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
    public bool tracking = false;
    public bool record = false;
    public DisplayOption display = DisplayOption.TRACKED;
    public bool replaying = false;

    [SerializeField] private LineRenderer body = default;
    [SerializeField] private LineRenderer leftLeg = default;
    [SerializeField] private LineRenderer rightLeg = default;
    [SerializeField] private LineRenderer leftArm = default;
    [SerializeField] private LineRenderer rightArm = default;

    private GameObject bodyParentGO;

    public Skeleton trackedBody;
    public int frame = 0;
    private const float FPS_30_DELTA = 0.033f;
    private float currentTimeStep;

    private void Start()
    {
        currentTimeStep = FPS_30_DELTA;
        bodyParentGO = body.transform.parent.gameObject;
    }

    private Slider frameSlider;
    //private Button playPauseButton;

    private bool playing = true;
    private Motion motion;
    private int replayFrame;
    private Joint[] joints;

    public void InitUIComponents(Slider frameSlider, Button playPauseButton)
    {
        playPauseButton.onClick.AddListener(() => playing = !playing);

        this.frameSlider = frameSlider;
        //this.playPauseButton = playPauseButton;

    }

    public void SwitchDisplayType(int option)
    {
        switch (option)
        {
            case 0:
                {
                    display = DisplayOption.TRACKED;
                    break;
                }
            case 1:
                {
                    replayFrame = 0;
                    display = DisplayOption.LOADED;
                    break;
                }
            case 2:
                {
                    display = DisplayOption.NONE;
                    break;
                }
        }
    }

    private void Update()
    {
        if (currentTimeStep <= 0f)
        {
            if (KinectDeviceManager.Instance.bodyTracking)
            {
                joints = ResolveSkeleton();
            }

            switch (display)
            {
                case DisplayOption.TRACKED:
                    {
                        if (KinectDeviceManager.Instance.bodyTracking)
                        {
                            if (!bodyParentGO.activeInHierarchy)
                            {
                                OnBeginDisplay();
                            }
                            Display(joints);
                        }
                        break;
                    }
                case DisplayOption.LOADED:
                    {

                        motion = SkeletonTracker.Instance.loadedMotion;
                        if(motion != null)
                        {
                            if (!bodyParentGO.activeInHierarchy)
                            {
                                OnBeginDisplay();
                            }
                            DisplayLoaded();
                        }
                        break;
                    }
                case DisplayOption.NONE:
                    {
                        if (bodyParentGO.activeInHierarchy)
                        {
                            OnStopDisplay();
                        }
                        break;
                    }
            }
            currentTimeStep = FPS_30_DELTA;
        }
        else
        {
            currentTimeStep -= Time.deltaTime;
        }
    }

    public void DisplayLoaded()
    {
        if(motion.motion.Count == 1)
        {
            Display(motion.motion[0]);
            return;
        }

        replayFrame = Mathf.RoundToInt(frameSlider.value * (motion.motion.Count));
        if (playing)
        {
            replayFrame = (replayFrame + 1) % motion.motion.Count;
            frameSlider.value = replayFrame / (float)motion.motion.Count;
        }
        else
        {
            if(replayFrame == motion.motion.Count)
            {
                replayFrame--;
            }
        }
        Display(motion.motion[replayFrame]);
    }

    public void OnBeginDisplay()
    {
        bodyParentGO.SetActive(true);
    }

    public void OnStopDisplay()
    {
        bodyParentGO.SetActive(false);
    }

    public Joint[] ResolveSkeleton()
    {
        //get all joint positions
        Joint[] joints = new Joint[27];

        for (int i = 0; i < 27; i++)
        {
            //Joint joint = skeleton.GetJoint(i);
            joints[i] = trackedBody.GetJoint(i);
        }

        if (record)
        {
            SkeletonTracker.Instance.StoreFrame(joints);
        }
        return joints;
    }

    public void CompareTrackedWithLoaded()
    {
        ComparePoses(joints, SkeletonTracker.Instance.loadedMotion.motion[0]);
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

public enum DisplayOption
{
    TRACKED,
    LOADED,
    NONE
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