using UnityEngine;

public class GameSelectionInput : InputSource
{
    [SerializeField]
    KeyCode TriviaQuiz = KeyCode.Alpha1, MotionMemory = KeyCode.Alpha2, Duplik = KeyCode.Alpha3;
    const int 
        triviaQuizBuildIndex = 2,
        motionMemoryBuildIndex = 6,
        duplikBuildIndex = 4;


    public override void TestForInput()
    {
        int index = -1;
        if (Input.GetKeyDown(TriviaQuiz))
        {
            index = triviaQuizBuildIndex;
        }
        else if (Input.GetKeyDown(MotionMemory))
        {
            index = motionMemoryBuildIndex;
        }
        else if (Input.GetKeyDown(Duplik))
        {
            index = duplikBuildIndex;
        }

        if(index != -1)
        {
            GameController.Instance.StartGame(index);
        }
    }
}
