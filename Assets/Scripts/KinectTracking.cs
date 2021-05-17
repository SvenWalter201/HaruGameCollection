using System;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;

public class KinectTracking : MonoBehaviour
{
    public int width;
    public int height;
    public Color[] pixels;

    public bool showColorImage;
    public bool showDepthImage;


    public void Init()
    {
        KinectDeviceManager.Instance.Init();
    }

    public void Close()
    {
        KinectDeviceManager.Instance.Close(null, null);
    }

    public void StartTrackingRGBFrame()
    {
        ImageReceiver.Instance.StartTracking();
        ImageReceiver.Instance.FrameArrivedEvent += ReceiveImage;
    }

    public void StopTrackingRGBFrame()
    {
        ImageReceiver.Instance.FrameArrivedEvent -= ReceiveImage;
        ImageReceiver.Instance.StopTracking();
    }

    public void ShowImage()
    {
        Texture2D tex = TextureGenerator.TextureFromColourMap(pixels, width, height);
        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = tex;
    }

    public void ReceiveImage(object sender, ColorFrameArrivedEventArgs e)
    {
        Image image;
        if (showColorImage)
        {
            image = e.colorImage;
            if (image == null)
            {
                Debug.Log("RGBImage was null");
                return;
            }
            width = image.WidthPixels;
            height = image.HeightPixels;

            Memory<BGRA> mem = image.GetPixels<BGRA>();
            pixels = new Color[width * height];
            BGRA[] colorInfo = mem.ToArray();

            for (int i = 0; i < colorInfo.Length; i++)
            {
                pixels[i] = new Color(colorInfo[i].R, colorInfo[i].G, colorInfo[i].B, colorInfo[i].A);
            }
        }
        else if (showDepthImage)
        {

            image = e.depthImage;
            if (image == null)
            {
                return;
            }
            width = image.WidthPixels;
            height = image.HeightPixels;

            Memory<ushort> mem = image.GetPixels<ushort>();
            pixels = new Color[width * height];
            ushort[] colorInfo = mem.ToArray();

            for (int i = 0; i < colorInfo.Length; i++)
            {
                pixels[i] = new Color(colorInfo[i], colorInfo[i], colorInfo[i], 255);
            }
        }


        
    }
}
