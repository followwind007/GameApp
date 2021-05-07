using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnumFlagsAttribute : PropertyAttribute { }

namespace Tools.ResCheck
{
    public class ResCheckSettings : ScriptableObject
    {
        [System.Flags]
        public enum Useage
        {
            NormalMap = 1,
            ChannelMap = 2,
            Texture_NonAlpha = 4,
            Texture_Alpha = 8,
        }


        [Serializable]
        public class TexturePlatformSettings
        {
            [Tooltip("Standalone, iPhone, Android, WebGL, Windows Store Apps, PS4, XboxOne, Nintendo 3DS, tvOS")]
            public string platform;
            
            public TextureImporterPlatformSettings Settings
            {
                get
                {
                    var st = new TextureImporterPlatformSettings
                    {
                        allowsAlphaSplitting = allowsAlphaSplitting,
                        androidETC2FallbackOverride = androidEtc2FallbackOverride,
                        compressionQuality = compressionQuality,
                        crunchedCompression = crunchedCompression,
                        format = format,
                        maxTextureSize = maxTextureSize,
                        overridden = overriden,
                        resizeAlgorithm = resizeAlgorithm,
                        textureCompression = textureCompression,
                        name = platform
                    };
                    return st;
                }
            }

            public bool allowsAlphaSplitting = true;
            public AndroidETC2FallbackOverride androidEtc2FallbackOverride = AndroidETC2FallbackOverride.Quality32Bit;
            [Range(0, 100)]
            public int compressionQuality = 100;
            public bool crunchedCompression;
            public TextureImporterFormat format = TextureImporterFormat.RGBA32;
            public int maxTextureSize = 1024;
            public bool overriden;
            public TextureResizeAlgorithm resizeAlgorithm;
            public TextureImporterCompression textureCompression = TextureImporterCompression.Uncompressed;
        }
        
        [Serializable]
        public class TextureConfig
        {
            public string path;

            [EnumFlags]
            public Useage useage = Useage.Texture_Alpha;

            [Space]
            public string name = "未命名规则";

            [Space]
            public TextureImporterType type = TextureImporterType.Sprite;
            /// <summary>
            /// multiple of four
            /// </summary>
            public bool useMOF = true;
            public bool usePakingTag;

            [Space]
            public bool forceAlphaChannel;
            public bool forceNotReadWrite = true;
            public bool generateMipMaps;

            public SpriteMeshType spriteMeshType = SpriteMeshType.FullRect;
            
            [Space]
            public Vector2Int maxSize = new Vector2Int(1024, 1024);
            public FilterMode filterMode = FilterMode.Bilinear;
            public TextureImporterCompression compression = TextureImporterCompression.Uncompressed;

            public List<TexturePlatformSettings> platformSettings = new List<TexturePlatformSettings>()
            {
                new TexturePlatformSettings()
                {
                    platform   = "Standalone",
                    textureCompression = TextureImporterCompression.Uncompressed,
                    overriden = true,
                    format = TextureImporterFormat.RGBA32
                },
                new TexturePlatformSettings()
                {
                    platform   = "Android",
                    textureCompression = TextureImporterCompression.Compressed,
                    overriden = true,
                    format = TextureImporterFormat.ASTC_4x4
                },
                new TexturePlatformSettings()
                {
                    platform   = "iOS",
                    textureCompression = TextureImporterCompression.Compressed,
                    overriden = true,
                    format = TextureImporterFormat.ETC2_RGBA8Crunched
                },
            };
        }
        
//        [MenuItem("Assets/Create/Custom/ResCheckSettings")]
//        private static void CreateSettings()
//        {
//            AssetUtil.CreateAsset<ResCheckSettings>("ResCheckSettings");
//        }

        public static ResCheckSettings Instance
        {
            get
            {
                var data = Resources.Load<ResCheckSettings>("ResCheckSettings");
                if (data == null)
                {
                    Debug.LogError("can not find ResCheckSettings.asset!");
                }
                return data;
            }
        }

        public static TextureConfig AtlasConfig
        {
            get
            {
                foreach (var config in Instance.textureConfigs)
                {
                    if (config.path.Contains("Atlas"))
                    {
                        return config;
                    }
                }
                return null;
            }
        }
        
        public List<TextureConfig> textureConfigs = new List<TextureConfig>();
        
        [Space]
        public string atlasPath = "Assets/Resources/UI/Atlas/";
        public string uiPrefabRoot = "Assets/Resources/UI/Prefab/";
        public List<string> extraPrefabList = new List<string>();


        [NonSerialized]private List<TextureConfig> _sortedConfigs = null;
        /// <summary>
        /// 外边需要对这个进行排序，但是并不想影响config本身的数据顺序，
        /// 这里就new一个list，rua一遍返回了
        /// 返回了一个按照长度排序的，外边直接使用来遍历就行了
        /// </summary>
        /// <returns></returns>
        public List<TextureConfig> GetTextureConfigCopy()
        {
            //if (_sortedConfigs == null)
            {
                _sortedConfigs = new List<TextureConfig>(textureConfigs);
                _sortedConfigs.Sort((a, b) =>
                {
                    string path1 = a.path;
                    string path2 = b.path;
                    if (path1.Length > path2.Length) { return -1; }
                    else if (path1.Length < path2.Length) { return 1; }
                    else { return 0; }
                });
            }
           
            return _sortedConfigs;
        }

        public TextureConfig GetTextureConfigByPath(string name)
        {
            TextureConfig result = null;
            foreach (var c in textureConfigs)
            {
                if (c.path == name)
                {
                    result = c;
                    break;
                }
            }
            return result;
        }
    }
}