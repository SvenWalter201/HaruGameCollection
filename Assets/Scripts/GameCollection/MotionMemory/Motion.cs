using System.Collections.Generic;
using System;

[Serializable]
public class Motion
{
    public int fps;
    public List<UJoint[]> motion;
}

[Serializable]
public class Pose
{
    public string name;
    public UJoint[] pose;
}
