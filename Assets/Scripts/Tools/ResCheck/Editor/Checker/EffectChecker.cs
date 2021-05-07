using UnityEditor;
using UnityEngine;

namespace Tools.ResCheck
{
    public class EffectChecker : IChecker
    {
        private static EffectChecker _instance;
        public static EffectChecker Instance => _instance ?? (_instance = new EffectChecker());

        public void StartCheck()
        {
            var i = 0;
            foreach (var module in DataSource.Modules.Values)
            {
                var percent = (float) i / DataSource.Modules.Count;
                i++;

                foreach (var prefab in module.prefabs.Values)
                {
                    EditorUtility.DisplayProgressBar("Handle Effect", prefab.path, percent);
                    
                    var go = AssetDatabase.LoadAssetAtPath<GameObject>(prefab.path);
                    if (go == null) continue;
                    
                    prefab.particles.Clear();
                    var pss = go.GetComponentsInChildren<ParticleSystem>(true);
                    foreach (var ps in pss)
                    {
                        prefab.particles.Add(AssetUtil.GetPathInGameObject(ps.gameObject));
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }

    }
}