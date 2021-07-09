using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;

public class GenerativeGrammatiken : Singleton<GenerativeGrammatiken>
{
    private const string fileName = "/Resources/MasterData.json";
    //private Vocabulary vocabulary;
    MasterDataClass masterData = new MasterDataClass();
    public List<string> recentlyUsed = new List<string>();
    private bool probability = true; //singular
    private SentenceInformation si;
    private Subject currentperson;
    private string[] priorityKeywords;
    public Text sentenceText;
    private string sentence;
    public byte count = 0;
    List<Subject> 
        _subjects = new List<Subject>(),
        _persons = new List<Subject>(),
        _things = new List<Subject>(),
        _animals = new List<Subject>();
    List<Colour> _colours = new List<Colour>();
    List<Action> _actions = new List<Action>();
    List<Mood> _moods = new List<Mood>();

    private void Start()
    {
        //ClearRecentlyUsed();
        GetVocabulary();
    }
    private void GetVocabulary()
    {
        priorityKeywords = new string[] { "thing", "animal", "person"};
        string appDataPath = Application.dataPath;
        string filePath = appDataPath + fileName;
        if (FileManager.LoadJSON(filePath, out MasterDataClass data))
        {
            masterData = data;
            //Debug.Log("loading json");
        }

        _subjects = masterData.subjects;

        
        foreach (Subject s in _subjects)
        {
            switch (s.type)
            {
                //create list of persons
                case "person": 
                    _persons.Add(s);
                    break;
                //create list of things
                case "thing": 
                    _things.Add(s);
                    break;
                //create list of animals
                case "animal":
                    _animals.Add(s);
                    break;
            }
        }

        

    }


    public SentenceInformation GenerateSentence()
    {
        GetProbability();
        si = new SentenceInformation();
        si.Singular = true;
        //si.ClearInformation();
        string template = GetTemplate();
        sentence = FillInTemplate(template);
        //Debug.Log(sentence);
        return si;
    }

    public string PrintSentence() => sentence;
    

    public string FillInTemplate(string template)
    {
        if (template.Contains("@"))
        {
            string generator = Helper.LookForPriority(template,priorityKeywords);
            string replacement = "NO REPLACEMENT FOUND";

            switch (generator)
            {
                case "person":
                    {
                        replacement = GetPerson();
                        break;
                    }
                case "animal":
                    {
                        replacement = GetAnimal();
                        break;
                    }
                case "thing":
                    {
                        replacement = GetThing();
                        si.Singular = true;
                        if (!probability)
                        {
                            replacement = pluralize(replacement);
                        }
                        break;
                    }
                case "position":
                    {
                        replacement = GetPosition();
                        break;
                    }
                case "action":
                    {
                        replacement = GetAction();
                        break;
                    }
                case "mood":
                    {
                        replacement = GetMood();
                        break;
                    }
                case "linkingword":
                    {
                        replacement = GetLinkingword();
                        break;
                    }
                case "setting":
                    {
                        replacement = GetSetting();
                        break;
                    }
                case "colour":
                    {
                        replacement = GetColour();
                        break;
                    }

            }

            template = Helper.ReplaceWordBetweenTags(template, generator, replacement);
            return FillInTemplate(template);
        }

    #region GRAMMAR_PROOF
        //------------------------Grammatikalische Korrekturen---------------------

        if (template.Contains("["))
        {
            template = resolveOptions(template);
        }

        //adapt verbform
        if (template.Contains("<"))
        {
            template = ResolveVerb(template);
        }

        //adapt article
        if (template.Contains("{"))
        {
            template = ResolveUndefinedArticle(template);
        }

        //large and lower case
        template = FixLowerCase(template);

        //Debug.Log(si.PrintToString());

        return template;
    }
    #endregion

    #region GENERATORS
    //------------------GENERATORS-------------------------

    private string GetCorrectKonjugationAdjective(string adjective, char gender)
    {
        switch (gender)
        {
            case 'm':
                return adjective + "er";
            case 'n':
                return adjective + "es";
            case 'f':
                return adjective + "e";
            default:
                return adjective;
        }
    }

