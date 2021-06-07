using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;

public struct MemoryCard 
{
    public int x;
    public int y;
    public Motion pose;
    public GameObject uiElement; 
}
