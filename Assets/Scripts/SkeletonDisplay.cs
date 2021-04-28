using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.Kinect.BodyTracking;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;
using Vector3 = UnityEngine.Vector3;
using Quaternion = System.Numerics.Quaternion;
using TMPro;

public class SkeletonDisplay : Singleton<SkeletonDisplay>
{
    public bool tracking = false;
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
    private TextMeshProUGUI compareAccuracy;
    private TMP_InputField smoothingFrames;
    //private Button playPauseButton;

    private bool playing = true;
    private Motion motion;
    private int replayFrame;
    private Joint[] joints;

    public void InitUIComponents(Slider frameSlider, Button playPauseButton, TextMeshProUGUI compareAccuracy, TMP_InputField smoothingFrames)
    {
        playPauseButton.onClick.AddListener(() => playing = !playing);

        this.frameSlider = frameSlider;
        this.compareAccuracy = compareAccuracy;
        this.smoothingFrames = smoothingFrames;
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
            if (AppState.bodyTrackingRunning)
            {
                joints = ResolveSkeleton();
            }

            switch (display)
            {
                case DisplayOption.TRACKED:
                    {
                        if (AppState.bodyTrackingRunning)
                        {
                            if (!bodyParentGO.activeInHierarchy)
                            {
                                OnBeginDisplay();
                            }
                            //Display(joints);
                        }
                        break;
                    }
                case DisplayOption.LOADED:
                    {

                        motion = SkeletonTracker.Instance.loadedMotion;
                        if(motion != null && motion.motion != null)
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
            Display(GetVectors(motion.motion[0]));
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
        Display(GetInterpolatedValue(motion.motion, replayFrame)); //motion.motion[replayFrame]);
    }

    private int interpolationFrames = 6; //0.267 sec
    public Vector3[] GetInterpolatedValue(List<Joint[]> motion, int currentFrame)
    {
        string x = smoothingFrames.text;
        if(int.TryParse(x, out int f))
        {
            interpolationFrames = f;
        }
        else
        {
            interpolationFrames = 1;
        }
        Joint[] joints = motion[currentFrame];
        Vector3[] interpolatedFrames = new Vector3[joints.Length];

        for (int i = 0; i < joints.Length; i++)
        {
            Vector3 joint = Vector3.zero;
            float divisor = 0;
            float p = interpolationFrames + 1;
            for (int j = 0; j <= interpolationFrames; j++, p/=2f)
            {
                if(currentFrame-j < 0)
                {
                    break;
                }
                divisor += p;
                joint += motion[currentFrame - j][i].Position.ToUnityVector3() * p;
            }
            joint /= (divisor * -200f);
            interpolatedFrames[i] = joint;
        }

        return interpolatedFrames;
    }

    public Vector3[] GetVectors(Joint[] jointPositions)
    {
        Vector3[] positionFrame = new Vector3[jointPositions.Length];
        for (int i = 0; i < jointPositions.Length; i++)
        {
            positionFrame[i] = jointPositions[i].Position.ToUnityVector3() / -200f;

        }
        return positionFrame;

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

        if (AppState.recording)
        {
            SkeletonTracker.Instance.StoreFrame(joints);
        }
        return joints;
    }

    private bool bodyCompareRunning = false;
    public void RunBodyCompare()
    {
        if (bodyCompareRunning)
        {
            bodyCompareRunning = false;
        }
        else
        {
            StartCoroutine(BodyCompareCoroutine());
        }
    }

    public IEnumerator BodyCompareCoroutine()
    {
        float timer = 0f;
        while (AppState.bodyCompareRunning)
        {
            if(timer <= 0f)
            {
                ComparePoses(joints, SkeletonTracker.Instance.loadedMotion.motion[0]);
                timer = 0.033f;
            }
            else
            {
                timer -= Time.deltaTime;
            }

            yield return null;
        }
        compareAccuracy.text = "";
    }

    public int comparePercentage;
    public IEnumerator BodyCompareCoroutine(Joint[] pose, float compareTime)
    {
        float timer = 0f;
        comparePercentage = 0;
        while (compareTime > 0f)
        {
            compareTime -= Time.deltaTime;
            if (timer <= 0f)
            {
                comparePercentage = ComparePoses(joints, pose);
                timer = 0.033f;
            }
            else
            {
                timer -= Time.deltaTime;
            }

            yield return null;
        }
        //compareAccuracy.text = "";
    }

    public void Display(Vector3[] joints)
    {
        //body
        body.SetPosition(0, joints[(int)JointId.Pelvis]);
        body.SetPosition(1, joints[(int)JointId.SpineNavel]);
        body.SetPosition(2, joints[(int)JointId.SpineChest]);
        body.SetPosition(3, joints[(int)JointId.Neck]);
        body.SetPosition(4, joints[(int)JointId.Head]);

        //left arm
        leftArm.SetPosition(0, joints[(int)JointId.SpineChest]);
        leftArm.SetPosition(1, joints[(int)JointId.ClavicleLeft]);
        leftArm.SetPosition(2, joints[(int)JointId.ShoulderLeft]);
        leftArm.SetPosition(3, joints[(int)JointId.ElbowLeft]);
        leftArm.SetPosition(4, joints[(int)JointId.WristLeft]);
        leftArm.SetPosition(5, joints[(int)JointId.HandLeft]);
        leftArm.SetPosition(6, joints[(int)JointId.HandTipLeft]);

        //left arm
        rightArm.SetPosition(0, joints[(int)JointId.SpineChest]);
        rightArm.SetPosition(1, joints[(int)JointId.ClavicleRight]);
        rightArm.SetPosition(2, joints[(int)JointId.ShoulderRight]);
        rightArm.SetPosition(3, joints[(int)JointId.ElbowRight]);
        rightArm.SetPosition(4, joints[(int)JointId.WristRight]);
        rightArm.SetPosition(5, joints[(int)JointId.HandRight]);
        rightArm.SetPosition(6, joints[(int)JointId.HandTipRight]);

        leftLeg.SetPosition(0, joints[(int)JointId.Pelvis]);
        leftLeg.SetPosition(1, joints[(int)JointId.HipLeft]);
        leftLeg.SetPosition(2, joints[(int)JointId.KneeLeft]);
        leftLeg.SetPosition(3, joints[(int)JointId.AnkleLeft]);
        leftLeg.SetPosition(4, joints[(int)JointId.FootLeft]);

        rightLeg.SetPosition(0, joints[(int)JointId.Pelvis]);
        rightLeg.SetPosition(1, joints[(int)JointId.HipRight]);
        rightLeg.SetPosition(2, joints[(int)JointId.KneeRight]);
        rightLeg.SetPosition(3, joints[(int)JointId.AnkleRight]);
        rightLeg.SetPosition(4, joints[(int)JointId.FootRight]);
    }

    public int ComparePoses(Joint[] originalPose, Joint[] comparePose)
    {
        Vector3 posDifferenceSum = Vector3.zero;

        Vector3 originalPelvisPosition = originalPose[(int)JointId.Pelvis].Position.ToUnityVector3();
        Vector3 comparePelvisPosition = comparePose[(int)JointId.Pelvis].Position.ToUnityVector3();

        //handle unequal length arrays
        if (originalPose.Length != comparePose.Length)
        {
            Debug.Log("poses had different amount of joints");
            return -1;
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

            //take absolute values?
            Vector3 difference = relativeComparePosition - relativeOriginalPosition;
            posDifferenceSum += difference.Square();
            comparedJointsCount++;
        }

        //average positional difference in mm
        Vector3 posDifference = posDifferenceSum /= comparedJointsCount;
        float posDifferenceD = Mathf.Sqrt(posDifference.x * posDifference.x + posDifference.y + posDifference.y + posDifference.z + posDifference.z);

        int percent;
        //20000 = 0% |100 = 100%
        if(posDifferenceD > 35000)
        {
            percent = 0;
        }
        else if(posDifferenceD < 200)
        {
            percent = 100;
        }
        else
        {
            //1% = 348
            percent = 100 - Mathf.RoundToInt((posDifferenceD-200) / 348);
        }

        compareAccuracy.text = "Accuracy: " + percent + " %";
        //Debug.Log("Positional difference in mm: " + posDifference);
        return percent;
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