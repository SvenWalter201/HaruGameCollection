using System;
using System.Collections.Generic;


[Serializable]
class MasterDataClass
{
    public string[] template;
    public string[] position;
    public string[] linkingword;
    public string[] setting;
    public string[] settingdescription;
    public List<Subject> subjects;
    public Dictionary<string, Colour> colours; //dictionary enum auf colour
    public Dictionary<string, Action> actions;
    public Dictionary<string, Mood> moods;


}

public struct Subject
{
    public string type { get; set; } //person, animal or thing
    public string name { get; set; }
    public char gender { get; set; }
    public string model { get; set; }
    public string asset { get; set; }
    public string texture { get; set; }
    public bool scalable { get; set; }
    public bool rotatable { get; set; }
    public List<string> positions { get; set; }
    public List<string> colours { get; set; } // liste an enums
    public List<string> actions { get; set; }
    public List<string> moods { get; set; }

}


public struct Colour
{
    public string type { get; set; }
    public string name { get; set; }
    public string[] translation { get; set; }
}

public struct Action
{
    public string type { get; set; }
    public string name { get; set; }
    public string model { get; set; }
    public string animation { get; set; } //path for the animation pose?
}

public struct Mood
{
    public string type { get; set; }
    public string name { get; set; }
    public string[] translation { get; set; }
    public string[] textures { get; set; }

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
