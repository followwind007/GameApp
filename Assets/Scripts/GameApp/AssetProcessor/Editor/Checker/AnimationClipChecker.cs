using UnityEngine;
using UnityEditor;

namespace GameApp.AssetProcessor
{
    public class AnimationClipChecker
    {
        public class CompressConfig
        {
            public bool useScale = true;
        }

        [MenuItem("Assets/Import Model/Compress Anim Use Scale")]
        public static void CompressSelectedWithScale()
        {
            CompressSelected(true);
        }
        
        [MenuItem("Assets/Import Model/Compress Anim Not Use Scale")]
        public static void CompressSelectedWithoutScale()
        {
            CompressSelected(false);
        }

        public static void CompressSelected(bool useScale)
        {
            var config = new CompressConfig() { useScale = useScale };
            foreach (var obj in Selection.objects)
            {
                if (obj is AnimationClip clip)
                {
                    var path = AssetDatabase.GetAssetPath(clip);
                    var compressClip = Compress(clip, config);
                    AssetDatabase.CreateAsset(compressClip, path);
                }
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static AnimationClip Compress(AnimationClip sourceClip, CompressConfig config)
        {
            var newClip = Object.Instantiate(sourceClip);

            var curveBindings = AnimationUtility.GetCurveBindings(sourceClip);
            
            foreach (var t in curveBindings)
            {
                if (!config.useScale && t.propertyName.Contains("Scale"))
                {
                    AnimationUtility.SetEditorCurve(newClip, t, null);
                }
            }
            
            var curves = new AnimationClipCurveData[curveBindings.Length];
            for (var index = 0; index < curves.Length; ++index)
            {
                curves[index] = new AnimationClipCurveData(curveBindings[index])
                {
                    curve = AnimationUtility.GetEditorCurve(newClip, curveBindings[index])
                };
            }
            
            foreach (var curveDate in curves)
            {
                if (curveDate.curve == null) continue;
                var keyFrames = curveDate.curve.keys;
                for (var i = 0; i < keyFrames.Length; i++)
                {
                    var key = keyFrames[i];
                    //key.time = float.Parse(key.time.ToString("f4"));
                    
                    key.value = float.Parse(key.value.ToString("f3"));
                    key.inTangent = float.Parse(key.inTangent.ToString("f3"));
                    key.outTangent = float.Parse(key.outTangent.ToString("f3"));
                    key.inWeight = float.Parse(key.inWeight.ToString("f3"));
                    key.outWeight = float.Parse(key.outWeight.ToString("f3"));
                    
                    keyFrames[i] = key;
                }
                curveDate.curve.keys = keyFrames;
                newClip.SetCurve(curveDate.path, curveDate.type, curveDate.propertyName, curveDate.curve);
            }

            return newClip;
        }

    }
}