using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Tools.Table
{
    public class AssetReader
    {
        private readonly string _path;

        private const string NameColIndex = "__index";
        private const string NameColId = "id";
        
        public AssetReader(string path)
        {
            _path = path;
        }

        public TableObject Read()
        {
            var asset = AssetDatabase.LoadAssetAtPath<TableAsset>(_path);
            if (asset == null)
            {
                Debug.LogError($"can not find asset at: {_path}");
                return null;
            }
            
            var table = new TableObject();
            var colId = asset.columns.First(c => c.name == NameColId);
            if (colId == null)
            {
                Debug.LogError($"can not find id column in table: {_path}");
                return null;
            }

            foreach (var v in colId.values)
            {
                var entry = new TableEntry();
                entry.AddField(NameColIndex, colId[v.index]);
                table.AddEntry(colId[v.index], entry);
            }

            foreach (var c in asset.columns)
            {
                foreach (var v in c.values)
                {
                    var entry = table.GetEntry(v.index);
                    entry?.AddField(c.name, c[v.index]);
                }
            }

            return table;
        }
        
    }
}