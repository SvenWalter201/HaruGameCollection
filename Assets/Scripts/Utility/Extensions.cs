using System;
using System.Collections.Generic;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;
using static UnityEngine.Mathf;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
public static class Extensions
{
    public static void Assign(this LineRenderer lR, int index, Joint joint) => 
        lR.SetPosition(index, -joint.Position.ToUnityVector3() / 200f);

    public static Vector3 ToUnityVector3(this System.Numerics.Vector3 vec) => 
        new Vector3(vec.X, vec.Y, vec.Z);

    public static Quaternion ToUnityQuaternion(this System.Numerics.Quaternion q) => 
        new Quaternion(q.X, q.Y, q.Z, q.W);

    public static Vector3 ToXZVector3(this Vector2 v2) => 
        v2.ToXZVector3(0f);

    public static Vector3 ToXZVector3(this Vector2 v2, float y) => 
        new Vector3(v2.x, y, v2.y);

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

    const int 
        Blue_MAX_VALUE = 500, 
        RED_MAX_VALUE_LOWER = 1500, 
        RED_MAX_VALUE_UPPER = 2000, 
        GREEN_MAX_VALUE_LOWER = 1000, 
        GREEN_MAX_VALUE_UPPER = 1500;

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

        static float GetDepthColor(int depth, int rangeLower, int rangeUpper)
        {
            int c;

            if (depth <= rangeUpper && depth >= rangeLower)
                c = 255;
            else
                c = (depth < rangeLower) ? 
                    (500 - Abs(depth - rangeLower)) / 2 : 
                    (500 - Abs(depth - rangeUpper)) / 2;

            if (c < 0)
                c = 0;

            return c / 255f;
        }
    }


    public static Vector3 GetPerpendicular(this Vector3 v3)
    {
        int nonNullIndex = -1;
        for (int i = 0; i < 3; i++)
        {
            if(v3[i] != 0)
            {
                nonNullIndex = i;
                break;
            }
        }
        if(nonNullIndex == -1)
        {
            Debug.LogError("Passed in Vector3.zero. There exists no perpendicular vector");
            return Vector3.zero;
        }

        float cA = 0, cB = 0, cC = v3[nonNullIndex];
        switch (nonNullIndex)
        {
            case 0:
                cA = v3[1];
                cB = v3[2];
                break;
            case 1:
                cA = v3[0];
                cB = v3[2];
                break;
            case 2:
                cA = v3[0];
                cB = v3[1];
                break;
        }

        float newCC = (cA + cB) * -1;
        newCC /= cC;

        switch (nonNullIndex)
        {
            case 0:
                return new Vector3(newCC, 1, 1);
            case 1:
                return new Vector3(1, newCC, 1);
            case 2:
                return new Vector3(1, 1, newCC);
        }

        return Vector3.zero;
    }
    
    public static Vector3 Absolute(this Vector3 v3) => new Vector3(Abs(v3.x), Abs(v3.y), Abs(v3.z));

    public static Vector3 Square(this Vector3 v3) => new Vector3(v3.x * v3.x, v3.y * v3.y, v3.z * v3.z);

    public static bool IntersectSphere(this Bounds box, Vector3 sphereCenter, float sphereRadius)
    {
        // get box closest point to sphere center by clamping
        var x = Max(box.min.x, Min(sphereCenter.x, box.max.x));
        var y = Max(box.min.y, Min(sphereCenter.y, box.max.y));
        var z = Max(box.min.z, Min(sphereCenter.z, box.max.z));

        // this is the same as isPointInsideSphere
        var distance = Sqrt((x - sphereCenter.x) * (x - sphereCenter.x) +
                                 (y - sphereCenter.y) * (y - sphereCenter.y) +
                                 (z - sphereCenter.z) * (z - sphereCenter.z));


        return distance < sphereRadius;
    }

    public static bool IntersectXZCircle(this Bounds box, Vector3 circleCenter, float circleRadius)
    {
        // get box closest point to sphere center by clamping
        var x = Max(box.min.x, Min(circleCenter.x, box.max.x));
        var z = Max(box.min.z, Min(circleCenter.z, box.max.z));

        // this is the same as isPointInsideSphere
        var distance = Sqrt((x - circleCenter.x) * (x - circleCenter.x) + (z - circleCenter.z) * (z - circleCenter.z));
        Debug.Log("Distance: " + distance);

        return distance < circleRadius;
    }

    public static Bounds GetBoundingBox(this List<Vector3> v3s)
    {
        Vector3 lL = v3s[0];
        Vector3 uR = v3s[0];

        foreach (var p in v3s)
        {
            if (p.x < lL.x) lL.x = p.x; 
            if (p.y < lL.y) lL.y = p.y; 
            if (p.z < lL.z) lL.z = p.z; 
            if (p.x > uR.x) uR.x = p.x; 
            if (p.y > uR.y) uR.y = p.y; 
            if (p.z > uR.z) uR.z = p.z; 
        }

        Vector3 extents = (uR - lL) / 2f;

        //return new Bounds(lL + extents, uR - lL);

        return new Bounds
        {
            center = lL + extents,
            min = lL,
            max = uR,
            extents = extents
        };
    }
}

