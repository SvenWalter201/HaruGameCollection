using System;
public class SentenceInformation
{
    private string position;
    private action action;
    private mood mood;
    private subject subject;
    private char gender;
    private bool singular;
    private colour colour;


    public string Position { get => position; set => position = value; }
    public action Action { get => action; set => action = value; }
    public mood Mood { get => mood; set => mood = value; }
    public subject Subject { get => subject; set => subject = value; }
    public char Gender { get => gender; set => gender = value; }
    public bool Singular { get => singular; set => singular = value; }
    public colour Colour { get => colour; set => colour = value; }

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
        return position + " | " + action + " | " + mood + " | " + subject + " | " + gender + " | " + singular;
    }
}
