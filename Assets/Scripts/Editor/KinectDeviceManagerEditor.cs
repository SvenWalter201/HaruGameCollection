using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KinectDeviceManager))]
public class KinectDeviceManagerEditor : Editor
{
    private KinectDeviceManager kinectDeviceManager;

    public override void OnInspectorGUI()
    {
        if (kinectDeviceManager == null)
        {
            return;
        }

        if (GUILayout.Button("Init"))
        {
            kinectDeviceManager.Init();
        }
        if (GUILayout.Button("Close"))
        {
            kinectDeviceManager.Close();
        }

        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        if(kinectDeviceManager == null)
        {
            return;
        }
        //kinectDeviceManager.ShowImage();
    }

    private void OnEnable()
    {
        //EditorApplication.update += OnSceneGUI;
        kinectDeviceManager = target as KinectDeviceManager;
    }
}

