using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Animations;
#else
using UnityEngine.Experimental.Animations;
#endif

namespace GameApp.AnimationRigging
{
    public static class PropertyUtil
    {
        public static readonly string[] PropIdxNames = new string[] {".x", ".y", ".z", ".w"};

        internal static readonly Dictionary<Type, PropertyDescriptor> SupportedPropertyTypeToDescriptor = new Dictionary<Type, PropertyDescriptor>
        {
            { typeof(float)      , new PropertyDescriptor{ size = 1, type = PropertyType.Float } },
            { typeof(int)        , new PropertyDescriptor{ size = 1, type = PropertyType.Int   } },
            { typeof(bool)       , new PropertyDescriptor{ size = 1, type = PropertyType.Bool  } },
            { typeof(Vector2)    , new PropertyDescriptor{ size = 2, type = PropertyType.Float } },
            { typeof(Vector3)    , new PropertyDescriptor{ size = 3, type = PropertyType.Float } },
            { typeof(Vector4)    , new PropertyDescriptor{ size = 4, type = PropertyType.Float } },
            { typeof(Quaternion) , new PropertyDescriptor{ size = 4, type = PropertyType.Float } },
            { typeof(Vector3Int) , new PropertyDescriptor{ size = 3, type = PropertyType.Int   } },
            { typeof(Vector3Bool), new PropertyDescriptor{ size = 3, type = PropertyType.Bool  } }
        };
        
        public static string PropName(string property)
        {
            return property;
        }

        public static string PropIdxName(string property, int idx)
        {
            return property + PropIdxNames[idx];
        }

        public static string CustomPropName(Component component, string property)
        {
            return component.transform.GetInstanceID() + "/" + component.GetType() + "/" + property;
        }
        
        public static void ExtractAllSyncableData(Animator animator, IList<RigLayer> layers, out List<Transform> syncableTransforms, out List<SyncableProperties> syncableProperties)
        {
            syncableTransforms = new List<Transform>();
            syncableProperties = new List<SyncableProperties>(layers.Count);

            var typeToSyncableFields = new Dictionary<Type, FieldInfo[]>();
            foreach (var layer in layers)
            {
                var constraints = layer.Constraints;

                var allConstraintProperties = new List<ConstraintProperties>(constraints.Length);

                foreach (var constraint in constraints)
                {
                    var data = constraint;
                    var dataType = constraint.GetType();
                    
                    if (!typeToSyncableFields.TryGetValue(dataType, out var syncableFields))
                    {
                        var allFields = dataType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        var filteredFields = new List<FieldInfo>(allFields.Length);
                        foreach (var field in allFields)
                            if (field.GetCustomAttribute<SyncToStreamAttribute>() != null)
                                filteredFields.Add(field);

                        syncableFields = filteredFields.ToArray();
                        typeToSyncableFields[dataType] = syncableFields;
                    }

                    var properties = new List<Property>(syncableFields.Length);
                    foreach (var field in syncableFields)
                    {
                        if (ExtractTransformType(animator, field, ref data, syncableTransforms)) continue;
                        if (ExtractPropertyType(field, properties)) continue;

                        throw new NotSupportedException($"Field type {field.FieldType} is not a supported syncable property type.");
                    }

                    allConstraintProperties.Add(
                        new ConstraintProperties {
                            component = constraint as Component,
                            properties = properties.ToArray()
                        }
                    );
                }

                syncableProperties.Add(
                    new SyncableProperties {
                        rig = new RigProperties { component = layer },
                        constraints = allConstraintProperties.ToArray()
                    }
                );
            }

            var extraTransforms = GetSyncableRigTransforms(animator);
            if (extraTransforms != null) syncableTransforms.AddRange(extraTransforms);
        }
        
        private static bool ExtractTransformType(Animator animator, FieldInfo field, ref IRigConstraint data, List<Transform> syncableTransforms)
        {
            var handled = true;

            var fieldType = field.FieldType;
            if (fieldType == typeof(Transform))
            {
                var value = (Transform)field.GetValue(data);
                if (value != null && value.IsChildOf(animator.transform))
                    syncableTransforms.Add(value);
            }
            else if (fieldType == typeof(Transform[]) || fieldType == typeof(List<Transform>))
            {
                var list = (IEnumerable<Transform>)field.GetValue(data);
                foreach (var element in list)
                    if (element != null && element.IsChildOf(animator.transform))
                        syncableTransforms.Add(element);
            }
            else handled = false;

            return handled;
        }

        private static bool ExtractPropertyType(FieldInfo field, List<Property> syncableProperties)
        {
            if (!SupportedPropertyTypeToDescriptor.TryGetValue(field.FieldType, out var descriptor)) return false;

            syncableProperties.Add(new Property { name = PropName(field.Name), descriptor = descriptor });

            return true;
        }
        
        private static Transform[] GetSyncableRigTransforms(Animator animator)
        {
            var rigTransforms = animator.GetComponentsInChildren<RigSyncTransform>();
            if (rigTransforms.Length == 0)
                return null;

            var transforms = new Transform[rigTransforms.Length];
            for (var i = 0; i < transforms.Length; ++i)
                transforms[i] = rigTransforms[i].transform;

            return transforms;
        }
        
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SyncToStreamAttribute : Attribute { }

