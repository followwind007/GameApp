namespace Pangu.Const
{
    public static class LayerDefine
    {
        public const int LAYER_DEFAULT = 0;
        public const int LAYER_TRANSPARENT_FX = 1;
        public const int LAYER_IGNORE_RAYCAST = 2;

        public const int LAYER_WATER = 4;
        public const int LAYER_UI = 5;

        public const int LAYER_UI_TOPLOGO = 8;
        public const int LAYER_UI_3D_MODEL = 9;
        public const int LAYER_NO_LIGHT = 10;

        public const int LAYER_GROUND = 11; //地表, 石头(用作地表), 路面, 桥
        public const int LAYER_GROUND0 = 12; //地标、标志性建筑（最远消失距离）
        public const int LAYER_GROUND1 = 13; //树、建筑、碎石头等（远消失距离）
        public const int LAYER_GROUND2 = 14; //三级物件、花花草草等（中消失距离）
        public const int LAYER_GROUND3 = 15; //小物件（近消失距离）
        public const int LAYER_GROUND_DYNAMIC = 16; //动态物理
        public const int LAYER_SKYBOX = 17;
        public const int LAYER_WATER_REFL = 18;
        public const int LAYER_AIR_WALL = 19; //空气墙

        public const int LAYER_MAIN_PLAYER = 21; //主角
        public const int LAYER_MAIN_PLAYER_TRIGGER = 22;
        public const int LAYER_PLAYER = 23; //玩家
        public const int LAYER_NPC = 24;
        public const int LAYER_MONSTER = 25;
        public const int LAYER_BOSS = 26; //带碰撞的怪物
        public const int LAYER_TRIGGER = 27;
        public const int LAYER_BUILDING = 28;
    }

}