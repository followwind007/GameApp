using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    public class ParameterBoardSlot : VisualElement
    {
        private readonly ParameterBoardProvider _board;
        private readonly List<AnimatorParameterHost> _hosts;
        private readonly int _index;
        
        public ParameterBoardSlot(ParameterBoardProvider board, List<AnimatorParameterHost> hosts, int index)
        {
            _board = board;
            _hosts = hosts;
            _index = index;
            
            var host = hosts[index];

            var obj = host.obj;
            
            var t = host.obj.T;
            
            if (t == typeof(int))
            {
                var field = new IntegerField { value = obj.GetData<int>() };
                field.RegisterValueChangedCallback(v => SaveChange(v.newValue));
                Add(field);
            }
            else if (t == typeof(float))
            {
                var field = new FloatField { value = obj.GetData<float>() };
                field.RegisterValueChangedCallback(v => SaveChange(v.newValue));
                Add(field);
            }
            else if (t == typeof(bool))
            {
                var hor = new VisualElement();
                hor.style.flexDirection = FlexDirection.Row;
                Add(hor);
                
                var field = new Toggle { value = obj.GetData<bool>() };
                field.RegisterValueChangedCallback(v =>
                {
                    if (host.isTrigger && v.newValue) field.value = false;
                    else SaveChange(v.newValue);
                });
                hor.Add(field);
                
                var triggerLabel = new Label("Trigger:");
                triggerLabel.style.fontSize = 12;
                triggerLabel.style.marginLeft = 20;
                hor.Add(triggerLabel);
                
                var triggerField = new Toggle { value = host.isTrigger };
                triggerField.RegisterValueChangedCallback(v =>
                {
                    if (v.newValue) field.value = false;
                    var h = _hosts[_index];
                    h.isTrigger = v.newValue;
                    _hosts[index] = h;
                });
                hor.Add(triggerField);
            }
            else
            {
                Debug.LogError($"error in mapping field type {t}");
            }
        }

        private void SaveChange(object value)
        {
            _board.view.RegisterUndo("change paramteter");
            var host = _hosts[_index];
            host.obj.SaveDataStr(value);
            _hosts[_index] = host;
        }
        
    }
}