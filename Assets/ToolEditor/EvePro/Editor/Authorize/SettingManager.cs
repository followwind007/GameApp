using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using MySql.Data.MySqlClient;
using UnityEngine;
using System.Text;
using ToolEditor.EvePro.Editor.Database;

namespace ToolEditor.EvePro.Editor.Authorize
{
    public class SettingManager
    {
        internal struct Group
        {
            public string name;
            public int mode;
        }

        internal struct UserInfo
        {
            public string userName;
            public string password;
            public int status;
            public string group;
        }

        private static SettingManager _instance;
        public static SettingManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SettingManager();
                }
                return _instance;
            }
        }

        public MySqlConnection Connection { get { return DbManager.Connection; } }

        private List<Group> _groupList = new List<Group>();
        private List<UserInfo> _userList = new List<UserInfo>();

        public bool UploadAuthorityData()
        {
            var cmdDelGroup = new MySqlCommand("TRUNCATE TABLE auth", Connection);
            cmdDelGroup.ExecuteScalar();
            cmdDelGroup.Dispose();

            GetGroup();
            if (_groupList.Count == 0) return false;

            StringBuilder values = new StringBuilder();
            foreach (var group in _groupList)
            {
                values.AppendFormat("('{0}', {1}),", group.name, group.mode);
            }
            values.Remove(values.Length - 1, 1);
            string cmdStr = string.Format("INSERT INTO auth VALUES {0}", values.ToString());

            var cmdInsertGroup = new MySqlCommand(cmdStr, Connection);
            int result = cmdInsertGroup.ExecuteNonQuery();
            cmdInsertGroup.Dispose();
            return result >= 0;
        }

        public bool UploadUserData()
        {
            var existUserDict = GetExistUserInfo();
            var cmdDelUser = new MySqlCommand("TRUNCATE TABLE user", Connection);
            cmdDelUser.ExecuteScalar();
            cmdDelUser.Dispose();

            GetUser();
            if (_userList.Count == 0) return false;

            StringBuilder values = new StringBuilder();
            for (int i = 0; i < _userList.Count; i++)
            {
                var user = _userList[i];
                if (existUserDict.ContainsKey(user.userName))
                    user.password = existUserDict[user.userName].password;
                values.AppendFormat("('{0}', '{1}', {2}, '{3}'),", user.userName, user.password, user.status, user.group);
            }
            values.Remove(values.Length - 1, 1);
            string cmdStr = string.Format("INSERT INTO user VALUES {0}", values.ToString());

            var cmdInsertUser = new MySqlCommand(cmdStr, Connection);
            int result = cmdInsertUser.ExecuteNonQuery();
            cmdInsertUser.Dispose();
            return result >= 0;
        }

        private void GetGroup()
        {
            ExcelPackage package = new ExcelPackage(new FileInfo(EveConst.EveAuthorizationTable));
            var groupSheet = package.Workbook.Worksheets["Sheet1"];
            var table = groupSheet.Tables[0];

            int rowStart = table.Address.Start.Row + 1;
            int rowEnd = table.Address.End.Row;

            int colStart = table.Address.Start.Column;
            int colEnd = table.Address.End.Column;

            _groupList.Clear();
            for (int row = rowStart; row <= rowEnd; row++)
            {
                string groupName = groupSheet.GetValue<string>(row, colStart);
                int mode = 0;
                for (int col = colStart + 1; col <= colEnd; col++)
                {
                    mode |= groupSheet.GetValue<int>(row, col) << (col - colStart - 1);
                }
                Group group = new Group()
                {
                    name = groupName,
                    mode = mode,
                };
                _groupList.Add(group);
                Debug.Log(string.Format("{0},{1}", groupName, mode));
            }
        }

        private void GetUser()
        {
            ExcelPackage package = new ExcelPackage(new FileInfo(EveConst.USER_TABLE_PATH));
            var userSheet = package.Workbook.Worksheets["Sheet1"];
            var table = userSheet.Tables[0];

            int rowStart = table.Address.Start.Row + 1;
            int rowEnd = table.Address.End.Row;

            _userList.Clear();
            for (int row = rowStart; row <= rowEnd; row++)
            {
                string userName = userSheet.GetValue<string>(row, EveConst.USER_COL_NAME);
                if (string.IsNullOrEmpty(userName)) break;
               
                int status = userSheet.GetValue<int>(row, EveConst.USER_COL_STATUS);
                string group = userSheet.GetValue<string>(row, EveConst.USER_COL_GROUP);
                UserInfo user = new UserInfo()
                {
                    userName = userName,
                    password = "",
                    status = status,
                    group = group,
                };
                _userList.Add(user);
                Debug.Log(string.Format("{0},{1},{2}", userName, status, group));
            }
        }

        private Dictionary<string, UserInfo> GetExistUserInfo()
        {
            var exsitUserDict = new Dictionary<string, UserInfo>();
            var cmd = new MySqlCommand("SELECT * FROM user", Connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader.HasRows)
                {
                    var user = new UserInfo()
                    {
                        userName = reader["id"] as string,
                        password = reader["password"] as string,
                        status = (int)reader["status"],
                        group = reader["group"] as string,
                    };
                    exsitUserDict[user.userName] = user;
                }
            }
            reader.Close();
            cmd.Dispose();
            return exsitUserDict;
        }


    }
}