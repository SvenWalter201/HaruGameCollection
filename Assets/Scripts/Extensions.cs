﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

public static class Extensions
{
    public static Color[] CreateColourMap(this Image image, double dpiX = 300, double dpiY = 300)
    {
        Color[] pixels;
        using (Image reference = image.Reference())
        {
            int width = reference.WidthPixels;
            int height = reference.HeightPixels;

            Memory<BGRA> mem = reference.GetPixels<BGRA>();
            pixels = new Color[width * height];
            BGRA[] colorInfo = mem.ToArray();

            //Debug.Log("R" + colorInfo[0].R + ", G: " + colorInfo[0].G + ", B: " + colorInfo[0].B + ", A: " + colorInfo[0].A);

            for (int i = 0; i < colorInfo.Length; i++)
            {
                pixels[i] = new Color32(colorInfo[i].R, colorInfo[i].G, colorInfo[i].B, colorInfo[i].A);
            }
        }

        return pixels;
    }

    public static Color[] CreateIRMap(this Image image, double dpiX = 300, double dpiY = 300)
    {
        Color[] pixels;
        using (Image reference = image.Reference())
        {
            int width = reference.WidthPixels;
            int height = reference.HeightPixels;

            Memory<ushort> mem = reference.GetPixels<ushort>();
            pixels = new Color[width * height];
            ushort[] colorInfo = mem.ToArray();

            for (int i = 0; i < colorInfo.Length; i++)
            {
                float normalizedValue = colorInfo[i] / 255f;
                pixels[i] = new Color(normalizedValue, normalizedValue, normalizedValue);
            }
        }
        return pixels;
    }
}
