using UnityEngine;
using UnityEditor;
using ToolEditor.EvePro.Editor.Asset.Object;
using System;

namespace ToolEditor.EvePro.Editor.Authorize
{
    public class LoginWindow : EditorWindow
    {
        private enum WindowMode
        {
            Login, ChangePassword
        }

        private WindowMode _mode = WindowMode.Login;

        private string _userName;
        private string _password;
        private string _passwordNew;

        public Action<LoginWindow> Callback;

        public static LoginWindow Instance { get; private set; }

        [MenuItem("Tools/Eve Pro/Login", false, 100)]
        public static void ShowLoginWindow()
        {
            var window = GetWindow<LoginWindow>("Eve Pro Login");
            window.Init();
        }

        public void Init()
        {
            Instance = this;
            _userName = LoginManager.Instance.UserName;
            _password = LoginManager.Instance.Password;
        }

        private void OnGUI()
        {
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical();

            _userName = EditorGUILayout.TextField(LanguageAsset.GetValue("USER_NAME"), _userName);
            GUILayout.Space(5);

            switch (_mode)
            {
                case WindowMode.Login:
                    DrawLogin();
                    break;
                case WindowMode.ChangePassword:
                    DrawChangePassword();
                    break;
                default:
                    break;
            }
            GUILayout.Space(10);
            if (LoginManager.Instance.IsLogin)
                EditorGUILayout.LabelField(string.Format("{0}: {1}", LanguageAsset.GetValue("LOGIN_STATUS"), LoginManager.Instance.UserName));
            else
                EditorGUILayout.LabelField(string.Format("{0}: {1}", LanguageAsset.GetValue("LOGIN_STATUS"), LanguageAsset.GetValue("LOGIN_NULL")));

            GUILayout.FlexibleSpace();

            DrawButtons();

            GUILayout.EndVertical();
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
        }

        private void DrawLogin()
        {
            _password = EditorGUILayout.PasswordField(LanguageAsset.GetValue("PASSWORD"), _password);
        }

        private void DrawChangePassword()
        {
            _password = EditorGUILayout.PasswordField(LanguageAsset.GetValue("PASSWORD"), _password);
            GUILayout.Space(5);
            _passwordNew = EditorGUILayout.PasswordField(LanguageAsset.GetValue("PASSWORD_NEW"), _passwordNew);
        }

        private void DrawButtons()
        {
            if (GUILayout.Button(LanguageAsset.GetValue("LOGIN")))
            {
                if (_mode == WindowMode.Login)
                {
                    Login();
                }
                _mode = WindowMode.Login;
            }

            GUILayout.Space(10);
            if (GUILayout.Button(LanguageAsset.GetValue("LOGOUT")))
            {
                LoginManager.Instance.Logout();
                _mode = WindowMode.Login;
            }

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(LanguageAsset.GetValue("CHANGE_PASSWORD")))
            {
                if (_mode == WindowMode.ChangePassword)
                {
                    ChangePassword();
                }
                _mode = WindowMode.ChangePassword;
            }
            if (GUILayout.Button(LanguageAsset.GetValue("RESET_USER")))
            {
                LoginManager.Instance.Reset();
                _mode = WindowMode.Login;
                ClearFields();
            }
            GUILayout.EndHorizontal();
        }

        private void Login()
        {
            Debug.Log(string.Format("'{0}' try login", _userName));
            if (string.IsNullOrEmpty(_userName))
            {
                EditorUtility.DisplayDialog("Warning", LanguageAsset.GetValue("NEED_USER_NAME"), "OK");
                return;
            }
            bool res = LoginManager.Instance.Login(_userName, _password);
            if (!res)
            {
                EditorUtility.DisplayDialog("Error", LanguageAsset.GetValue("LOGIN_INFO_ERROR"), "OK");
            }
            else
            {
                if (Callback != null)
                {
                    Callback(this);
                }
                Close();
            }
        }

        private void ChangePassword()
        {
            Debug.Log(string.Format("user '{0}' change password from '{1}' to '{2}' ", _userName, _password, _passwordNew));
            if (string.IsNullOrEmpty(_userName))
            {
                EditorUtility.DisplayDialog("Warning", LanguageAsset.GetValue("NEED_USER_NAME"), "OK");
                return;
            }

            bool res = LoginManager.Instance.UpdatePassword(_userName, _password, _passwordNew);
            if (res)
            {
                EditorUtility.DisplayDialog("Message", LanguageAsset.GetValue("UPDATE_PASSWORD_SUCCESS"), "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Error", LanguageAsset.GetValue("UPDATE_PASSWORD_FAILE"), "OK");
            }
        }

        private void OnDestroy()
        {
            Instance = null;
            ClearFields();
        }

        private void ClearFields()
        {
            _userName = null;
            _password = null;
            _passwordNew = null;
        }

    }

}