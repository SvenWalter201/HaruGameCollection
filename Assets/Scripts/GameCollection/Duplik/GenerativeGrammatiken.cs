using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;

public class GenerativeGrammatiken : Singleton<GenerativeGrammatiken>
{
    private const string fileName = "MasterData";
    //private Vocabulary vocabulary;
    MasterDataClass masterData = new MasterDataClass();
    public List<string> recentlyUsed = new List<string>();
    private bool probability = true; //singular
    private SentenceInformation si;
    private Subject currentperson;
    private string[] priorityKeywords;
    public Text sentenceText;
    private string sentence;
    private string template;
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
        //string filePath = appDataPath + fileName;
        if (FileManager.LoadJSONFromResources(fileName, out MasterDataClass data))
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
        template = GetTemplate();
        sentence = FillInTemplate(template);
        Helper.UpperCase(sentence);
        //Debug.Log(sentence);
        return si;
    }

    public string PrintSentence() => sentence;

    /// <summary>
    /// Creating a sentence by filling in a template with vocabulary from MasterData.json. working recursivley.
    /// </summary>
    /// <param name="template"> template which will be filled in</param>
    /// <returns> <see cref="System.String"/> instance of the template partially or fully filled in </returns>
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

    /// <summary>
    /// resolving the correct gender grammar and returning the corrected adjective form
    /// </summary>
    /// <param name="adjective"></param>
    /// <param name="gender"></param>
    /// <returns> grammarly correct adjective</returns>
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

    /// <summary>
    /// randomly selecting a "person" object from all persons of MasterData.json
    /// </summary>
    /// <returns> <see cref="System.String"/> instance of person </returns>
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

    /// <summary>
    /// randomly selecting a "animal" object from all animals of MasterData.json
    /// </summary>
    /// <returns> <see cref="System.String"/> instance of animal </returns>
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

    /// <summary>
    /// randomly selecting a "thing" object from all things of MasterData.json
    /// </summary>
    /// <returns> <see cref="System.String"/> instance of thing </returns>
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

    /// <summary>
    /// getting the gender of the current person,thing or animal
    /// </summary>
    /// <param name="r"></param>
    /// <returns> <see cref="System.Char"/> instance of gender of the current person </returns>
    private char GetGender()
    {

        return currentperson.gender;
    }

    /// <summary>
    /// randomly selecting a position for current object according to possible possitoins
    /// of current object specified in the MasterData.json file
    /// </summary>
    /// <returns> <see cref="System.String"/> instance of position </returns>
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

    /// <summary>
    /// randomly selecting a template for current sentence according to possible templates
    /// specified in the MasterData.json file
    /// </summary>
    /// <returns> <see cref="System.String"/> instance of template </returns>
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

    /// <summary>
    /// randomly selecting a mood for current object according to possible moods
    /// of current object specified in the MasterData.json file
    /// </summary>
    /// <returns> <see cref="System.String"/> instance of mood </returns>
    private string GetMood()
    {

        //avoid duplicates
        int max_iterations = currentperson.moods.Count;

        int r = UnityEngine.Random.Range(0, max_iterations);

        if (currentperson.moods.Count != 0) { 
            if (masterData.moods.ContainsKey(currentperson.moods[r]))
            {
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

                si.Mood = masterData.moods[currentperson.moods[r]];
            }
        } 

        if (si.Singular == true)
        {
            switch (currentperson.gender)
            {
                case 'm':
                    return masterData.moods[currentperson.moods[r]].translation[0];

                case 'f':
                    return masterData.moods[currentperson.moods[r]].translation[1];

                case 'n':
                    return masterData.moods[currentperson.moods[r]].translation[2];

                default: return "default";
            }
        }
        else
        {
            return ""; //masterData.moods[currentperson.moods[r]].translation[1];
        }
    }

    /// <summary>
    /// randomly selecting a action for current object according to possible actions
    /// of current object specified in the MasterData.json file
    /// </summary>
    /// <returns> <see cref="System.String"/> instance of action </returns>
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

    /// <summary>
    /// randomly selecting a linkingword according to possible linkingwords
    /// specified in the MasterData.json file
    /// </summary>
    /// <returns> <see cref="System.String"/> instance of linkingword </returns>
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

    /// <summary>
    /// randomly selecting a setting according to possible settings
    /// specified in the MasterData.json file
    /// </summary>
    /// <returns> <see cref="System.String"/> instance of setting </returns>
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


    /// <summary>
    /// randomly selecting a colour for current object according to possible colours
    /// of current object specified in the MasterData.json file
    /// </summary>
    /// <returns> <see cref="System.String"/> instance of colour </returns>
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

    /// <summary>
    /// probability distribution
    /// </summary>
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

    /// <summary>
    /// clearing the global recentlyUsed list of gameobjects painted in the picture
    /// </summary>
    public void ClearRecentlyUsed()
    {
        recentlyUsed.Clear();
    }

    /// <summary>
    /// resolving options in "[]" if the are seperated by ","
    /// </summary>
    /// <param name="text">options to choose from in "[]" seperated by ","</param>
    /// <returns> <see cref="System.String"/> instance of chosen option </returns>
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

    /// <summary>
    /// choosing a random item from a string array
    /// </summary>
    /// <param name="list"> <see cref="System.Array"/> containing <see cref="System.String"/></param>
    /// <returns> <see cref="System.String"/> instance of chosen item </returns>
    public string pickRandomFromList(string[] list)
    {
        int random_index = UnityEngine.Random.Range(0, list.Length);
        return list[random_index];
    }

    /// <summary>
    /// resolving correct verb form of to be according to grammar
    /// </summary>
    /// <param name="text"> <see cref="System.String"/> instance of sentence possibly containing "<>"</param>
    /// <returns><see cref="System.String"/> instance of correct verbform of to be</returns>
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

    /// <summary>
    /// resolving correct undefined article in a sentence containing "{}" at the position of an undefined article"
    /// </summary>
    /// <param name="text"> <see cref="System.String"/> of the sentence possibly containing "{}" </param>
    /// <returns> <see cref="System.String"/> of correct undefined article </returns>
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

    /// <summary>
    /// correct pluralization of certain words not following the normal pluralization pattern
    /// </summary>
    /// <param name="word"></param>
    /// <returns> <see cref="System.String"/> of sentence with correct pluralized word from</returns>
    public string pluralize(string word)
    {
        si.Singular = false;
        switch (word)
        {
            case "Baum": //,einige Bäume,ein paar Bäume
                return "[eine Gruppe von Bäumen,einige Bäume,ein paar Bäume]";
            case "Haus": //,einige Häuser,ein paar Häuser
                return "[eine Gruppe von Häusern,einige Häuser,ein paar Häuser]";
            case "Busch"://,einige Büsche,ein paar Büsche
                return "[eine Gruppe von Büschen,einige Büsche,ein paar Büsche]";
            case "Wolkenkratzer":
                return "[eine Gruppe von Wolkenkratzern,einige Wolkenkratzer,ein paar Wolkenkratzer]";
            case "Auto":
                si.Singular = true;
                return "Auto";
            case "Vulkan":
                si.Singular = true;
                return "Vulkan";
            default://,einige" + word + "en,ein paar " + word + "en 
                return "[eine Gruppe von " + word + "n,einige " + word + "n,ein paar " + word + "n]";
        }
    }
    #endregion
}
