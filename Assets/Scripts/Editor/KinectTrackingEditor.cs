using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KinectTracking))]
public class KinectTrackingEditor : Editor
{
    private KinectTracking kinectTracking;
    
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        kinectTracking.showDepthImage = GUILayout.Toggle(kinectTracking.showDepthImage, "Show Depth Image");
        kinectTracking.showColorImage = GUILayout.Toggle(kinectTracking.showColorImage, "Show Color Image");

        if (GUILayout.Button("Init"))
        {
            kinectTracking.Init();
        }
        if (GUILayout.Button("Close"))
        {
            kinectTracking.Close();
        }
        if (GUILayout.Button("Start Tracking"))
        {
            kinectTracking.StartTrackingRGBFrame();
        }
        if(GUILayout.Button("Stop Tracking"))
        {
            kinectTracking.StopTrackingRGBFrame();
        }


    }
    private void OnSceneGUI()
    {
        kinectTracking.ShowImage();
    }

    private void OnEnable()
    {
        kinectTracking = target as KinectTracking;
    }
}
