using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetMemoryTest : MonoBehaviour {
    public RawImage image;
    private Texture _tex;


    private void OnGUI()
    {
        if (GUILayout.Button("read resource"))
        {
            _tex = Resources.Load<Texture>("bkg");
        }
        if (GUILayout.Button("set resource"))
        {
            image.texture = null;
            image.texture = _tex;
        }
        if (GUILayout.Button("unload resource"))
        {
            Resources.UnloadAsset(_tex);
        }
        if (GUILayout.Button("unload unused assets"))
        {
            Resources.UnloadUnusedAssets();
        }
    }

}
