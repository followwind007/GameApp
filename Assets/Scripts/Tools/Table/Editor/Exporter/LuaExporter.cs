using UnityEngine;
using System.Text;
using Tools.Table.Asset;

namespace Tools.Table.Exporter
{
    public class LuaExporter : IExporter
    {
        public string Export(TableObject table, TableConfig config)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("--{0}/{1}--\n\n", config.table, config.sheet);
            builder.AppendLine("local data = {");

            foreach (var kv in table.TableDict)
            {
                var line = GetContent(table, kv.Key);
                if (kv.Key is int)
                    builder.AppendFormat("\t[{0}]={1}\n", kv.Key, line);
                else if (kv.Key is string)
                    builder.AppendFormat("\t['{0}']={1}\n", kv.Key, line);
                else
                    Debug.LogError("unsupport id type: " + kv.Key.GetType());
            }

            builder.Append("}\n\n");
            builder.AppendLine("return data");

            Debug.Log(builder.ToString());
            return builder.ToString();
        }

        private string GetContent(TableObject table, object id)
        {
            var entry = table.GetEntry(id);
            var builder = new StringBuilder();

            builder.Append("{");
            foreach (var kv in entry.fieldDict)
            {
                var type = table.GetColumnType(kv.Key);
                builder.AppendFormat("{0}=", kv.Key);
                if (type == ValueType.Int || type == ValueType.Float)
                {
                    builder.Append(kv.Value);
                }
                else if (type == ValueType.Bool)
                {
                    builder.Append((bool)kv.Value ? "true" : "false");
                }
                else if (type == ValueType.String)
                {
                    if (kv.Value != null)
                    {
                        var content = ((string) kv.Value).Replace("\"", "\\\"");
                        content = content.Replace("'", "\\'");
                        content = content.Replace("\n", "\\n");
                        content = content.Replace("\t", "\\t");
                        builder.AppendFormat("'{0}'", content);
                    }
                }
                else if (type == ValueType.Elua)
                {
                    builder.Append(((string) kv.Value).Replace('"', '\''));
                }
                builder.Append(",");
            }
            builder.Append("},");

            return builder.ToString();
        }

    }
}