using System.Collections;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;

class RoomConfiguration : Game
{
    //bool useHue = false;
    //CoroutineTimer timer = new CoroutineTimer();
    [SerializeField] 
    Transform playerBounds, cornerMarker;

    const float TIMEOUT = 15f;

    void Start() => PlayGame();

    protected override IEnumerator Init()
    {
        KinectDeviceManager.Instance.BeginBodyTracking();
        return base.Init();
    }

    private void Update()
    {
        if(BodyDisplay.Instance.GetBodyBoundingBox(out Bounds b))
        {
            Vector3 bodyCenter = BodyDisplay.Instance.GetBodyPosition();
            playerBounds.position = bodyCenter;
            /*Vector3 center = b.center;
            center.y = 0;
            center.x *= -1f;
            center.z *= -1f;
            center.z += 8;
            playerBounds.position = center;
            
            Vector3 localScale = new Vector3(
                b.extents.x * 0.5f,
                1f,
                b.extents.z * 0.5f);

            playerBounds.localScale = localScale;
            */        
        }
    }

    protected override IEnumerator Execute()
    {
        yield return new WaitForSeconds(1);

        int cornersToConfigure = 3;
        Vector3[] corners = new Vector3[cornersToConfigure];
        BodyDisplay.PositionCompare hRC = BodyDisplay.handRaisedCompare;

        for (int i = 0; i < cornersToConfigure; i++)
        {
            //glow a lamp
            //show UI Text
            //leave some time for the person to raise their hand
            //save the position in the corners[]

            float remainingTime = TIMEOUT;
            while(remainingTime > 0f)
            {
                remainingTime -= Time.deltaTime;
                yield return null;

                if (BodyDisplay.Instance.JointCompare(JointId.Head, JointId.HandLeft, hRC) ||
                BodyDisplay.Instance.JointCompare(JointId.Head, JointId.HandRight, hRC)
                )
                {
                    Vector3 position = BodyDisplay.Instance.GetBodyPosition();
                    Instantiate(cornerMarker, position, Quaternion.Euler(90,0,0));
                    corners[i] = position;
                    //Debug.Log("DetectedHandRaised! Set Corner at " + position);
                    yield return new WaitForSeconds(3);
                    //set corner at position 
                    break;
                }
            }
            if(remainingTime <= 0f)
            {
                Debug.LogError("Corner Configuration timed out");
                yield break;
            }
        }

        RoomManager.Instance.CreateRoom(corners, true);
        yield break;
    }
}


