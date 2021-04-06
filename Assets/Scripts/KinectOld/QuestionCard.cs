using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;
using UnityEngine;

    class QuestionCard
    {


        public List<Color> ColorList = new List<Color>{ Color.red, Color.green, Color.blue, Color.white };
        public String[] AnswerCount = new String[] { "A:", "B:", "C:", "D:" };

        public string Question { get; set; }

        public string[] Answers { get; set; }

        //numerical value for true answer
        public byte TrueAnswer { get; set; }


        public void Print()
        {
            Console.WriteLine(Question);
            for (int i = 0; i < Answers.Length; i++)
            {
                Console.WriteLine(Answers[i]);
            }
            //Console.WriteLine("True answer: " + answers[trueAnswer]);

        }

        public void PrintWithLights()
        {
            LightManager lm = LightManager.Instance;

            Console.WriteLine(Question);
            for (int i = 0; i < Answers.Length; i++)
            {
            lm.SetLights(Command.BRIGHTEN, new List<string> { lm.availableIndizes[i] }, ColorList[i]);
            lm.SetLights(Command.ON, new List<string> { lm.availableIndizes[i] }, ColorList[i]);
                Console.WriteLine(AnswerCount[i] + Answers[i]);
                Thread.Sleep(5000);
                lm.SetLights(Command.OFF, new List<string> { lm.availableIndizes[i] }, ColorList[i]);
        }

            //Console.WriteLine("True answer: " + answers[trueAnswer]);
        }

    public bool ValidateAnswer(byte answer)
        {
        return answer == TrueAnswer;
        }
    }

