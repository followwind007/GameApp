using GameApp.AssetProcessor;
using GameApp.Timeline;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using AnimationPlayableAsset = UnityEngine.Timeline.AnimationPlayableAsset;
using ControlPlayableAsset = UnityEngine.Timeline.ControlPlayableAsset;

namespace Tools.PrefabsTool.Processor
{
    public static class PlayableDirectorProcessor
    {
        [ComponentChecker(typeof(PlayableDirector), "Animation轨道检查")]
        public static void ProcessAnimationTrack(ComponentCheckerContext ctx)
        {
            var director = (PlayableDirector)ctx.component;
            if (director.playableAsset == null) return;

            foreach (var output in director.playableAsset.outputs)
            {
                if (output.sourceObject is AnimationTrack track)
                {
                    var animator = director.GetGenericBinding(track) as Animator;
                    if (animator == null)
                    {
                        LogWarn(ctx, $"未绑定{track.name}");
                        continue;
                    }

                    var clips = track.GetClips();
                    foreach (var c in clips)
                    {
                        if (c.asset is AnimationPlayableAsset apa)
                        {
                            if (apa.clip != null && animator.runtimeAnimatorController != null && !track.muted)
                            {
                                LogWarn(ctx, $"{track.name} Animator物件上使用AnimationTrack");
                                if (ctx.doFix) track.muted = true;
                            }
                        }
                    }
                }
            }
        }

        [ComponentChecker(typeof(PlayableDirector), "Animator轨道检查")]
        public static void ProcessAnimatorTrack(ComponentCheckerContext ctx)
        {
            var director = (PlayableDirector)ctx.component;
            if (director.playableAsset == null) return;

            foreach (var output in director.playableAsset.outputs)
            {
                if (output.sourceObject is AnimatorPlayableTrack track)
                {
                    var go = director.GetGenericBinding(track) as GameObject;
                    if (go == null)
                    {
                        LogWarn(ctx, $"{track.name} 未绑定");
                    }
                }
            }
        }

        [ComponentChecker(typeof(PlayableDirector), "Control轨道检查")]
        public static void ProcessControlTrack(ComponentCheckerContext ctx)
        {
            var director = (PlayableDirector)ctx.component;
            if (director.playableAsset == null) return;
            foreach (var output in director.playableAsset.outputs)
            {
                if (output.sourceObject is ControlTrack track)
                {
                    foreach (var c in track.GetClips())
                    {
                        if (c.asset is ControlPlayableAsset ca)
                        {
                            if (ca.prefabGameObject != null)
                            {
                                var source = director.GetReferenceValue(ca.sourceGameObject.exposedName, out var isValid);
                                if (isValid && source == null)
                                {
                                    LogWarn(ctx, $"{track.name}-{c.displayName} parent未绑定");
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void LogWarn(ComponentCheckerContext ctx, string message)
        {
            var director = ctx.component;
            var path = $"{director.transform.GetPath(((GameObject)ctx.sourceObject).transform)}";
            Debug.LogWarning($"[Processor] {AssetDatabase.GetAssetPath(ctx.sourceObject)}/{path} [{message}]");
        }

    }
}