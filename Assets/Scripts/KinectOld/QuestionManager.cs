using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

class QuestionManager : Singleton<QuestionManager>
{
    [SerializeField]
    QuestionCatalogue defaultCatalogue;

    QuestionCard[] allQuestions;

    void LoadQuestions()
    {
<<<<<<< HEAD
        if(JsonFileManager.Load("Resources/Fragenkataog_Trivia", out QuestionCard[] allQuestions)){
=======
        if(FileManager.LoadJSON("", out QuestionCard[] allQuestions)){
>>>>>>> 027a30af0569ff4d25fa1e29af68134e70f43a0a
            this.allQuestions = allQuestions;
        }
        else
        {
            this.allQuestions = defaultCatalogue.QuestionCards.ToArray();
            Debug.Log("1st card later: "+this.allQuestions[0].Answers[1]);
        }
    }

    void Awake()
    {
        LoadQuestions();
    }

    public List<QuestionCard> GetQuestions(int amount)
    {
        if(allQuestions == null)
        {
            Debug.LogError("allQuestions was null");
            return null;
        }
        else if (allQuestions.Length < amount)
        {
            Debug.LogError("not enough Questions available");
            return null;
        }

        List<QuestionCard> questions = new List<QuestionCard>((QuestionCard[])allQuestions.Clone());
        while(questions.Count > amount)
        {
            int r = UnityEngine.Random.Range(0, questions.Count);
            questions.RemoveAt(r);
        }
        return questions;
    }

    /// <summary>
    /// reduces the amount of answers in each question to the specified amount
    /// </summary>
    /// <param name="questionCatalog"></param>
    /// <param name="newAnswerCount"></param>
    /// <returns></returns>
    public List<QuestionCard> TrimQuestions(List<QuestionCard> questionCatalog, int newAnswerCount, bool shuffleAnswers)
    {
        List<QuestionCard> newQuestionCatalog = new List<QuestionCard>(questionCatalog.Count);

        for (int i = 0; i < questionCatalog.Count; i++)
        {
            Debug.Log("before clone: "+questionCatalog[i].ToString());
            QuestionCard currentCard = (QuestionCard)questionCatalog[i].Clone();
            Debug.Log("after clone: " + currentCard.ToString());
            List<string> answers = currentCard.Answers;
            int amountToRemove = answers.Count - newAnswerCount;

            for (int removed = 0; removed < amountToRemove;)
            {
                int r = UnityEngine.Random.Range(0, answers.Count);

                if (r != currentCard.TrueAnswer)
                {
                    answers.RemoveAt(r);
                    removed++;
                }
            }

            if (shuffleAnswers)
            {
                currentCard.Reshuffle();
            }
            newQuestionCatalog.Add(currentCard);
        }
        
        return newQuestionCatalog;
    }
}


