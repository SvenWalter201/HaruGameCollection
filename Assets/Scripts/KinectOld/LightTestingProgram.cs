using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Threading;
using UnityEngine;
using Color = UnityEngine.Color;


class LightTestingProgram : IKinectProgram
{
    public void Execute()
    {
        LightManager lm = LightManager.Instance;

        //Thread.Sleep(2000);
        lm.SetLights(Command.OFF, null, Color.white);
        while (true)
        {
            Console.WriteLine("Select Color");
            Console.WriteLine("[0] Red");
            Console.WriteLine("[1] Blue");
            Console.WriteLine("[2] Yellow");
            Console.WriteLine("[3] Green");
            Console.WriteLine("[4] White");

            string input = Console.ReadLine();
            
            Color color;

            switch (input)
            {
                case "0":
                    {
                        color = Color.red;
                        lm.SetLights(Command.ON, new List<string> { "13" }, color);
                        break;
                    }
                case "1":
                    {
                        color = Color.blue;
                        lm.SetLights(Command.ON, new List<string> { "14" }, color);
                       break;
                    }
                case "2":
                    {
                        color = Color.yellow;
                        lm.SetLights(Command.ON, new List<string> { "17" }, color);
                        break;
                    }
                case "3":
                    {
                        color = Color.green;
                        lm.SetLights(Command.ON, null, color);
                        break;
                    }
                case "4":
                    {
                        color = Color.white;
                        lm.SetLights(Command.ON, null, color);
                        break;
                    }
            }

            Console.WriteLine("Turn off");
            Console.ReadLine();
            lm.SetLights(Command.OFF, null, Color.white);
        }
    }
}

