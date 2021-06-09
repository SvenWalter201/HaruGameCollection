using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringRes
{
    //"MotionMemory_GuessingPhase_Text1"
    static Dictionary<string, Dictionary<Lang, string>> stringResources;
    static readonly string filePath = "";

    public static void LoadStringResources()
    {
        string filePath = Application.dataPath + "/Resources/StringResources.json";

        if (JsonFileManager.Load(filePath, out Dictionary<string, Dictionary<Lang, string>> data))
        {
            stringResources = data;
        }
        else
            Debug.LogWarning("Couldn't load string resources");
    }

    public static string Get(string key) {
        if (stringResources.TryGetValue(key, out Dictionary<Lang, string> d))
        {
            return d[AppState.language];
        }
        else
        {
            Debug.LogWarning("StringKey was not found!");
            return null;
        }
    }

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
}

