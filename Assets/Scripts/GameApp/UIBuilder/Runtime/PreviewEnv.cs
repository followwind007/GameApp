#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameApp.UIBuilder
{
    [ExecuteInEditMode]
    public class PreviewEnv : MonoBehaviour
    {
        public Transform root;
        public Camera previewCamera;

        private RectTransform _canvasRectTrans;

        private bool _done;
        
        private class RenderTarget
        {
            public string Target;
            public string Path;
        }

        private readonly List<RenderTarget> _list = new List<RenderTarget>();
        
        private readonly WaitForEndOfFrame _end = new WaitForEndOfFrame();

        private int _curIndex;

        public void AddTarget(string path, string targetPath)
        {
            var target = new RenderTarget {Target = targetPath, Path = path};
            _list.Add(target);
        }

        private void Awake()
        {
            _canvasRectTrans = root.GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (_done)
            {
                return;
            }
            if (_curIndex < _list.Count)
            {
                var target = _list[_curIndex];
                #pragma warning disable Unity0001
                StartCoroutine(Capture(target));
                EditorUtility.DisplayProgressBar("Generate Preview", target.Target, (float) _curIndex / _list.Count);
            }
            else
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
                _done = true;
            }
        }

        private IEnumerator Capture(RenderTarget target)
        {
            for (var i = 0; i < root.childCount; i++)
            {
                DestroyImmediate(root.GetChild(0).gameObject);
            }
            
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(target.Target);
            var inst = Instantiate(prefab);
            inst.SetActive(true);
            
            var rectTrans = inst.GetComponent<RectTransform>();
            
            var instCanvas = inst.GetComponent<Canvas>();
            if (instCanvas)
                rectTrans.SetParentFull(root);
            else
                rectTrans.SetParent(root, false);
            
            yield return _end;
            
            var tex = GetPreviewTexture(rectTrans);
            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(target.Path, bytes);

            _curIndex++;
        }
        
        private Texture2D GetPreviewTexture(RectTransform rectTrans)
        {
            var rect = rectTrans.GetRect(_canvasRectTrans);
            var tex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);

            RenderTexture.active = previewCamera.targetTexture;
            tex.ReadPixels(rect, 0, 0);
            tex.Apply();
            return tex;
        }
        

    }
}
#endif