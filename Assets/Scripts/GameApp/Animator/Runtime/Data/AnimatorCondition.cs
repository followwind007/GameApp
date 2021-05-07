using System;
using System.Collections.Generic;
using GameApp.Serialize;

namespace GameApp.AnimatorBehaviour
{
    public enum AnimatorCompare
    {
        Equal = 0,
        LessEqual = 1,
        Less = 2,
        GreaterEqual = 3,
        Greater = 4,
        NotEqual = 5,
    }

    public enum AnimatorConditionLink
    {
        And = 0,
        Or = 1
    }

    [Serializable]
    public struct AnimatorCondition<T> where T : IComparable
    {
        public string parameterName;
        public T target;

        public AnimatorCompare compare;
        public AnimatorConditionLink linkType;

        public AnimatorCondition(string name, T conditionTarget, AnimatorCompare conditionCompare, AnimatorConditionLink link)
        {
            parameterName = name;
            compare = conditionCompare;
            target = conditionTarget;
            linkType = link;
        }

        public bool CheckValid(T value)
        {
            bool valid;
            switch (compare)
            {
                case AnimatorCompare.Equal:
                    valid = target.Equals(value);
                    break;
                case AnimatorCompare.LessEqual:
                    valid = target.CompareTo(value) <= 0;
                    break;
                case AnimatorCompare.Less:
                    valid = target.CompareTo(value) < 0;
                    break;
                case AnimatorCompare.GreaterEqual:
                    valid = target.CompareTo(value) >= 0;
                    break;
                case AnimatorCompare.Greater:
                    valid = target.CompareTo(value) > 0;
                    break;
                case AnimatorCompare.NotEqual:
                    valid = !target.Equals(value);
                    break;
                default:
                    valid = false;
                    break;
            }
            return valid;
        }
    }
        
    [Serializable]
    public struct AnimatorConditionHost
    {
        public string parameterName;
        public SerializedJsonObject target;
        public AnimatorCompare compare;
        public AnimatorConditionLink linkType;

        public AnimatorCondition<T> GetCondition<T>() where T : IComparable
        {
            var condition = new AnimatorCondition<T>(parameterName, (T)target.GetData(), compare, linkType);
            return condition;
        }
    }
    
    public struct AnimatorConditionGroup
    {
        public readonly List<AnimatorCondition<float>> conditionFloats;
        public readonly List<AnimatorCondition<int>> conditionInts;
        public readonly List<AnimatorCondition<bool>> conditionBools;

        public AnimatorConditionGroup(IEnumerable<AnimatorConditionHost> conditions)
        {
            conditionFloats = new List<AnimatorCondition<float>>();
            conditionInts = new List<AnimatorCondition<int>>();
            conditionBools = new List<AnimatorCondition<bool>>();

            foreach (var c in conditions)
            {
                var t = c.target.T;
                if (t == typeof(float))
                {
                    conditionFloats.Add(c.GetCondition<float>());
                }
                else if (t == typeof(int))
                {
                    conditionInts.Add(c.GetCondition<int>());
                }
                else if (t == typeof(bool))
                {
                    conditionBools.Add(c.GetCondition<bool>());
                }
            }
        }
    }
    
}