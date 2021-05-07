
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.Serialize
{
    [CustomPropertyDrawer(typeof(SerializedJsonObject))]
    public class SerializedJsonObjectDrawer : PropertyDrawer
    {
        public class JsonObjectView : VisualElement
        {
            public readonly SerializedProperty property;
            public readonly SerializedProperty propType;
            public readonly SerializedProperty propName;

            public JsonObjectView(SerializedProperty property)
            {
                AddToClassList("SerializedJsonObjectDrawer");
                this.AddStyleSheetPath("Styles/SerializedJsonDrawer");
                
                this.property = property;
                propType = property.FindPropertyRelative(SerializedJsonObject.IdType);
                propName = property.FindPropertyRelative(SerializedJsonObject.IdName);
                
                var nameField = new TextField { name = "name" };
                nameField.BindProperty(propName);
                Add(nameField);

                var dataDrawer = GetDrawer();
                if (dataDrawer != null)
                {
                    Add(dataDrawer);
                }

                var typeField = new Label(GetShortType(propType.stringValue))
                {
                    name = "type", 
                    tooltip = propType.stringValue
                };
                Add(typeField);
            }

            public SerializedJsonDrawer GetDrawer()
            {
                var t = propType.stringValue;
                if (string.IsNullOrEmpty(t)) return null;

                if (DrawerTypesUtil.TypeDrawers.TryGetValue(t, out var drawerType))
                {
                    return System.Activator.CreateInstance(drawerType, property) as SerializedJsonDrawer;
                }

                return null;
            }

            private string GetShortType(string t)
            {
                var parts = t.Split('.');
                return parts[parts.Length - 1];
            }
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new JsonObjectView(property);
        }
    }


    public abstract class SerializedJsonDrawer : VisualElement
    {
        public readonly SerializedProperty property;
        public readonly SerializedProperty propType;
        public readonly SerializedProperty propData;
        public readonly SerializedProperty propObj;
        
        protected SerializedJsonDrawer(SerializedProperty property)
        {
            AddToClassList("SerializedJsonDrawer");
            this.AddStyleSheetPath("Styles/SerializedJsonDrawer");
            
            this.property = property;
            propType = property.FindPropertyRelative(SerializedJsonObject.IdType);
            propData = property.FindPropertyRelative(SerializedJsonObject.IdData);
            propObj = property.FindPropertyRelative(SerializedJsonObject.IdObj);
        }

        protected void Save(object obj)
        {
            if (obj is Object o) propObj.objectReferenceValue = o;
            else propData.stringValue = JsonUtilityWrap.ToJson(obj);
            
            property.serializedObject.ApplyModifiedProperties();
        }

        protected T GetValue<T>()
        {
            return JsonUtilityWrap.FromJson<T>(propData.stringValue);
        }

        protected Object GetObject()
        {
            return propObj.objectReferenceValue;
        }
    }
    
    [CustomSerizlizedJsonDrawer(typeof(int))]
    public class SerializedIntDrawer : SerializedJsonDrawer
    {
        public SerializedIntDrawer(SerializedProperty property) : base(property)
        {
            var field = new IntegerField { value = GetValue<int>() };
            field.RegisterValueChangedCallback(e =>
            {
                Save(e.newValue);
            });
            Add(field);
        }
    }
    
    [CustomSerizlizedJsonDrawer(typeof(float))]
    public class SerializeFloatDrawer : SerializedJsonDrawer
    {
        public SerializeFloatDrawer(SerializedProperty property) : base(property)
        {
            var field = new FloatField { value = GetValue<float>() };
            field.RegisterValueChangedCallback(e =>
            {
                Save(e.newValue);
            });
            Add(field);
        }
    }
    
    [CustomSerizlizedJsonDrawer(typeof(string))]
    public class SerializeStringDrawer : SerializedJsonDrawer
    {
        public SerializeStringDrawer(SerializedProperty property) : base(property)
        {
            var field = new TextField { value = GetValue<string>() };
            field.RegisterValueChangedCallback(e =>
            {
                Save(e.newValue);
            });
            Add(field);
        }
    }
    
    [CustomSerizlizedJsonDrawer(typeof(Color))]
    public class SerializeColorDrawer : SerializedJsonDrawer
    {
        public SerializeColorDrawer(SerializedProperty property) : base(property)
        {
            var field = new ColorField { value = GetValue<Color>() };
            field.RegisterValueChangedCallback(e =>
            {
                Save(e.newValue);
            });
            Add(field);
        }
    }
    
    //todo: JsonUtility do not support, need to customize it
    /*[CustomSerizlizedJsonDrawer(typeof(AnimationCurve))]
    public class SerializeCurveDrawer : SerializedJsonDrawer
    {
        public SerializeCurveDrawer(SerializedProperty property) : base(property)
        {
            var field = new CurveField { value = GetValue<AnimationCurve>() };
            field.RegisterValueChangedCallback(e =>
            {
                Save(e.newValue);
            });
            Add(field);
        }
    }*/
    
    [CustomSerizlizedJsonDrawer(typeof(Vector2))]
    public class SerializeVector2Drawer : SerializedJsonDrawer
    {
        public SerializeVector2Drawer(SerializedProperty property) : base(property)
        {
            var field = new Vector2Field { value = GetValue<Vector2>() };
            field.RegisterValueChangedCallback(e =>
            {
                Save(e.newValue);
            });
            Add(field);
        }
    }
    
    [CustomSerizlizedJsonDrawer(typeof(Vector3))]
    public class SerializeVector3Drawer : SerializedJsonDrawer
    {
        public SerializeVector3Drawer(SerializedProperty property) : base(property)
        {
            var field = new Vector3Field { value = GetValue<Vector3>() };
            field.RegisterValueChangedCallback(e =>
            {
                Save(e.newValue);
            });
            Add(field);
        }
    }
    
    [CustomSerizlizedJsonDrawer(typeof(Vector4))]
    public class SerializeVector4Drawer : SerializedJsonDrawer
    {
        public SerializeVector4Drawer(SerializedProperty property) : base(property)
        {
            var field = new Vector4Field { value = GetValue<Vector4>() };
            field.RegisterValueChangedCallback(e =>
            {
                Save(e.newValue);
            });
            Add(field);
        }
    }
    
    [CustomSerizlizedJsonDrawer(typeof(Rect))]
    public class SerializeRectDrawer : SerializedJsonDrawer
    {
        public SerializeRectDrawer(SerializedProperty property) : base(property)
        {
            var field = new RectField { value = GetValue<Rect>() };
            field.RegisterValueChangedCallback(e =>
            {
                Save(e.newValue);
            });
            Add(field);
        }
    }
    
    [CustomSerizlizedJsonDrawer(typeof(Bounds))]
    public class SerializeBoundsDrawer : SerializedJsonDrawer
    {
        public SerializeBoundsDrawer(SerializedProperty property) : base(property)
        {
            var field = new BoundsField { value = GetValue<Bounds>() };
            field.RegisterValueChangedCallback(e =>
            {
                Save(e.newValue);
            });
            Add(field);
        }
    }
    
    [CustomSerizlizedJsonDrawer(typeof(Object))]
    public class SerializeObjectDrawer : SerializedJsonDrawer
    {
        public SerializeObjectDrawer(SerializedProperty property) : base(property)
        {
            var field = new ObjectField
            {
                objectType = typeof(Object), 
                value = GetObject()
            };
            field.RegisterValueChangedCallback(e =>
            {
                Save(e.newValue);
            });
            Add(field);
        }
    }
    
}