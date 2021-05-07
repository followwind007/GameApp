#if UNITY_EDITOR
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ToolEditor.Editor.UITools
{
    public class UIProfileResultItem: TreeViewItem
    {
        public ProfileTimeItem timeItem;
    }

    public class UIProfileResultTreeView : TreeView
    {
        private SearchField _searchField = new SearchField();

        public List<ProfileTimeItem> profileTimeList = new List<ProfileTimeItem>();

        public UIProfileResultTreeView(TreeViewState state) : base(state)
        {
            rowHeight = 20;
            showBorder = true;
            
        }

        public void Init()
        {
            MultiColumnHeaderState headerState = null;
            if (profileTimeList.Count > 0)
            {
                List<MultiColumnHeaderState.Column> stateList = new List<MultiColumnHeaderState.Column>()
                {
                    new MultiColumnHeaderState.Column()
                    {
                        headerContent = new GUIContent(ProfileTimeItem.COL_PATH),
                        minWidth = 200,
                        width = 200,
                    },
                    new MultiColumnHeaderState.Column()
                    {
                        headerContent = new GUIContent(ProfileTimeItem.COL_LOAD),
                        minWidth = 100,
                        width = 100,
                    },
                    new MultiColumnHeaderState.Column()
                    {
                        headerContent = new GUIContent(ProfileTimeItem.COL_INSTANTIATE),
                        minWidth = 100,
                        width = 100,
                    },
                    new MultiColumnHeaderState.Column()
                    {
                        headerContent = new GUIContent(ProfileTimeItem.COL_TOTAL),
                        minWidth = 100,
                        width = 100,
                    },
                    new MultiColumnHeaderState.Column()
                    {
                        headerContent = new GUIContent(ProfileTimeItem.COL_SIZE),
                        minWidth = 100,
                        width = 100,
                    },
                };
                headerState = new MultiColumnHeaderState(stateList.ToArray());
            }

            if (headerState != null)
            {
                multiColumnHeader = new MultiColumnHeader(headerState);
                multiColumnHeader.sortingChanged += OnSortingChanged;
            }
            Reload();
        }

        private void OnSortingChanged(MultiColumnHeader multiColumnHeader)
        {
            if (profileTimeList.Count > 0)
            {
                ProfileTimeItem.sortType = (ProfileTimeItem.SortType)multiColumnHeader.sortedColumnIndex;
                if (multiColumnHeader.IsSortedAscending(multiColumnHeader.sortedColumnIndex))
                    ProfileTimeItem.isInverse = false;
                else
                    ProfileTimeItem.isInverse = true;
                profileTimeList.Sort();
                Reload();
            }
        }

        public override void OnGUI(Rect rect)
        {
            var searchRect = rect;
            searchRect.height = 20;
            searchString = _searchField.OnGUI(searchRect, searchString);
            rect.y += 20;
            base.OnGUI(rect);
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new UIProfileResultItem()
            {
                id = -1,
                depth = -1,
            };
            return root;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            List<UIProfileResultItem> itemList = new List<UIProfileResultItem>();

            if (profileTimeList != null)
            {
                for (int i = 0; i < profileTimeList.Count; i++)
                {
                    var timeItem = profileTimeList[i];
                    if (string.IsNullOrEmpty(searchString) || timeItem.path.Contains(searchString))
                    {
                        var item = new UIProfileResultItem()
                        {
                            id = i,
                            timeItem = timeItem,
                        };
                        
                        root.AddChild(item);
                        itemList.Add(item);
                    }
                }
            }
            var rows = itemList.ToArray();
            SetupDepthsFromParentsAndChildren(root);
            return rows;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = args.item as UIProfileResultItem;
            
            if (item.timeItem != null)
            {
                DrawProfileTimeRow(item.timeItem, args);
            }
        }

        private void DrawProfileTimeRow(ProfileTimeItem timeItem, RowGUIArgs args)
        {
            for (int i = 0; i < 5; i++)
            {
                var rect = args.GetCellRect(i);
                switch (i)
                {
                    case 0:
                        EditorGUI.LabelField(rect, timeItem.SubPath);
                        break;
                    case 1:
                        EditorGUI.LabelField(rect, string.Format("{0:F}", timeItem.loadDura));
                        break;
                    case 2:
                        EditorGUI.LabelField(rect, string.Format("{0:F}", timeItem.instantiateDura));
                        break;
                    case 3:
                        EditorGUI.LabelField(rect, string.Format("{0:F}", timeItem.TotalDura));
                        break;
                    case 4:
                        EditorGUI.LabelField(rect, string.Format("{0:F}", timeItem.fileSize / 1000f));
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void DoubleClickedItem(int id)
        {
            var rows = GetRows();
            foreach (var row in rows)
            {
                if (row.id == id)
                {
                    var item = row as UIProfileResultItem;
                    if (item.timeItem != null)
                    {
                        Debug.Log(string.Format("path: {0}", item.timeItem.path));
                    }
                }
            }
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }


    }
}
#endif