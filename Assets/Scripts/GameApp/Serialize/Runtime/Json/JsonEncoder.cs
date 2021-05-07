using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace GameApp.Serialize
{
    public static class JsonEncoder
    {
        public static string EncodeToJson(this object item)
        {
            var stringBuilder = new StringBuilder();
            AppendValue(stringBuilder, item);
            return stringBuilder.ToString();
        }

        private static void AppendValue(StringBuilder stringBuilder, object item)
        {
            if (item == null)
            {
                stringBuilder.Append("null");
                return;
            }

            var type = item.GetType();
            if (type == typeof(string))
            {
                stringBuilder.Append('"');
                var str = (string)item;
                foreach (var t in str)
                {
                    if (t < ' ' || t == '"' || t == '\\')
                    {
                        stringBuilder.Append('\\');
                        var j = "\"\\\n\r\t\b\f".IndexOf(t);
                        if (j >= 0)
                            stringBuilder.Append("\"\\nrtbf"[j]);
                        else
                            stringBuilder.AppendFormat("u{0:X4}", (uint)t);
                    }
                    else
                    {
                        stringBuilder.Append(t);
                    }
                }
                stringBuilder.Append('"');
            }
            else if (type == typeof(byte) || type == typeof(int))
            {
                stringBuilder.Append(item);
            }
            else if (type == typeof(float))
            {
                stringBuilder.Append(((float)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else if (type == typeof(double))
            {
                stringBuilder.Append(((double)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else if (type == typeof (decimal)) {
                stringBuilder.Append (((decimal)item).ToString (System.Globalization.CultureInfo.InvariantCulture));
            }
            else if (type == typeof(bool))
            {
                stringBuilder.Append((bool)item ? "true" : "false");
            }
            else if (type.IsEnum)
            {
                stringBuilder.Append('"');
                stringBuilder.Append(item);
                stringBuilder.Append('"');
            }
            else if (item is IList list)
            {
                stringBuilder.Append('[');
                var isFirst = true;
                foreach (var t in list)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(',');
                    AppendValue(stringBuilder, t);
                }
                stringBuilder.Append(']');
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var keyType = type.GetGenericArguments()[0];

                //Refuse to output dictionary keys that aren't of type string
                if (keyType != typeof(string))
                {
                    stringBuilder.Append("{}");
                    return;
                }

                stringBuilder.Append('{');
                var dict = item as IDictionary;
                var isFirst = true;
                if (dict != null)
                {
                    foreach (var key in dict.Keys)
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            stringBuilder.Append(',');
                        stringBuilder.Append('\"');
                        stringBuilder.Append((string) key);
                        stringBuilder.Append("\":");
                        AppendValue(stringBuilder, dict[key]);
                    }
                }

                stringBuilder.Append('}');
            }
            else
            {
                stringBuilder.Append('{');

                var isFirst = true;
                var fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                foreach (var t in fieldInfos)
                {
                    if (t.IsDefined(typeof(IgnoreDataMemberAttribute), true))
                        continue;

                    var value = t.GetValue(item);
                    if (value != null)
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            stringBuilder.Append(',');
                        stringBuilder.Append('\"');
                        stringBuilder.Append(GetMemberName(t));
                        stringBuilder.Append("\":");
                        AppendValue(stringBuilder, value);
                    }
                }
                var propertyInfo = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                foreach (var t in propertyInfo)
                {
                    if (!t.CanRead || t.IsDefined(typeof(IgnoreDataMemberAttribute), true))
                        continue;

                    var value = t.GetValue(item, null);
                    if (value != null)
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            stringBuilder.Append(',');
                        stringBuilder.Append('\"');
                        stringBuilder.Append(GetMemberName(t));
                        stringBuilder.Append("\":");
                        AppendValue(stringBuilder, value);
                    }
                }

                stringBuilder.Append('}');
            }
        }

        private static string GetMemberName(MemberInfo member)
        {
            if (member.IsDefined(typeof(DataMemberAttribute), true))
            {
                var dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
                if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                    return dataMemberAttribute.Name;
            }

            return member.Name;
        }
    }
}
