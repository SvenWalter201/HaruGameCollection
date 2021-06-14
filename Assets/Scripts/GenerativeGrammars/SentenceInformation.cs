using System;
public class SentenceInformation
{
    private string position;
    private Action action;
    private Mood mood;
    private Subject subject;
    private char gender;
    private bool singular;
    private Colour colour;


    public string Position { get => position; set => position = value; }
    public Action Action { get => action; set => action = value; }
    public Mood Mood { get => mood; set => mood = value; }
    public Subject Subject { get => subject; set => subject = value; }
    public char Gender { get => gender; set => gender = value; }
    public bool Singular { get => singular; set => singular = value; }
    public Colour Colour { get => colour; set => colour = value; }

    /*
    public void ClearInformation()
        {

        if(position != null && action != null && mood != null && subject != null && gender != null )
        {
            this.position = null;
            this.action = null;
            this.mood = null;
            this.subject = null;
            this.gender = ' ';
            this.singular = true;
            this.colour = null;
        }
        
        }
    */

    public string PrintToString()
    {
        return position + " | " + action.name + " | " + mood.name + " | " + subject.name + " | " + gender + " | " + singular;
    }
}
