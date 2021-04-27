using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerativeGrammatiken))]
public class TestingGenerativeGrammarsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GenerativeGrammatiken gen = (GenerativeGrammatiken)target;

        if(GUILayout.Button("Generate Sentence"))
        {
            gen.GenerateSentence();
        }
    }
}
