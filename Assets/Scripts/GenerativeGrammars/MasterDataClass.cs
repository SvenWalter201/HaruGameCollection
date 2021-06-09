using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


[Serializable]
class MasterDataClass
{
    public string[] template;
    public string[] position;
    public string[] linkingword;
    public string[] setting;
    public string[] settingdescription;
    public List<subject> subjects;
    public Dictionary<string, colour> colours; //dictionary enum auf colour
    public Dictionary<string, action> actions;
    public Dictionary<string, mood> moods;


}

public struct subject
{
    public string type { get; set; } //person, animal or thing
    public string name { get; set; }
    public char gender { get; set; }
    public string model { get; set; }
    public string texture { get; set; }
    public bool scalable { get; set; }
    public bool rotatable { get; set; }
    public List<string> positions { get; set; }
    public List<string> colours { get; set; } // liste an enums
    public List<string> actions { get; set; }
    public List<string> moods { get; set; }

}


public struct colour
{
    public string type { get; set; }
    public string name { get; set; }
    public string[] translation { get; set; }
}

public struct action
{
    public string type { get; set; }
    public string name { get; set; }
    public string model { get; set; }
    public string animation { get; set; } //path for the animation pose?
}

public struct mood
{
    public string type { get; set; }
    public string name { get; set; }
    public string[] translation { get; set; }

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
