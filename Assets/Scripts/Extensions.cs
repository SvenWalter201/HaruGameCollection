using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Kinect.Sensor;
using Microsoft.Azure.Kinect.BodyTracking;
using UnityEngine;
using System.Numerics;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;
using static UnityEngine.Mathf;
using Vector3 = UnityEngine.Vector3;
public static class Extensions
{
    public static void Assign(this LineRenderer lR, int index, Joint joint)
    {
        lR.SetPosition(index, -joint.Position.ToUnityVector3() / 200f);
    }


    public static Vector3 ToUnityVector3(this System.Numerics.Vector3 vec)
    {
        return new Vector3(vec.X, vec.Y, vec.Z);
    }

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

    private const int Blue_MAX_VALUE = 500;
    private const int RED_MAX_VALUE_LOWER = 1500;
    private const int RED_MAX_VALUE_UPPER = 2000;
    private const int GREEN_MAX_VALUE_LOWER = 1000;
    private const int GREEN_MAX_VALUE_UPPER = 1500;

    public static Color[] CreateDepthImage(this Capture capture, Transformation transformation)
    {
        Image img = capture.Color;
        int colourWidth = img.WidthPixels;
        int colourHeight = img.HeightPixels;

        using (Image transformedDepth = new Image(ImageFormat.Depth16, colourWidth, colourHeight, colourWidth * sizeof(ushort)))
        {
            // Transform the depth image to the colour capera perspective.
            transformation.DepthImageToColorCamera(capture, transformedDepth);

            // Get the transformed pixels (colour camera perspective but depth pixels).
            Span<ushort> depthBuffer = transformedDepth.GetPixels<ushort>().Span;

            Color[] depthPixels = img.CreateColourMap();

            // Create a new image with data from the depth and colour image.
            for (int i = 0; i < depthPixels.Length; i++)
            {
                // We'll use the colour image if the depth is less than 1 metre. 
                var depth = depthBuffer[i];

                if (depth == 0 || depth >= 2000) // No depth image.
                {
                    depthPixels[i].r = 0;
                    depthPixels[i].g = 0;
                    depthPixels[i].b = 0;
                }

                depthPixels[i].r = GetDepthColor(depth, RED_MAX_VALUE_LOWER, RED_MAX_VALUE_UPPER);
                depthPixels[i].b = GetDepthColor(depth, Blue_MAX_VALUE, Blue_MAX_VALUE);
                depthPixels[i].g = GetDepthColor(depth, GREEN_MAX_VALUE_LOWER, GREEN_MAX_VALUE_UPPER);
            }

            return depthPixels;
        }

        float GetDepthColor(int depth, int rangeLower, int rangeUpper)
        {
            int c;
            if (depth <= rangeUpper && depth >= rangeLower)
            {
                c = 255;
            }
            else
            {
                if (depth < rangeLower)
                {
                    c = (500 - Abs(depth - rangeLower)) / 2;
                }
                else
                {
                    c = (500 - Abs(depth - rangeUpper)) / 2;
                }
            }

            if (c < 0)
            {
                c = 0;
            }
            return c / 255f;
        }

        //depthPixels = img.CreateColourMap();   
    }

    
    public static Vector3 Absolute(this Vector3 v3)
    {
        return new Vector3(Abs(v3.x), Abs(v3.y), Abs(v3.z));
    }

    public static Vector3 Square(this Vector3 v3)
    {
        return new Vector3(v3.x * v3.x, v3.y * v3.y, v3.z * v3.z);
    }
}

