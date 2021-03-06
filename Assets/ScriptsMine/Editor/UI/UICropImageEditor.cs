using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    /// <summary>
    /// Editor class used to edit UI Images.
    /// </summary>
    [CustomEditor(typeof(UICropImage), true)]
    [CanEditMultipleObjects]
    public class UICropImageEditor : GraphicEditor
    {
        SerializedProperty m_Texture;

        //add
        SerializedProperty m_SubTexture;
        SerializedProperty m_Shader;
        //end add

        SerializedProperty m_UVRect;
        GUIContent m_UVRectContent;

        protected override void OnEnable()
        {
            base.OnEnable();

            // Note we have precedence for calling rectangle for just rect, even in the Inspector.
            // For example in the Camera component's Viewport Rect.
            // Hence sticking with Rect here to be consistent with corresponding property in the API.
            m_UVRectContent = new GUIContent("UV Rect");

            m_Texture = serializedObject.FindProperty("m_Texture");

            //add
            m_SubTexture = serializedObject.FindProperty("m_SubTexture");
            m_Shader = serializedObject.FindProperty("m_Shader");
            //end add

            m_UVRect = serializedObject.FindProperty("m_UVRect");

            SetShowNativeSize(true);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_Texture);

            //add
            EditorGUILayout.PropertyField(m_SubTexture);
            EditorGUILayout.PropertyField(m_Shader);
            //end add

            AppearanceControlsGUI();
            RaycastControlsGUI();
            EditorGUILayout.PropertyField(m_UVRect, m_UVRectContent);
            SetShowNativeSize(false);
            NativeSizeButtonGUI();

            serializedObject.ApplyModifiedProperties();
        }

        void SetShowNativeSize(bool instant)
        {
            base.SetShowNativeSize(m_Texture.objectReferenceValue != null, instant);
        }

        private static Rect Outer(RawImage rawImage)
        {
            Rect outer = rawImage.uvRect;
            outer.xMin *= rawImage.rectTransform.rect.width;
            outer.xMax *= rawImage.rectTransform.rect.width;
            outer.yMin *= rawImage.rectTransform.rect.height;
            outer.yMax *= rawImage.rectTransform.rect.height;
            return outer;
        }

        /// <summary>
        /// Allow the texture to be previewed.
        /// </summary>

        public override bool HasPreviewGUI()
        {
            RawImage rawImage = target as RawImage;
            if (rawImage == null)
                return false;

            var outer = Outer(rawImage);
            return outer.width > 0 && outer.height > 0;
        }

        /// <summary>
        /// Draw the Image preview.
        /// </summary>

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            RawImage rawImage = target as RawImage;
            Texture tex = rawImage.mainTexture;

            if (tex == null)
                return;

            //var outer = Outer(rawImage);
            //SpriteDrawUtility.DrawSprite(tex, rect, outer, rawImage.uvRect, rawImage.canvasRenderer.GetColor());
        }

        /// <summary>
        /// Info String drawn at the bottom of the Preview
        /// </summary>

        public override string GetInfoString()
        {
            RawImage rawImage = target as RawImage;

            // Image size Text
            string text = string.Format("RawImage Size: {0}x{1}",
                    Mathf.RoundToInt(Mathf.Abs(rawImage.rectTransform.rect.width)),
                    Mathf.RoundToInt(Mathf.Abs(rawImage.rectTransform.rect.height)));

            return text;
        }
    }
}