    private string GetPerson()
    {
        //avoid duplicates
        int max_iterations = _persons.Count;

        int r = UnityEngine.Random.Range(0, max_iterations);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(_persons[r].name))
            {
                r = UnityEngine.Random.Range(0, max_iterations);
            }
            else
            {
                break;
            }
        }
        recentlyUsed.Add(_persons[r].name);
        si.Gender = _persons[r].gender;
        si.Singular = true;
        si.Subject = _persons[r];
        currentperson = _persons[r];
        return _persons[r].name;
    }


    private string GetAnimal()
    {
        //avoid duplicates
        int max_iterations = _animals.Count;

        int r = UnityEngine.Random.Range(0, max_iterations);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(_animals[r].name))
            {
                r = UnityEngine.Random.Range(0, max_iterations);
            }
            else
            {
                break;
            }
        }
        recentlyUsed.Add(_animals[r].name);
        si.Gender = _animals[r].gender;
        si.Subject = _animals[r];
        si.Singular = true;
        currentperson = _animals[r];
        return _animals[r].name;
    }


    private string GetThing()
    {
        //avoid duplicates
        int max_iterations = _things.Count;

        int r = UnityEngine.Random.Range(0, max_iterations);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(_things[r].name))
            {
                r = UnityEngine.Random.Range(0, max_iterations);
            }
            else
            {
                break;
            }
        }
        recentlyUsed.Add(_things[r].name);
        si.Gender = _things[r].gender;
        si.Subject = _things[r];
        currentperson = _things[r];
        return _things[r].name;
    }

    //returns GENDER, adding gender to sentenceStructrue
    private char GetGender(int r)
    {

        return currentperson.gender;
    }

    private string GetPosition()
    {

        //avoid duplicates
        int max_iterations = currentperson.positions.Count;

        int r = UnityEngine.Random.Range(0, max_iterations);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(currentperson.positions[r]))
            {
                r = UnityEngine.Random.Range(0, max_iterations);
            }
            else
            {
                break;
            }
        }
        if (currentperson.positions[r] == "links" || currentperson.positions[r] == "rechts")
        {
            recentlyUsed.Add(currentperson.positions[r + 1]);
        }
        else if (currentperson.positions[r] == "vorne links" || currentperson.positions[r] == "vorne rechts")
        {
            recentlyUsed.Add(currentperson.positions[r - 1]);
        }
        recentlyUsed.Add(currentperson.positions[r]);
        si.Position = currentperson.positions[r];
        //not giving position from position list yet
        return currentperson.positions[r];
    }

    private string GetTemplate()
    {

        if (count == 0)
        {
            recentlyUsed.Add(masterData.template[0]);
            count++;
            return masterData.template[0];
        }
        else
        {
            int r = UnityEngine.Random.Range(1, masterData.template.Length);
            //Debug.Log(masterData.template[r]);
            return masterData.template[r];
        }


    }

    private string GetMood()
    {
        //avoid duplicates
        int max_iterations = currentperson.moods.Count;

        int r = UnityEngine.Random.Range(0, max_iterations);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(currentperson.moods[r]))
            {
                r = UnityEngine.Random.Range(0, max_iterations);

            }
            else //if word is not recently used, use it.
            {
                break;
            }
        }
        recentlyUsed.Add(currentperson.moods[r]);
        if (masterData.moods.ContainsKey(currentperson.moods[r]))
        {
            si.Mood = masterData.moods[currentperson.moods[r]];
        }

        /*
        WAS MACH ICH HIER WARUM????
        //find correct adjective and insert in sentence
        if (si.Mood != default)
        {
            return currentperson.moods[r];
        }
        */

        if (si.Singular == true)
        {
            Debug.Log("IN THE IF STATEMENT");

            switch (currentperson.gender)
            {
                case 'm':
                    //Debug.Log("peron mood (m) : " + currentperson.moods[r]);
                    //Debug.Log("printed word M " + masterData.moods[currentperson.moods[r]].translation[0]);
                    return masterData.moods[currentperson.moods[r]].translation[0];

                case 'f':
                    //Debug.Log("peron mood (f): " + currentperson.moods[r]);
                    //Debug.Log("printed word F " + masterData.moods[currentperson.moods[r]].translation[1]);
                    return masterData.moods[currentperson.moods[r]].translation[1];

                case 'n':
                    //Debug.Log("peron mood (n): " + currentperson.moods[r]);
                    //Debug.Log("printed word N " + masterData.moods[currentperson.moods[r]].translation[2]);
                    return masterData.moods[currentperson.moods[r]].translation[2];

                default: return "default";
            }
        }
        else
        {
            Debug.Log(si.Singular);
            Debug.Log(masterData.moods[currentperson.moods[r]].name);
            return "default:" + masterData.moods[currentperson.moods[r]].name;
        }


    }

    private string GetAction()
    {

        int max_iterations = currentperson.actions.Count;
        if (max_iterations != 0)
        {
            //avoid duplicates
            int r = UnityEngine.Random.Range(0, max_iterations);

            for (int i = 0; i < max_iterations; i++)
            {
                if (recentlyUsed.Contains(currentperson.actions[r]))
                {
                    r = UnityEngine.Random.Range(0, max_iterations);
                }
                else
                {
                    break;
                }
            }
            //Debug.Log(currentperson.name);
            recentlyUsed.Add(currentperson.actions[r]);
            if (masterData.actions.ContainsKey(currentperson.actions[r]))
            {
                si.Action = masterData.actions[currentperson.actions[r]];
            }
            return currentperson.actions[r];
        }
        else
        {
            return "";

        }
    }

    private string GetLinkingword()
    {
        //avoid duplicates
        int max_iterations = masterData.linkingword.Length;

        int r = UnityEngine.Random.Range(0, max_iterations);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(masterData.linkingword[r]))
            {
                r = UnityEngine.Random.Range(0, max_iterations);
            }
            else
            {
                break;
            }
        }
        recentlyUsed.Add(masterData.linkingword[r]);
        return masterData.linkingword[r];

    }

    private string GetSetting()
    {
        //avoid duplicates
        int max_iterations = masterData.setting.Length;

        int r = UnityEngine.Random.Range(0, max_iterations);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(masterData.setting[r]))
            {
                r = UnityEngine.Random.Range(0, max_iterations);
            }
            else
            {
                break;
            }
        }
        recentlyUsed.Add(masterData.setting[r]);
        return masterData.setting[r];
    }


    private string GetColour()
    {
        //avoid duplicates
        int max_iterations = currentperson.colours.Count;

        int r = UnityEngine.Random.Range(0, max_iterations);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(currentperson.colours[r]))
            {
                r = UnityEngine.Random.Range(0, max_iterations);
                //Debug.Log("Getting Colour");
            }
            else
            {
                break;
            }
        }
        recentlyUsed.Add(currentperson.colours[r]);
        si.Colour = masterData.colours[currentperson.colours[r]];

        switch (si.Gender)
        {
            case 'm':
                {
                    return masterData.colours[currentperson.colours[r]].translation[0];
                }
            case 'f':
                {
                    return masterData.colours[currentperson.colours[r]].translation[1];
                }
            case 'n':
                {
                    return masterData.colours[currentperson.colours[r]].translation[2];
                }
            default:
                {
                    return masterData.colours[currentperson.colours[r]].name + "default!!";
                }
        }

    }
    #endregion

    #region UTILITY
    //-------------------------------------UTILITY------------------------------------------
    
    private void GetProbability()
    {
        //50/50 chance for plural or singular
        int r = UnityEngine.Random.Range(0, 10);
        if (r < 5) //change here to change probability distribution
        {
            probability = true;
        }
        else probability = false;
    }


    public void ClearRecentlyUsed()
    {
        recentlyUsed.Clear();
    }

    public string resolveOptions(string text)
    {
        if (text.Contains("["))
        {
            string options = Helper.FindTextBetweenTags(text, '[', ']');
            string option = pickRandomFromList(options.Split(','));
            text = Helper.ReplaceBetweenTags(text, option, '[', ']');
            // recursively fill in all options
            return resolveOptions(text);
        }
        return text;
    }

    public string pickRandomFromList(string[] list)
    {
        int random_index = UnityEngine.Random.Range(0, list.Length);
        return list[random_index];
    }

    private string ResolveVerb(string text)
    {
        string verbform;

        if (text.Contains("<"))
        {


            if (si.Singular == false && text.Contains("Gruppe"))
            {
                verbform = "ist";
            }
            else if (si.Singular == false)
            {
                verbform = "sind";
            }
            else
            {
                verbform = "ist";
            }
            text = Helper.ReplaceBetweenTags(text, verbform, '<', '>');

            return ResolveVerb(text);
        }
        return text;
    }

    private string ResolveUndefinedArticle(string text)
    {
        string article = "";

        if (text.Contains("{"))
        {
            if (si.Singular == false && text.Contains("Gruppe"))
            {
                article = "";
            }
            else if (si.Singular == false)
            {
                article = "";
            }
            else if (si.Singular == true)
            {
                if (currentperson.gender == 'f')
                {
                    article = "eine";
                }
                else
                {
                    article = "ein";
                }
                
            }
            text = Helper.ReplaceBetweenTags(text, article, '{', '}');

            return ResolveUndefinedArticle(text);
        }
        return text;
    }

    string FixLowerCase(string text)
    {
        if (Char.IsLower(text, 0))
        {
            return Char.ToUpper((char)text[0]) + text.Substring(1);
        }
        else return text;
    }

    public string pluralize(string word)
    {
        si.Singular = false;
        switch (word)
        {
            case "Baum": //,einige Bäume,ein paar Bäume
                return "[eine @mood@ Gruppe von Bäumen,einige Bäume,ein paar Bäume]";
            case "Haus": //,einige Häuser,ein paar Häuser
                return "[eine @mood@ Gruppe von Häusern,einige Häuser,ein paar Häuser]";
            case "Busch"://,einige Büsche,ein paar Büsche
                return "[eine @mood@ Gruppe von Büschen,einige Büsche,ein paar Büsche]";
            case "Wolkenkratzer":
                return "[eine @mood@ Gruppe von Wolkenkratzern,einige Wolkenkratzer, ein paar Wolkenkratzer]";
            case "Auto":
                return "einige Autos";
            default://,einige" + word + "en,ein paar " + word + "en 
                return "[eine Gruppe von " + word + "n,einige " + word + "n, ein paar " + word + "n ]";
        }
    }
    #endregion
}
