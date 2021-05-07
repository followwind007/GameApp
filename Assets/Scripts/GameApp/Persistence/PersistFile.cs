using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameApp.Pesistence
{
    public static class PersistFile
    {
        public const string Platform =
        #if UNITY_EDITOR_WIN
            "win";
        #elif UNITY_EDITOR_OSX
            "mac";
        #elif UNITY_ANDROID
            "android";
        #elif UNITY_IOS
            "ios";
        #else
            "default";
        #endif
        
        public static readonly string DataPath = 
        #if UNITY_EDITOR
            $"{new DirectoryInfo(Application.dataPath).Parent?.FullName}/{Platform}";
        #else
            $"{Application.persistentDataPath}/{Platform}";
        #endif

        public static async Task AsyncWriteToFile(byte[] bytes, string path, PersistProfile profile)
        {
            path = GetFilePath(path, profile.PType);
            var encoded = await profile.AsyncEncoder(bytes);
            using (var s = File.OpenWrite(path))
            {
                s.Seek(0, SeekOrigin.Begin);
                await s.WriteAsync(encoded, 0, encoded.Length);
            }
        }

        public static async Task<byte[]> AsyncReadFile(string path, PersistProfile profile)
        {
            path = GetFilePath(path, profile.PType);
            using (var s = File.OpenRead(path))
            {
                var bytes = new byte[s.Length];
                await s.ReadAsync(bytes, 0, (int) s.Length);
                bytes = await profile.AsyncDecoder(bytes);
                return bytes;
            }
        }

        public static void ClearFile(IEnumerable<string> paths, PersistProfile.PersistType persistType)
        {
            foreach (var p in paths)
            {
                if (string.IsNullOrEmpty(p)) continue;
                
                var path = GetFilePath(p, persistType);
                try
                {
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    throw new ApplicationException($"delete file error: {path}", e);
                }
            }
        }

        public static void ClearDirectory(string rootDirecory, PersistProfile.PersistType persistType)
        {
            if (string.IsNullOrEmpty(rootDirecory))
            {
                Debug.LogError("rootDirectory is null, delete all file is not allowed!");
                return;
            }
            var path = GetFilePath(rootDirecory, persistType);
            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception e)
            {
                throw new ApplicationException($"delete folder error: {path}", e);
            }
        }

        public static string GetFilePath(string relativePath, PersistProfile.PersistType type)
        {
            string rootPath;

            switch (type)
            {
                case PersistProfile.PersistType.Data:
                    rootPath = DataPath;
                    break;
                case PersistProfile.PersistType.Default:
                    rootPath = "";
                    break;
                default:
                    rootPath = "";
                    break;
            }
            
            var path = Path.Combine(rootPath, relativePath);
            var file = new FileInfo(path);
            if (file.Directory != null && !file.Directory.Exists)
            {
                file.Directory.Create();
            }
            return path;
        }

    }
}