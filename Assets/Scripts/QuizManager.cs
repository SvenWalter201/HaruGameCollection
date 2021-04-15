using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuizManager : MonoBehaviour
{
    public List<QnA> QuestionsAnswers;
    public GameObject[] options;
    public int currentQuestion;

    [SerializeField] private Text QuestionText;
    [SerializeField] private Text[] Answers;

    

    [SerializeField] private GameObject[] AnswerPanel;
    [SerializeField] private GameObject[] Lamps;
    [SerializeField] private Mesh normal;
    [SerializeField] private Mesh glowing;


     
    [Header("LampSprites")]


    [Space]


    [SerializeField] private float glowTime = 4;
    [SerializeField] private float answerTime = 10;

    private bool blocked = false;

    private void Start()
    {
        
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

    /// <summary>
    /// The disabled Lamps and answers get enabled to display the next or first question.
    /// A random question with matching answers is chosen and gets displayed.
    /// The Coroutine is started.
    /// </summary>
    /// <param name="Question"></param>
    /// <returns></returns>
    private void ChooseQuestion()
    {

        for (int i = 0; i < 4; i++)
        {

            Lamps[i].GetComponent<Renderer>().enabled = true;
            AnswerPanel[i].SetActive(true);
            Answers[i].enabled = true;
            Lamps[i].GetComponent<MeshFilter>().mesh = normal;
         
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
    /// The Lamps glow for the set "glowTime" while reading the available Answers.
    /// The mesh filter changes from glowing to normal after the glowTime is finished.
    /// The User has the set time "answerTime" to think what the right answer is.
    /// The Lamp corresponding to the right answer glows up and the other Lamps and answers disappear (are disabled).
    /// </summary>
    /// <param name="Question"></param>
    /// <returns></returns>
    private IEnumerator Changing(QnA Question)
    {

        yield return new WaitForSeconds(glowTime);
        for (int i = 0; i < Question.Answers.Length; i++)
        {

            Lamps[i].GetComponent<MeshFilter>().mesh = glowing;
            yield return new WaitForSeconds(glowTime);
            Lamps[i].GetComponent<MeshFilter>().mesh = normal;

        }

        yield return new WaitForSeconds(answerTime);

        for (int i = 0; i < Question.Answers.Length; i++)
        {
            if (i != Question.CorrectAnswer)
            {
                AnswerPanel[i].SetActive(false);
                Lamps[i].GetComponent<Renderer>().enabled = false;
                Answers[i].enabled = false;
            }
        }


        Lamps[Question.CorrectAnswer].GetComponent<MeshFilter>().mesh = glowing;
        
        blocked = false;
    }
}
