using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "QuestionCatalogue", menuName = "ScriptableObjects/QuestionCatalogue", order = 1)]
public class QuestionCatalogue : ScriptableObject, ICloneable
{
    [SerializeField]
    List<QuestionCard> questionCards;

    public List<QuestionCard> QuestionCards => questionCards;

    public object Clone()
    {
        return CreateInstance<QuestionCatalogue>();
    }
}
