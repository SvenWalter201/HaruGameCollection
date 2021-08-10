using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using TMPro;
using Microsoft.Azure.Kinect.BodyTracking;

public class TriviaQuizLeftorRight : Game
{
    [SerializeField]
    TextMeshProUGUI countDownText, answerFeedback;
    [SerializeField]
    Image countDownMask, countDownBar, leftArrow, rightArrow, centerImg;
    [SerializeField]
    Text question;
    [SerializeField]
    GameObject panel;

    [SerializeField]
    Transform panelHolder;
    [SerializeField, Range(0f, 30f)]
    float questionTime = 4f, answerTime = 10f, showCorrectAnswerTime = 3f;
    [SerializeField]
    int questionAmount = 2;
    [SerializeField]
    VisualEffect e;
    [SerializeField]
    Slider slider;

    [SerializeField]
    CountMode countMode;

    [SerializeField]
    AnsweringMode answeringMode;

    enum CountMode
    {
        Numeric,
        Alphabetic,
        Roman
    }

    AnswerPanel[] panels;

    static readonly string[] 
        numeric = new string[] { "1", "2", "3", "4", "5", "6", "7" },
        alphabetic = new string[] { "A", "B", "C", "D", "E", "F", "G" }, 
        roman = new string[] { "I", "II", "III", "IV", "V", "VI", "VII" };
    
    List<QuestionCard> questions;

    BodyDisplay.PositionCompare hRC;
    void Awake()
    {
        countDownBar.enabled = false;
        countDownText.text = "";
        leftArrow.enabled = false;
        rightArrow.enabled = false;
        answerFeedback.gameObject.SetActive(false);
        centerImg.gameObject.SetActive(false);
    }

    void Start()
    {
        PlayGame();
    }

    protected override IEnumerator Init()
    {
        //get Questionary from json
        questions = QuestionManager.Instance.GetQuestions(questionAmount);

        panels = new AnswerPanel[2];

        GameObject left = Instantiate(panel, panelHolder, false);
        left.transform.SetAsFirstSibling();
        GameObject right = Instantiate(panel, panelHolder, false);

        panels[0] = left.GetComponent<AnswerPanel>();
        panels[1] = right.GetComponent<AnswerPanel>();


        ToggleSlider(false);

        hRC = BodyDisplay.handRaisedCompare;

        yield break;
    }

    void ToggleSlider(bool enable)
    {
        int childCount = slider.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            slider.transform.GetChild(i).gameObject.SetActive(enable);
        }

