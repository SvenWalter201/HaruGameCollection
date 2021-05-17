using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

class QuestionManager : Singleton<QuestionManager>
{

    QuestionCard[] allQuestions;

    QuestionCard[] defaultQuestions = new QuestionCard[]
    {
        new QuestionCard("What is the biggest animal", new List<string>(){ "Elefant","Anakonda","Whaleshark","Blue whale" }, 3),
        new QuestionCard("What color is blue", new List<string>(){ "blue","yellow","red","green" }, 0)
    };

    void LoadQuestions()
    {
        if(JsonFileManager.Load("", out QuestionCard[] allQuestions)){
            this.allQuestions = allQuestions;
        }
        else
        {
            this.allQuestions = defaultQuestions;
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
            QuestionCard currentCard = (QuestionCard)questionCatalog[i].Clone();
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
            newQuestionCatalog[i] = currentCard;
        }
        
        return newQuestionCatalog;
    }
}


