using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise
{
    public enum NormalizeMode
    {
        Local, Global
    };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
    {
        if (scale <= 0)
        {
            scale = 0.0001f;
        }
        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }
        float[,] noiseMap = new float[mapWidth, mapHeight];
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;
                // obliczanie wartosci szumu dla piksela
                for (int k = 0; k < octaves; k++)
                {

                    float temp_x = (j - halfWidth + +octaveOffsets[k].x) / scale * frequency;
                    float temp_y = (i - halfHeight + octaveOffsets[k].y) / scale * frequency;
                    float perlinValue = Mathf.PerlinNoise(temp_x, temp_y) * 2 - 1;

                    //noiseMap[j, i] = perlinValue;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxLocalNoiseHeight) maxLocalNoiseHeight = noiseHeight;
                else if (noiseHeight < minLocalNoiseHeight) minLocalNoiseHeight = noiseHeight;
                noiseMap[j, i] = noiseHeight;
            }
        }
        // normalizacja szumu
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                if (normalizeMode == NormalizeMode.Local)
                    noiseMap[j, i] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[j, i]);
                else
                {
                    float normalizedHeight = (noiseMap[j, i] + 1) / (maxPossibleHeight);
                    noiseMap[j, i] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }
        return noiseMap;
    }

}