        leftArrow.enabled = enable;
        rightArrow.enabled = enable;
    }

    protected override IEnumerator Execute()
    {
        KinectDeviceManager.Instance.BeginBodyTracking();

        while (questions.Count > 0)
        {
            QuestionCard q = questions[Random.Range(0, questions.Count)];

            yield return StartCoroutine(AskQuestion(q));
            questions.Remove(q);
        }

        yield return new WaitForSeconds(3);

        ClearQuestionUI();
    }

    /// <summary>
    /// The Lamps glow for the set "glowTime" while reading the available Answers.
    /// The mesh filter changes from glowing to normal after the glowTime is finished.
    /// The User has the set time "answerTime" to think what the right answer is.
    /// The Lamp corresponding to the right answer glows up and the other Lamps and answers disappear (are disabled).
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    IEnumerator AskQuestion(QuestionCard q)
    {
        q.Reshuffle();
        InitQuestionUI(q);

        yield return new WaitForSeconds(questionTime);

        bool correct = false;

        switch (answeringMode)
        {
            case AnsweringMode.MOVE_X_AXIS:
                {
                    Vector3 playerPosition = Vector3.zero;

                    ToggleSlider(true);

                    float remainingTime = answerTime;
                    countDownBar.enabled = true;
                    countDownMask.gameObject.SetActive(true);
                    while (remainingTime > 0f)
                    {
                        playerPosition = BodyDisplay.Instance.GetBodyPosition();
                        float adjustedX = Mathf.Clamp(playerPosition.x, -1, 1f);
                        adjustedX *= 0.5f;
                        adjustedX += 0.5f;
                        slider.value = adjustedX;

                        remainingTime -= Time.deltaTime;
                        countDownMask.fillAmount = 1 - remainingTime / answerTime;
                        countDownText.text = Mathf.RoundToInt(remainingTime).ToString();
                        yield return null;
                    }

                    //yield return timer.UITimer(answerTime, countDownMask, countDownText);

                    countDownText.text = "";
                    countDownBar.enabled = false;
                    countDownMask.gameObject.SetActive(false);
                    ToggleSlider(false);

                    correct = (q.TrueAnswer == 0) ?
                        playerPosition.x <= 0f :
                        playerPosition.x > 0f;
                    break;
                }
            case AnsweringMode.RAISE_HANDS:
                {
                    float remainingTime = answerTime / 2f;
                    countDownBar.enabled = true;
                    countDownMask.gameObject.SetActive(true);
                    while (remainingTime > 0f)
                    {
                        remainingTime -= Time.deltaTime;
                        countDownMask.fillAmount = 1 - remainingTime / answerTime;
                        countDownText.text = Mathf.RoundToInt(remainingTime).ToString();
                        yield return null;
                    }

                    //yield return timer.UITimer(answerTime, countDownMask, countDownText);

                    countDownText.text = "";
                    countDownBar.enabled = false;
                    countDownMask.gameObject.SetActive(false);

                    bool leftHandRaised = BodyDisplay.Instance.JointCompare(JointId.Head, JointId.HandLeft, hRC);
                    bool rightHandRaised = BodyDisplay.Instance.JointCompare(JointId.Head, JointId.HandRight, hRC);

                    //Debug.Log("Left" + leftHandRaised);
                    //Debug.Log("Right" + rightHandRaised);
                    
                    correct = (q.TrueAnswer == 0) ?
                        leftHandRaised && !rightHandRaised :
                        rightHandRaised && !leftHandRaised;
    
                    break;
                }
        }

        answerFeedback.gameObject.SetActive(true);
        countDownBar.gameObject.SetActive(false);
        countDownText.gameObject.SetActive(false);

        if (correct)
        {
            answerFeedback.text = StringRes.Get("_RightAnswer");
            ConfettiBurst();
        }
        else
        {
            answerFeedback.text = StringRes.Get("_WrongAnswer");
        }

        for (int i = 0; i < q.Answers.Count; i++)
        {
            AnswerPanel p = panels[i];
            if (i != q.TrueAnswer)
            {
                p.gameObject.SetActive(false);
            }
        }

        centerImg.gameObject.SetActive(false);

        yield return new WaitForSeconds(showCorrectAnswerTime);

        answerFeedback.gameObject.SetActive(false);
        countDownBar.gameObject.SetActive(true);
        countDownText.gameObject.SetActive(true);
    }

    void InitQuestionUI(QuestionCard q)
    {
        centerImg.gameObject.SetActive(true);

        question.text = q.Question;

        for (int i = 0; i < q.Answers.Count; i++)
        {
            AnswerPanel p = panels[i];
            p.gameObject.SetActive(true);
            switch (countMode)
 
           {
                case CountMode.Numeric:
                    p.AnswerNumber.text = numeric[i];
                    break;
                case CountMode.Alphabetic:
                    p.AnswerNumber.text = alphabetic[i];
                    break;
                case CountMode.Roman:
                    p.AnswerNumber.text = roman[i];
                    break;
                default:
                    p.AnswerNumber.text = numeric[i];
                    break;
            }

            p.AnswerText.text = q.Answers[i];
        }

        centerImg.sprite = q.Image;


    }

    void ClearQuestionUI()
    {
        for (int i = 0; i < panels.Length; i++)
            Destroy(panels[i].gameObject);

        question.text = "";
    }

    void ConfettiBurst() =>
    e.SendEvent("TriggerBurst");

    public enum AnsweringMode
    {
        RAISE_HANDS,
        MOVE_X_AXIS
    }
}