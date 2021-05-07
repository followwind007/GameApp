using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.AssetProcessor
{
    public static class ComponentCheckerSample
    {
        [ComponentChecker(typeof(PlayableDirector), "Sample1")]
        public static void Check1(ComponentCheckerContext ctx)
        {
            var director = (PlayableDirector)ctx.component;
            Debug.Log($"check1 director:{director.gameObject.name}, do fix:{ctx.doFix}, source:{ctx.sourceObject.name}");
        }
    }
}