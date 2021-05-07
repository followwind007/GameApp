using System;
using System.Text.RegularExpressions;
using GameApp.AssetProcessor;
using GameApp.Timeline;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using ControlPlayableAsset = UnityEngine.Timeline.ControlPlayableAsset;
using Object = UnityEngine.Object;

namespace Tools.PrefabsTool.Processor
{
    public static class LodModelProcessor
    {
        public class RebindContext
        {
            public GameObject source;
            public GameObject target;
            public PlayableDirector director;

            private PlayableDirector _sourceDirector;
            public PlayableDirector SourceDirector
            {
                get
                {
                    if (_sourceDirector == null)
                    {
                        var path = director.transform.GetPath(target.transform);
                        _sourceDirector = source.GetComponent(path, typeof(PlayableDirector)) as PlayableDirector;
                    }

                    return _sourceDirector;
                }
            }

            public TrackAsset track;
            
            public TrackAsset SourceTrack
            {
                get
                {
                    if (SourceDirector != null)
                    {
                        foreach (var output in SourceDirector.playableAsset.outputs)
                        {
                            if (output.sourceObject != null && output.sourceObject.name == track.name)
                            {
                                return output.sourceObject as TrackAsset;
                            }
                        }
                    }

                    return null;
                }
            } 
        }
        
        [GameObjectChecker("同步Lod1 Prefab")]
        public static void SyncLodModel(GameObjectCheckerContext ctx)
        {
            var source = (GameObject)ctx.sourceObject;
            var path = AssetDatabase.GetAssetPath(source);
            if (!Regex.IsMatch(path, @"_login\.prefab")) return;
            
            var targetPath = path.Replace(source.name, $"{source.name}_lod1");
            if (ctx.doFix) AssetDatabase.DeleteAsset(targetPath);
            
            PrefabUtil.ClonePrefab(path, targetPath);
            var target = PrefabUtility.LoadPrefabContents(targetPath);

            var sContainer = source.transform.Find("Charactor");
            var sct = sContainer.GetChild(0);

            var tContainer = target.transform.Find("Charactor");
            var tct = tContainer.GetChild(0);

            if (tct == null || sct == null) return;

            //sync character at Character/
            if (sct.gameObject.name == tct.gameObject.name)
            {
                if (ctx.doFix)
                {
                    //source prefab
                    var sp = PrefabUtility.GetCorrespondingObjectFromSource(sct.gameObject);
                    var sPath = AssetDatabase.GetAssetPath(sp);
                    var lodPath = sPath.Replace(sp.name, $"{sp.name}_lod1");
                    Debug.LogWarning($"In {targetPath}, Replace {targetPath} With: {lodPath}");
                    
                    var lodGo = AssetDatabase.LoadAssetAtPath<GameObject>(lodPath);
                    var inst = (GameObject)PrefabUtility.InstantiatePrefab(lodGo, tContainer);
                    ComponentUtility.CopyComponent(tct);
                    ComponentUtility.PasteComponentValues(inst.transform);

                    Object.DestroyImmediate(tct.gameObject);
                }
            }

            if (ctx.doFix)
            {
                RebindDirector(source, target);
                PrefabUtility.SaveAsPrefabAsset(target, targetPath);
            }
            
            PrefabUtility.UnloadPrefabContents(target);
        }

        public static void RebindDirector(GameObject source, GameObject target)
        {
            var dirs = target.GetComponentsInChildren<PlayableDirector>();
            foreach (var dir in dirs)
            {
                if (dir.playableAsset == null) continue;
                foreach (var track in dir.playableAsset.outputs)
                {
                    var ctx = new RebindContext {source = source, target = target, director = dir, track = track.sourceObject as TrackAsset};
                    var sourceObj = track.sourceObject;
                    if (sourceObj is AnimationTrack)
                    {
                        RebindAnimationTrack(ctx);
                    }
                    else if (sourceObj is ControlTrack)
                    {
                        RebindControlTrack(ctx);
                    }
                    else if (sourceObj is ActivationTrack)
                    {
                        RebindActivationTrack(ctx);
                    }
                    else if (sourceObj is AnimatorPlayableTrack)
                    {
                        RebindAnimatorTrack(ctx);
                    }
                }
            }
        }

        public static void RebindAnimationTrack(RebindContext ctx)
        {
            RebindTargetComponent(ctx, typeof(Animator));
        }

        public static void RebindControlTrack(RebindContext ctx)
        {
            var track = (ControlTrack)ctx.track;

            var sourceTrack = (ControlTrack)ctx.SourceTrack;
            var sourceDirector = ctx.SourceDirector;
            
            foreach (var tc in track.GetClips())
            {
                var tca = tc.asset as ControlPlayableAsset;
                if (tca == null) continue;
                
                var targetSg = tca.sourceGameObject;
                foreach (var sc in sourceTrack.GetClips())
                {
                    var sca = sc.asset as ControlPlayableAsset;
                    //same source and target ControlPlayableAsset
                    if (sca == null || sca.name != tca.name) continue;
                    
                    var sourceSg = sca.sourceGameObject;
                    var sourceObj = sourceDirector.GetReferenceValue(sourceSg.exposedName, out var isSourceValid);
                    var targetObj = ctx.director.GetReferenceValue(targetSg.exposedName, out var isTargetValid);
                    
                    //id not valid
                    if (!isSourceValid || !isTargetValid) break;

                    //both null
                    if (sourceObj == null && targetObj == null) break;

                    //sync target
                    if (sourceObj == null)
                    {
                        //sync null
                        ctx.director.SetReferenceValue(targetSg.exposedName, null);
                    }
                    else
                    {
                        //sync source game object
                        var sourceGo = (GameObject) sourceObj;
                        var path = sourceGo.transform.GetPath(ctx.source.transform);

                        var targetTrans = ctx.target.transform.Find(path);
                        if (targetTrans != null)
                        {
                            ctx.director.SetReferenceValue(targetSg.exposedName, targetTrans.gameObject);
                        }
                    }
                }
            } 
        }

        public static void RebindActivationTrack(RebindContext ctx)
        {
            RebindTargetGameObject(ctx);
        }

        public static void RebindAnimatorTrack(RebindContext ctx)
        {
            RebindTargetGameObject(ctx);
        }

        public static void RebindTargetComponent(RebindContext ctx, Type type)
        {
            var sBind = ctx.SourceDirector.GetGenericBinding(ctx.SourceTrack) as Component;
            if (sBind != null)
            {
                var path = sBind.transform.GetPath(ctx.source.transform);
                var tBind = ctx.target.GetComponent(path, type);
                if (tBind != null)
                {
                    ctx.director.SetGenericBinding(ctx.track, tBind);
                }
            }
        }
        
        public static void RebindTargetGameObject(RebindContext ctx)
        {
            var sBind = ctx.SourceDirector.GetGenericBinding(ctx.SourceTrack) as GameObject;
            if (sBind != null)
            {
                var path = sBind.transform.GetPath(ctx.source.transform);
                var tTrans = ctx.target.transform.Find(path);
                if (tTrans != null)
                {
                    ctx.director.SetGenericBinding(ctx.track, tTrans.gameObject);
                }
            }
        }

    }
}