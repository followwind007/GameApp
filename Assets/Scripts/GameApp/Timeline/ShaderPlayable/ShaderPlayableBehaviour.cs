using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using GameApp.ScenePlayable;

namespace GameApp.Timeline
{
    [System.Serializable]
    public class ShaderPlayableBehavior : PlayableBehaviour, IPlayableTarget
    {
        [Header("设置材质")]
        public List<MaterialElement> MaterialToSet;
        [Header("普通 Command")]
        public List<ShaderPlayableCommand> commands = null;
        [Header("渐变 Command")]
        public List<ShaderPlayableTweenCommand> tweenCommands = null;
        
        public GameObject Target { get; set; }

        //runtime property should not be serialized
        private List<Material> TargetMaterial { get; set; }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (Target == null) return;
            
            SetMaterial();
            GetMaterial();
            if (tweenCommands != null)
            {
                foreach (var tc in tweenCommands)
                    tc.Init(TargetMaterial);
            }
            DoCommands();
        }
        
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (Target == null) return;
            if (tweenCommands == null || tweenCommands.Count == 0) return;
            float proc = (float)(playable.GetTime() / playable.GetDuration());

            if (Mathf.Abs(proc - 1f) > 0.01f)
            {
                DoTweenCommand(proc);
            }
        }

        private void SetMaterial()
        {
            if (Target == null) return;
            Renderer rd = Target.GetComponent<Renderer>();
            if (rd == null) return;

            List<Material> matList = new List<Material>();
            //for preview in editor mode
            if (MaterialToSet == null || MaterialToSet.Count == 0)
            {
                if (!Application.isPlaying)
                {
                    foreach (var oldMat in rd.sharedMaterials)
                        if (oldMat) matList.Add(new Material(oldMat));
                }
                else
                {
                    foreach (var oldMat in rd.materials)
                        matList.Add(oldMat);
                }
            }
            else if(MaterialToSet != null && MaterialToSet.Count > 0)
            {
                for (int i = 0; i < MaterialToSet.Count; i++)
                {
                    var matEle = MaterialToSet[i];
                    if (matEle.selected && matEle.resetMaterial)
                    {
                        Material mat = PlayableLoader.LoadAssetAtPath<Material>(matEle.materialPath);
                        if (!Application.isPlaying && mat)
                            mat = new Material(mat);
                        if (mat != null)
                            matList.Add(mat);
                    }
                    else
                    {
                        if (Application.isPlaying)
                            matList.Add(rd.materials[i]);
                        else
                            matList.Add(new Material(rd.sharedMaterials[i]));
                    }
                }
            }
            if (Application.isPlaying)
                rd.sharedMaterials = matList.ToArray();
            else
                rd.materials = matList.ToArray();
        }

        private void GetMaterial()
        {
            Renderer rd = Target.GetComponent<Renderer>();
            TargetMaterial = new List<Material>();
            
            Material[] mats;
            if (Application.isPlaying)
                mats = rd.materials;
            else
                mats = rd.sharedMaterials;

            if (MaterialToSet != null && MaterialToSet.Count > 0)
            {
                for (int i = 0; i < MaterialToSet.Count; i++)
                    if (MaterialToSet[i].selected && i < mats.Length)
                        TargetMaterial.Add(mats[i]);
            }
            else
            {
                foreach (Material mat in mats)
                    TargetMaterial.Add(mat);
            }
        }

        private void DoCommands()
        {
            if (commands == null) return;

            foreach (var mat in TargetMaterial)
                foreach (ShaderPlayableCommand command in commands)
                    command.DoCommand(mat);
        }

        private void DoTweenCommand(float proc)
        {
            if (tweenCommands == null) return;
            
            foreach (var mat in TargetMaterial)
            {
                foreach (var tweenCommand in tweenCommands)
                {
                    tweenCommand.DoCommand(mat, proc);
                }
            }    
        }

    }
}
