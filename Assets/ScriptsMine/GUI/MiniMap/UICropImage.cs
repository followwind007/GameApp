using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    public class UICropImage : MaskableGraphic
    {
        [FormerlySerializedAs("m_Tex")]
        [SerializeField] Texture m_Texture;

        //add
        [SerializeField] Texture m_SubTexture;
        [SerializeField] Shader m_Shader;
        //end add

        [SerializeField] Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

        protected UICropImage()
        {
            useLegacyMeshGeneration = false;
        }

        /// <summary>
        /// Returns the texture used to draw this Graphic.
        /// </summary>
        public override Texture mainTexture
        {
            get
            {
                if (m_Texture == null)
                {
                    if (material != null && material.mainTexture != null)
                    {
                        return material.mainTexture;
                    }
                    return s_WhiteTexture;
                }

                return m_Texture;
            }
        }

        //add
        public override Material material
        {
            get
            {
                if (m_Material == null)
                {
                    if (m_Shader != null)
                    {
                        m_Material = new Material(m_Shader);
                    }
                    else
                    {
                        m_Material = Canvas.GetDefaultCanvasMaterial();
                    }
                    if (texture)
                    {
                        m_Material.SetTexture("_MainTex", texture);
                    }
                    if (subTexture)
                    {
                        m_Material.SetTexture("_Mask", m_SubTexture);
                    }
                }
                return m_Material;
            }
            set
            {
                m_Material = value;
                SetMaterialDirty();
            }
        }

        public Texture subTexture
        {
            get
            {
                return m_SubTexture;
            }
            set
            {
                if (m_SubTexture == value)
                {
                    return;
                }
                m_SubTexture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        public Shader shader
        {
            get
            {
                return m_Shader;
            }
            set
            {
                if (m_Shader == value)
                {
                    return;
                }
                m_Shader = value;
                SetMaterialDirty();
            }
        }
        //end add

        /// <summary>
        /// Texture to be used.
        /// </summary>
        public Texture texture
        {
            get
            {
                return m_Texture;
            }
            set
            {
                if (m_Texture == value)
                    return;

                m_Texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// UV rectangle used by the texture.
        /// </summary>
        public Rect uvRect
        {
            get
            {
                return m_UVRect;
            }
            set
            {
                if (m_UVRect == value)
                    return;
                m_UVRect = value;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Adjust the scale of the Graphic to make it pixel-perfect.
        /// </summary>

        public override void SetNativeSize()
        {
            Texture tex = mainTexture;
            if (tex != null)
            {
                int w = Mathf.RoundToInt(tex.width * uvRect.width);
                int h = Mathf.RoundToInt(tex.height * uvRect.height);
                rectTransform.anchorMax = rectTransform.anchorMin;
                rectTransform.sizeDelta = new Vector2(w, h);
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Texture tex = mainTexture;
            vh.Clear();
            if (tex != null)
            {
                var r = GetPixelAdjustedRect();
                var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
                var scaleX = tex.width * tex.texelSize.x;
                var scaleY = tex.height * tex.texelSize.y;
                {
                    var color32 = color;
                    vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(m_UVRect.xMin * scaleX, m_UVRect.yMin * scaleY));
                    vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(m_UVRect.xMin * scaleX, m_UVRect.yMax * scaleY));
                    vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(m_UVRect.xMax * scaleX, m_UVRect.yMax * scaleY));
                    vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(m_UVRect.xMax * scaleX, m_UVRect.yMin * scaleY));

                    vh.AddTriangle(0, 1, 2);
                    vh.AddTriangle(2, 3, 0);
                }
            }
        }

    }
}
