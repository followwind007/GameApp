using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GameApp.AnimationRigging
{
    public class RigDynamicBoneConstraint : RigConstraintBehaviour
    {
        public struct Particle
        {
            public TransformStreamHandle joint;
            public Vector3 prevPosition;
            public AffineTransform drivenTx;
            public AffineTransform bindTx;
            public float damping;
            public float elasticity;
            public float stiffness;
            public float inert;
        }
        
        public struct RigDynamicBoneJob : IAnimationJob
        {
            public FloatProperty weight;
            
            public float frameRate;
            public float radius;

            public Vector3 gravity;
            public Vector3 force;

            public TransformStreamHandle root;

            public NativeArray<Particle> particles;

            public float time;

            public Vector3 prevPosition;
            public Vector3 move;

            public void ProcessAnimation(AnimationStream stream)
            {
                if (particles.Length == 0) return;

                UpdateJoints(stream);
            }

            public void ProcessRootMotion(AnimationStream stream)
            {
            }
            

            private void UpdateJoints(AnimationStream stream)
            {
                var loop = 0;
                var t = stream.deltaTime;
                if (frameRate > 0)
                {
                    var dt = 1f / frameRate;
                    time += t;
                    while (time >= dt)
                    {
                        time -= dt;
                        if (++loop >= 3)
                        {
                            time = 0;
                            break;
                        }
                        
                        var curPos = root.GetPosition(stream);
                        move = curPos - prevPosition;
                        prevPosition = curPos;
                        
                        UpdateParticles(stream);
                    }
                }
                
                ApplyTransform(stream);
            }

            private void UpdateParticles(AnimationStream stream)
            {
                var p0 = particles[0];
                p0.prevPosition = p0.drivenTx.translation;
                p0.drivenTx.Set(root.GetPosition(stream), root.GetRotation(stream));
                particles[0] = p0;
                
                var weightVal = weight.Get(stream);
                
                for (var cur = 1; cur < particles.Length; cur++)
                {
                    var prev = cur - 1;
                    var pp = particles[prev];
                    var pc = particles[cur];
                    
                    //force and move
                    var v = pc.drivenTx.translation - pc.prevPosition;
                    var rmove = move * pc.inert;
                    pc.prevPosition = pc.drivenTx.translation + rmove;
                    pc.drivenTx.translation += v * (1 - pc.damping) + force + rmove;

                    //modify
                    var ppPos = pp.joint.GetPosition(stream);
                    var pcPos = pc.joint.GetPosition(stream);
                    
                    var len = (pcPos - ppPos).magnitude;
                    var stiffness = Mathf.Lerp(1f, particles[cur].stiffness, weightVal);
                    if (stiffness > 0 || pc.elasticity > 0)
                    {
                        var sourceTx = new AffineTransform(ppPos, pp.joint.GetRotation(stream));
                        var targetTx = sourceTx * pc.bindTx;

                        var deltaPos = targetTx.translation - pc.drivenTx.translation;
                        pc.drivenTx.translation += deltaPos * pc.elasticity;

                        if (stiffness > 0)
                        {
                            var sDeltaPos = targetTx.translation - pc.drivenTx.translation;
                            var sLen = sDeltaPos.magnitude;
                            var maxlen = len * (1 - stiffness) * 2;
                            if (sLen > maxlen)
                                pc.drivenTx.translation += sDeltaPos * ((sLen - maxlen) / sLen);
                        }
                    }
                    
                    //keep length
                    var dir = pp.drivenTx.translation - pc.drivenTx.translation;
                    var dist = dir.magnitude;
                    if (dist > 0) pc.drivenTx.translation += dir * ((dist - len) / dist);

                    particles[cur] = pc;
                }
            }

            private void ApplyTransform(AnimationStream stream)
            {
                for (var i = 1; i < particles.Length; i++)
                {
                    var p = particles[i];
                    p.joint.SetPosition(stream, p.drivenTx.translation);
                }
            }
            
        }

        [SyncToStream][Range(0f, 1f)]
        public float weight;

        public Transform joint;

        [Range(0.01f, 60f)]
        public float frameRate;
        
        [Range(0f, 1f)]
        public float damping = 0.1f;
        
        public AnimationCurve dampingCurve;
        
        [Range(0f, 1f)]
        public float elasticity = 0.1f;

        public AnimationCurve elasticityCurve;
        
        [Range(0f, 1f)]
        public float stiffness = 0.1f;

        public AnimationCurve stiffnessCurve;
        
        [Range(0f, 1f)]
        public float inert = 0.1f;

        public AnimationCurve inertCurve;
        
        public float radius;

        public Vector3 gravity;
        public Vector3 force;

        private List<Transform> _childs;
        private List<AffineTransform> _initTxs;

        public override IAnimationJob CreateJob(Animator animator)
        {
            var job = new RigDynamicBoneJob()
            {
                weight = FloatProperty.Bind(animator, this, PropertyUtil.PropName(nameof(weight))),
                frameRate = frameRate,
                radius = radius,
                gravity = gravity,
                force = force,
                time = 0f,
            };
            
            _childs = new List<Transform>();
            _initTxs = new List<AffineTransform>();
            
            var cur = joint;
            while (cur != null)
            {
                _childs.Add(cur);
                _initTxs.Add(new AffineTransform(cur.position, cur.rotation));
                if (cur.childCount == 0) break;
                cur = cur.GetChild(0);
            }


            var pCount = _childs.Count;

            if (pCount > 0)
            {
                var root = _childs[0];
                job.root = animator.BindStreamTransform(root);
                job.prevPosition = root.position;
            }
            
            job.particles = new NativeArray<Particle>(pCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            
            for (var i = 0; i < _childs.Count; i++)
            {
                var t = i / (pCount - 1f);
                var c = _childs[i];
                var p = new Particle
                {
                    joint = animator.BindStreamTransform(c),
                    prevPosition = c.position,
                    drivenTx = new AffineTransform(c.position, c.rotation),
                    damping = dampingCurve.length > 0 ? dampingCurve.Evaluate(t) * damping : damping,
                    elasticity = elasticityCurve.length > 0 ? elasticityCurve.Evaluate(t) * elasticity : elasticity,
                    stiffness = stiffnessCurve.length > 0 ? stiffnessCurve.Evaluate(t) * stiffness : stiffness,
                    inert = inertCurve.length > 0 ? inertCurve.Evaluate(t) * inert : inert
                };

                if (i > 0)
                {
                    var prev = job.particles[i - 1];
                    p.bindTx = prev.bindTx.InverseMul(p.drivenTx);
                }
                
                job.particles[i] = p;
            }

            return job;
        }
        
        public override AnimationScriptPlayable CreatePlayable(PlayableGraph graph, IAnimationJob job)
        {
            return AnimationScriptPlayable.Create(graph, (RigDynamicBoneJob)job);
        }

        public override void DestroyJob(IAnimationJob job)
        {
            var dbJob = (RigDynamicBoneJob)job;

            if (_childs != null && _initTxs != null && _childs.Count == _initTxs.Count)
            {
                for (var i = 1; i < dbJob.particles.Length; i++)
                {
                    _childs[i].position = _initTxs[i].translation;
                }
            }

            dbJob.particles.Dispose();
        }
    }
}