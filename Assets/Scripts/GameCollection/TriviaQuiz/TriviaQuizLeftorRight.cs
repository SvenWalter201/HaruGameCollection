#define STEP_BY_STEP
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using TMPro;
public class TriviaQuizLeftorRight : Game
{
    [SerializeField]
    TextMeshProUGUI countDownText;
    [SerializeField]
    Image countDownMask, countDownBar;
    [SerializeField]
    Text question;
    [SerializeField]
    GameObject panel, panelHolder;
    [SerializeField]
    Mesh normal, glowing;
    [SerializeField, Range(0f, 30f)]
    float glowTime = 4f, answerTime = 10f, showCorrectAnswerTime = 3f;
    [SerializeField]
    int questionAmount = 2;
    [SerializeField]
    Material bulbMaterial;
    [SerializeField]
    VisualEffect e;
    [SerializeField]
    CountMode countMode;

    enum CountMode
    {
        Numeric,
        Alphabetic,
        Roman
    }

    CoroutineTimer timer = new CoroutineTimer();
    AnswerPanel[] panels;

    static readonly Color[] colorsDimm = new Color[] { Color.red / 4f, Color.yellow / 4f, Color.green / 4f, Color.cyan / 4f };
    static readonly Color[] colorsBright = new Color[] { Color.red * 3f, Color.yellow * 3f, Color.green * 3f, Color.cyan * 3f };

    static readonly string[] numeric = new string[] { "1", "2", "3", "4", "5", "6", "7" };
    static readonly string[] alphabetic = new string[] { "A", "B", "C", "D", "E", "F", "G" };
    static readonly string[] roman = new string[] { "I", "II", "III", "IV", "V", "VI", "VII" };

    static readonly int emissionId = Shader.PropertyToID("_EmissionColor");

    Room room;

    private void Awake()
    {
        countDownBar.enabled = false;
        countDownText.text = "";
    }

    void Start()
    {
        PlayGame();
    }

    protected override IEnumerator Init()
    {
        yield break;
    }

    protected override IEnumerator Execute()
    {
        /*
        room = RoomManager.Instance.LoadRoom();
        if (room == null)
        {
            yield break;
        }
        */


        //get Questionary from json
        List<QuestionCard> questions = QuestionManager.Instance.GetQuestions(questionAmount);

        panels = new AnswerPanel[2];

        for (int i = 0; i < panels.Length; i++)
        {
            GameObject p = Instantiate(panel);
            p.transform.SetParent(panelHolder.transform, false);
            AnswerPanel a = p.GetComponent<AnswerPanel>();
            a.Bulb.GetComponent<MeshRenderer>().material = GetMaterial(colorsDimm[i]);
            panels[i] = a;
        }

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
        InitQuestionUI(q);

        //Vector3 cornerPosition = room.Corners[q.TrueAnswer];

        //highlight the answers
        for (int i = 0; i < q.Answers.Count; i++)
        {
            GameObject bulb = panels[i].Bulb;

            MeshFilter mF = bulb.GetComponent<MeshFilter>();
            MeshRenderer mR = bulb.GetComponent<MeshRenderer>();
            mF.mesh = glowing;
            mR.material.SetColor(emissionId, colorsBright[i]);
            yield return new WaitForSeconds(glowTime);
            mR.material.SetColor(emissionId, colorsDimm[i]);
            mF.mesh = normal;
        }

        countDownBar.enabled = true;
        yield return timer.UITimer(answerTime, countDownMask, countDownText);

        Vector3 playerPosition = BodyDisplay.Instance.GetBodyPosition();

        countDownBar.enabled = false;

        bool correct = false;

        correct = (q.TrueAnswer == 0) ?
            playerPosition.x < 0f :
            playerPosition.x > 0f;

        if (correct)
            ConfettiBurst();
        
        for (int i = 0; i < q.Answers.Count; i++)
        {
            AnswerPanel p = panels[i];
            if (i != q.TrueAnswer)
            {
                p.gameObject.SetActive(false);
            }
            else
            {
                p.Bulb.GetComponent<MeshFilter>().mesh = glowing;
                p.Bulb.GetComponent<MeshRenderer>().material.SetColor(emissionId, colorsBright[i]);
            }
        }

        yield return new WaitForSeconds(showCorrectAnswerTime);
    }

    void InitQuestionUI(QuestionCard q)
    {
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
            p.Bulb.GetComponent<MeshRenderer>().enabled = true;
            p.Bulb.GetComponent<MeshRenderer>().material.SetColor(emissionId, colorsDimm[i]);
        }
    }

    void ClearQuestionUI()
    {
        for (int i = 0; i < panels.Length; i++)
            Destroy(panels[i].gameObject);

        question.text = "";
    }

    Material GetMaterial(Color color)
    {
        Material ins = Instantiate(bulbMaterial);
        ins.SetColor(emissionId, color);
        return ins;
    }

    void ConfettiBurst() =>
    e.SendEvent("TriggerBurst");
}
