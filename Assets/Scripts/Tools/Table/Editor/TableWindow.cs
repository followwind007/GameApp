using UnityEngine;
using UnityEditor;
using System.IO;

namespace Tools.Table
{
    public class TableWindow : EditorWindow
    {
        private string _svnMessage;

        [MenuItem("Tools/Table", false, 201)]
        public static void ShowWindow()
        {
            GetWindow<TableWindow>("Table Tools");
        }

        private void OnGUI()
        {
            DrawMenu();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();

            _svnMessage = EditorGUILayout.TextField("SVN Message", _svnMessage);

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Tables", GUILayout.Height(20)))
            {
                TableManager.Instance.ExportToLua();
            }
            if (!string.IsNullOrEmpty(_svnMessage))
            {
                if (GUILayout.Button("Commit To SVN", GUILayout.Height(20)))
                    OnClickBtnCommit();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }

        private void DrawMenu()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(position.width));
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                TableManager.Instance.LoadConfig();
            }
            if (GUILayout.Button("Config", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                Selection.activeObject = TableManager.Instance.Config;
            }
            GUILayout.EndHorizontal();
        }

        private void OnClickBtnCommit()
        {
#if TEMPLATE_MODE
            var dir = new DirectoryInfo(TableManager.Instance.Config.exportPath);
            string[] cmds =
            {
                string.Format("svn add {0} --force", dir.FullName),
                string.Format("svn commit -m \"{0}\" {1}", _svnMessage, dir.FullName),
            };
            var output = CmdUtil.RunCmd(cmds);
            Debug.Log(output);
#endif
        }


    }

}