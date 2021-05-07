using UnityEngine;
using UnityEditor;
using System;
using MySql.Data.MySqlClient;
using ToolEditor.EvePro.Editor.Database;
using ToolEditor.EvePro.Editor.Asset.Object;

namespace ToolEditor.EvePro.Editor
{
    public class LoginManager
    {
        public static LoginManager _instance;
        public static LoginManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoginManager();
                    _instance.Init();
                }
                return _instance;
            }
        }

        public MySqlConnection Connection { get { return DbManager.Connection; } }

        public string Group { get; private set; }
        public int Mode { get; private set; }
        public bool IsLogin { get; private set; }

        public void Init()
        {
            Group = null;
            Mode = 0;
            IsLogin = false;
        }

        public string UserName
        {
            get
            {
                return PlayerPrefs.GetString(EveConst.USER_NAME);
            }
            set
            {
                PlayerPrefs.SetString(EveConst.USER_NAME, value);
            }
        }
        
        public string Password
        {
            get
            {
                return PlayerPrefs.GetString(EveConst.PASSWORD);
            }
            set
            {
                PlayerPrefs.SetString(EveConst.PASSWORD, value);
            }
        }

        public string LastLoginTime
        {
            get
            {
                return PlayerPrefs.GetString(EveConst.LAST_LOGIN_TIME);
            }
            set
            {
                PlayerPrefs.SetString(EveConst.LAST_LOGIN_TIME, value);
            }
        }

        public bool IsAdmin
        {
            get
            {
                return Group != null && Group.Equals("admin");
            }
        }

        public bool CheckNeedLogin()
        {
            if (IsLogin)
            {
                return false;
            }
            else if (!string.IsNullOrEmpty(LastLoginTime))
            {
                DateTime lastTime = DateTime.ParseExact(LastLoginTime, EveConst.TIME_FORMAT, System.Globalization.CultureInfo.CurrentCulture);
                var span = DateTime.Now - lastTime;
                if (span.Seconds < EveConst.LOGIN_EXPIRE)
                {
                    return !Login(UserName, Password);
                }
            }
            return true;
        }

        public void UpdateLoginInfo(string userName, string password)
        {
            UserName = userName;
            Password = password;
            LastLoginTime = DateTime.Now.ToString(EveConst.TIME_FORMAT, System.Globalization.CultureInfo.CurrentCulture);
        }

        public bool UpdatePassword(string userName, string password, string passwordNew)
        {
            if (!Login(userName, password))
            {
                EditorUtility.DisplayDialog("Waring", LanguageAsset.GetValue("LOGIN_INFO_ERROR"), "OK");
                return false;
            }
            string passwordNewEncrypt = string.IsNullOrEmpty(passwordNew) ? "" : EncryptHelper.SHAmd5Encrypt(passwordNew);

            string cmdStr = string.Format("UPDATE user SET password='{0}' WHERE id='{1}'", passwordNewEncrypt, UserName);
            var cmd = new MySqlCommand(cmdStr, Connection);
            int result = cmd.ExecuteNonQuery();
            cmd.Dispose();
            if (result > 0)
            {
                return Login(userName, passwordNew);
            }
            return false;
        }

        public bool Login(string userName, string password)
        {
            string passwordEncrypt = string.IsNullOrEmpty(password) ? "" : EncryptHelper.SHAmd5Encrypt(password);
            string cmdStr = string.Format("SELECT * FROM user JOIN auth ON user.group=auth.group WHERE user.id='{0}' AND user.password='{1}' ", 
                userName, passwordEncrypt);
            var cmd = new MySqlCommand(cmdStr, Connection);

            var reader = cmd.ExecuteReader();
            bool exist = false;
            while (reader.Read())
            {
                if (reader.HasRows)
                {
                    Group = reader["group"] as string;
                    Mode = (int)reader["authority"];
                    UpdateLoginInfo(userName, password);
                    IsLogin = true;
                    exist = true;
                    break;
                }
            }
            reader.Close();
            cmd.Dispose();
            return exist;
        }

        public void Logout()
        {
            IsLogin = false;
            Mode = 0;
            Group = null;
        }

        public void Reset()
        {
            Logout();
            UserName = null;
            Password = null;
        }

    }
}