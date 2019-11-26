using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//generates the a random noise map for the textures
public class TextureNoise {
    public static Color[] CreateNoise(int w, int h, System.Random r)
    {
   
        float[,] noiseMap = new float[w,h];
        Color[] colorMap = new Color[w * h];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                //picks a random number between 0 and 250 in increments of 25
                noiseMap[x, y] = r.Next(0, 10) / 10f;
                
                float n = noiseMap[x, y];

                colorMap[x + y * w] = new Color(n, n, n);
            }
        }

        return colorMap;
    }
}
