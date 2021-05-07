using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime
{
    public static class BehaviourEx
    {
        public static void SendEventEx(this Behavior behavior, string name, object obj)
        {
            behavior.SendEvent(name, obj);
        }

        public static void SendEventEx(this Behavior behavior, string name, object obj1, object obj2)
        {
            behavior.SendEvent(name, obj1, obj2);
        }
        
        public static void SendEventEx(this Behavior behavior, string name, object obj1, object obj2, object obj3)
        {
            behavior.SendEvent(name, obj1, obj2, obj3);
        }

        public static ActionBinder GetActionBinder(this Behavior behavior, string name)
        {
            return behavior.FindTaskWithName(name) as ActionBinder;
        }
        
    }
}