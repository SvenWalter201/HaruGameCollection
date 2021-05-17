using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;

[Serializable]
public class Motion
{
    public int fps;
    public List<Joint[]> motion;
}

[Serializable]
public class Pose
{
    public string name;
    public Joint[] pose;
}
