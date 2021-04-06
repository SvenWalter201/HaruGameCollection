using System;
using System.Collections.Generic;
using System.Configuration;
using System.Numerics;
using Newtonsoft.Json;
using System.IO;

class Room
{
    public Dictionary<Corner, Vector3> m_Corners { get; private set; }

    public Room()
    {
        m_Corners = new Dictionary<Corner, Vector3>();

        
        try
        {
            string appSettings = File.ReadAllText(@"AppSettings.json");

            if (appSettings.Length == 0)
            {
                Console.WriteLine("AppSettings is empty.");
            }
            else
            {
                List<CornerValues> cv = JsonConvert.DeserializeObject<List<CornerValues>>(appSettings);


                for(int i = 0; i < cv.Count; i ++)
                {

                    m_Corners.Add( (Corner)cv[i].corner, StringToVec(cv[i].pos));

                }

            }
        }
        catch (Exception)
        {
            Console.WriteLine("Error reading app settings");
        }
    }



    private Corner GetEnum(string key)
    {
        switch (key)
        {
            case "LEFT_UP": return Corner.LEFT_UP;
            case "LEFT_DOWN": return Corner.LEFT_DOWN;
            case "RIGHT_UP": return Corner.RIGHT_UP; 
            case "RIGHT_DOWN": return Corner.RIGHT_DOWN; 
            default:
                throw new NotImplementedException();
                Console.WriteLine("The configuration file seems to have invalid data");
        }
    }

    private Vector3 StringToVec(string vecValue)
    {
        Vector3 vec = new Vector3();

        //Console.WriteLine(vecValue);
        //Console.ReadLine();
        string[] xyz = vecValue.Split(new Char[] { ';' });

        vec.X = float.Parse(xyz[0]);
        vec.Y = float.Parse(xyz[1]);
        vec.Z = float.Parse(xyz[2]);

        return vec;
    }


}

public enum Corner
{
    LEFT_UP,
    LEFT_DOWN,
    RIGHT_UP,
    RIGHT_DOWN
}

public class CornerValues
{
    public int corner { get; set; }
    public string pos { get; set; }

    public CornerValues(int corner, string pos)
    {
        this.corner = corner;
        this.pos = pos;
    }

    public void print()
    {
        Console.WriteLine(corner + " : " + pos);
    }
}




