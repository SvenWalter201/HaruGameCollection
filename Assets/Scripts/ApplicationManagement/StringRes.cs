using System.Collections.Generic;
using UnityEngine;

public static class StringRes
{
    static Dictionary<string, Dictionary<Lang, string>> stringResources;
    static string fileName = "StringResources";

    public static bool LoadStringResources()
    {
        if (FileManager.LoadJSONFromResources(fileName, out Dictionary<string, Dictionary<Lang, string>> data))
        {
            stringResources = data;
            return true;
        }
        else 
        {
            Debug.LogWarning("Couldn't load string resources");
            return false;
        }
    }

    public static string Get(string key) {
        if (stringResources.TryGetValue(key, out Dictionary<Lang, string> d))
            return d[AppManager.language];
        
        else
        {
            Debug.LogWarning("StringKey was not found!");
            return null;
        }
    }

    /*
    public static void SaveSample()
    {
        Dictionary<Lang, string> d = new Dictionary<Lang, string>();
        d.Add(Lang.ENG, "Welcome");
        d.Add(Lang.DE, "Willkommen");

        Dictionary<string, Dictionary<Lang, string>> sampleResources = new Dictionary<string, Dictionary<Lang, string>>();
        sampleResources.Add("_WelcomeText", d);

        string filePath = Application.dataPath + "/Resources/StringResources.json";
        JsonFileManager.Save(filePath, sampleResources);
    }
    */
}

