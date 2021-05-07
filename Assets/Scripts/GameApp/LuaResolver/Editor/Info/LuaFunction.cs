using System;
using System.Collections.Generic;
using System.Text;

namespace GameApp.LuaResolver
{
    [Serializable]
    public class LuaFunction
    {
        public string name;
        public string selfName;

        public string summary;

        public List<ParameterInfo> paramters = new List<ParameterInfo>();
        public ReturnInfo returnInfo = new ReturnInfo();

        public bool useSelf;
        public string Id => GetId();

        public int defineLine;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{selfName}.{name}(");

            for (var i = 0; i < paramters.Count; i++)
            {
                var p = paramters[i];
                var desc = $"{p.typeName} {p.name}";
                sb.Append(i < paramters.Count - 1 ? $"{desc}, " : $"{desc}");
            }

            sb.Append(")");
            if (returnInfo != null)
            {
                sb.Append(": ");
                var ts = returnInfo.typeNames;
                for (var i = 0; i < ts.Count; i++)
                {
                    var t = ts[i];
                    sb.Append(i < ts.Count - 1 ? $"{t}, " : $"{t}");
                }
            }

            return sb.ToString();
        }

        private string GetId()
        {
            var sb = new StringBuilder();
            sb.Append($"{name}(");

            for (var i = 0; i < paramters.Count; i++)
            {
                var p = paramters[i];
                var desc = $"{ParseUtil.GetTypeShortName(p.typeName)}";
                sb.Append(i < paramters.Count - 1 ? $"{desc}, " : $"{desc}");
            }

            sb.Append(")");
            return sb.ToString();
        }
        
    }
}