using UnityEngine;
using UnityEditor;
using ToolEditor.EvePro.Editor.Asset.Object;

namespace ToolEditor.EvePro.Editor.Authorize
{
    public class AdminWindow : EditorWindow
    {
        [MenuItem("Tools/Eve Pro/Settings", false, 101)]
        public static void ShowAdminWindow()
        {
            if (LoginManager.Instance.CheckNeedLogin())
            {
                LoginWindow.ShowLoginWindow();
            }
            GetWindow<AdminWindow>("Eve Pro Setting");
        }

        private void OnGUI()
        {
            GUILayout.Space(15);

            if (!LoginManager.Instance.IsLogin)
            {
                DrawNotLogin();
                return;
            }
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical();

            if (LoginManager.Instance.IsAdmin)
            {
                DrawAdmin();
            }

            GUILayout.EndVertical();

            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
        }

        private void Update()
        {
            if (focusedWindow != this) Repaint();
        }

        private void DrawNotLogin()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Login First!", GUILayout.Width(70));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawAdmin()
        {
            if (GUILayout.Button(LanguageAsset.GetValue("COMMIT_AUTHORITY")))
            {
                OnClickUploadAuthority();
            }
            GUILayout.Space(5);
            if (GUILayout.Button(LanguageAsset.GetValue("COMMIT_USERINFO")))
            {
                OnClickUploadUserData();
            }
        }

        private void OnClickUploadAuthority()
        {
            if (SettingManager.Instance.UploadAuthorityData())
            {
                EditorUtility.DisplayDialog("Message", LanguageAsset.GetValue("UPLOAD_AUTH_SUCCESS"), "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", LanguageAsset.GetValue("UPLOAD_AUTH_FAILE"), "OK");
            }
        }

        private void OnClickUploadUserData()
        {
            if (SettingManager.Instance.UploadUserData())
            {
                EditorUtility.DisplayDialog("Message", LanguageAsset.GetValue("UPLOAD_USER_DATA_SUCCESS"), "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", LanguageAsset.GetValue("UPLOAD_USER_DATA_FAILE"), "OK");
            }
        }


    }
}