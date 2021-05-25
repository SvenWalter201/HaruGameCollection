using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SkeletonDisplay))]
public class SkeletonDisplayEditor : Editor
{
    private SkeletonDisplay skeletonDisplay;
    /*

    private void OnSceneGUI()
    {
        //Handles.DrawLine(Vector3.one, Vector3.zero);

        //Debug.Log(jC.posA + "   " + jC.posB);

        if (skeletonDisplay == null)
        {
            return;
        }

        List<JointConnection> jointConnections = skeletonDisplay.ResolveSkeleton();
        Handles.color = Color.green;
        for (int i = 0; i < jointConnections.Count; i++)
        {
            JointConnection jC = jointConnections[i];
            if(jC.posA != null && jC.posB != null)
            {
                if(jC.posA != Vector3.zero || jC.posB != Vector3.zero)
                {
                    Debug.Log(jC.posA);
                }
                Handles.DrawLine(jC.posA, jC.posB);
            }
        }
    }*/

    private void OnEnable()
    {
        //EditorApplication.update += OnSceneGUI;
        skeletonDisplay = target as SkeletonDisplay;
    }
}
