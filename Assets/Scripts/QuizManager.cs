using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuizManager : MonoBehaviour
{
    public List<QnA> QuestionsAnswers;
    public GameObject[] options;
    public int currentQuestion;

    public Text QuestionText;
    public Text[] Answers;

    public Image[] Lamps;


    [Header("LampSprites")]

    [SerializeField] private Sprite[] LampsSprites;
    [SerializeField] private Sprite[] glowingLampsSprites;

    [Space]


    [SerializeField] private float glowTime = 4;
    [SerializeField] private float answerTime = 10;

    private bool blocked = false;

    private void Start()
    {
        // chooseQuestion();
    }

    public void Correct()
    {
        QuestionsAnswers.RemoveAt(currentQuestion);

        ChooseQuestion();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !blocked && QuestionsAnswers.Count > 0)
        {
            ChooseQuestion();
        }
    }

    private void ChooseQuestion()
    {

        for (int i = 0; i < 4; i++)
        {
            Lamps[i].enabled = true;
            Answers[i].enabled = true;
            Lamps[i].sprite = LampsSprites[i];
        }
        blocked = true;

        currentQuestion = Random.Range(0, QuestionsAnswers.Count);
        QnA question = QuestionsAnswers[currentQuestion];

        QuestionText.text = question.Question;
        for (int i = 0; i < question.Answers.Length; i++)
        {
            Answers[i].text = question.Answers[i];

        }
        QuestionsAnswers.Remove(question);
        StartCoroutine(Changing(question));

    }

    /// <summary>
    /// Bla Bla
    /// </summary>
    /// <param name="Question"></param>
    /// <returns></returns>
    private IEnumerator Changing(QnA Question)
    {

        yield return new WaitForSeconds(glowTime);
        for (int i = 0; i < Question.Answers.Length; i++)
        {
            Lamps[i].sprite = glowingLampsSprites[i];
            yield return new WaitForSeconds(glowTime);
            Lamps[i].sprite = LampsSprites[i];
        }

        yield return new WaitForSeconds(answerTime);

        for (int i = 0; i < Question.Answers.Length; i++)
        {
            if (i != Question.CorrectAnswer)
            {
                Lamps[i].enabled = false;
                Answers[i].enabled = false;
            }
        }
        Lamps[Question.CorrectAnswer].sprite = glowingLampsSprites[Question.CorrectAnswer];
        //image1.enabled = false;
        blocked = false;
    }
}
