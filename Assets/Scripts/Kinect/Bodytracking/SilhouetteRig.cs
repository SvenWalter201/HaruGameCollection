using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using static Microsoft.Azure.Kinect.BodyTracking.JointId;

public class SilhouetteRig : MonoBehaviour
{
    Transform
        pelvis, 
        hipLeft, kneeLeft, ankleLeft, 
        hipRight, kneeRight, ankleRight, 
        spineNaval, spinechest, neck, head,
        shoulderLeft, elbowLeft, wristLeft,
        shoulderRight, elbowRight, wristRight;

    void Awake()
    {
        ResolveRig();
        //SampleResolve();
    }

    void ResolveRig()
    {
        pelvis = transform.GetChild(0);
        spineNaval = pelvis.GetChild(0);
        spinechest = spineNaval.GetChild(0);
        shoulderRight = spinechest.GetChild(0);
        shoulderLeft = spinechest.GetChild(1);
        neck = spinechest.GetChild(2);
        elbowRight = shoulderRight.GetChild(0);
        wristRight = elbowRight.GetChild(0);
        elbowLeft = shoulderLeft.GetChild(0);
        wristLeft = elbowLeft.GetChild(0);
        head = neck.GetChild(0);
        hipRight = pelvis.GetChild(1);
        hipLeft = pelvis.GetChild(2);
        kneeRight = hipRight.GetChild(0);
        kneeLeft = hipLeft.GetChild(0);
        ankleRight = kneeRight.GetChild(0);
        ankleLeft = kneeLeft.GetChild(0);
    }

    public void Resolve(Vector3[] positions)
    {
        pelvis.position = positions[(int)Pelvis];

        Rotate(pelvis, Pelvis, SpineNavel);
        Rotate(spineNaval, SpineNavel, SpineChest);
        Rotate(spinechest, SpineChest, Neck);
        Rotate(neck, Neck, Head);

        Rotate(shoulderRight, ShoulderRight, ElbowRight);
        Rotate(elbowRight, ElbowRight, WristRight);
        Rotate(wristRight, WristRight, HandRight);

        Rotate(shoulderLeft, ShoulderLeft, ElbowLeft);
        Rotate(elbowLeft, ElbowLeft, WristLeft);
        Rotate(wristLeft, WristLeft, HandLeft);

        Rotate(hipRight, HipRight, KneeRight);
        Rotate(kneeRight, KneeRight, AnkleRight);
        Rotate(ankleRight, AnkleRight, FootRight);

        Rotate(hipLeft, HipLeft, KneeLeft);
        Rotate(kneeLeft, KneeLeft, AnkleLeft);
        Rotate(ankleLeft, AnkleLeft, FootLeft);

        void Rotate(Transform parentTransform, JointId parent, JointId child)
        {
            Vector3 pp = positions[(int)parent];
            Vector3 cp = positions[(int)child];

            Vector2 dir = pp - cp;
            dir.Normalize();

            float rotationAngle = Vector2.SignedAngle(-Vector3.right, dir);
            parentTransform.rotation = Quaternion.Euler(0, 0, rotationAngle);
        }
    }

    /*
    void SampleResolve()
    {
        //Sample Arm Rotation
        Resolve(shoulderRight, new Vector3(0, 0, 0), new Vector3(1,1,0));
        Resolve(elbowRight, new Vector3(1, 1, 0), new Vector3(1,2,0));
        Resolve(shoulderLeft, new Vector3(0, 0, 0), new Vector3(-1,-1,0));
        Resolve(elbowLeft, new Vector3(-1, -1, 0), new Vector3(-2,-1,0));

        //Sample Leg Rotation
        Resolve(hipRight, new Vector3(0, 0, 0), new Vector3(1, -1, 0));
        Resolve(kneeRight, new Vector3(1, -1, 0), new Vector3(1, -2, 0));
        Resolve(hipLeft, new Vector3(0, 0, 0), new Vector3(-1, -1, 0));
        Resolve(kneeLeft, new Vector3(-1, -1, 0), new Vector3(-1, -2, 0));

        void Resolve(Transform parentTransform, Vector3 parent, Vector3 child)
        {
            Vector2 dir = parent - child;
            dir.Normalize();
            float rotationAngle = Vector2.SignedAngle(-Vector3.right, dir);
            parentTransform.rotation = Quaternion.Euler(0, 0, rotationAngle);
        }
    }
    */
}


/**
 *         pelvis, hipSupportLeft, hipSupportRight,
        hipLeft, kneeLeft, ankleLeft, footLeft,
        hipRight, kneeRight, ankleRight, footRight,
        spineNaval, spinechest, neck, head,
        clavicleSupportLeft, clavicleLeft, shoulderLeft, elbowLeft, wristLeft,
        clavicleSupportRight, clavicleRight, shoulderRight, elbowRight, wristRight;
 * 
 * hip -> hip to knee
 * knee -> knee to ankle
 * foot -> ankle to foot
 * 
 * pelvis -> pelvis to spineNaval
 * spineNaval -> spineNaval to spineChest
 * spineChest -> spineChest to Neck
 * neck -> neck to head
 * head -> inhertit neck
 * 
 * shoulder -> shoulder to elbow
 * elbow -> elbow to wrist
 * wrist -> wrist to hand
 */
