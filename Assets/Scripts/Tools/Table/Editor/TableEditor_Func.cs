using UnityEngine;

namespace Tools.Table
{
    public partial class TableEditor
    {
        private static void DrawExposedFuncs(PropertyItem item, int indent)
        {
            foreach (var lfs in item.Attrs.exposedLineFuncs)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(LayoutUtil.GetIndent(indent));
                foreach (var f in lfs)
                {
                    var method = f.method;
                    var ps = method.GetParameters();
                    if (ps.Length == 0)
                    {
                        if (GUILayout.Button(f.DisplayName))
                        {
                            f.method.Invoke(item.obj, null);
                        }
                    }
                    else
                    {
                        
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}