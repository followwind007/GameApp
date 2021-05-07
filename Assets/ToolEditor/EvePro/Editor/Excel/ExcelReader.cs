using OfficeOpenXml;
using System.IO;
using LuaInterface;

namespace ToolEditor.EvePro.Editor.Excel
{
    public class ExcelReader
    {
        public int nameRow = ExcelHelper.NAME_ROW;
        public int typeRow = ExcelHelper.TYPE_ROW;
        public int startRow = ExcelHelper.START_ROW;
        public int startCol = ExcelHelper.START_COL;

        private int _endRow;
        private int _endCol;

        private readonly string _path;

        private ExcelWorksheet _sheet;

        public ExcelReader(string path)
        {
            _path = path;
        }

        public TableObject Read(string sheetName)
        {
            var package = new ExcelPackage(new FileInfo(_path));
            if (package == null) return null;

            _sheet = package.Workbook.Worksheets[sheetName];
            if (_sheet == null) return null;

            UpdateEndIndex();

            TableObject table = new TableObject();
            for (int row = startRow; row <= _endRow; row++)
            {
                TableEntry entry = new TableEntry();
                for (int col = startCol; col <= _endCol; col++)
                    AddField(entry, row, col);

                table.AddEntry(GetFieldId(row), entry);
            }

            return table;
        }

        private void AddField(TableEntry entry, int row, int col)
        {
            if (!GetFieldAvailable(row, col)) return;
            if (CheckFieldNull(row, col)) return;

            string fieldType = GetFieldType(col);
            string fieldName = GetFieldName(col);

            switch (fieldType)
            {
                case ExcelHelper.TYPE_INT:
                    entry.AddField<int>(fieldName, _sheet.GetValue<int>(row, col));
                    break;
                case ExcelHelper.TYPE_FLOAT:
                    entry.AddField<float>(fieldName, _sheet.GetValue<float>(row, col));
                    break;
                case ExcelHelper.TYPE_STRING:
                    entry.AddField<string>(fieldName, _sheet.GetValue<string>(row, col));
                    break;
                case ExcelHelper.TYPE_ELUA:
                    entry.AddFieldWithString<LuaTable>(fieldName, _sheet.GetValue<string>(row, col));
                    break;
                default:
                    entry.AddField<string>(fieldName, _sheet.GetValue<string>(row, col));
                    break;
            }
        }

        private bool CheckFieldNull(int row, int col)
        {
            var field = _sheet.GetValue<string>(row, col);
            return string.IsNullOrEmpty(field);
        }

        private void UpdateEndIndex()
        {
            for (int row = startRow; row < ExcelHelper.LIMIT_ROW; row++)
            {
                string val = _sheet.GetValue<string>(row, startCol);
                if (!string.IsNullOrEmpty(val)) _endRow = row;
                else break;
            }

            for (int col = startCol; col < ExcelHelper.LIMIT_COL; col++)
            {
                string val = _sheet.GetValue<string>(nameRow, col);
                if (!string.IsNullOrEmpty(val)) _endCol = col;
                else break;
            }
        }

        private bool GetFieldAvailable(int row, int col)
        {
            if (string.IsNullOrEmpty(GetFieldName(col)) || string.IsNullOrEmpty(GetFieldType(col)))
            {
                return false;
            }
            return true;
        }

        private string GetFieldName(int col)
        {
            return _sheet.GetValue<string>(nameRow, col);
        }

        private string GetFieldType(int col)
        {
            return _sheet.GetValue<string>(typeRow, col);
        }

        private int GetFieldId(int row)
        {
            return _sheet.GetValue<int>(row, startCol);
        }

    }

    

}