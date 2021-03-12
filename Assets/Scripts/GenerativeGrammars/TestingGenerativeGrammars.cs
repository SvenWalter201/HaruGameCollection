using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingGenerativeGrammars : MonoBehaviour
{

    public void GenerateSentence()
    {
        string sentence = FillInTemplate("<at> the @position@ there <is> <a> @object@");
        Debug.Log(sentence);
    }

    public string FillInTemplate(string template) {
        if (template.Contains("@")) {
            string generator = FindTextBetween(template, '@');
            string replacement = "";

            switch (generator)
            {
                case "person":
                    {
                        replacement = "randomPerson";
                        break;
                    }
                case "position":
                    {
                        replacement = "left";
                        break;
                    }
                case "animal":
                    {
                        replacement = "randomAnimal";
                        break;
                    }
                case "object":
                    {
                        replacement = "tree";
                        break;
                    }
            }

            template = ReplaceBetweenTags(template, replacement, '@');
            return FillInTemplate(template);
        }
        return template;
    }


    public string ReplaceBetweenTags(string template, string replacement, char tag)
    {
        return ReplaceBetweenTags(template, replacement, tag,  tag);
    }

    public string ReplaceBetweenTags(string template, string replacement, char leftTag, char rightTag)
    {
        int leftIndex = template.IndexOf(leftTag);
        if (leftIndex == -1)
        {
            throw new System.Exception("no '" + leftTag + "'-tags found in the string " + template);
        }
        int rightIndex = template.IndexOf(rightTag, leftIndex+1);
        if (rightIndex == -1)
        {
            throw new System.Exception("uneven numbers of '" + leftTag + "'-characters found in the sentence " + template);
        }
        int lengthFromRightIndex = template.Length - 1 - rightIndex;

        return template.Substring(0, leftIndex) + replacement + template.Substring(rightIndex + 1, lengthFromRightIndex);
    }

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
}
