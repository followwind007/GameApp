using MySql.Data.MySqlClient;

namespace ToolEditor.EvePro.Editor.Database
{
    public class DbManager
    {
        private static DbManager _instance;
        public static DbManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DbManager();
                    _instance.Init();
                }
                return _instance;
            }
        }

        public static MySqlConnection Connection
        {
            get
            {
                return Instance._conn;
            }
        }

        private MySqlConnection _conn;

        public void Init()
        {
            string connStr = "server=10.246.53.48;port=3306;uid=pm07;pwd=pm07;database=eve_pro";
            _conn = new MySqlConnection(connStr);
            _conn.Open();
        }

        
    }
}