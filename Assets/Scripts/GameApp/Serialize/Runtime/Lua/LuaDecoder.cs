using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace GameApp.Serialize
{
    public static class LuaDecoder
    {
        [ThreadStatic] private static Stack<List<string>> _splitArrayPool;
        [ThreadStatic] private static StringBuilder _stringBuilder;
        [ThreadStatic] private static Dictionary<Type, Dictionary<string, FieldInfo>> _fieldInfoCache;
        [ThreadStatic] private static Dictionary<Type, Dictionary<string, PropertyInfo>> _propertyInfoCache;
        [ThreadStatic] private static bool _initDone;

        public static T DecodeFromLua<T>(this string str)
        {
            if (!_initDone)
            {
                // Initialize, if needed, the ThreadStatic variables
                _propertyInfoCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
                _fieldInfoCache = new Dictionary<Type, Dictionary<string, FieldInfo>>();
                _stringBuilder = new StringBuilder();
                _splitArrayPool = new Stack<List<string>>();
                _initDone = true;
            }

            //Remove all whitespace not within strings to make parsing simpler
            _stringBuilder.Length = 0;
            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (c == '"')
                {
                    i = AppendUntilStringEnd(true, i, str);
                    continue;
                }
                if (char.IsWhiteSpace(c)) continue;

                _stringBuilder.Append(c);
            }

            //Parse the thing!
            return (T)StringToVal(typeof(T), _stringBuilder.ToString());
        }

        private static int AppendUntilStringEnd(bool appendEscapeCharacter, int startIdx, string str)
        {
            _stringBuilder.Append(str[startIdx]);
            for (var i = startIdx + 1; i<str.Length; i++)
            {
                if (str[i] == '\\')
                {
                    if (appendEscapeCharacter) _stringBuilder.Append(str[i]);
                    _stringBuilder.Append(str[i + 1]);
                    //Skip next character as it is escaped
                    i++;
                }
                else if (str[i] == '"')
                {
                    _stringBuilder.Append(str[i]);
                    return i;
                }
                else
                    _stringBuilder.Append(str[i]);
            }
            return str.Length - 1;
        }

        //Splits {<value>=<value>,<value>=<value>} and {<value>,<value>} into a list of <value> strings
        private static List<string> Split(string str)
        {
            return SplitAndCheckList(str, out _);
        }
        
        private static List<string> SplitAndCheckList(string str, out bool isList)
        {
            isList = true;
            var splitArray = _splitArrayPool.Count > 0 ? _splitArrayPool.Pop() : new List<string>();
            splitArray.Clear();
            if(str.Length == 2) return splitArray;
            
            var parseDepth = 0;
            _stringBuilder.Length = 0;

            for (var i = 1; i < str.Length - 1; i++)
            {
                var c = str[i];
                
                if (c == '{') parseDepth++;
                else if (c == '}') parseDepth--;
                else if (c == '"')
                {
                    i = AppendUntilStringEnd(true, i, str);
                    continue;
                }
                else if (c == ',' || c == '=')
                {
                    if (c == '=') isList = false;
                    
                    if (parseDepth == 0)
                    {
                        if (!string.IsNullOrEmpty(_stringBuilder.ToString())) 
                            splitArray.Add(_stringBuilder.ToString());
                        _stringBuilder.Length = 0;
                        continue;
                    }
                }

                _stringBuilder.Append(str[i]);
            }
            
            if (!string.IsNullOrEmpty(_stringBuilder.ToString())) 
                splitArray.Add(_stringBuilder.ToString());
            
            return splitArray;
        }

        internal static object StringToVal(Type type, string str)
        {
            if (type == typeof(string))
            {
                if (str.Length <= 2)
                    return string.Empty;
                var parseStringBuilder = new StringBuilder(str.Length);
                for (var i = 1; i<str.Length-1; ++i)
                {
                    if (str[i] == '\\' && i + 1 < str.Length - 1)
                    {
                        var j = "\"\\nrtbf/".IndexOf(str[i + 1]);
                        if (j >= 0)
                        {
                            parseStringBuilder.Append("\"\\\n\r\t\b\f/"[j]);
                            ++i;
                            continue;
                        }
                        if (str[i + 1] == 'u' && i + 5 < str.Length - 1)
                        {
                            if (uint.TryParse(str.Substring(i + 2, 4), System.Globalization.NumberStyles.AllowHexSpecifier, null, out var c))
                            {
                                parseStringBuilder.Append((char)c);
                                i += 5;
                                continue;
                            }
                        }
                    }
                    parseStringBuilder.Append(str[i]);
                }
                return parseStringBuilder.ToString();
            }
            if (type.IsPrimitive)
            {
                var result = Convert.ChangeType(str, type, System.Globalization.CultureInfo.InvariantCulture);
                return result;
            }
            if (type == typeof(decimal))
            {
                decimal.TryParse(str, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var result);
                return result;
            }
            if (str == "nil")
            {
                return null;
            }
            if (type.IsEnum)
            {
                if (str[0] == '"')
                    str = str.Substring(1, str.Length - 2);
                try
                {
                    return Enum.Parse(type, str, false);
                }
                catch
                {
                    return 0;
                }
            }
            if (type.IsArray)
            {
                var arrayType = type.GetElementType();
                if (str[0] != '{' || str[str.Length - 1] != '}')
                    return null;

                var elems = Split(str);
                var newArray = Array.CreateInstance(arrayType ?? throw new InvalidOperationException(), elems.Count);
                for (var i = 0; i < elems.Count; i++)
                    newArray.SetValue(StringToVal(arrayType, elems[i]), i);
                _splitArrayPool.Push(elems);
                return newArray;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var listType = type.GetGenericArguments()[0];
                if (str[0] != '{' || str[str.Length - 1] != '}')
                    return null;

                var elems = Split(str);
                var list = (IList)type.GetConstructor(new[] { typeof(int) })?.Invoke(new object[] { elems.Count });
                foreach (var t in elems) if (list != null) list.Add(StringToVal(listType, t));

                _splitArrayPool.Push(elems);
                return list;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type keyType, valueType;
                {
                    var args = type.GetGenericArguments();
                    keyType = args[0];
                    valueType = args[1];
                }

                //Refuse to parse dictionary keys that aren't of type string
                if (keyType != typeof(string))
                    return null;
                //Must be a valid dictionary element
                if (str[0] != '{' || str[str.Length - 1] != '}')
                    return null;
                //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid str
                var elems = Split(str);
                if (elems.Count % 2 != 0)
                    return null;

                var dictionary = (IDictionary)type.GetConstructor(new[] { typeof(int) })?.Invoke(new object[] { elems.Count / 2 });
                for (var i = 0; i < elems.Count; i += 2)
                {
                    if (elems[i].Length <= 2) continue;
                    var keyStr = elems[i].Substring(1, elems[i].Length - 2);
                    var keyValue = ParseAnonymousValue(keyStr);
                    var val = StringToVal(valueType, elems[i + 1]);
                    if (dictionary != null) dictionary.Add(keyValue, val);
                }
                return dictionary;
            }
            if (type == typeof(object))
            {
                return ParseAnonymousValue(str);
            }
            if (str[0] == '{' && str[str.Length - 1] == '}')
            {
                return ParseObject(type, str);
            }

            return null;
        }

        private static object ParseAnonymousValue(string str)
        {
            if (str.Length == 0)
                return null;
            if (str[0] == '{' && str[str.Length - 1] == '}')
            {
                var elems = SplitAndCheckList(str, out var isList);
                if (isList)
                {
                    var finalList = new List<object>(elems.Count);
                    foreach (var t in elems) finalList.Add(ParseAnonymousValue(t));

                    return finalList;
                }
                else
                {
                    if (elems.Count % 2 != 0) return null;
                    var dict = new Dictionary<string, object>(elems.Count / 2);
                    for (var i = 0; i < elems.Count; i += 2)
                        dict.Add(elems[i].Substring(1, elems[i].Length - 2), ParseAnonymousValue(elems[i + 1]));
                    return dict;
                }
            }
            if (str[0] == '"' && str[str.Length - 1] == '"')
            {
                var str1 = str.Substring(1, str.Length - 2);
                return str1.Replace("\\", string.Empty);
            }
            if (char.IsDigit(str[0]) || str[0] == '-')
            {
                if (str.Contains("."))
                {
                    double.TryParse(str, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var result);
                    return result;
                }
                else
                {
                    int.TryParse(str, out var result);
                    return result;
                }
            }
            if (str == "true")
                return true;
            if (str == "false")
                return false;
            // handles str == "null" as well as invalid str
            return null;
        }

        private static Dictionary<string, T> CreateMemberNameDictionary<T>(T[] members) where T : MemberInfo
        {
            var nameToMember = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            foreach (var member in members)
            {
                if (member.IsDefined(typeof(IgnoreDataMemberAttribute), true))
                    continue;

                var name = member.Name;
                if (member.IsDefined(typeof(DataMemberAttribute), true))
                {
                    var dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
                    if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                        name = dataMemberAttribute.Name;
                }

                nameToMember.Add(name, member);
            }

            return nameToMember;
        }

        private static object ParseObject(Type type, string str)
        {
            var instance = FormatterServices.GetUninitializedObject(type);

            //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid str
            var elems = Split(str);
            if (elems.Count % 2 != 0)
                return instance;

            if (!_fieldInfoCache.TryGetValue(type, out var nameToField))
            {
                nameToField = CreateMemberNameDictionary(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
                _fieldInfoCache.Add(type, nameToField);
            }
            if (!_propertyInfoCache.TryGetValue(type, out var nameToProperty))
            {
                nameToProperty = CreateMemberNameDictionary(type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
                _propertyInfoCache.Add(type, nameToProperty);
            }

            for (var i = 0; i < elems.Count; i += 2)
            {
                if (elems[i].Length <= 2) continue;
                var key = elems[i].Substring(0, elems[i].Length);
                var value = elems[i + 1];

                if (nameToField.TryGetValue(key, out var fieldInfo))
                    fieldInfo.SetValue(instance, StringToVal(fieldInfo.FieldType, value));
                else if (nameToProperty.TryGetValue(key, out var propertyInfo))
                    propertyInfo.SetValue(instance, StringToVal(propertyInfo.PropertyType, value), null);
            }

            return instance;
        }
    }
}