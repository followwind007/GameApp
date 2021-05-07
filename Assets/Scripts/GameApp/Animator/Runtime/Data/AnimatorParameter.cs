using System;
using GameApp.Serialize;

namespace GameApp.AnimatorBehaviour
{
    [Serializable]
    public struct AnimatorParameter<T> where T : IComparable
    {
        public readonly string name;
        public T value;
        public bool isTrigger;

        public AnimatorParameter(string parameterName, T parameterValue, bool trigger)
        {
            name = parameterName;
            value = parameterValue;
            isTrigger = trigger;
        }
    }

    
    [Serializable]
    public struct AnimatorParameterHost
    {
        public string name;
        public SerializedJsonObject obj;
        public bool isTrigger;

        public object GetValue()
        {
            return obj.GetData();
        }

        public AnimatorParameter<T> GetParameter<T>() where T : IComparable
        {
            var param = new AnimatorParameter<T>(name, (T)GetValue(), isTrigger);
            return param;
        }

        public override string ToString()
        {
            return $"{name}:{obj.data} \t[{obj.T.Name}]";
        }
    }

}