using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace GameApp.DebugConsole
{
    public class CommandItem : TreeViewItem
    {
        public ICommand command;
    }

    public class CommandTreeView : TreeView
    {
        public List<ICommand> commandList = new List<ICommand>();

        public System.Action<ICommand> useCommand;

        private readonly SearchField _searchField = new SearchField();

        public CommandTreeView(TreeViewState state) : base(state)
        {
            rowHeight = 24;
            showBorder = true;
        }

        public void Init()
        {
            var stateList = new List<MultiColumnHeaderState.Column>()
            {
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("Command"),
                    minWidth = 100,
                    width = 100,
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("Describe"),
                    minWidth = 120,
                    width = 120,
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("Operation"),
                    minWidth = 120,
                    width = 120,
                },

            };
            var headerState = new MultiColumnHeaderState(stateList.ToArray());

            multiColumnHeader = new MultiColumnHeader(headerState)
            {
                canSort = false
            };
            Reload();
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
            var root = new CommandItem()
            {
                id = -1,
                depth = -1,
            };
            return root;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var itemList = new List<TreeViewItem>();

            if (commandList != null)
            {
                for (var i = 0; i < commandList.Count; i++)
                {
                    var commandItem = commandList[i];
                    if (!string.IsNullOrEmpty(searchString) && !commandItem.Name.Contains(searchString)) continue;
                    var item = new CommandItem
                    {
                        id = i,
                        command = commandItem,
                    };

                    root.AddChild(item);
                    itemList.Add(item);
                }
            }
            var rows = itemList.ToArray();
            SetupDepthsFromParentsAndChildren(root);
            return rows;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = args.item as CommandItem;

            for (var i = 0; i < 3; i++)
            {
                var rect = args.GetCellRect(i);
                switch (i)
                {
                    case 0:
                        if (item != null) EditorGUI.LabelField(rect, item.command.Name);
                        break;
                    case 1:
                        if (item != null) EditorGUI.LabelField(rect, item.command.Describe);
                        break;
                    case 2:
                        const int btnWidth = 56;
                        var btnHeight = rect.height - 4;
                        var useRect = new Rect(rect.x, rect.y + 2, btnWidth, btnHeight);
                        if (GUI.Button(useRect, "Use"))
                            OnClickUse(item);
                        var deleteRect = new Rect(rect.x + btnWidth + 4, rect.y + 2, btnWidth, btnHeight);
                        if (GUI.Button(deleteRect, "Delete"))
                            OnClickDelete(item);
                        break;
                }
            }
        }

        protected override void DoubleClickedItem(int id)
        {
            if (GetRows()[id] is CommandItem item) item.command.DoCommand();
        }

        private void OnClickDelete(CommandItem item)
        {
            CommandManager.Instance.DeleteLuaCommand(item.command.Name);
            Reload();
        }

        private void OnClickUse(CommandItem item)
        {
            useCommand?.Invoke(item.command);
        }


    }
}