    public enum PropertyType : byte { Bool, Int, Float };

    public struct PropertyDescriptor
    {
        public int size;
        public PropertyType type;
    }

    public struct Property
    {
        public string name;
        public PropertyDescriptor descriptor;
    }
    
    public struct RigProperties
    {
        public Component component;
    }

    public struct ConstraintProperties
    {
        public Component component;
        public Property[] properties;
    }

    [Serializable]
    public struct Vector3Bool
    {
        public bool x, y, z;

        public Vector3Bool(bool val)
        {
            x = y = z = val;
        }

        public Vector3Bool(bool x, bool y, bool z)
        {
            this.x = x; this.y = y; this.z = z;
        }
    }
    
    public struct SyncableProperties
    {
        public RigProperties rig;
        public ConstraintProperties[] constraints;
    }
    
    public interface IAnimatableProperty<T>
    {
        T Get(AnimationStream stream);
        void Set(AnimationStream stream, T value);
    }

    public struct BoolProperty : IAnimatableProperty<bool>
    {
        public PropertyStreamHandle value;

        public static BoolProperty Bind(Animator animator, Component component, string name)
        {
            return new BoolProperty()
            {
                value = animator.BindStreamProperty(component.transform, component.GetType(), name)
            };
        }

        public static BoolProperty BindCustom(Animator animator, string property)
        {
            return new BoolProperty
            {
                value = animator.BindCustomStreamProperty(property, CustomStreamPropertyType.Bool)
            };
        }
        
        public bool Get(AnimationStream stream) => value.GetBool(stream);
        public void Set(AnimationStream stream, bool v) => value.SetBool(stream, v);
    }

    public struct IntProperty : IAnimatableProperty<int>
    {
        public PropertyStreamHandle value;

        public static IntProperty Bind(Animator animator, Component component, string name)
        {
            return new IntProperty()
            {
                value = animator.BindStreamProperty(component.transform, component.GetType(), name)
            };
        }

        public static IntProperty BindCustom(Animator animator, string property)
        {
            return new IntProperty
            {
                value = animator.BindCustomStreamProperty(property, CustomStreamPropertyType.Int)
            };
        }

        public int Get(AnimationStream stream) => value.GetInt(stream);
        public void Set(AnimationStream stream, int v) => value.SetInt(stream, v);
    }

    public struct FloatProperty : IAnimatableProperty<float>
    {
        public PropertyStreamHandle value;

        public static FloatProperty Bind(Animator animator, Component component, string name)
        {
            return new FloatProperty()
            {
                value = animator.BindStreamProperty(component.transform, component.GetType(), name)
            };
        }

        public static FloatProperty BindCustom(Animator animator, string property)
        {
            return new FloatProperty
            {
                value = animator.BindCustomStreamProperty(property, CustomStreamPropertyType.Float)
            };
        }

        public float Get(AnimationStream stream) => value.GetFloat(stream);
        public void Set(AnimationStream stream, float v) => value.SetFloat(stream, v);
    }

    public struct Vector2Property : IAnimatableProperty<Vector2>
    {
        public PropertyStreamHandle x;
        public PropertyStreamHandle y;

