using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class SentenceInformation 
    {
        private string position;
        private string action;
        private string mood;
        private string person;
        private char gender;
        private bool singular;
        private string colour;
       

        public string Position { get => position; set => position = value; }
        public string Action { get => action; set => action = value; }
        public string Mood { get => mood; set => mood = value; }
        public string Person { get => person; set => person = value; }
        public char Gender { get => gender; set => gender = value; }
        public bool Singular { get => singular; set => singular = value; }
        public string Colour { get => colour; set => colour = value; }


    public void ClearInformation()
        {

        if(position != null && action != null && mood != null && person != null && gender != null )
        {
            this.position = null;
            this.action = null;
            this.mood = null;
            this.person = null;
            this.gender = ' ';
            this.singular = true;
            this.colour = null;
        }
        
        }
    
    public string PrintToString()
    {
        return position + " | " + action + " | " + mood + " | " + person + " | " + gender + " | " + singular;
    }
}
