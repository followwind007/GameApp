using UnityEngine;
using UnityEngine.UIElements;
using GameApp.DataBinder;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace GameApp.LogicGraph
{
    public class PropertyBoardProvider
    {
        public Blackboard PropBoard { get; }

        public LogicGraphEditorView GraphEditor { get; }
        public LogicGraphObject GraphObject
        {
            get { return GraphEditor.GraphObject; }
        }

        private readonly BlackboardSection _section;

        private GenericMenu _gm;

        private LogicGraphSettings.LuaTypes LuaTypes => LogicGraphSettings.Instance.luaTypes;
        
        public PropertyBoardProvider(LogicGraphEditorView graphEditorView)
        {
            GraphEditor = graphEditorView;

            PropBoard = new Blackboard
            {
                scrollable = true,
                subTitle = "Property",
                editTextRequested = EditTextRequested,
                addItemRequested = AddItemRequested,
                moveItemRequested = MoveItemRequested,
                //layout = new Rect(0, 0, 160, 300),
            };

            _section = new BlackboardSection { headerVisible = false };
            PropBoard.Add(_section);
            InitMenu();
        }

        public void RefreshProps()
        {
            _section.Clear();
            foreach (var prop in GraphObject.properties)
            {
                var wrap = GraphObject.binds.GetWrap(prop.name);
                var field = new BlackboardField(null, prop.name, wrap.type.ToString()) { userData = prop };
                var row = new BlackboardRow(field, new PropertyBoardFieldView(this, prop)) {userData = prop};
            
                _section.Add(row);
            }
        }

        private void InitMenu()
        {
            _gm = new GenericMenu();
            _gm.AddItem(new GUIContent("Int"), false, () => AddProperty(ValueType.Int, "number"));
            _gm.AddItem(new GUIContent("Float"), false, () => AddProperty(ValueType.Float, "number"));
            _gm.AddItem(new GUIContent("String"), false, () => AddProperty(ValueType.String, "string"));
            _gm.AddItem(new GUIContent("Bool"), false, () => AddProperty(ValueType.Bool, "boolean"));
            _gm.AddItem(new GUIContent("Vector2"), false, () => AddProperty(ValueType.Vector2, "Vector2"));
            _gm.AddItem(new GUIContent("Vector3"), false, () => AddProperty(ValueType.Vector3, "Vector3"));
            _gm.AddItem(new GUIContent("Vector4"), false, () => AddProperty(ValueType.Vector4, "Vector4"));
            _gm.AddItem(new GUIContent("Rect"), false, () => AddProperty(ValueType.Rect, "Rect"));
            _gm.AddItem(new GUIContent("Bounds"), false, () => AddProperty(ValueType.Bounds, "Bounds"));
            _gm.AddItem(new GUIContent("Curve"), false, () => AddProperty(ValueType.Curve, "AnimationCurve"));
            _gm.AddItem(new GUIContent("Color"), false, () => AddProperty(ValueType.Color, "Color"));
            _gm.AddItem(new GUIContent("Object"), false, () => AddProperty(ValueType.Object, "Object"));
            _gm.AddItem(new GUIContent("Elua"), false, () => AddProperty(ValueType.String, "Elua"));
        }
        
        private void EditTextRequested(Blackboard blackboard, VisualElement visualElement, string newText)
        {
            var prop = (GraphProperty)visualElement.userData;
            foreach (var wrap in GraphObject.binds.wraps)
            {
                if (wrap.name == prop.name)
                {
                    wrap.name = newText;
                    break;
                }
            }
            
            prop.name = newText;

            var field = (BlackboardField)visualElement;
            field.text = newText;
        }
        
        private void AddItemRequested(Blackboard blackboard)
        {
            _gm.ShowAsContext();
        }

        private void AddProperty(ValueType type, string luaType)
        {
            var props = GraphObject.properties;
            var binds = GraphObject.binds;
            var name = $"property_{props.Count}";
            
            foreach (var w in binds.wraps)
            {
                if (w.name == name)
                {
                    Debug.LogWarning($"exist same property name: {name}");
                    return;
                }
            }

            var prop = new GraphProperty {name = name, type = luaType, scope = GraphProperty.Scope.Graph};
            props.Add(prop);
            
            var wrap = new ValueWrap {type = type, name = name, value = BindableValues.GetDefault(type)};
            if (luaType == LuaTypes.typeElua) wrap.value = "nil";
            
            GraphObject.binds.wraps.Add(wrap);
            
            binds.Regenerate();
            
            var field = new BlackboardField(null, name, type.ToString()) { userData = prop };
            var row = new BlackboardRow(field, new PropertyBoardFieldView(this, prop)) {userData = prop};
            
            _section.Add(row);
        }

        private void MoveItemRequested(Blackboard blackboard, int newIndex, VisualElement visualElement)
        {
            
        }
        
        
    }
}