        public static Vector2Property Bind(Animator animator, Component component, string name)
        {
            var type = component.GetType();
            return new Vector2Property
            {
                x = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 0)),
                y = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 1))
            };
        }

        public static Vector2Property BindCustom(Animator animator, string name)
        {
            return new Vector2Property
            {
                x = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 0), CustomStreamPropertyType.Float),
                y = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 1), CustomStreamPropertyType.Float)
            };
        }

        public Vector2 Get(AnimationStream stream) =>
            new Vector2(x.GetFloat(stream), y.GetFloat(stream));

        public void Set(AnimationStream stream, Vector2 value)
        {
            x.SetFloat(stream, value.x);
            y.SetFloat(stream, value.y);
        }
    }

    public struct Vector3Property : IAnimatableProperty<Vector3>
    {
        public PropertyStreamHandle x;
        public PropertyStreamHandle y;
        public PropertyStreamHandle z;

        public static Vector3Property Bind(Animator animator, Component component, string name)
        {
            var type = component.GetType();
            return new Vector3Property
            {
                x = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 0)),
                y = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 1)),
                z = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 2))
            };
        }

        public static Vector3Property BindCustom(Animator animator, string name)
        {
            return new Vector3Property
            {
                x = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 0), CustomStreamPropertyType.Float),
                y = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 1), CustomStreamPropertyType.Float),
                z = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 2), CustomStreamPropertyType.Float)
            };
        }

        public Vector3 Get(AnimationStream stream) =>
            new Vector3(x.GetFloat(stream), y.GetFloat(stream), z.GetFloat(stream));

        public void Set(AnimationStream stream, Vector3 value)
        {
            x.SetFloat(stream, value.x);
            y.SetFloat(stream, value.y);
            z.SetFloat(stream, value.z);
        }
    }

    public struct Vector3IntProperty : IAnimatableProperty<Vector3Int>
    {
        public PropertyStreamHandle x;
        public PropertyStreamHandle y;
        public PropertyStreamHandle z;

        public static Vector3IntProperty Bind(Animator animator, Component component, string name)
        {
            var type = component.GetType();
            return new Vector3IntProperty
            {
                x = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 0)),
                y = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 1)),
                z = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 2))
            };
        }

        public static Vector3IntProperty BindCustom(Animator animator, string name)
        {
            return new Vector3IntProperty
            {
                x = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 0), CustomStreamPropertyType.Int),
                y = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 1), CustomStreamPropertyType.Int),
                z = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 2), CustomStreamPropertyType.Int)
            };
        }

        public Vector3Int Get(AnimationStream stream) =>
            new Vector3Int(x.GetInt(stream), y.GetInt(stream), z.GetInt(stream));

        public void Set(AnimationStream stream, Vector3Int value)
        {
            x.SetInt(stream, value.x);
            y.SetInt(stream, value.y);
            z.SetInt(stream, value.z);
        }
    }

    public struct Vector3BoolProperty : IAnimatableProperty<Vector3Bool>
    {
        public PropertyStreamHandle x;
        public PropertyStreamHandle y;
        public PropertyStreamHandle z;

        public static Vector3BoolProperty Bind(Animator animator, Component component, string name)
        {
            var type = component.GetType();
            return new Vector3BoolProperty
            {
                x = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 0)),
                y = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 1)),
                z = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 2))
            };
        }

        public static Vector3BoolProperty BindCustom(Animator animator, string name)
        {
            return new Vector3BoolProperty
            {
                x = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 0), CustomStreamPropertyType.Bool),
                y = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 1), CustomStreamPropertyType.Bool),
                z = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 2), CustomStreamPropertyType.Bool)
            };
        }

        public Vector3Bool Get(AnimationStream stream) =>
            new Vector3Bool(x.GetBool(stream), y.GetBool(stream), z.GetBool(stream));

        public void Set(AnimationStream stream, Vector3Bool value)
        {
            x.SetBool(stream, value.x);
            y.SetBool(stream, value.y);
            z.SetBool(stream, value.z);
        }
    }

    public struct Vector4Property : IAnimatableProperty<Vector4>
    {
        public PropertyStreamHandle x;
        public PropertyStreamHandle y;
        public PropertyStreamHandle z;
        public PropertyStreamHandle w;

        public static Vector4Property Bind(Animator animator, Component component, string name)
        {
            var type = component.GetType();
            return new Vector4Property
            {
                x = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 0)),
                y = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 1)),
                z = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 2)),
                w = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 3))
            };
        }

        public static Vector4Property BindCustom(Animator animator, string name)
        {
            return new Vector4Property
            {
                x = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 0), CustomStreamPropertyType.Float),
                y = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 1), CustomStreamPropertyType.Float),
                z = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 2), CustomStreamPropertyType.Float),
                w = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 3), CustomStreamPropertyType.Float)
            };
        }

        public Vector4 Get(AnimationStream stream) =>
            new Vector4(x.GetFloat(stream), y.GetFloat(stream), z.GetFloat(stream), w.GetFloat(stream));

        public void Set(AnimationStream stream, Vector4 value)
        {
            x.SetFloat(stream, value.x);
            y.SetFloat(stream, value.y);
            z.SetFloat(stream, value.z);
            w.SetFloat(stream, value.w);
        }
    }
    
    public struct QuaternionProperty : IAnimatableProperty<Quaternion>
    {
        public PropertyStreamHandle x;
        public PropertyStreamHandle y;
        public PropertyStreamHandle z;
        public PropertyStreamHandle w;
        
        public static QuaternionProperty Bind(Animator animator, Component component, string name)
        {
            var type = component.GetType();
            return new QuaternionProperty
            {
                x = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 0)),
                y = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 1)),
                z = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 2)),
                w = animator.BindStreamProperty(component.transform, type, PropertyUtil.PropIdxName(name, 3))
            };
        }

        public static QuaternionProperty BindCustom(Animator animator, string name)
        {
            return new QuaternionProperty
            {
                x = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 0), CustomStreamPropertyType.Float),
                y = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 1), CustomStreamPropertyType.Float),
                z = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 2), CustomStreamPropertyType.Float),
                w = animator.BindCustomStreamProperty(PropertyUtil.PropIdxName(name, 3), CustomStreamPropertyType.Float)
            };
        }

        public Quaternion Get(AnimationStream stream) =>
            new Quaternion(x.GetFloat(stream), y.GetFloat(stream), z.GetFloat(stream), w.GetFloat(stream));

        public void Set(AnimationStream stream, Quaternion value)
        {
            x.SetFloat(stream, value.x);
            y.SetFloat(stream, value.y);
            z.SetFloat(stream, value.z);
            w.SetFloat(stream, value.w);
        }
    }
}