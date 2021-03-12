using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestingGenerativeGrammars))]
public class TestingGenerativeGrammarsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TestingGenerativeGrammars gen = (TestingGenerativeGrammars)target;

        if(GUILayout.Button("Generate Sentence"))
        {
            gen.GenerateSentence();
        }
    }
}
