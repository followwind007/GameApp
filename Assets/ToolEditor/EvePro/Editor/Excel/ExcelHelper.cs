
namespace ToolEditor.EvePro.Editor.Excel
{
    public class ExcelHelper
    {
        public const int NAME_ROW = 9;
        public const int TYPE_ROW = 10;
        public const int START_ROW = 14;
        public const int START_COL = 2;

        public const int LIMIT_ROW = int.MaxValue;
        public const int LIMIT_COL = int.MaxValue;

        public const string TYPE_STRING = "S";
        public const string TYPE_INT = "I";
        public const string TYPE_FLOAT = "F";
        public const string TYPE_ELUA = "Elua";

        public static TableObject SpawnPoint { get; private set; }
        public static TableObject SpaceItem { get; private set; }
        public static TableObject Monster { get; private set; }

        public static void UpdateTables()
        {
            SpawnPoint = ReadTable(EveConst.SpawnPointTable, EveConst.SpawnPointSheet);
            SpaceItem = ReadTable(EveConst.SpaceItemTable, EveConst.SpaceItemSheet);
            var monsterReader = new ExcelReader(EveConst.MonsterTable)
            {
                startCol = 3
            };
            Monster = monsterReader.Read(EveConst.MonsterSheet);
        }

        public static TableObject ReadTable(string path, string sheetName)
        {
            var reader = new ExcelReader(path);
            return reader.Read(sheetName);
        }

    }

}