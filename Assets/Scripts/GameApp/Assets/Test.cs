using System.Collections.Generic;
using Framework.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace GameApp.Assets
{
    public class Test : MonoBehaviour
    {
        public RectTransform root;
        public string imgPath = "Assets/Art/UI/Atlas/Card/x_5_8.png";
        public Image img;
        public string texPath = "Assets/Art/UI/Icon/FaceMid/J-1-5.jpg";
        public RawImage rawImage;
        public string prefabPath;

        public List<string> animations;

        private string _info;

        private void Start()
        {
            AssetLoader.Init(() =>
            {
                var req = AssetLoader.LoadAssetAsync<TMP_Settings>("Assets/Art/Editor/Resources/TMP Settings.asset");
                req.OnComplete += () => { TMPManager.SetSettings(req.Result); };
            });
        }

        public void OnGUI()
        {
            if (GUILayout.Button("Test Load", GUILayout.Height(40), GUILayout.Width(80)))
            {
                var path = $"{AssetConfig.BundleDataPath}/assets/art/ui/atlas/card_0";
                var req = AssetBundle.LoadFromFileAsync(path);
                Debug.Log($"{path} {Time.realtimeSinceStartup}");
                req.completed += operation =>
                {
                    Debug.Log($"{req.isDone} {Time.realtimeSinceStartup}");
                };
                Debug.Log($"next {Time.realtimeSinceStartup}");
                //while (!req.isDone) { }
                
            }
            
            if (GUILayout.Button("Load Atlas", GUILayout.Height(40), GUILayout.Width(80)))
            {
                Debug.Log(AssetConfig.BundleDataPath);
                _info = Application.streamingAssetsPath;
            }
            
            if (GUILayout.Button("Load Sprite", GUILayout.Height(40), GUILayout.Width(80)))
            {
                var req = AssetLoader.LoadAssetAsync<Sprite>(texPath);
                req.OnComplete += () =>
                {
                    img.sprite = req.Result;
                };
            }
            
            if (GUILayout.Button("Load Tex", GUILayout.Height(40), GUILayout.Width(80)))
            {
                var req = AssetLoader.LoadAssetAsync<Texture2D>(texPath);
                req.OnComplete += () =>
                {
                    rawImage.texture = req.Result;
                };
            }
            
            if (GUILayout.Button("Load Scene", GUILayout.Height(40), GUILayout.Width(80)))
            {
                var req = AssetLoader.LoadSceneAsync("Assets/Scenes/SampleScene.unity");
                req.OnComplete += () =>
                {
                    SceneManager.SetActiveScene(req.Result);
                };
            }

            if (GUILayout.Button("Load Prefab", GUILayout.Height(40), GUILayout.Width(80)))
            {
                var handle = AssetLoader.LoadAssetAsync<GameObject>(prefabPath);
                handle.OnComplete += () =>
                {
                    var inst = Instantiate(handle.Result);
                    var rect = inst.GetComponent<RectTransform>();
                    rect.SetParentCenter(root);
                };
            }
            
            if (GUILayout.Button("Load Anim", GUILayout.Height(40), GUILayout.Width(80)))
            {
                foreach (var aniStr in animations)
                {
                    AssetLoader.LoadAssetAsync<AnimationClip>(aniStr);
                }
            }

            GUILayout.Label(_info);
        }
        
    }
}