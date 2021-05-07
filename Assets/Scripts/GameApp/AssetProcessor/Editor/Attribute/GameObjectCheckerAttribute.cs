using System;

namespace GameApp.AssetProcessor
{
    public class GameObjectCheckerAttribute : Attribute
    {
        public readonly string id;

        public GameObjectCheckerAttribute(string id)
        {
            this.id = id;
        }
    }
}