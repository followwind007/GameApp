using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Pangu.GUIs;

public class TestTextureTools : MonoBehaviour
{
    public Texture2D tex1;
    public Texture2D tex2;

    public RawImage img;

    float _x = 0;
    float _y = 0;

    private void OnGUI()
    {
        GUILayout.Space(30);
        _x = GUILayout.HorizontalSlider(_x, -128, 1334, GUILayout.Width(300));
        GUILayout.Space(30);
        _y = GUILayout.HorizontalSlider(_y, -128, 750, GUILayout.Width(300));
        GUILayout.Space(30);

        if (GUILayout.Button("Overlay", GUILayout.Width(200), GUILayout.Height(40)))
        {
            Texture tex = TextureTools.OverlayWithTexture(tex1, tex2, new Vector2(_x, _y));
            img.texture = tex;
        }

        if (GUILayout.Button("Overlay Polygon", GUILayout.Width(200), GUILayout.Height(40)))
        {
            Vector2[] polygon = new Vector2[] 
            {
                new Vector2(0.1f, 0.1f),
                new Vector2(0.2f, 0.5f),
                new Vector2(0.1f, 0.9f),
                new Vector2(0.9f, 0.9f),
                new Vector2(0.9f, 0.1f),
            };
            Texture tex = TextureTools.OverlayWithPolygon(tex1, polygon, new Color(0, 0, 0, 0.9f));
            img.texture = tex;
        }

        if (GUILayout.Button("Overlay Multi", GUILayout.Width(200), GUILayout.Height(40)))
        {
            List<Vector2> polygon1 = new List<Vector2>
            {
                new Vector2(0.1f, 0.1f),
                new Vector2(0.2f, 0.4f),
                new Vector2(0.3f, 0.1f),
            };

            List<Vector2> polygon2 = new List<Vector2>
            {
                new Vector2(0.4f, 0.2f),
                new Vector2(0.4f, 0.7f),
                new Vector2(0.7f, 0.7f),
                new Vector2(0.7f, 0.2f),
            };
            List<List<Vector2>> polygons = new List<List<Vector2>>
            {
                polygon1,
                polygon2,
            };
            Texture tex = TextureTools.OverlayWithPolygon(tex1, polygons, new Color(0, 0, 0, 0.9f));
            img.texture = tex;
        }

    }

}
