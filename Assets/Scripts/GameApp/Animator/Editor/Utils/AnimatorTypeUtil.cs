using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameApp.Util;
using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GameApp.AnimatorBehaviour
{
    public static class AnimatorTypeUtil
    {
        private static IEnumerable<Type> _types;
        public static IEnumerable<Type> AllTypes => TypeUtil.AllTypes;

        private static Type[] _stateTypes;
        public static IEnumerable<Type> StateTypes
        {
            get
            {
                if (_stateTypes == null)
                {
                    _stateTypes = GetTypes(typeof(AnimatorState)).ToArray();
                }

                return _stateTypes;
            }
        }

        private static Type[] _stateOverrideTypes;

        public static IEnumerable<Type> StateOverrideTypes
        {
            get
            {
                if (_stateOverrideTypes == null)
                {
                    _stateOverrideTypes = GetTypes(typeof(AnimatorStateOverride)).ToArray();
                }

                return _stateOverrideTypes;
            }
        }

        private static Type[] _transferTypes;
        public static IEnumerable<Type> TransferTypes
        {
            get
            {
                if (_transferTypes == null)
                {
                    _transferTypes = GetTypes(typeof(AnimatorTransfer)).ToArray();
                }
                return _transferTypes;
            }
        }

        private static Type[] _stateOverrideViewTypes;

        public static IEnumerable<Type> StateOverrideViewTypes
        {
            get
            {
                if (_stateOverrideViewTypes == null)
                {
                    _stateOverrideViewTypes = GetTypes(typeof(AnimatorStateOverrideView)).ToArray();
                }

                return _stateOverrideViewTypes;
            }
        }

        private static List<Type> GetTypes(Type target)
        {
            var list = new List<Type>();
            foreach (var t in AllTypes)
            {
                if (!t.IsAbstract && !t.IsGenericType && target.IsAssignableFrom(t))
                {
                    list.Add(t);
                }
            }

            return list;
        }

        public static List<Type> GetTransfer(AnimatorState from, AnimatorState to)
        {
            var types = new List<Type>();
            var ft = from.GetType();
            var tt = to.GetType();
            foreach (var t in TransferTypes)
            {
                var attrs = t.GetCustomAttributes();
                var exist = attrs.Any(a => a is TransforCompatible tc && tc.from.IsAssignableFrom(ft) && tc.to.IsAssignableFrom(tt));
                if (exist)
                {
                    types.Add(t);
                }
            }
            return types;
        }

        public static StateColor GetStateColor(AnimatorState s)
        {
            var attrs = s.GetType().GetCustomAttributes(typeof(StateColor));
            return attrs.FirstOrDefault() as StateColor;
        }

        public static AnimatorStateOverrideView GetStateOverrideView(AnimatorStateOverride stateOverride)
        {
            foreach (var t in StateOverrideViewTypes)
            {
                var attr = t.GetCustomAttribute<CustomStateOverrideProvider>();
                if (attr != null && attr.target == stateOverride.GetType())
                {
                    return (AnimatorStateOverrideView) Activator.CreateInstance(t, new object[] {stateOverride});
                }
            }

            return null;
        }

        public static VisualElement GetCustomEditorView(Object obj)
        {
            foreach (var t in AllTypes)
            {
                var attr = t.GetCustomAttribute<CustomEditorProvider>();
                if (attr != null && attr.type == obj.GetType())
                {
                    return (VisualElement) Activator.CreateInstance(t, new SerializedObject(obj));
                }
            }
            return null;
        }

        public static Type GetStateOverrideType(AnimatorState state)
        {
            foreach (var t in StateOverrideTypes)
            {
                var attr = t.GetCustomAttribute<StateOverrideType>();
                if (attr != null && attr.type == state.GetType())
                {
                    return t;
                }
            }
            return null;
        }

    }
}