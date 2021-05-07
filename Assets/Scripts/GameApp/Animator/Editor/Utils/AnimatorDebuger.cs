using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    public class AnimatorDebuger
    {
        public AnimatorRunner runner;

        private List<AnimatorNode> _nodes = new List<AnimatorNode>();

        public void Reset(AnimatorRunner animatorRunner, AnimatorView view)
        {
            if (animatorRunner == null || view == null)
            {
                OnStateChange(null);
                OnTransfer(null);
                return;
            }
            
            if (runner != null)
            {
                runner.onPlayState -= OnStateChange;
                runner.onTransfer -= OnTransfer;
                runner.onStopState -= OnStateChange;
            }
            
            runner = animatorRunner;
            runner.onPlayState += OnStateChange;
            runner.onTransfer += OnTransfer;
            runner.onStopState += OnStateChange;

            _nodes = view.Graph.Query<AnimatorNode>().ToList();
            
            OnStateChange(runner.CurrentStateName);
        }

        public void OnUpdate()
        {
            if (!Application.isPlaying)
            {
                OnStateChange(null);
                OnTransfer(null);
            }
        }

        private void OnStateChange(string stateName)
        {
            _nodes.ForEach(n =>
            {
                n.isRunning = runner.CurrentStateName == n.State.StateName || runner.IsAdditive(n.State.StateName);
            });
        }

        private void OnTransfer(AnimatorTransfer transfer)
        {
            _nodes.ForEach(n => { n.isRunning = transfer && transfer.to == n.State; });
        }

    }
}