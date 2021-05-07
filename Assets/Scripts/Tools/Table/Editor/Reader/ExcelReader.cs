using OfficeOpenXml;
using System.IO;
using System.Collections.Generic;
using LuaInterface;
using UnityEditor;
using UnityEngine;

namespace Tools.Table.Excel
{
    public class ExcelReader
    {
        public int nameRow = ExcelHelper.NAME_ROW;
        public int typeRow = ExcelHelper.TYPE_ROW;
        public int startRow = ExcelHelper.START_ROW;
        public int startCol = ExcelHelper.START_COL;
        public int nullableRow = ExcelHelper.NULLABLE_ROW;

        private int _endRow;
        private int _endCol;

        private readonly string _path;

        private ExcelWorksheet _sheet;

        private bool _nullNoticeShown;

        public ExcelReader(string path)
        {
            _path = path;
        }

        public TableObject Read(string sheetName)
        {
            var fileInfo = new FileInfo(_path);
            var package = new ExcelPackage(fileInfo);

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
            SetTableColmnType(table);
            return table;
        }

        private void SetTableColmnType(TableObject table)
        {
            var columnType = new Dictionary<string, ValueType>();
            for (int i = startCol; i <= _endCol; i++)
            {
                var fieldName = GetFieldName(i);
                var fieldType = GetFieldType(i);
                switch (fieldType)
                {
                    case ExcelHelper.TYPE_INT:
                        columnType[fieldName] = ValueType.Int;
                        break;
                    case ExcelHelper.TYPE_FLOAT:
                        columnType[fieldName] = ValueType.Float;
                        break;
                    case ExcelHelper.TYPE_BOOL:
                        columnType[fieldName] = ValueType.Bool;
                        break;
                    case ExcelHelper.TYPE_STRING:
                        columnType[fieldName] = ValueType.String;
                        break;
                    case ExcelHelper.TYPE_ELUA:
                        columnType[fieldName] = ValueType.Elua;
                        break;
                    default:
                        break;
                }
            }
            table.SetColumnType(columnType);
        }

        private void AddField(TableEntry entry, int row, int col)
        {
            if (!GetFieldAvailable(col)) return;
            if (CheckFieldNull(row, col)) return;

            string fieldType = GetFieldType(col);
            string fieldName = GetFieldName(col);

            switch (fieldType)
            {
                case ExcelHelper.TYPE_INT:
                    entry.AddField(fieldName, _sheet.GetValue<int>(row, col));
                    break;
                case ExcelHelper.TYPE_FLOAT:
                    entry.AddField(fieldName, _sheet.GetValue<float>(row, col));
                    break;
                case ExcelHelper.TYPE_BOOL:
                    var str = _sheet.GetValue<string>(row, col);
                    var res = false;
                    if (!string.IsNullOrEmpty(str))
                    {
                        res = str.ToLower().Contains("true");
                    }
                    entry.AddField(fieldName, res);
                    break;
                case ExcelHelper.TYPE_STRING:
                    entry.AddField(fieldName, _sheet.GetValue<string>(row, col));
                    break;
                case ExcelHelper.TYPE_ELUA:
                    entry.AddFieldWithString<LuaTable>(fieldName, _sheet.GetValue<string>(row, col));
                    break;
                default:
                    entry.AddField(fieldName, _sheet.GetValue<string>(row, col));
                    break;
            }
        }

        private bool CheckFieldNull(int row, int col)
        {
            var field = _sheet.GetValue<string>(row, col);
            var isNullable = GetFieldNullable(col);
            
            if (!isNullable && string.IsNullOrEmpty(field) && !_nullNoticeShown)
            {
                _nullNoticeShown = true;
                EditorUtility.DisplayDialog("Error", string.Format("\"{0}({1})\" at row:{2} col:{3} can not be null", _path, _sheet.Name, row, col), "OK");
            }
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

        private bool GetFieldAvailable(int col)
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

        private object GetFieldId(int row)
        {
            var idType = GetFieldType(startCol);
            if (idType == ExcelHelper.TYPE_INT)
            {
                return _sheet.GetValue<int>(row, startCol);
            }
            else if (idType == ExcelHelper.TYPE_STRING)
            {
                return _sheet.GetValue<string>(row, startCol);
            }
            Debug.LogError("unsupport id type: " + idType);
            return null;
        }

        private bool GetFieldNullable(int col)
        {
            var field = _sheet.GetValue<string>(nullableRow, col);
            return string.IsNullOrEmpty(field) || !field.ToLower().Equals("y");
        }

    }



}