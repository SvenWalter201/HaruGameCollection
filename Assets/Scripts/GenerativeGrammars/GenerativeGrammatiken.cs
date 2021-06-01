using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;

public class GenerativeGrammatiken : Singleton<GenerativeGrammatiken>
{
    private const string fileName = "/Resources/Daten.json";
    private Vocabulary vocabulary;
    public List<string> recentlyUsed = new List<string>();
    private bool probability = true; //singular
    private SentenceInformation si;
    public Text sentenceText;
    private string sentence;
    public byte count = 0;

    private void Start()
    {
        ClearRecentlyUsed();
        GetVocabulary();

    }
    private void GetVocabulary()
    {
        string appDataPath = Application.dataPath;
        string filePath = appDataPath + fileName;
        string jsonString = File.ReadAllText(filePath);
        vocabulary = JsonConvert.DeserializeObject<Vocabulary>(jsonString);
    }

    public SentenceInformation GenerateSentence()
    {
        Debug.Log("Generating Sentence??");
        GetProbability();
        si = new SentenceInformation();
        si.ClearInformation();
        string template = GetTemplate();
        sentence = FillInTemplate(template);
        //Print to UI
        //sentenceText.text = sentence;
        //Debug.Log(sentence);
        return si;
    }

    public string FillInTemplate(string template)
    {
        if (template.Contains("@"))
        {
            string generator = FindTextBetween(template, '@');
            string replacement = "NO REPLACEMENT FOUND";
            List<string> parameters = new List<string>();

            switch (generator)
            {
                case "person":
                    {
                        replacement = GetPerson();
                        si.Singular = true;
                        break;
                    }
                case "animal":
                    {
                        replacement = GetAnimal();
                        break;
                    }
                case "position":
                    {
                        replacement = GetPosition();
                        break;
                    }
                case "thing":
                    {
                        replacement = GetThing();
                        si.Singular = true;
                        Debug.Log(probability + " true für singular, false für plural");
                        if (!probability)
                        {
                            replacement = pluralize(replacement);
                        }
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

            template = ReplaceBetweenTags(template, replacement, '@');
            return FillInTemplate(template);
        }

        //------------------------Grammatikalische Korrekturen---------------------

        if (template.Contains("["))
        {
            template = resolveOptions(template);
        }
        
        //Verbform
        if (template.Contains("<"))
        {
            template = ResolveVerb(template);
        }

        //unbestimmter Artikel
        if (template.Contains("{"))
        {
            template = ResolveUndefinedArticle(template);
        }


        //Debug.Log(si.PrintToString());

        return template;
        
    }



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

    private string GetAnimal()
    {
        //avoid duplicates
        int max_iterations = vocabulary.animal.Length;

        int r = UnityEngine.Random.Range(0, vocabulary.animal.Length);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(vocabulary.animal[r]))
            {
                r = UnityEngine.Random.Range(0, vocabulary.animal.Length);
            }
            else
            {
                break;
            }
        }
        recentlyUsed.Add(vocabulary.animal[r]);
        si.Gender = vocabulary.animal[r][vocabulary.animal[r].Length - 1];
        //Debug.Log(si.Gender);
        si.Person = vocabulary.animal[r].Substring(0, vocabulary.animal[r].Length - 2);
        si.Singular = true;
        return vocabulary.animal[r].Substring(0, vocabulary.animal[r].Length - 2);
    }


