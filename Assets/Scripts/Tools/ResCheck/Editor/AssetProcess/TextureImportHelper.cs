using UnityEditor;
using UnityEngine;

namespace Tools.ResCheck
{
    public class TextureImportHelper : AssetPostprocessor
    {
        private void OnPostprocessTexture(Texture2D texture)
        {
            var importer = assetImporter as TextureImporter;
            if (importer)
            {
                //DealTextureImporter(importer, texture);
            }
        }

        public static string DealTextureImporter(TextureImporter importer, Texture2D texture, bool showError = true)
        {
            var rc = ResCheckSettings.Instance;
            var path = importer.assetPath;

            
            
            var res = "";
            //deal conditions
            foreach (var config in rc.textureConfigs)
            {
                if (!path.Contains(config.path)) continue;
                string err;
                
                importer.textureType = config.type;
                
                importer.spritePackingTag = config.usePakingTag ? AssetUtil.GetFileFolderName(path) : null;
                TextureImporterSettings tis = new TextureImporterSettings();
                importer.ReadTextureSettings(tis);
                tis.spriteMeshType = config.spriteMeshType;
                importer.SetTextureSettings(tis);
                
                importer.mipmapEnabled = config.generateMipMaps;
                if (config.forceNotReadWrite)
                {
                    importer.isReadable = false;
                }
                
                //texture size
                if (texture.width > config.maxSize.x || texture.height > config.maxSize.y)
                {
                    err = $"{path}: 当前尺寸({texture.width}, {texture.height})超出{config.maxSize}\n";
                    if (showError) Debug.LogError(err, texture);
                    res += err;
                }
                else
                {
                    importer.maxTextureSize = config.maxSize.x;
                }

                if (config.useMOF)
                {
                    //texture size odd
                    if (texture.width % 4 != 0 || texture.height % 4 != 0)
                    {
                        err = $"{path}: 当前尺寸({texture.width}, {texture.height}) 宽高非4的倍数\n";
                        if (showError) Debug.LogError(err);
                        res += err;
                    }    
                }
                
                
                importer.filterMode = config.filterMode;
                importer.textureCompression = config.compression;
                
                if (config.forceAlphaChannel && !importer.DoesSourceTextureHaveAlpha())
                {
                    //alpha通道不是必要提交 by wk
//                    err = $"{path}: 不包含alpha通道\n";
//                    if (showError) Debug.LogError(err);
//                    res += err;
                }

                foreach (var ps in config.platformSettings)
                {
                    importer.SetPlatformTextureSettings(ps.Settings);
                }
                
            }

            AssetDatabase.WriteImportSettingsIfDirty(importer.assetPath);
            return res;
        }

    }
}