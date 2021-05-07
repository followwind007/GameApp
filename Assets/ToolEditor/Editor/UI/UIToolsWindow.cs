#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace ToolEditor.Editor.UITools
{
    public class UIToolsWindow : EditorWindow
    {
        private readonly string[] _tabNames = { UIConst.NAME_UI_PROFILE, UIConst.NAME_RES_MANAGE };

        private int _selectedTab = 0;

        #region UI Profile
        private bool _isUseTarget;
        private GameObject _targetPrefab;
        private string _targetPath = UIConst.PATH_GUI_PREFAB;
        private UIProfiler _profiler;

        private TreeViewState _treeViewState;
        private UIProfileResultTreeView _timeResultTreeView;
        #endregion

        [MenuItem("Tools/UI/Profile")]
        private static void ShowUITools()
        {
            GetWindow<UIToolsWindow>(UIConst.NAME_MODULE);
        }

        private void OnGUI()
        {
            _selectedTab = GUILayout.Toolbar(_selectedTab, _tabNames);
            GUILayout.Space(UIConst.LINE_SPACE);
            switch (_selectedTab)
            {
                case 0:
                    DrawUIProfile();
                    break;
                case 1:
                    break;
                default:
                    break;
            }
            GUILayout.Space(10);
        }

        private void DrawUIProfile()
        {
            _isUseTarget = EditorGUILayout.Toggle(UIConst.NAME_USE_TARGET, _isUseTarget);
            GUILayout.Space(UIConst.LINE_SPACE);
            if (_isUseTarget)
            {
                _targetPrefab = EditorGUILayout.ObjectField(UIConst.NAME_TARGET, _targetPrefab, typeof(GameObject), true) as GameObject;
            }
            else
            {
                Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(position.size.x - 5));
                Rect rectLeft = new Rect(rect.x, rect.y, UIConst.LABEL_WIDTH, rect.height);
                Rect rectRight = new Rect(rect.x + UIConst.LABEL_WIDTH, rect.y, rect.width - UIConst.LABEL_WIDTH, rect.height);
                EditorGUI.LabelField(rectLeft, UIConst.NAME_TARGET_PATH);
                _targetPath = EditorGUI.TextField(rectRight, _targetPath);

                if ((Event.current.type == EventType.DragUpdated
                  || Event.current.type == EventType.DragExited)
                  && rect.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        _targetPath = DragAndDrop.paths[0];
                    }
                }
            }
            GUILayout.Space(UIConst.LINE_SPACE);
            DrawProfileResult();
            GUILayout.FlexibleSpace();

            if (!Application.isPlaying)
                EditorGUILayout.HelpBox(UIConst.NOTICE_RUN_IN_PLAY_MODE, MessageType.None);
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button(UIConst.NAME_PROFILE))
                ProfileTime();

            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        private void DrawProfileProgress(int index, int count, string current)
        {
            if (index < count)
            {
                var desc = string.Format("{0} ({1}/{2})", current, index, count);
                EditorUtility.DisplayProgressBar(UIConst.NAME_MODULE, desc, (float)index / count);
            }
            else
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void DrawProfileResult()
        {
            if (_timeResultTreeView != null)
            {
                var rect = EditorGUILayout.GetControlRect(GUILayout.Width(position.size.x - 5), GUILayout.Height(position.size.y - 150));
                _timeResultTreeView.OnGUI(rect);
            }
        }

        private void ProfileTime()
        {
            _profiler = new UIProfiler
            {
                fullPath = _targetPath,
            };
            _profiler.progressCallback = DrawProfileProgress;
            _profiler.Profile();

            if (_profiler.ResultTimeList.Count > 0)
            {
                _treeViewState = new TreeViewState();
                _timeResultTreeView = new UIProfileResultTreeView(_treeViewState)
                {
                    profileTimeList = _profiler.ResultTimeList,
                };
                _timeResultTreeView.Init();
            }
        }



    }

}
#endif