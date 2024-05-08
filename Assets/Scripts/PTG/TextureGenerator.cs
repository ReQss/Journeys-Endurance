using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture2D = new Texture2D(width, height);
        texture2D.filterMode = FilterMode.Point;
        texture2D.wrapMode = TextureWrapMode.Clamp;
        texture2D.SetPixels(colourMap);
        texture2D.Apply();
        return texture2D;
    }
    public static Texture2D TextureFromHeightMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Texture2D texture2D = new Texture2D(width, height);
        Color[] colourMap = new Color[width * height];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                colourMap[i * width + j] = Color.Lerp(Color.black, Color.white, noiseMap[j, i]);
            }


        }
        texture2D.SetPixels(colourMap);
        texture2D.Apply();
        return TextureFromColourMap(colourMap, width, height);
    }
}
