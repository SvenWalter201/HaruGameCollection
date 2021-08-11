using System;

public static class Helper
{
    public static string ReplaceBetweenTags(string template, string replacement, char tag)
    {
        return ReplaceBetweenTags(template, replacement, tag, tag);
    }

    public static string UpperCase(string sentence)
    {
        if(sentence[0].Equals(" "))
        {
            return UpperCase(sentence.Substring(1));
        }
        else if (Char.IsLower(sentence[0]))
        {
            return  Char.ToUpper( sentence[0]) + sentence.Substring(1);
        }
        else
        {
            return sentence;
        }
    }

    public static string ReplaceBetweenTags(string template, string replacement, char leftTag, char rightTag)
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


    public static string ReplaceWordBetweenTags(string template, string word, string replacement)
    {
        int wordIndex = template.IndexOf(word);
        //Console.WriteLine(template.Substring(0, wordIndex - 1) + replacement + template.Substring(wordIndex + word.Length +1));
        return template.Substring(0, wordIndex - 1) + replacement + template.Substring(wordIndex + word.Length +1);
    }


    //finding a string between certain tags
    public static string FindTextBetween(string text, char tag)
    {
        return FindTextBetween(text, tag, tag);
    }

    public static string FindTextBetween(string text, char left, char right)
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
    public static string FindTextBetween(string text, char tag, int index)
    {
        return FindTextBetween(text.Substring(index), tag, tag);
    }
    public static string FindTextBetweenTags(string text, char left, char right)
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

    public static string LookForPriority(string text, string[] prioKeywords)
    {
        for (int i = 0; i < prioKeywords.Length; i++)
        {
            if (text.Contains(prioKeywords[i]))
            {
                return Helper.FindTextBetween(text, '@', text.IndexOf(prioKeywords[i]) - 1);
            }
            else
            {
                continue;
            }
        }
        return Helper.FindTextBetween(text, '@');
    }

    /*retrun
     * index of second time a char "tag" is seen in string "text"
     */
    static int SubstringOfSecond(string text, char tag)
    {
        int first = text.IndexOf(tag);
        return text.Substring(first).IndexOf('@');
    }
}