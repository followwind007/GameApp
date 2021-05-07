using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Tools.ResCheck
{
    [Serializable]
    public class ModelSetting
    {
        public string path;
        public string textureDir = "Texture";
        public string materialDir = "Material";
        public string modelDir = "Model";

        public bool extractAnimation;
        [Tooltip("animation extract to this folder")]
        public string animationResDir = "Animation";

        public bool extractMesh;
        public string meshResDir = "Mesh";

        public bool extractAvatar;
        public string avatarResDir = "Avatar";

        public List<string> reserves = new List<string>();

        public bool prefabUseAssetPath;
        public string prefabDir = "";
        
        public string GetPrefabNameFromModel(string modelPath)
        {
            var fileInfo = new FileInfo(modelPath);
            return fileInfo.Exists ? fileInfo.Directory?.Parent?.Name : null;
        }

        public bool IsReserve(string name)
        {
            return reserves.Any(name.StartsWith);
        }

        public string GetPrefabFolderFromModel(string modelPath)
        {
            return prefabUseAssetPath ? prefabDir : GetModelParentFolder(modelPath);
        }

        public string GetModelParentFolder(string modelPath)
        {
            var fileInfo = new FileInfo(modelPath);
            if (!fileInfo.Exists) return null;

            var parent = fileInfo.Directory?.Parent;
            return parent != null ? AssetUtil.GetRelativePath(parent.FullName) : null;
        }

        public string GetMeshAssetName(string modelPath, string meshName)
        {
            return $"{GetModelParentFolder(modelPath)}/{meshResDir}/{GetPrefabNameFromModel(modelPath)}@{meshName}.asset";
        }

        public string GetAvatarAssetName(string modelPath, string avatarName)
        {
            return $"{GetModelParentFolder(modelPath)}/{avatarResDir}/{avatarName}.asset";
        }

        public string GetAnimationAssetName(string modelPath, string animationName)
        {
            return $"{GetModelParentFolder(modelPath)}/{animationResDir}/{animationName}.anim";
        }

    }

    [CreateAssetMenu(fileName = "ModelImportSetting", menuName = "Custom/Res/ModelImportSetting", order = 100)]
    public class ModelImportSetting : ScriptableObject
    {
        public const string Name = "ModelImportSetting";
        
        private static ModelImportSetting _instance;
        public static ModelImportSetting Instance => _instance ? _instance : _instance = Resources.Load<ModelImportSetting>(Name);

        public bool autoImport;
        public List<ModelSetting> models = new List<ModelSetting>();

        public static ModelSetting GetSetting(string path)
        {
            return Instance.models.FirstOrDefault(m => path.Contains(m.path));
        }

    }
}