using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Azure.Kinect.BodyTracking;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;
using Vector3 = UnityEngine.Vector3;
using TMPro;
using Unity.Mathematics;
using static Microsoft.Azure.Kinect.BodyTracking.JointId;

public class BodyDisplay : Singleton<BodyDisplay>
{
    public bool tracking = false, replaying = false;
    public DisplayOption display = DisplayOption.TRACKED;

    [SerializeField] 
    LineRenderer body = default, leftLeg = default, rightLeg = default, leftArm = default, rightArm = default;

    [SerializeField]
    bool useHumanoid = true;

    [SerializeField]
    Transform rootBone;

    GameObject bodyParentGO;

    public Skeleton trackedBody;

    [HideInInspector]
    public int frame = 0, comparePercentage = -1;

    float currentTimeStep;
    const float FPS_30_DELTA = 0.033333f;

    Slider frameSlider;
    TextMeshProUGUI compareAccuracy;
    TMP_InputField smoothingFrames;

    bool playing = true, bodyCompareRunning = false;
    Motion loadedMotion;
    int replayFrame, interpolationFrames = 6; //0.267 sec

    UJoint[] trackedJoints;

    void Awake()
    {
        currentTimeStep = FPS_30_DELTA;
        OnValidate();
    }

    void OnValidate()
    {

        

        if (useHumanoid)
        {
            rootBone.gameObject.SetActive(true);

            if (bodyParentGO.activeInHierarchy)
                bodyParentGO.SetActive(false);

        }
        else
        {
            if (body != null)
                bodyParentGO = body.transform.parent.gameObject;
            else
                return;

            /*
                     if (display == DisplayOption.IGNORE || display == DisplayOption.NONE)
            return;
             */
            bodyParentGO.SetActive(true);

            if (rootBone != null && rootBone.gameObject.activeInHierarchy)
                rootBone.gameObject.SetActive(false);
        }
    }

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
                display = DisplayOption.TRACKED;
                break;
            case 1:
                replayFrame = 0;
                display = DisplayOption.LOADED;
                break;
            case 2:
                display = DisplayOption.NONE;
                break;
        }
    }

    private void Update()
    {
        if (currentTimeStep <= 0f)
        {
            if (AppState.bodyTrackingRunning)
                trackedJoints = ResolveSkeleton();

            switch (display)
            {
                case DisplayOption.TRACKED:
                    if (AppState.bodyTrackingRunning)
                    {
                        if (!bodyParentGO.activeInHierarchy)
                        {
                            OnBeginDisplay();
                        }
                        Display(trackedJoints);
                    }
                    break;
                case DisplayOption.LOADED:
                    loadedMotion = MotionManager.Instance.loadedMotion;
                    if (loadedMotion != null && loadedMotion.motion != null)
                    {
                        if (!useHumanoid)
                        {
                            if (!bodyParentGO.activeInHierarchy)
                            {
                                OnBeginDisplay();
                            }
                            DisplayLoaded();
                        }
                        else
                        {
                            if (!rootBone.gameObject.activeInHierarchy)
                            {
                                rootBone.gameObject.SetActive(true);
                            }
                            ResolveRotations(loadedMotion.motion[0]);
                        }


                    }
                    break;
                case DisplayOption.NONE:
                    if (bodyParentGO != null && bodyParentGO.activeInHierarchy)
                        OnStopDisplay();
                    break;

                default:
                    break;
            }
            currentTimeStep = FPS_30_DELTA;
        }
        else
            currentTimeStep -= Time.deltaTime;
    }

    public void DisplayLoaded()
    {
        if (loadedMotion.motion.Count == 1)
        {
            Display(loadedMotion.motion[0]);
            return;
        }

        replayFrame = Mathf.RoundToInt(frameSlider.value * (loadedMotion.motion.Count));
        if (playing)
        {
            replayFrame = (replayFrame + 1) % loadedMotion.motion.Count;
            frameSlider.value = replayFrame / (float)loadedMotion.motion.Count;
        }
        else if (replayFrame == loadedMotion.motion.Count)
                replayFrame--;

        Display(GetInterpolatedValue(loadedMotion.motion, replayFrame));
    }

    public Vector3[] GetInterpolatedValue(List<UJoint[]> motion, int currentFrame)
    {
        string x = smoothingFrames.text;
        interpolationFrames = int.TryParse(x, out int f) ? f : 1;

        UJoint[] joints = motion[currentFrame];
        Vector3[] interpolatedFrames = new Vector3[joints.Length];

        for (int i = 0; i < joints.Length; i++)
        {
            Vector3 joint = Vector3.zero;
            float divisor = 0;
            float p = interpolationFrames + 1;
            for (int j = 0; j <= interpolationFrames; j++, p /= 2f)
            {
                if (currentFrame - j < 0)
                    break;

                divisor += p;
                joint += motion[currentFrame - j][i].Position * p;
            }
            interpolatedFrames[i] = joint;
        }

        return interpolatedFrames;
    }

    public Quaternion[] GetRotations(Joint[] joints)
    {
        Quaternion[] rotationFrame = new Quaternion[joints.Length];

        for (int i = 0; i < joints.Length; i++)
            rotationFrame[i] = joints[i].Quaternion.ToUnityQuaternion();

        return rotationFrame;
    }

    
    public Vector3[] GetPositions() => GetPositions(trackedJoints);

    /// <summary>
    /// Get the adjusted positions vector from a given list of joints
    /// </summary>
    /// <param name="joints"></param>
    /// <returns></returns>
    public Vector3[] GetPositions(UJoint[] joints)
    {
        Vector3[] positions = new Vector3[joints.Length];

        for (int i = 0; i < joints.Length; i++)
            positions[i] = joints[i].Position;

        return positions;
    }

    public List<Vector3> GetConfidentPositions(UJoint[] joints)
    {
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < joints.Length; i++)
        {
            UJoint j = joints[i];
            if (j.Confidence == JointConfidenceLevel.Low || j.Confidence == JointConfidenceLevel.None)
                continue;

            positions.Add(j.Position);
        }

        return positions;
    }

    public void OnBeginDisplay() {
        if (useHumanoid)
        {
            
        }
        else
           bodyParentGO.SetActive(true);
    }


    
    public void OnStopDisplay() {
        if (useHumanoid)
        {

        }
        else
        {
            bodyParentGO.SetActive(false);
        }
    } 
    
    /// <summary>
    /// Get all joints with their original position and rotation
    /// </summary>
    /// <returns></returns>
    public UJoint[] ResolveSkeleton()
    {
        //Debug.Log("Resolving Skeleton");
        //get all joint positions
        UJoint[] joints = new UJoint[27];

        for (int i = 0; i < 27; i++) {
            Joint j = trackedBody.GetJoint(i);
            UJoint unityJoint = new UJoint(j);
            joints[i] = unityJoint;
        }

        if (AppState.recording)
            MotionManager.Instance.StoreFrame(joints);

        return joints;
    }


    public void RunBodyCompare()
    {
        if (bodyCompareRunning)
            bodyCompareRunning = false;

        else
            StartCoroutine(BodyCompareCoroutine());
    }

    public IEnumerator BodyCompareCoroutine()
    {
        float timer = 0f;
        while (AppState.bodyCompareRunning)
        {
            if (timer <= 0f)
            {
                int percent = ComparePoses(trackedJoints, MotionManager.Instance.loadedMotion.motion[0]);
                compareAccuracy.text = "Accuracy: " + percent + " %";

                timer = FPS_30_DELTA;
            }
            else
                timer -= Time.deltaTime;

            yield return null;
        }
        compareAccuracy.text = "";
    }

    public IEnumerator BodyCompareCoroutine(UJoint[] pose, float compareTime)
    {
        float timer = 0f;
        comparePercentage = 0;
        while (compareTime > 0f)
        {
            compareTime -= Time.deltaTime;
            if (timer <= 0f)
            {
                comparePercentage = ComparePoses(trackedJoints, pose);
                timer = FPS_30_DELTA;
            }
            else
            {
                timer -= Time.deltaTime;
            }

            yield return null;
        }
    }

    public Vector3 GetBodyPosition() =>
            GetBodyPosition(trackedJoints);

    /// <summary>
    /// Return the pelvis position in adjusted scale, not accounting for y position. 
    /// </summary>
    /// <param name="joints"></param>
    /// <returns></returns>
    public Vector3 GetBodyPosition(UJoint[] joints) => 
        joints[(int)Pelvis].Position;

    public bool GetBodyBoundingBox(out Bounds b) =>
        GetBodyBoundingBox(trackedJoints, out b);

    public bool GetBodyBoundingBox(UJoint[] joints, out Bounds b)
    {
        b = new Bounds();

        if(joints == null)
        {
            Debug.LogWarning("joint was null. Can't create bounding box");
            return false;
        }

        if (joints.Length < 22)
        {
            Debug.LogWarning("Not enough joint information to compose bounding box");
            return false;
        }

        List<Vector3> confidentPositions = GetConfidentPositions(joints);

        if(confidentPositions.Count == 0)
        {
            Debug.LogWarning("Not enough joint information to compose bounding box");
            return false;
        }

        b = confidentPositions.GetBoundingBox();
        return true;
    }

    static JointId[] bodyJoints = new JointId[]
    {
        Pelvis,
        SpineNavel,
        SpineChest,
        Neck,
        Head,
        ClavicleLeft,
        ShoulderLeft,
        ClavicleRight,
        ShoulderRight,
        HipLeft,
        HipRight,

    };


    public void Display(Vector3[] joints)
    {
        SetPosition(body, 0, Pelvis);
        SetPosition(body, 1, SpineNavel);
        SetPosition(body, 2, SpineChest);
        SetPosition(body, 3, Neck);
        SetPosition(body, 4, Head);

        SetPosition(leftArm, 0, SpineChest);
        SetPosition(leftArm, 1, ClavicleLeft);
        SetPosition(leftArm, 2, ShoulderLeft);
        SetPosition(leftArm, 3, ElbowLeft);
        SetPosition(leftArm, 4, WristLeft);
        SetPosition(leftArm, 5, HandLeft);
        SetPosition(leftArm, 6, HandTipLeft);

        SetPosition(rightArm, 0, SpineChest);
        SetPosition(rightArm, 1, ClavicleRight);
        SetPosition(rightArm, 2, ShoulderRight);
        SetPosition(rightArm, 3, ElbowRight);
        SetPosition(rightArm, 4, WristRight);
        SetPosition(rightArm, 5, HandRight);
        SetPosition(rightArm, 6, HandTipRight);

        SetPosition(leftLeg, 0, Pelvis);
        SetPosition(leftLeg, 1, HipLeft);
        SetPosition(leftLeg, 2, KneeLeft);
        SetPosition(leftLeg, 3, AnkleLeft);
        SetPosition(leftLeg, 4, FootLeft);

        SetPosition(rightLeg, 0, Pelvis);
        SetPosition(rightLeg, 1, HipRight);
        SetPosition(rightLeg, 2, KneeRight);
        SetPosition(rightLeg, 3, AnkleRight);
        SetPosition(rightLeg, 4, FootRight);

        void SetPosition(LineRenderer l, int index, JointId id) =>
            l.SetPosition(index, joints[(int)id]);
    }

    public void Display(UJoint[] joints)
    {
        SetPosition(body, 0, Pelvis);
        SetPosition(body, 1, SpineNavel);
        SetPosition(body, 2, SpineChest);
        SetPosition(body, 3, Neck);
        SetPosition(body, 4, Head);

        SetPosition(leftArm, 0, SpineChest);
        SetPosition(leftArm, 1, ClavicleLeft);
        SetPosition(leftArm, 2, ShoulderLeft);
        SetPosition(leftArm, 3, ElbowLeft);
        SetPosition(leftArm, 4, WristLeft);
        SetPosition(leftArm, 5, HandLeft);
        SetPosition(leftArm, 6, HandTipLeft);

        SetPosition(rightArm, 0, SpineChest);
        SetPosition(rightArm, 1, ClavicleRight);
        SetPosition(rightArm, 2, ShoulderRight);
        SetPosition(rightArm, 3, ElbowRight);
        SetPosition(rightArm, 4, WristRight);
        SetPosition(rightArm, 5, HandRight);
        SetPosition(rightArm, 6, HandTipRight);

        SetPosition(leftLeg, 0, Pelvis);
        SetPosition(leftLeg, 1, HipLeft);
        SetPosition(leftLeg, 2, KneeLeft);
        SetPosition(leftLeg, 3, AnkleLeft);
        SetPosition(leftLeg, 4, FootLeft);

        SetPosition(rightLeg, 0, Pelvis);
        SetPosition(rightLeg, 1, HipRight);
        SetPosition(rightLeg, 2, KneeRight);
        SetPosition(rightLeg, 3, AnkleRight);
        SetPosition(rightLeg, 4, FootRight);

        void SetPosition(LineRenderer l, int index, JointId id) => 
            l.SetPosition(index, joints[(int)id].Position);
    }

    public struct PositionCompare
    {
        public Vector3 posDiff;
        public Compare[] comp; 
    }

    public static readonly PositionCompare handRaisedCompare = new PositionCompare
    {
        posDiff = new Vector3(0,-0.1f,0),
        comp = new Compare[] { Compare.NONE, Compare.POS, Compare.NONE }
    };

    public enum Compare
    {
        POS,
        NEG,
        NONE
    }

    public bool JointCompare(JointId a, JointId b, PositionCompare c) => 
        JointCompare(trackedJoints, a, b, c);

    /// <summary>
    /// Compares two joints based on a comparision function
    /// </summary>
    /// <param name="a">lhs</param>
    /// <param name="b">rhs</param>
    /// <param name="c">Comparision Constrains</param>
    /// <returns></returns>
    public bool JointCompare(UJoint[] joints, JointId a, JointId b, PositionCompare c)
    {
        if(joints == null)
        {
            Debug.LogError("joints was null");
            return false;
        }

        Vector3 posA = joints[(int)a].Position;
        Vector3 posB = joints[(int)b].Position;

        Vector3 posDiff = c.posDiff;
        bool3 xyz = new bool3(true);

        for (int i = 0; i < c.comp.Length; i++)
        {
            switch (c.comp[i])
            {
                case Compare.NONE:
                    break;
                case Compare.POS:
                    xyz[i] = posB[i] - posDiff[i] > posA[i];
                    break;
                case Compare.NEG:
                    xyz[i] = posB[i] - posDiff[i] < posA[i];
                    break;
            }
        }

        return xyz.x && xyz.y && xyz.z;
    }


    /// <summary>
    /// Determine how much alike two poses are to one another. Return the result in percent
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public int ComparePoses(UJoint[] lhs, UJoint[] rhs)
    {
        if(lhs == null || rhs == null)
            return -1;

        Vector3 posDifferenceSum = Vector3.zero;

        Vector3 originalPelvisPosition = lhs[(int)Pelvis].Position;
        Vector3 comparePelvisPosition = rhs[(int)Pelvis].Position;

        //handle unequal length arrays
        if (lhs.Length != rhs.Length)
        {
            Debug.Log("poses had different amount of joints");
            return -1;
        }

        int comparedJointsCount = 0;

        for (int i = 1; i < lhs.Length; i++)
        {
            UJoint pI = lhs[i];

            //don't compare obstructed or out of range joints
            if(pI.Confidence == JointConfidenceLevel.Low || pI.Confidence == JointConfidenceLevel.None)
                continue;

            //get the position of the joint in relation to the pelvis
            Vector3 relativeOriginalPosition = lhs[i].Position - originalPelvisPosition;
            Vector3 relativeComparePosition = rhs[i].Position - comparePelvisPosition;

            //take absolute values?
            Vector3 difference = relativeComparePosition - relativeOriginalPosition;
            posDifferenceSum += difference.Square();
            comparedJointsCount++;
        }

        if(comparedJointsCount == 0)
            Debug.LogWarning("All joints had JointConfidenceLevel.Low or .None");

        //average positional difference in mm
        Vector3 posDifference = posDifferenceSum /= comparedJointsCount;

        float posDifferenceD = Mathf.Sqrt(posDifference.x * posDifference.x + posDifference.y * posDifference.y + posDifference.z * posDifference.z);

        if (posDifferenceD == float.NaN)
            return 0;

        int percent;
        float zero = 0.5f;
        float hund = 0.01f;
        if (posDifferenceD > zero)
            percent = 0;
        else if (posDifferenceD < hund)
            percent = 100;
        else
            percent = 100 - Mathf.RoundToInt((posDifferenceD-hund) / (zero*0.01f));

        /*
        //20000 = 0% |100 = 100%
        if(posDifferenceD > 80)
            percent = 0;

        else if(posDifferenceD < 0.2)
            percent = 100;

        else
            //1% = 348
            percent = 100 - Mathf.RoundToInt((posDifferenceD- 0.200f) / 0.798f);
        */
        //Debug.Log("Positional difference in mm: " + posDifferenceD);
        return percent;
    }

    public void ResolveRotations(UJoint[] jointPositions)
    {
        //get all the transforms from the hierarchy

        Transform
            pelvis = rootBone.GetChild(0),
            hipLeft = pelvis.GetChild(0),
            hipRight = pelvis.GetChild(1),
            spineNaval = pelvis.GetChild(2),
            legLeft = hipLeft.GetChild(0),
            kneeLeft = legLeft.GetChild(0),
            ankleLeft = kneeLeft.GetChild(0),
            footLeft = ankleLeft.GetChild(0),
            legRight = hipRight.GetChild(0),
            kneeRight = legRight.GetChild(0),
            ankleRight = kneeRight.GetChild(0),
            footRight = ankleRight.GetChild(0),
            clavicleLeft = spineNaval.GetChild(0),
            clavicleRight = spineNaval.GetChild(1),
            spinechest = spineNaval.GetChild(2),
            neck = spinechest.GetChild(0),
            head = neck.GetChild(0),
            shoulderLeft = clavicleLeft.GetChild(0),
            shoulderRight = clavicleRight.GetChild(0),
            armLeft = shoulderLeft.GetChild(0),
            armRight = shoulderRight.GetChild(0),
            elbowLeft = armLeft.GetChild(0),
            elbowRight = armRight.GetChild(0),
            wristLeft = elbowLeft.GetChild(0),
            wristRight = elbowRight.GetChild(0);

        pelvis.position = jointPositions[(int)Pelvis].Position;
        hipLeft.position = jointPositions[(int)HipLeft].Position;
        hipRight.position = jointPositions[(int)HipRight].Position;

        SetLocal(legLeft, KneeLeft, HipLeft);
        SetLocal(kneeLeft, AnkleLeft, KneeLeft);
        SetLocal(ankleLeft, FootLeft, AnkleLeft);

        SetLocal(legRight, KneeRight, HipRight);
        SetLocal(kneeRight, AnkleRight, KneeRight);
        SetLocal(ankleRight, FootRight, AnkleRight);

        SetLocal(spineNaval, SpineChest, SpineNavel);
        SetLocal(spinechest, Neck, SpineChest);
        SetLocal(neck, Head, Neck);

        SetLocal(clavicleLeft, ShoulderLeft, ClavicleLeft);
        SetLocal(shoulderLeft, ElbowLeft, ShoulderLeft);
        SetLocal(elbowLeft, WristLeft, ElbowLeft);

        SetLocal(clavicleRight, ShoulderRight, ClavicleRight);
        SetLocal(shoulderRight, ElbowRight, ShoulderRight);
        SetLocal(elbowRight, WristRight, ElbowRight); 

        void SetLocal(Transform t, JointId c, JointId p)
        {
            Vector3 cPos = jointPositions[(int)c].Position;
            Vector3 pPos = jointPositions[(int)p].Position;
            Vector3 newUp = (cPos - pPos).normalized;
            Vector3 newForward = newUp.GetPerpendicular();
            Vector3 originalForward = t.forward;

            t.rotation = Quaternion.LookRotation(newForward, newUp);

            int bestAngle = 0;
            float bestDot = -1;
            for (int i = 0; i < 360; i += 15)
            {
                t.Rotate(newUp, i);
                float dot = Vector3.Dot(originalForward, t.forward);
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestAngle = i;
                }
                t.Rotate(newUp, -i);
            }
            t.Rotate(newUp, bestAngle);
        }

        /*
        void SetForward(Transform t)
        {
            Vector3 pRight = t.parent.right;

            //possible vectors ortogonal to the up vector
            Vector3[] candidates = new Vector3[]
            {
                t.forward,
                -t.forward,
                -t.right,
                (t.forward + t.right).normalized,
                (t.forward - t.right).normalized,
                (-t.forward + t.right).normalized,
                (-t.forward - t.right).normalized
            };
            Vector3 bestCandidate = t.right;
            float bestDot = Vector3.Dot(t.right, pRight);

            for (int i = 0; i < candidates.Length; i++)
            {
                float dot = Vector3.Dot(candidates[i], pRight);
                if(dot > bestDot)
                {
                    bestDot = dot;
                    bestCandidate = candidates[i];
                }
            }
            Debug.Log("Best dot: " + bestDot);

            Vector3 newForward = Vector3.Cross(bestCandidate, t.up);
            if(Vector3.Dot(newForward, t.parent.forward) < 0)
            {
                Debug.Log("?");
                newForward = Vector3.Cross(bestCandidate, t.up);
            }
            float angle = Vector3.Angle(newForward, t.forward);
            t.Rotate(transform.up, angle, Space.Self);
            //t.rotation =  Quaternion.LookRotation(newForward);
        }
        */
    }
}

public enum DisplayOption
{
    TRACKED,
    LOADED,
    NONE,
    IGNORE
}

/// <summary>
/// A joint using Unitys data structures and a different scale
/// </summary>
[System.Serializable]
public struct UJoint
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public JointConfidenceLevel Confidence { get; set; }

    public UJoint(Joint joint)
    {
        Vector3 kinectSpacePosition = joint.Position.ToUnityVector3();
        Position = AdjustScale(kinectSpacePosition);
        Rotation = joint.Quaternion.ToUnityQuaternion();
        Confidence = joint.ConfidenceLevel;
    }

    public static Vector3 AdjustScale(Vector3 v3)
    {
        Vector3 n_v3 = new Vector3(v3.x, -v3.y, v3.z);
        n_v3 *= 0.001f;
        return n_v3;
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



/**
 * D = XD,YD,ZD
 * U = 0,1,0
 */