    private string GetThing()
    {
        //avoid duplicates
        int max_iterations = vocabulary.thing.Length;

        int r = UnityEngine.Random.Range(0, vocabulary.thing.Length);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(vocabulary.thing[r]))
            {
                r = UnityEngine.Random.Range(0, vocabulary.thing.Length);
            }
            else
            {
                break;
            }
        }
        recentlyUsed.Add(vocabulary.thing[r]);
        si.Gender = vocabulary.thing[r][vocabulary.thing[r].Length - 1];
        si.Person = vocabulary.thing[r].Substring(0, vocabulary.thing[r].Length - 2);
        return vocabulary.thing[r].Substring(0, vocabulary.thing[r].Length - 2);
    }

    //returns GENDER, adding gender to sentenceStructrue
    private char GetGender(int r)
    {
        
        return vocabulary.person[r][vocabulary.person[r].Length - 1];
    }

    private string GetPosition()
    {

        //avoid duplicates
        int max_iterations = vocabulary.position.Length;

        int r = UnityEngine.Random.Range(0, vocabulary.position.Length);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(vocabulary.position[r]))
            {
                r = UnityEngine.Random.Range(0, vocabulary.position.Length);
            }
            else
            {
                break;
            }
        }
        if(vocabulary.position[r] == "links" || vocabulary.position[r] == "rechts")
        {
            recentlyUsed.Add(vocabulary.position[r+1]);
        }else if(vocabulary.position[r] == "vorne links" || vocabulary.position[r] == "vorne rechts")
        {
            recentlyUsed.Add(vocabulary.position[r - 1]);
        }
        recentlyUsed.Add(vocabulary.position[r]);
        si.Position = vocabulary.position[r];
        return vocabulary.position[r];
    }

    private string GetTemplate()
    {
        //"@position@ <ist> <ein> @mood@ @thing@.",
        
        if(count == 0)
        {
            recentlyUsed.Add(vocabulary.template[0]);
            //Debug.Log(vocabulary.template[0]);
            count++;
            return vocabulary.template[0];
        }
        else
        {
            int r = UnityEngine.Random.Range(1, vocabulary.template.Length);
            //Debug.Log(vocabulary.template[r]);
            return vocabulary.template[r];
        }
    }

    private string GetPerson()
    {
        //avoid duplicates
        int max_iterations = vocabulary.person.Length;

        int r = UnityEngine.Random.Range(0, vocabulary.person.Length);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(vocabulary.person[r]))
            {
                r = UnityEngine.Random.Range(0, vocabulary.person.Length);
            }
            else
            {
                break;
            }
        }
        recentlyUsed.Add(vocabulary.person[r]);
        si.Gender = vocabulary.person[r][vocabulary.person[r].Length - 1];
        si.Person = vocabulary.person[r].Substring(0, vocabulary.person[r].Length - 2);
        //Debug.Log(si.Gender);
        return vocabulary.person[r].Substring(0, vocabulary.person[r].Length - 2);
    }

    private string GetMood()
    {
        //avoid duplicates
        int max_iterations = vocabulary.mood.Length;

        int r = UnityEngine.Random.Range(0, vocabulary.mood.Length);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(vocabulary.mood[r]))
            {
                r = UnityEngine.Random.Range(0, vocabulary.mood.Length);
                
            }
            else //falls das word nicht recently used, benutz es.
            {
                break;
            }
        }
        recentlyUsed.Add(vocabulary.mood[r]);
        si.Mood = FindTextBetween(vocabulary.mood[r], '[', ']').Split(',')[0];        


        //find correct adjective and insert in sentence
        if (si.Action != null)
        {
            return FindTextBetween(vocabulary.mood[r], '[', ']').Split(',')[0];
        }

        
        if(si.Singular == true)
        {

            switch (si.Gender)
            {
                case 'm':
                    {
                        return FindTextBetween(vocabulary.mood[r], '[', ']').Split(',')[1];
                        
                    }
                case 'f':
                    {
                        return FindTextBetween(vocabulary.mood[r], '[', ']').Split(',')[2];
                        
                    }
                case 'n':
                    {
                        return FindTextBetween(vocabulary.mood[r], '[', ']').Split(',')[3];
                        
                    }
                default: return FindTextBetween(vocabulary.mood[r], '[', ']').Split(',')[0];
            }
        }
        else
        {
            return FindTextBetween(vocabulary.mood[r], '[', ']').Split(',')[2];
        }
        
        
    }

    
    private string GetColour()
    {
        //avoid duplicates
        int max_iterations = vocabulary.colour.Length;

        int r = UnityEngine.Random.Range(0, max_iterations);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(vocabulary.colour[r]))
            {
                r = UnityEngine.Random.Range(0, vocabulary.colour.Length);
                //Debug.Log("Getting Colour");
            }
            else
            {
                break;
            }
        }

        recentlyUsed.Add(vocabulary.colour[r]);
        si.Colour = FindTextBetween(vocabulary.colour[r], '[', ']').Split(',')[1];

        switch (si.Gender)
        {
            case 'm':
                {
                    return FindTextBetween(vocabulary.colour[r], '[', ']').Split(',')[0];
                }
            case 'f':
                {
                    return FindTextBetween(vocabulary.colour[r], '[', ']').Split(',')[1];
                }
            case 'n':
                {
                    return FindTextBetween(vocabulary.colour[r], '[', ']').Split(',')[2];
                }
            default:
                {
                    return FindTextBetween(vocabulary.colour[r], '[', ']').Split(',')[2] + "default!!";
                }
        }
    }

    private string GetAction()
    {
        //avoid duplicates
        int max_iterations = vocabulary.action.Length;

        int r = UnityEngine.Random.Range(0, vocabulary.action.Length);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(vocabulary.action[r]))
            {
                r = UnityEngine.Random.Range(0, vocabulary.action.Length);
            }
            else
            {
                break;
            }
        }
        recentlyUsed.Add(vocabulary.action[r]);
        si.Action = vocabulary.action[r];
        return vocabulary.action[r];
    }

    private string GetLinkingword()
    {
        //avoid duplicates
        int max_iterations = vocabulary.linkingword.Length;

        int r = UnityEngine.Random.Range(0, vocabulary.linkingword.Length);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(vocabulary.linkingword[r]))
            {
                r = UnityEngine.Random.Range(0, vocabulary.linkingword.Length);
            }
            else
            {
                break;
            }
        }
        recentlyUsed.Add(vocabulary.linkingword[r]);
        return vocabulary.linkingword[r];

        /*
        int r = UnityEngine.Random.Range(0, vocabulary.linkingword.Length);
        while (recentlyUsed.Contains(vocabulary.linkingword[r]))
        {
            r = UnityEngine.Random.Range(0, vocabulary.linkingword.Length);
        }
        recentlyUsed.Add(vocabulary.linkingword[r]);
        return vocabulary.linkingword[r];
        */
    }

    private string GetSetting()
    {
        //avoid duplicates
        int max_iterations = vocabulary.setting.Length;

        int r = UnityEngine.Random.Range(0, vocabulary.setting.Length);

        for (int i = 0; i < max_iterations; i++)
        {
            if (recentlyUsed.Contains(vocabulary.setting[r]))
            {
                r = UnityEngine.Random.Range(0, vocabulary.setting.Length);
            }
            else
            {
                break;
            }
        }
        recentlyUsed.Add(vocabulary.setting[r]);
        return vocabulary.setting[r];
    }



    // replacing at <<replacement>> in the template
    public string ReplaceBetweenTags(string template, string replacement, char tag)
    {
        return ReplaceBetweenTags(template, replacement, tag, tag);
    }

    public string ReplaceBetweenTags(string template, string replacement, char leftTag, char rightTag)
    {
        int leftIndex = template.IndexOf(leftTag);
        if (leftIndex == -1)
        {
            throw new System.Exception("no '" + leftTag + "'-tags found in the string " + template);
        }
        int rightIndex = template.IndexOf(rightTag, leftIndex + 1);
        if (rightIndex == -1)
        {
            throw new System.Exception("uneven numbers of '" + leftTag + "'-characters found in the sentence " + template);
        }
        int lengthFromRightIndex = template.Length - 1 - rightIndex;

        return template.Substring(0, leftIndex) + replacement + template.Substring(rightIndex + 1, lengthFromRightIndex);
    }


    //-------------------------------------------------------------------

    //finding a string between certain tags
    public string FindTextBetween(string text, char tag)
    {
        return FindTextBetween(text, tag, tag);
    }

    public string FindTextBetween(string text, char left, char right)
    {
        int beginIndex = text.IndexOf(left);
        if (beginIndex == -1)
        {
            throw new System.Exception("no tags found in the string");
        }

        beginIndex++;

        int endIndex = text.IndexOf(right, beginIndex);
        if (endIndex == -1)
        {
            throw new System.Exception("uneven numbers of '" + left + "'-characters found in the sentence " + text);
        }

        return text.Substring(beginIndex, endIndex - beginIndex).Trim();
    }

    public string FindTextBetweenTags(string text, char left, char right)
    {
        int beginIndex = text.IndexOf(left);
        if (beginIndex == -1)
        {
            throw new System.Exception("no tags found in the string");
        }

        beginIndex++;

        int endIndex = text.IndexOf(right, beginIndex);
        if (endIndex == -1)
        {
            throw new System.Exception("uneven numbers of '" + left + "'-characters found in the sentence " + text);
        }

        return text.Substring(beginIndex, endIndex - beginIndex).Trim();
    }
    //-----------------------------------------------------------------------------------

    // ------------------------------------ UTILITY ------------------------------------

    // Returns list of lines between #category_name: and #end in the data file
    /*
    private string getCategory(category_name)
    {
        let start_tag = `#${category_name}:\n`;
	    let end_tag = '\n#end';
        return getTextBetweenTags(data, start_tag, end_tag).split('\n');
    }*/


    public string PrintSentence()
    {
        Debug.Log(sentence);
        return sentence;
    }

    public void ClearRecentlyUsed()
    {
        recentlyUsed.Clear();
    }

    // Replace comma-separated entries inside square brackets with random entry
    public string resolveOptions(string text)
    {
        if (text.Contains("["))
        {
            string options = FindTextBetweenTags(text, '[', ']'); 
            string option = pickRandomFromList(options.Split(','));
            text = ReplaceBetweenTags(text, option, '[', ']');
            // recursively fill in all options
            return resolveOptions(text);
        }
        return text;
    }

    private string ResolveVerb(string text)
    {
        string verbform;

        if (text.Contains("<")) {


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
            text = ReplaceBetweenTags(text, verbform, '<', '>');

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
            else if(si.Singular == true)
            {
                if(si.Gender == 'f')
                {
                    article = "eine";
                }
                article = "ein";
            }
            text = ReplaceBetweenTags(text, article, '{', '}');

            return ResolveVerb(text);
        }
        return text;
    }

    private void GetProbability()
    {
        //50/50 chance for plural or singular
        int r = UnityEngine.Random.Range(0, 10);
        if (r < 5)
        {
            probability = true;
        }
        else probability = false;
    }

    public string pickRandomFromList(string[] list)
    {
        int random_index = UnityEngine.Random.Range(0, list.Length);
        return list[random_index];
    }

    //picks a random parameter from the specified category you want to get the parameter from
    //public string pickRandom(string category_name)

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

}

public enum Position
{
    UNTEN,
    OBEN,
    RECHTS,
    LINKS,
    VORDERGUND,
    HINTERGRUND
}