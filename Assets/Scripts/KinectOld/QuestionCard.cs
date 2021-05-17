using System;
using System.Collections.Generic;
using UnityEngine;

class QuestionCard : ICloneable
{
    public static List<Color> colorList = new List<Color> { Color.red, Color.green, Color.blue, Color.white };

    readonly string question;
    readonly List<string> answers;
    int trueAnswer;
    public string Question => question;
    public List<string> Answers => answers;

    //index of true answer
    public int TrueAnswer { get => trueAnswer; set => trueAnswer = value; }

    public QuestionCard(string question, List<string> answers, int trueAnswer)
    {
        this.question = question;
        this.answers = answers;
        this.trueAnswer = trueAnswer;
    }

    public bool ValidateAnswer(int answer)
    {
        return answer == TrueAnswer;
    }

    public void Reshuffle()
    {
        string trueAnswer = Answers[TrueAnswer];
        // Knuth shuffle algorithm :: courtesy of Wikipedia
        List<string> answers = Answers;
        for (int i = 0; i < answers.Count; i++)
        {

            int r = UnityEngine.Random.Range(0, answers.Count);
            if (r == i)
            {
                continue;
            }
            string tmp = answers[i];
            answers[i] = answers[r];
            answers[r] = tmp;
        }

        for (int i = 0; i < answers.Count; i++)
        {
            if (answers[i] == trueAnswer)
            {
                TrueAnswer = i;
                break;
            }
        }
    }

    public object Clone()
    {
        return new QuestionCard(question, answers, trueAnswer);
    }
}

