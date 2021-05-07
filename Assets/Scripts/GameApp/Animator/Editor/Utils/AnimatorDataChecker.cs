using System;
using System.Collections.Generic;
using UnityEditor;

namespace GameApp.AnimatorBehaviour
{
    public class AnimatorDataChecker
    {
        public struct CheckResult
        {
            public string desc;
            public Action action;
        }
        
        private AnimatorData Data => AnimatorWindow.Instance ? AnimatorWindow.Instance.Data : null;
        
        public List<CheckResult> GetCheckRes()
        {
            var list = new List<CheckResult>();
            if (!Data) return list;
            
            //check default state
            if (Data.enterState == null)
            {
                list.Add(new CheckResult
                {
                    desc = "No default state selected",
                    action = () => { EditorUtility.DisplayDialog("waring!", "right click the node and click 'Set as default'", "ok"); }
                });
            }
            
            return list;
        }

    }
}