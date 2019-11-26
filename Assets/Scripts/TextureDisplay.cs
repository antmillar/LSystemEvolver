using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//applies noise map to a texture
public static class TextureDisplay {

    public static void applyTexture(Color[] colorMap, RawImage raw, int w, int h) {

        Texture2D texture = new Texture2D(w, h);
        texture.SetPixels(colorMap);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        //raw.transform.localScale = new Vector3(w, h, 1);

        raw.texture = texture;

    }

}
