using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using System.Threading.Tasks;
using Q42.HueApi.Converters;
using Q42.HueApi.ColorConverters.Original;
using Q42.HueApi.ColorConverters.Gamut;
using Q42.HueApi.ColorConverters.HSB;
using System.Drawing;
using UnityEngine;
using Light = Q42.HueApi.Light;

class LightManager
{
	private const string APPLICATION = "HaruXSmarthome";
	private const string DEVICE_NAME = "Philips_hue";
	private const string IP = "192.168.178.23";
	public List<string> availableIndizes = new List<string>();

	private static readonly LightManager instance = new LightManager();

	static LightManager()
	{
	}
	private LightManager()
	{

	}
	public static LightManager Instance
	{
		get
		{
			return instance;
		}
	}
	
	private string appKey;
	private ILocalHueClient client;
	public async void Init()
	{

		IBridgeLocator locator = new HttpBridgeLocator(); //Or: LocalNetworkScanBridgeLocator, MdnsBridgeLocator, MUdpBasedBridgeLocator
		var bridges = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
		List<Q42.HueApi.Models.Bridge.LocatedBridge> list = bridges.ToList();


		//Advanced Bridge Discovery options:
		//bridges = await HueBridgeDiscovery.CompleteDiscoveryAsync(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));
		//bridges = await HueBridgeDiscovery.FastDiscoveryWithNetworkScanFallbackAsync(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));
		//bridges = await HueBridgeDiscovery.CompleteDiscoveryAsync(TimeSpan.FromSeconds(5));


		//register new client
		client = new LocalHueClient(IP);

        while (true)
        {
			Console.WriteLine("Please press the button on the Hue Bridge, then press any key to continue");
			Console.ReadKey();
			try
            {
				//Make sure the user has pressed the button on the bridge before calling RegisterAsync
				//It will throw an LinkButtonNotPressedException if the user did not press the button
				appKey = await client.RegisterAsync(APPLICATION, DEVICE_NAME);
				break;
            }
			catch(LinkButtonNotPressedException e)
            {
				Console.WriteLine(e.Message);
            }
        }


		client.Initialize(appKey);
		Console.WriteLine("Connected");
		RoomConfigurationProgram.inited = true;
	}

	public int GetLamps()
    {
		var lights = client.GetLightsAsync();
		var result = lights.Result;
		List<Light> l = result.ToList();
		
		for (int i = 0; i < l.Count; i++)
		{
			Light current = l[i];
			bool? state = current.State.IsReachable;
			if (state != null)
			{
				bool stateNonNull = (bool)state;
				if (stateNonNull)
				{
					availableIndizes.Add(current.Id);
				}
			}
		}
		return availableIndizes.Count;
		//Save the app key for later use
	}

	public void TurnOffLights()
    {
		TurnOffLights(null);
	}

	public void TurnOffLights(List<string> lamps)
    {
		var command = new LightCommand
		{
			On = true
		};
		command.TurnOff();
		client.SendCommandAsync(command, lamps);
	}

	public void SetLights(Command cmd, Color color)
    {
		SetLights(cmd, null, color);
	}

	public void SetLights(Command cmd, List<string> lamps, Color color)
    {
	
		var command = new LightCommand
		{
			On = true
		};

		switch (cmd)
        {
			case Command.ON:
                {
					List<double> xyColor = GetRGBtoXY(color);
					command.TurnOn().SetColor(xyColor[0], xyColor[1]);
					break;
                }
			case Command.OFF:
                {
					command.TurnOff();
					break;
                }
			case Command.DIM:
                {
					command.Brightness = 40;
					break;
                }
			case Command.BRIGHTEN:
                {
					command.Brightness = 255;
					break;
                }
		}
		client.SendCommandAsync(command, lamps);
	}

	public List<double> GetRGBtoXY(Color c)
	{
		// For the hue bulb the corners of the triangle are:
		// -Red: 0.675, 0.322
		// -Green: 0.4091, 0.518
		// -Blue: 0.167, 0.04
		double[] normalizedToOne = new double[3];

		normalizedToOne[0] = c.r / (float)255;
		normalizedToOne[1] = c.g / (float)255;
		normalizedToOne[2] = c.b / (float)255;

		float red, green, blue;

		// Make red more vivid
		if (normalizedToOne[0] > 0.04045)
		{
			red = (float)Math.Pow((normalizedToOne[0] + 0.055) / 1.055, 2.4);
		}
		else
		{
			red = (float)(normalizedToOne[0] / 12.92);
		}

		// Make green more vivid
		if (normalizedToOne[1] > 0.04045)
		{
			green = (float)Math.Pow((normalizedToOne[1] + 0.055) / 1.055, 2.4);
		}
		else
		{
			green = (float)(normalizedToOne[1] / 12.92);
		}

		// Make blue more vivid
		if (normalizedToOne[2] > 0.04045)
		{
			blue = (float)Math.Pow((normalizedToOne[2] + 0.055) / 1.055, 2.4);
		}
		else
		{
			blue = (float)(normalizedToOne[2] / 12.92);
		}

		float X = (float)(red * 0.649926 + green * 0.103455 + blue * 0.197109);
		float Y = (float)(red * 0.234327 + green * 0.743075 + blue * 0.022598);
		float Z = (float)(red * 0.0000000 + green * 0.053077 + blue * 1.035763);

		float x = X / (X + Y + Z);
		float y = Y / (X + Y + Z);

		double[] xy = new double[]{x,y};
		return xy.OfType<double>().ToList();
	}
}

public enum Command
{
	DEFAULT,
	FLASH_ONCE,
	BLINK,
	INCREMENT,
	ON,
	OFF,
	DIM,
	BRIGHTEN

}