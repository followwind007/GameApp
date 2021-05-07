using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    public class AnimatorView : VisualElement
    {
        public class EdgeConnectorListener : IEdgeConnectorListener
        {
            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
                var provider = AnimatorWindow.Instance.View.SearchProvider;
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(position)), provider);
            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                AnimatorWindow.Instance.View.AddEdge(edge);
            }
        }
        
        public AnimatorSearchProvider SearchProvider { get; }

        public AnimatorGraph Graph { get; }

        public readonly ParameterBoardProvider parameterBoard;
        public readonly InspectorBoardProvider inspectorBoard;

        private AnimatorPrefs Prefs => AnimatorPrefs.Instance;

        public EdgeConnectorListener ConnectorListener { get; }

        public AnimatorData Data { get; private set; }

        private AnimatorDataChecker _checker = new AnimatorDataChecker();
        private readonly ToolbarMenu _warningMenu;

        private bool _isLoading;

        public AnimatorView()
        {
            this.AddStyleSheetPath("Animator/Styles/AnimatorView");
            
            var toolbar = new Toolbar();
            Add(toolbar);

            var paramTgl = new ToolbarToggle
            {
                text = "Parameters", 
                value = Prefs.isParamOn
            };
            paramTgl.RegisterValueChangedCallback(e =>
            {
                Prefs.isParamOn = e.newValue;
                OnParamTglValueChange();
            });
            toolbar.Add(paramTgl);
            
            var inspectorTgl = new ToolbarToggle
            {
                text = "Inspector",
                value = Prefs.isInspectorOn
            };
            inspectorTgl.RegisterValueChangedCallback(e =>
            {
                Prefs.isInspectorOn = e.newValue;
                OnInspectorValueChange();
            });
            toolbar.Add(inspectorTgl);

            var sp = new ToolbarSpacer {flex = true};
            toolbar.Add(sp);
            
            _warningMenu = new ToolbarMenu()
            {
                text = "Info..."
            };
            toolbar.Add(_warningMenu);

            var content = new VisualElement { name = "content" };
            Add(content);
            
            Graph = new AnimatorGraph(this)
            {
                name = "graph",
                graphViewChanged = OnGraphViewChanged
            };
            content.Add(Graph);
            
            parameterBoard = new ParameterBoardProvider(this);
            Graph.Add(parameterBoard.board);
            
            inspectorBoard = new InspectorBoardProvider(this);
            Graph.Add(inspectorBoard.board);

            SearchProvider = ScriptableObject.CreateInstance<AnimatorSearchProvider>();

            ConnectorListener = new EdgeConnectorListener();
            
            Graph.nodeCreationRequest = c =>
            {
                SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), SearchProvider);
            };
            
            RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.S && evt.ctrlKey)
                {
                    DoCheck();
                }
            });
            RegisterCallback<MouseMoveEvent>(evt =>
            {
                AnimatorPrefs.Instance.paramPos = parameterBoard.board.GetPosition();
                AnimatorPrefs.Instance.inspectorPos = inspectorBoard.board.GetPosition();
            });

            RefreshPrefs();
        }

        public void SetAnimatorData(AnimatorData data)
        {
            Data = data;
            _isLoading = true;
            
            Graph.edges.ForEach(e => e.RemoveFromHierarchy());
            Graph.nodes.ForEach(n => n.RemoveFromHierarchy());
            
            LoadAnimatorData();
            
            Graph.UpdateViewTransform();
            
            parameterBoard.Refresh();
            DoCheck();
            OnSelectObject(null);
            
            _isLoading = false;
        }

        public void AddState(AnimatorState state)
        {
            var node = new AnimatorNode(this, state);
            Graph.AddElement(node);
        }

        public void AddTransfer(AnimatorTransfer transfer)
        {
            var fromPort = Graph.GetFromPort(transfer);
            var toPort = Graph.GetToPort(transfer);

            if (fromPort == null || toPort == null)
            {
                Debug.LogError("can not find transfer input port or output port");
                return;
            }
            
            var edge = new AnimatorEdge(this, transfer)
            {
                userData = transfer,
                input = toPort,
                output = fromPort
            };
            
            edge.output.Connect(edge);
            edge.input.Connect(edge);
            Graph.AddElement(edge);
        }

        public void AddEdge(Edge edge)
        {
            var from = (AnimatorNode) edge.output.node;
            var to = (AnimatorNode) edge.input.node;
            var types = AnimatorTypeUtil.GetTransfer(from.State, to.State);
            
            if (types.Count == 0)
            {
                Debug.LogError($"can not transfer from [{from.State.GetType()}] to [{to.State.GetType()}]");
                return;
            }
            
            var transfer = from.State.InternalAddTransfer(to.State, types[0]);
            AddTransfer(transfer);
        }
        
        public void RegisterUndo(string mark, Object obj = null)
        {
            obj = obj ? obj : Data;
            Undo.RegisterCompleteObjectUndo(obj, mark);
        }

        public void DoCheck()
        {
            var res = _checker.GetCheckRes();
            _warningMenu.text = res.Count > 0 ? $"{res.Count} Warning !" : "No Warning";
            _warningMenu.menu.MenuItems().Clear();
            
            res.ForEach(r =>
            {
                _warningMenu.menu.AppendAction(r.desc, action => { r.action?.Invoke(); });
            });
        }

        public void OnSelectObject(Object obj)
        {
            inspectorBoard.CurrentAsset = obj;
        }

        private void LoadAnimatorData()
        {
            if (!Data) return;
            
            Data.states.ForEach(AddState);
            Data.InternalGetTransfers().ForEach(AddTransfer);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            if (_isLoading) return change;
            var illegalRemoves = new List<GraphElement>();
            change.elementsToRemove?.ForEach(e =>
            {
                if (e is AnimatorNode node)
                {
                    Data.InternalRemoveState(node.State);
                }
                else if (e is Edge edge)
                {
                    var transfer = (AnimatorTransfer) edge.userData;
                    transfer.from.InternalRemoveTransfer(transfer);
                }
                else if (e is BlackboardField field)
                {
                    var index = (int) field.userData;
                    var res = Data.InternalRemoveParameter(index);
                    if (res) parameterBoard.Refresh();
                    else illegalRemoves.Add(e);
                }
            });
            illegalRemoves.ForEach(i => { change.elementsToRemove?.Remove(i); });
            
            change.movedElements?.ForEach(m =>
            {
                if (m is AnimatorNode node)
                {
                    RegisterUndo("node move", node.State);
                    node.State.position = m.GetPosition().position;
                }
            });
            return change;
        }

        private void RefreshPrefs()
        {
            OnParamTglValueChange();
            OnInspectorValueChange();
        }

        private void OnParamTglValueChange()
        {
            parameterBoard.board.visible = AnimatorPrefs.Instance.isParamOn;
        }

        private void OnInspectorValueChange()
        {
            inspectorBoard.board.visible = AnimatorPrefs.Instance.isInspectorOn;
        }

    }
}
