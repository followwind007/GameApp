using System;
using System.Globalization;
using System.Reflection;
using Tools.Table;
using UnityEditor;
using UnityEngine;

namespace GameApp.AnimationRigging
{
    [CustomEditor(typeof(RigBoneRender))]
    public class RigBoneRenderEditor : TableEditor
    {
    }

    [InitializeOnLoad]
    public static class RigBoneRenderUtil
    {
        static Type windowDisplayOptionType;
        static Type windowFunctionDelegateType;

        static MethodInfo windowFunc;
        
        public delegate void WindowFunction(UnityEngine.Object target, SceneView sceneView);
        
        static RigBoneRenderUtil()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            
            var mSceneViewOverlayType = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneViewOverlay");
            windowDisplayOptionType = mSceneViewOverlayType.GetNestedType("WindowDisplayOption");
            windowFunctionDelegateType = mSceneViewOverlayType.GetNestedType("WindowFunction");

            windowFunc = mSceneViewOverlayType.GetMethod(
                "Window",
                BindingFlags.Public | BindingFlags.Static,
                null,
                CallingConventions.Any,
                new[] { typeof(GUIContent), windowFunctionDelegateType, typeof(int), windowDisplayOptionType},
                null);
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            var go = Selection.activeGameObject;
            if (go == null) return;

            var render = go.GetComponentInParent<RigBoneRender>();
            if (render && render.joints.Contains(go.transform))
            {
                Window(new GUIContent(go.name), 1200, (obj, sv) =>
                {
                    var selGo = Selection.activeGameObject;
                    if (selGo)
                    {
                        GUILayout.BeginHorizontal(GUILayout.Width(210));
                        var matrix = selGo.transform.localToWorldMatrix;
                        GUILayout.Label($"{matrix.GetRow(0)}\n{matrix.GetRow(1)}\n{matrix.GetRow(2)}\n{matrix.GetRow(3)}");
                        GUILayout.EndHorizontal();
                    }
                });
            }
        }


        public static void Window(GUIContent title, int order, WindowFunction sceneViewFunc)
        {
            var windowFunctionDelegate = Delegate.CreateDelegate(windowFunctionDelegateType, null, sceneViewFunc.Method);

            if (windowFunc != null)
                windowFunc.Invoke(null, BindingFlags.InvokeMethod, null, 
                    new[] {title, windowFunctionDelegate, order, Enum.ToObject(windowDisplayOptionType , 2)}, CultureInfo.CurrentCulture);
        }
    }
}