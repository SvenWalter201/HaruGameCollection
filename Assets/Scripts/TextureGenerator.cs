using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TextureGenerator
{
    public static Texture2D TextureFromColourMap(Color[] colorMap, int width, int height)
    {
        if (width == 0 || height == 0)
        {
            return Texture2D.whiteTexture;
        }

        if (colorMap.Length < (width * height))
        {
            return Texture2D.whiteTexture;
        }

        Texture2D texture = new Texture2D(width, height);


        texture.SetPixels(colorMap);
        texture.Apply();

        //texture.filterMode = FilterMode.Bilinear;
        //texture.wrapMode = TextureWrapMode.Clamp;

        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColourMap(colorMap, width, height);
    }

    public static Texture2D TextureFromHeightMap(int[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float value = heightMap[x, y] / 255f;
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, value);
            }
        }

        return TextureFromColourMap(colorMap, width, height);
    }
}
