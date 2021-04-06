using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;
using System.Numerics;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;

using Vector3 = System.Numerics.Vector3;



//a testprogram, that checks, if a person is in a specified corner or not
class PositionRecognitionProgram : IKinectProgram
{

    private const uint ANSWER_TIME = 5;
    private const long TIME_INTERVAL = 1000L;
    private long bodyHandle;
    private readonly RoomManager instance = RoomManager.Instance;
    public void Execute()
    {
        LightManager lightManager = LightManager.Instance;
        
        instance.m_Room = new Room();
        bodyHandle = TempDataManager.Instance.CreateNewDataCollection(TIME_INTERVAL);
        TrackingManager.Instance.FrameArrivedEvent += PositionRecognitionProgram_OnFrameArrived;
        Console.WriteLine("QuestionProgram");

        //get Questionary from json
        int questionAmount = QuestionManager.Instance.GetQuestionAmount();
        QuestionCard[] questionSet = QuestionManager.Instance.GetQuestions(questionAmount);
        QuestionCard[] questions = QuestionManager.Instance.TrimQuestions(questionSet, KinectDeviceManager.Instance.corners);


        for (uint i = 0; i < questionAmount; i++)
        {
            QuestionCard q = questions[i];
            q.PrintWithLights();

            lightManager.SetLights(Command.DIM, Color.white);
            
            CountDown();
            long toTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            long fromTime = toTime - TIME_INTERVAL;
            Vector3 bodyPosition = TempDataManager.Instance.GetAverageBetweenFromCollection(bodyHandle, fromTime, toTime);
            bool correctCorner = RoomManager.Instance.CheckIfPersonIsInCorner((Corner)q.TrueAnswer, bodyPosition);

            lightManager.TurnOffLights();
            lightManager.SetLights(Command.BRIGHTEN, new List<string> { lightManager.availableIndizes[q.TrueAnswer] }, q.ColorList[q.TrueAnswer]);
            lightManager.SetLights(Command.ON, new List<string> { lightManager.availableIndizes[q.TrueAnswer] }, q.ColorList[q.TrueAnswer]);
            if (correctCorner)
            {

                Console.WriteLine("Yes, the correct answer is " + q.AnswerCount[q.TrueAnswer] + q.Answers[q.TrueAnswer]);
            }
            else
            {
                Console.WriteLine("No, the correct answer was " + q.AnswerCount[q.TrueAnswer] + q.Answers[q.TrueAnswer]);
            }
            Console.ReadLine();
            lightManager.TurnOffLights();
            Thread.Sleep(500);
        }
        lightManager.TurnOffLights();
    }

    public void PositionRecognitionProgram_OnFrameArrived(object sender, BodyFrameArrivedEventArgs e)
    { 
        Vector3 body = e.bodyFrame.GetBody(0).Skeleton.GetJoint(JointId.SpineNavel).Position;
        
        //Vector3 body = e.bodyFrame.GetBody(0).Skeleton.GetJoint(JointId.SpineNavel).Position;
        if(body != null)
        {
            Vector3 pos = new Vector3(body.X, body.Y, body.Z);
            TempDataManager.Instance.AddDataToCollection(bodyHandle, pos);
        }
    }


    public static void CountDown()
        {
        for (uint i = ANSWER_TIME; i >= 1; i--)
        {
            Console.WriteLine(i);
            Thread.Sleep(1000);

        }
        Console.WriteLine("0");
        }
}



