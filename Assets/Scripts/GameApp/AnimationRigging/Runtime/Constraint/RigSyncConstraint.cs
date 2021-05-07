using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Animations;
#else
using UnityEngine.Experimental.Animations;
#endif
using UnityEngine.Playables;

namespace GameApp.AnimationRigging
{
    public class RigSyncConstraint : IRigConstraint
    {
        public struct RigSyncJob : IAnimationJob
        {
            public struct TransformSyncer : System.IDisposable
            {
                public NativeArray<TransformSceneHandle> sceneHandles;
                public NativeArray<TransformStreamHandle> streamHandles;

                public static TransformSyncer Create(int size)
                {
                    return new TransformSyncer() {
                        sceneHandles = new NativeArray<TransformSceneHandle>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
                        streamHandles = new NativeArray<TransformStreamHandle>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory)
                    };
                }

                public void Dispose()
                {
                    if (sceneHandles.IsCreated)
                        sceneHandles.Dispose();

                    if (streamHandles.IsCreated)
                        streamHandles.Dispose();
                }

                public void BindAt(int index, Animator animator, Transform transform)
                {
                    sceneHandles[index] = animator.BindSceneTransform(transform);
                    streamHandles[index] = animator.BindStreamTransform(transform);
                }

                public void Sync(ref AnimationStream stream)
                {
                    for (int i = 0, count = sceneHandles.Length; i < count; ++i)
                    {
                        var sceneHandle = sceneHandles[i];
                        if (!sceneHandle.IsValid(stream))
                            continue;

                        var streamHandle = streamHandles[i];
                        #if UNITY_2019_3_OR_NEWER
                        sceneHandle.GetLocalTRS(stream, out var scenePos, out var sceneRot, out var sceneScale);
                        streamHandle.SetLocalTRS(stream, scenePos, sceneRot, sceneScale, true);
                        #else
                        streamHandle.SetPosition(stream, sceneHandle.GetPosition(stream));
                        streamHandle.SetRotation(stream, sceneHandle.GetRotation(stream));
                        streamHandle.SetLocalScale(stream, sceneHandle.GetLocalScale(stream));
                        #endif
                        

                        streamHandles[i] = streamHandle;
                        sceneHandles[i] = sceneHandle;
                    }
                }
            }
            public struct PropertySyncer : System.IDisposable
            {
                public NativeArray<PropertySceneHandle> sceneHandles;
                public NativeArray<PropertyStreamHandle> streamHandles;
                public NativeArray<float> buffer;

                public static PropertySyncer Create(int size)
                {
                    return new PropertySyncer() {
                        sceneHandles = new NativeArray<PropertySceneHandle>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
                        streamHandles = new NativeArray<PropertyStreamHandle>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory),
                        buffer = new NativeArray<float>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory)
                    };
                }

                public void Dispose()
                {
                    if (sceneHandles.IsCreated)
                        sceneHandles.Dispose();

                    if (streamHandles.IsCreated)
                        streamHandles.Dispose();

                    if (buffer.IsCreated)
                        buffer.Dispose();
                }

                public void BindAt(int index, Animator animator, Component component, string property)
                {
                    sceneHandles[index] = animator.BindSceneProperty(component.transform, component.GetType(), property);
                    streamHandles[index] = animator.BindStreamProperty(component.transform, component.GetType(), property);
                }

                public void Sync(ref AnimationStream stream)
                {
                    #if UNITY_2019_3_OR_NEWER 
                    AnimationSceneHandleUtility.ReadFloats(stream, sceneHandles, buffer);
                    AnimationStreamHandleUtility.WriteFloats(stream, streamHandles, buffer, true);
                    #else
                    for (var i = 0; i < sceneHandles.Length; i++)
                    {
                        buffer[i] = sceneHandles[i].GetFloat(stream);
                        streamHandles[i].SetFloat(stream, buffer[i]);
                    }
                    #endif
                }

                public NativeArray<float> StreamValues(ref AnimationStream stream)
                {
                    #if UNITY_2019_3_OR_NEWER 
                    AnimationStreamHandleUtility.ReadFloats(stream, streamHandles, buffer);
                    #else
                    for (var i = 0; i < streamHandles.Length; i++) buffer[i] = streamHandles[i].GetFloat(stream);
                    #endif
                    
                    return buffer;
                }
            }

            public TransformSyncer transformSyncer;
            public PropertySyncer propertySyncer;
            
            public void ProcessAnimation(AnimationStream stream)
            {
                transformSyncer.Sync(ref stream);
                propertySyncer.Sync(ref stream);
            }

            public void ProcessRootMotion(AnimationStream stream)
            {
                
            }
        }

        public IAnimationJob CreateJob(Animator animator)
        {
            return new RigSyncJob();
        }

        public IAnimationJob CreateJob(Animator animator, List<RigLayer> layers)
        {
            PropertyUtil.ExtractAllSyncableData(animator, layers, out var transforms, out var properties);
            
            var job = new RigSyncJob();
            
            var tSyncer = RigSyncJob.TransformSyncer.Create(transforms.Count);
            job.transformSyncer = tSyncer;
            
            for (var i = 0; i < transforms.Count; i++)
            {
                tSyncer.BindAt(i, animator, transforms[i]);
            }

            var propCount = 0;
            foreach (var sp in properties)
            {
                foreach (var c in sp.constraints)
                foreach (var p in c.properties) 
                    propCount += p.descriptor.size;
            }
            
            var pSyncer = RigSyncJob.PropertySyncer.Create(propCount);
            job.propertySyncer = pSyncer;

            var propIdx = 0;
            foreach (var sp in properties)
            {
                foreach (var c in sp.constraints)
                {
                    foreach (var p in c.properties)
                    {
                        if (p.descriptor.size == 1)
                            pSyncer.BindAt(propIdx++, animator, c.component, p.name);
                        else
                        {
                            Debug.Assert(p.descriptor.size <= PropertyUtil.PropIdxNames.Length);
                            for (var i = 0; i < p.descriptor.size; i++)
                                pSyncer.BindAt(propIdx++, animator, c.component, PropertyUtil.PropIdxName(p.name, i));
                        }
                    }
                }
            }

            return job;
        }

        public void DestroyJob(IAnimationJob job)
        {
            var syncJob = (RigSyncJob) job;
            syncJob.transformSyncer.Dispose();
            syncJob.propertySyncer.Dispose();
        }

        public AnimationScriptPlayable CreatePlayable(PlayableGraph graph, IAnimationJob job)
        {
            return AnimationScriptPlayable.Create(graph, (RigSyncJob)job);
        }

        public void UpdateJob(Animator animator, IAnimationJob job)
        {
            
        }
    }
}