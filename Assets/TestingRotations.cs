using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;

public class TestingRotations : MonoBehaviour
{
    [SerializeField]
    Transform root, t, t2, t3, t4;

    UJoint[] joints;

    public Transform
        pelvis, hipSupportLeft, hipSupportRight,
        hipLeft, kneeLeft, ankleLeft, footLeft,
        hipRight, kneeRight, ankleRight, footRight,
        spineNaval, spinechest, neck, head,
        clavicleSupportLeft, clavicleLeft, shoulderLeft, elbowLeft, wristLeft,
        clavicleSupportRight, clavicleRight, shoulderRight, elbowRight, wristRight;

    /*
    void Awake()
    {
        ResolveRig();
        LoadSampleMotion();
        RotateArms();
    }*/

    void ResolveRig()
    {
        pelvis = root.GetChild(0);
        hipSupportLeft = pelvis.GetChild(0);
        hipSupportRight = pelvis.GetChild(1);

        hipLeft = hipSupportLeft.GetChild(0);
        kneeLeft = hipLeft.GetChild(0);
        ankleLeft = kneeLeft.GetChild(0);
        footLeft = ankleLeft.GetChild(0);

        hipRight = hipSupportRight.GetChild(0);
        kneeRight = hipRight.GetChild(0);
        ankleRight = kneeRight.GetChild(0);
        footRight = ankleRight.GetChild(0);

        spineNaval = pelvis.GetChild(2);
        spinechest = spineNaval.GetChild(0);
        clavicleSupportLeft = spinechest.GetChild(0);
        neck = spinechest.GetChild(1);
        clavicleSupportRight = spinechest.GetChild(2);
        head = neck.GetChild(0);

        clavicleLeft = clavicleSupportLeft.GetChild(0);
        shoulderLeft = clavicleLeft.GetChild(0);
        elbowLeft = shoulderLeft.GetChild(0);
        wristLeft = elbowLeft.GetChild(0);

        clavicleRight = clavicleSupportRight.GetChild(0);
        shoulderRight = clavicleRight.GetChild(0);
        elbowRight = shoulderRight.GetChild(0);
        wristRight = elbowRight.GetChild(0);
    }

    void LoadSampleMotion()
    {
        Motion m = MotionManager.Instance.Load("w");
        joints = m.motion[0];
    }

    void RotateArms()
    {
        SetLocalArm(shoulderRight, JointId.ShoulderRight);
        SetLocalArm(elbowRight, JointId.ElbowRight);

        SetLocalArm(shoulderLeft, JointId.ShoulderLeft);
        SetLocalArm(elbowLeft, JointId.ElbowLeft);

        t2.rotation = joints[(int)JointId.ShoulderLeft].Rotation;

        void SetLocalArm(Transform t, JointId j)
        {
            t.rotation = joints[(int)j].Rotation;
            //t.RotateAround(t.position, t.right, -90);
        }
    }
}
