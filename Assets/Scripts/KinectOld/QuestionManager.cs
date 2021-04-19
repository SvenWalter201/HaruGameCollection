using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Linq;


class QuestionManager
{
    private static readonly QuestionManager instance = new QuestionManager();

    static QuestionManager()
    {
    }
    private QuestionManager()
    {

    }
    public static QuestionManager Instance
    {
        get
        {
            return instance;
        }
    }


    public int GetQuestionAmount()
    {
        int questionAmount;
        Console.WriteLine("How many questions should be asked?");
        while (true)
        {
            string input = Console.ReadLine();
            try
            {
                questionAmount = int.Parse(input);
                break;
            }
            catch (Exception)
            {
                Console.WriteLine("Please type an Integer number");
            }
        }

        return questionAmount;
    }

    public QuestionCard[] GetQuestions(int amount)
    {
        string questionary = File.ReadAllText(@"Questionary4Answers.json");

        QuestionCard[] allQuestions = JsonConvert.DeserializeObject<QuestionCard[]>(questionary);

        return allQuestions;
    }

    public QuestionCard[] TrimQuestions(QuestionCard[] questionCatalog, int trimToXAnswers)
    {
        QuestionCard[] newQuestionCatalog = new QuestionCard[questionCatalog.Length];

        Array.Copy(questionCatalog, newQuestionCatalog,questionCatalog.Length);

        for (int i = 0; i < questionCatalog.Length; i++)
        {
            QuestionCard currentCard = questionCatalog[i];
            List<string> newAnswers = currentCard.Answers.ToList();
            string trueAnswer = currentCard.Answers[currentCard.TrueAnswer];
            int amountToRemove = currentCard.Answers.Length - trimToXAnswers;

            for (int removed = 0; removed < amountToRemove;)
            {
                Random r = new Random();
                byte rnd = (byte)r.Next(0, newAnswers.Count);

                if (newAnswers.ElementAt(rnd) != trueAnswer)
                {
                    newAnswers.RemoveAt(rnd);
                    removed++;
                }
            }
            newQuestionCatalog[i].Answers = newAnswers.ToArray();
            Reshuffle(currentCard, trueAnswer);
            
            
        }

        return newQuestionCatalog;
    }


    private void Reshuffle(QuestionCard qc, string currentTrueAnswer)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia
        string[] texts = qc.Answers;
        for (int t = 0; t < texts.Length; t++)
        {
            string tmp = texts[t];

            Random r = new Random();
            byte random = (byte)r.Next(t, texts.Length);
            texts[t] = texts[random];
            texts[random] = tmp;
        }

        for (int i = 0; i < texts.Length - 1; i++)
        {
            if (texts[i].Equals(currentTrueAnswer))
            {
                qc.TrueAnswer = (byte)i;
                break;
            }
        }

    }

}


