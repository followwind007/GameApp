using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    public class ParameterBoardProvider
    {
        public const string Title = "Parameter";
        
        public readonly AnimatorView view;

        public readonly Blackboard board;

        private readonly BlackboardSection _section;

        public ParameterBoardProvider(AnimatorView view)
        {
            this.view = view;

            var gm = new GenericMenu();
            gm.AddItem(new GUIContent("int"), false, () => AddParameter(typeof(int), 0) );
            gm.AddItem(new GUIContent("float"), false, () => AddParameter(typeof(float), 0) );
            gm.AddItem(new GUIContent("bool"), false, () => AddParameter(typeof(bool), false) );

            board = new Blackboard
            {
                scrollable = true,
                subTitle = Title,
                editTextRequested = EditTextRequested,
                addItemRequested = b => { gm.ShowAsContext(); },
            };

            board.SetPosition(AnimatorPrefs.Instance.paramPos);
            
            _section = new BlackboardSection { headerVisible = false };
            
            board.Add(_section);
        }

        public void Refresh()
        {
            _section.Clear();
            if (view.Data == null) return;
            for (var i = 0; i < view.Data.parameters.Count; i++)
            {
                AddParameter(i);
            }
        }

        private void AddParameter(Type type, object value)
        {
            view.Data.InternalCreateParameter(type, value);
            AddParameter(view.Data.parameters.Count - 1);
        }

        private void AddParameter(int index)
        {
            var parameters = view.Data.parameters;
            var p = parameters[index];
            var field = new BlackboardField(null, p.name, p.obj.T.Name) {userData = index};
            var parameterView = new ParameterBoardSlot(this, parameters, index);
            var row = new BlackboardRow(field, parameterView);
            _section.Add(row);
        }

        private void EditTextRequested(Blackboard blackboard, VisualElement visualElement, string newText)
        {
            var index = (int) visualElement.userData;
            var p = view.Data.parameters[index];
            p.name = newText;
            view.Data.parameters[index] = p;

            var field = (BlackboardField) visualElement;
            field.text = newText;
        }
        
        
    }
}