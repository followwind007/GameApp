
namespace Tools.Table.Excel
{ 
    public class ExcelHelper
    {
        public const int NAME_ROW = 6;
        public const int TYPE_ROW = 7;
        public const int NULLABLE_ROW = 4;
        
        public const int START_ROW = 9;
        public const int START_COL = 2;
        
        public const int LIMIT_ROW = int.MaxValue;
        public const int LIMIT_COL = int.MaxValue;

        public const string TYPE_STRING = "S";
        public const string TYPE_BOOL = "B";
        public const string TYPE_INT = "I";
        public const string TYPE_FLOAT = "F";
        public const string TYPE_ELUA = "Elua";

        public static TableObject ReadTable(string path, string sheetName)
        {
            var reader = new ExcelReader(path);
            return reader.Read(sheetName);
        }

    }

}