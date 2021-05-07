using UnityEditor;
using UnityEngine;

namespace ToolEditor.EvePro.Editor
{
    public static class EveConst
    {
        public const int DragAndDropId = 5678;
        public const string DragAndDropTempObjectName = "Drag And Drop Temp Object";
        public const int SelectionGridMinHeight = 114;
        public const int SelectionGridMaxHeight = 130;
        public const int SelectionGridWidth = 96;
        public const string EveGuiStyles = "Assets/ToolEditor/EvePro/Editor/Resources/GuiStyles/EveGuiStyles.asset";
        public const string LANGUAGE_OBJECT_PATH = "Assets/ToolEditor/EvePro/Editor/Asset/language.asset";

        public const string SvnPath = @"..\tables\bin\svn.exe";

        public const string RouteTable = "../tables/路径点配置表.xlsx";
        public const string ExcelConfigTable = "../tables/Editor/EVE标识.xlsx";
        public const string EveAuthorizationTable = "../tables/Editor/EVE权限表.xlsx";

        public const string USER_TABLE_PATH = "../tables/Editor/EVE用户表.xlsx";
        public const int USER_COL_NAME = 1;
        public const int USER_COL_STATUS = 2;
        public const int USER_COL_GROUP = 3;

        public const string SpawnPointTable = "../tables/产出点配置表.xlsx";
        public const string SpawnPointSheet = "产出点配置表";

        public const string SpaceItemTable = "../tables/场景物件配置表.xlsx";
        public const string SpaceItemSheet = "场景物件表";

        public const string MonsterTable = "../tables/怪物配置表.xlsx";
        public const string MonsterSheet = "怪物配置表";

        public const string SceneTable = "../tables/场景配置表.xlsx";
        public const string NpcTable = "../tables/NPC_配置表.xlsx";
        public const string OutputScene = "../code/server/scripts/data/spawn/";

        public const string ResFolder = "Assets/Res/";
        public const string ResTempFolder = "Assets/ResTemp/";
        public const string LoadModelId = "Character/{0}/prefabs/{0}.prefab";
        public const string LoadScene = "Level/{0}/{0}.unity";

        public const string EveRootName = "EveRoot";
        public const string SpawnPointRootName = "SpawnPoint";
        public const string SpaceItemRootName = "SpaceItem";
        public const string MonsterRootName = "Monster";
        public const string NpcRootName = "Npc";

        #region Login
        public const float LOGIN_EXPIRE = 3600 * 24 * 1; //登陆过期时间 秒
        public const string LAST_LOGIN_TIME = "EveLastLoginTime";
        public const string USER_NAME = "EveUserName";
        public const string PASSWORD = "EvePassword";
        public const string TIME_FORMAT = "yyyy/MM/dd HH:mm:ss";
        #endregion

        public const string WireframeMaterial = "Assets/ToolEditor/EvePro/Editor/Resources/Materials/Wireframe.mat";
        public const string MissingPrefab = "Assets/ToolEditor/EvePro/Editor/Resources/CustomGizmo/MissingPrefab.prefab";
        public const string IndicatorPrefab = "Assets/ToolEditor/EvePro/Editor/Resources/CustomGizmo/Indicator.prefab";
        public const string MultiplyPrefab = "Assets/ToolEditor/EvePro/Editor/Resources/CustomGizmo/x.prefab";
        public static readonly string[] NumbersPrefab = new string[10]{
            "Assets/ToolEditor/EvePro/Editor/Resources/CustomGizmo/0.prefab",
            "Assets/ToolEditor/EvePro/Editor/Resources/CustomGizmo/1.prefab",
            "Assets/ToolEditor/EvePro/Editor/Resources/CustomGizmo/2.prefab",
            "Assets/ToolEditor/EvePro/Editor/Resources/CustomGizmo/3.prefab",
            "Assets/ToolEditor/EvePro/Editor/Resources/CustomGizmo/4.prefab",
            "Assets/ToolEditor/EvePro/Editor/Resources/CustomGizmo/5.prefab",
            "Assets/ToolEditor/EvePro/Editor/Resources/CustomGizmo/6.prefab",
            "Assets/ToolEditor/EvePro/Editor/Resources/CustomGizmo/7.prefab",
            "Assets/ToolEditor/EvePro/Editor/Resources/CustomGizmo/8.prefab",
            "Assets/ToolEditor/EvePro/Editor/Resources/CustomGizmo/9.prefab",
        };

        public static Mesh GetCubeMesh()
        {
            Vector3[] vertices =
            {
                new Vector3(0, 0, 0),
                new Vector3(.1f, 0, 0),
                new Vector3(.1f, .1f, 0),
                new Vector3(0, .1f, 0),
                new Vector3(0, .1f, .1f),
                new Vector3(.1f, .1f, .1f),
                new Vector3(.1f, 0, .1f),
                new Vector3(0, 0, .1f)
            };
            int[] triangles =
            {
                0, 2, 1, //face front
                0, 3, 2,
                2, 3, 4, //face top
                2, 4, 5,
                1, 2, 5, //face right
                1, 5, 6,
                0, 7, 4, //face left
                0, 4, 3,
                5, 4, 7, //face back
                5, 7, 6,
                0, 6, 7, //face bottom
                0, 1, 6
            };
            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles
            };
            return mesh;
        }
        
    }
}