using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Tools.Table.Asset;
using Tools.Table.Excel;
using Tools.Table.Exporter;

namespace Tools.Table
{
    public class TableManager
    {
        private struct ExportElement
        {
            public TableObject table;
            public TableConfig config;
            public ExportElement(TableObject table, TableConfig config)
            {
                this.table = table;
                this.config = config;
            }
        }

        private static TableManager _instance;
        public static TableManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TableManager();
                    _instance.Init();
                }
                return _instance;
            }
        }

        public ExportConfig Config { get; private set; }

        private List<ExportElement> _exportList = new List<ExportElement>();

        private void Init()
        {
            LoadConfig();
        }

        public void LoadConfig()
        {
            Config = Resources.Load<ExportConfig>("ExportConfig");
            if (Config == null)
            {
                Debug.LogError("Can not find ExportConfig.asset");
            }
        }

        public void ExportToLua()
        {
            UpdateExportList();
            var luaExporter = new LuaExporter();
            for (int i = 0; i < _exportList.Count; i++)
            {
                var export = _exportList[i];

                var desc = string.Format("{0}{1}", export.config.sheet, export.config.export);
                EditorUtility.DisplayProgressBar("Write Lua table", desc, (float)i / _exportList.Count);

                var content = luaExporter.Export(export.table, export.config);
                var exportPath = Config.GetExportPath(export.config, "lua");
                File.WriteAllText(exportPath, content);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        private void UpdateExportList()
        {
            _exportList.Clear();
            var count = Config.tables.list.Count;
            for (int i = 0; i < count; i++)
            {
                var config = Config.tables.list[i];
                var path = Config.GetTablePath(config);
                
                var desc = string.Format("{0}/{1}", path, config.sheet);
                Debug.Log(desc);
                EditorUtility.DisplayProgressBar("Read table", desc, (float)i / count);

                var table = ExcelHelper.ReadTable(path, config.sheet);
                _exportList.Add(new ExportElement(table, config));
            }
            EditorUtility.ClearProgressBar();
        }

        private void DrawProfileProgress(int index, int count, string current)
        {
            if (index < count)
            {
                var desc = string.Format("{0} ({1}/{2})", current, index, count);
                EditorUtility.DisplayProgressBar("", desc, (float)index / count);
            }
            else
            {
                EditorUtility.ClearProgressBar();
            }
        }

    }
}
