using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameApp.RenderTarget;

public class TestParticle : MonoBehaviour {

    public GameObject prefab;

    public GameObject particle;

    public RenderTarget _target;

    private void OnGUI()
    {
        if (GUILayout.Button("create target", GUILayout.Height(50), GUILayout.Width(100)))
        {
            _target = RenderTargetManager.instance.GetRenderTextureTarget(prefab, true);
        }
        if (GUILayout.Button("add element", GUILayout.Height(50), GUILayout.Width(100)))
        {
            RenderElement element = new RenderElement(particle);
            _target.AddElement(element);
            _target.InitRenderTexture();
        }
        if (GUILayout.Button("release target", GUILayout.Height(50), GUILayout.Width(100)))
        {
            RenderTargetManager.instance.ReleaseItem(prefab, _target.gameObject);
        }
        
        if (GUILayout.Button("add element go", GUILayout.Height(50), GUILayout.Width(100)))
        {
            _target.AddElement(particle);
        }

    }

}
