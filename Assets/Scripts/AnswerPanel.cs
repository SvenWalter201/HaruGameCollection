using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnswerPanel : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI answerNumber, answerText;
    [SerializeField]
    GameObject bulb;

    public TextMeshProUGUI AnswerNumber => answerNumber;
    public TextMeshProUGUI AnswerText => answerText;
    public GameObject Bulb => bulb;
}
