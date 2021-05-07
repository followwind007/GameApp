using UnityEditor;
using UnityEngine;

namespace Tools.ResCheck
{
    public static class MaterialCopyPaste
    {
        private static Material _copyed;

        [MenuItem("Assets/Material/Copy", true, 601)]
        private static bool CheckCopyMaterial()
        {
            return Selection.activeObject is Material;
        }
        
        [MenuItem("Assets/Material/Copy", false, 601)]
        private static void CopyMaterial()
        {
            if (Selection.activeObject is Material mat)
            {
                _copyed = mat;
            }
        }

        [MenuItem("Assets/Material/Paste", true, 602)]
        private static bool CheckPasterMaterial()
        {
            return _copyed && Selection.activeObject is Material target && target.shader == _copyed.shader;
        }

        [MenuItem("Assets/Material/Paste", false, 602)]
        private static void PasterMaterial()
        {
            if (Selection.activeObject is Material target)
            {
                if (target.shader != _copyed.shader)
                {
                    EditorUtility.DisplayDialog("Warning", "Shader type mismatch", "Ok");
                    return;
                }
                Undo.RegisterCompleteObjectUndo(target, "paste material");
                target.CopyPropertiesFromMaterial(_copyed);
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }
        
    }
}