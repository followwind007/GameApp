using UnityEditor;
using UnityEngine;

namespace GameApp.AssetProcessor
{
    public static class GameObjectCheckerSample
    {
        [GameObjectChecker("Sample1")]
        public static void Checker1(GameObjectCheckerContext ctx)
        {
            Debug.Log($"check1 game object:{AssetDatabase.GetAssetPath(ctx.sourceObject)}");
        }
    }
}