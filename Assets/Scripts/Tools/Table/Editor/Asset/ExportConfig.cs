using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Tools.Table.Asset
{
    [System.Serializable]
    public class TableConfig
    {
        public string table;
        public string sheet;
        public string export;
        public bool isUse;
    }

    [System.Serializable]
    public class TableConfigs
    {
        [SerializeField]
        public List<TableConfig> list;
    }

    public class ExportConfig : ScriptableObject
    {
        [MenuItem("Assets/Create/Table/ExportConfig")]
        public static void CreateAsset()
        {
            AssetUtil.CreateAsset<ExportConfig>("ExportConfig");
        }

        public string excelPath;
        public string exportPath;

        public TableConfigs tables;

        public string GetTablePath(TableConfig config)
        {
            return string.Format("{0}{1}.xlsx", excelPath, config.table);
        }

        public string GetExportPath(TableConfig config, string fileType)
        {
            return string.Format("{0}{1}.{2}", exportPath, config.export, fileType);
        }
 
    }

}