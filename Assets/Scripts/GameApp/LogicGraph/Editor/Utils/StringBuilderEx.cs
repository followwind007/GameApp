using System.Text;

namespace GameApp.LogicGraph
{
    public static class StringBuilderEx
    {
        public static void IndentAppend(this StringBuilder sb, string content, int indent)
        {
            sb.Append($"{GetIndentString(indent)}{content}");
        }
        
        public static void IndentAppendLine(this StringBuilder sb, string content, int indent)
        {
            sb.AppendLine($"{GetIndentString(indent)}{content}");
        }

        private static string GetIndentString(int indent)
        {
            var strIndent = "";
            for (var i = 0; i < indent; i++)
            {
                strIndent += "\t";
            }

            return strIndent;
        }
        
    }
}