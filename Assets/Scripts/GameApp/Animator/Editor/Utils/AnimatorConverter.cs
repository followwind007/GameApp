using System;
using System.Linq;
using GameApp.Serialize;
using Tools;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    public static class AnimatorConverter
    {
        [MenuItem("Assets/Create/Animator/Convert to animator data", true)]
        public static bool ConvertFromControllerChecker()
        {
            var controller = Selection.activeObject as AnimatorController;
            return controller != null;
        }
        
        [MenuItem("Assets/Create/Animator/Convert to animator data")]
        public static void ConvertFromController()
        {
            var controller = Selection.activeObject as AnimatorController;
            if (controller == null) return;
            
            var data = AssetUtil.CreateAsset<AnimatorData>(controller.name);

            //state
            foreach (var l in controller.layers)
            {
                foreach (var s in l.stateMachine.states)
                {
                    var c = s.state.motion as AnimationClip;
                    
                    var cs = data.InternalCreateState<ClipState>();
                    cs.stateName = s.state.name;
                    cs.clip = c;
                    cs.position = s.position;
                }
            }

            //parameter
            foreach (var p in controller.parameters)
            {
                var host = new AnimatorParameterHost();
                switch (p.type)
                {
                    case AnimatorControllerParameterType.Float:
                        host = CreateHost(typeof(float), p.defaultFloat);
                        break;
                    case AnimatorControllerParameterType.Int:
                        host = CreateHost(typeof(int), p.defaultInt);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        host = CreateHost(typeof(bool), p.defaultBool);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        host = CreateHost(typeof(bool), false);
                        host.isTrigger = true;
                        break;
                }

                host.name = p.name;
                data.parameters.Add(host);
            }

            //transfer
            foreach (var l in controller.layers)
            {
                foreach (var s in l.stateMachine.states)
                {
                    foreach (var t in s.state.transitions)
                    {
                        var scs = data.InternalGetState<ClipState>(s.state.name);
                        var tcs = data.InternalGetState<ClipState>(t.destinationState.name);
                        if (!scs || !tcs) continue;

                        foreach (var ct in t.conditions)
                        {
                            var tr = scs.InternalAddTransfer<ClipTransfer>(tcs);
                            var p = data.parameters.FirstOrDefault(host => host.name == ct.parameter);
                            tr.conditions.Add(CreateCondition(ct, p));
                        }
                    }
                }
            }

            //default state
            var dcs = data.InternalGetState<ClipState>(controller.layers[0].stateMachine.defaultState.name);
            if (dcs) data.enterState = dcs;
            
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }


        private static AnimatorParameterHost CreateHost(Type type, object value)
        {
            var host = new AnimatorParameterHost
            {
                name = "",
                obj = new SerializedJsonObject(type, value)
            };
            return host;
        }

        private static AnimatorConditionHost CreateCondition(AnimatorCondition ct, AnimatorParameterHost host)
        {
            var c = new AnimatorConditionHost();
            switch (ct.mode)
            {
                case AnimatorConditionMode.If:
                    break;
                case AnimatorConditionMode.IfNot:
                    break;
                case AnimatorConditionMode.Greater:
                    c.compare = AnimatorCompare.Greater;
                    break;
                case AnimatorConditionMode.Less:
                    c.compare = AnimatorCompare.Less;
                    break;
                case AnimatorConditionMode.Equals:
                    c.compare = AnimatorCompare.Equal;
                    break;
                case AnimatorConditionMode.NotEqual:
                    c.compare = AnimatorCompare.NotEqual;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var t = host.obj.T;
            if (t == typeof(int))
            {
                c.target = new SerializedJsonObject(typeof(int), (int)ct.threshold);
            }
            else if (t == typeof(float))
            {
                c.target = new SerializedJsonObject(typeof(float), ct.threshold);
            }
            else if (t == typeof(bool))
            {
                c.target = new SerializedJsonObject(typeof(bool), ct.threshold > 0);
            }

            c.linkType = AnimatorConditionLink.And;
            
            return c;
        }
        
    }
}