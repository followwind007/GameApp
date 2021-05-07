namespace Pangu.Const
{
    public class MessageName
    {
        public const string START_UP = "START_UP";
        public const string ENTER_SCENE = "ENTER_SCENE";
        public const string ENTER_DUNGEON_SCENE = "ENTER_DUNGEON_SCENE";
        public const string CHANGE_SCENE = "CHANGE_SCENE";



        public const string NET_CONNECTED = "NET_CONNECTED";
        public const string NET_DISCONNECTED = "NET_DISCONNECTED";
        public const string SOCKET_DATA = "SOCKET_DATA";
        public const string SOCKET_CONNECT = "SOCKET_CONNECT";
        public const string MSG_SYNC_TIME = "onGetServerTime";
        public const string TOGGLE_BACKGROUND = "TOGGLE_BACKBROUND";
        public const string APPLICATION_QUIT = "APPLICATION_QUIT";
        public const string CHANGE_SETTING_SHOW_COLLIDER = "CHANGE_SETTING_SHOW_COLLIDER";

        public const string DRAG_END = "DRAG_END";
        public const string DRAG_START = "DRAG_START";

        // Combat
        public const string PLAYER_START_SPELL = "PLAYER_START_SPELL";
        public const string MAIN_PLAYER_DEAD = "MAIN_PLAYER_DEAD";
        public const string MAIN_PLAYER_BORN = "MAIN_PLAYER_BORN";

        // Skill
        public const string PLAYER_UPDATE_SKILL_INFO = "PLAYER_UPDATE_SKILL_INFO";
        public const string PLAYER_UPDATE_SKILL_GCD = "PLAYER_UPDATE_SKILL_GCD";
        public const string PLAYER_UPDATE_SKILL_QTE = "PLAYER_UPDATE_SKILL_QTE";

        public const string UI_REFRESH_UI = "UI_REFRESH_UI";
        public const string IN_LOADING_PANEL = "IN_LOADING_PANEL";
        public const string OUT_LOADING_PANEL = "OUT_LOADING_PANEL";
        public const string HIDE_LOADING_PANEL = "HIDE_LOADING_PANEL";

        public const string CHANGE_VIEW_MODE = "CHANGE_VIEW_MODE";
        public const string FALLBACK_VIEW_MODE = "FALLBACK_VIEW_MODE";
        public const string CHANGE_ORIENTATION_TO_AUTOROTATION = "CHANGE_ORIENTATION_TO_AUTOROTATION";
        public const string CHANGE_ORIENTATION_TO_PORTRAIT = "CHANGE_ORIENTATION_TO_PORTRAIT";

        // system_setting_panel
        public const string CHANGE_SETTING_SHOW_TOPLOGO_SETTING = "CHANGE_SETTING_SHOW_TOPLOGO_SETTING";

        public const string TEMP_SHOW_TOPLOGO = "TEMP_SHOW_TOPLOGO";
        public const string TEMP_HIDE_TOPLOGO = "TEMP_HIDE_TOPLOGO";
        public const string MODEL_CHANGED = "MODEL_CHANGED";
        public const string CHANGE_EQUIPS_MODEL = "CHANGE_EQUIPS_MODEL";
        public const string CHANGE_SINGLE_EQUIPS_MODEL = "CHANGE_SINGLE_EQUIPS_MODEL";
        public const string CHANGE_FACIAL_INFO = "CHANGE_FACIAL_INFO";
        public const string REPLACE_SINGLE_ACTION = "REPLACE_SINGLE_ACTION";
        public const string CAMERA_SHAKE = "CAMERA_SHAKE";
        public const string CAMERA_SHAKE_STOP = "CAMERA_SHAKE_STOP";
        public const string ENABLE_CINEMACHINE = "ENABLE_CINEMACHINE";
        public const string DISABLE_CINEMACHINE = "DISABLE_CINEMACHINE";
        public const string SHOW_SKILL_AREA = "SHOW_SKILL_AREA";
        public const string HIGHLIGHT_MODEL = "HIGHLIGHT_MODEL";
        public const string PLAY_GLOW_EFFECT = "PLAY_GLOW_EFFECT";
        public const string STOP_GLOW_EFFECT = "STOP_GLOW_EFFECT";
        public const string TIME_SCALE_MODE = "TIME_SCALE_MODE";
        public const string MSG_SETTING_CHANGED = "MSG_SETTING_CHANGED";

        public const string MSG_SETTING_INITIALIZED = "MSG_SETTING_INITIALIZED";



        //camera
        public const string CAMERA_AUTO_ROTATE = "CAMERA_AUTO_ROTATE";
        public const string CAMERA_AUTO_ROTATE_STOP = "CAMERA_AUTO_ROTATE_STOP";
        public const string CAMERA_NAVI_ROTATE = "CAMERA_NAVI_ROTATE";
        public const string CAMERA_NAVI_ROTATE_STOP = "CAMERA_NAVI_ROTATE_STOP";
        public const string CAMERA_BITE_START = "CAMERA_BITE_START";
        public const string CAMERA_BITE_END = "CAMERA_BITE_END";
        public const string SHOW_SCREEN_EFF_WAVE = "SHOW_SCREEN_EFF_WAVE";

        public const string CAMERA_RADIAL_BLUR = "CAMERA_RADIAL_BLUR";
        public const string CAMERA_RADIAL_BLUR_LOOP = "CAMERA_RADIAL_BLUR_LOOP";
        public const string CAMERA_RADIAL_BLUR_STOP = "CAMERA_RADIAL_BLUR_STOP";
        public const string RESET_CAMERA = "RESET_CAMERA";

        public const string CAMERA_FREEZE = "CAMERA_FREEZE";
        public const string CAMERA_UNFREEZE = "CAMERA_UNFREEZE";
        public const string CAMERA_SET_FREEZE = "CAMERA_SET_FREEZE";
        public const string CAMERA_DELAY = "CAMERA_DELAY";
        public const string CAMERA_DELAY_RESET = "CAMERA_DELAY_RESET";
        public const string SET_DIALOG_DOF = "SET_DIALOG_DOF";
        public const string SET_CLOSE_UP_DIALOG_CAMERA = "SET_CLOSE_UP_DIALOG_CAMERA";
        public const string RESUME_CLOSE_UP_DIALOG_CAMERA = "RESUME_CLOSE_UP_DIALOG_CAMERA";

        public const string SET_RECALL_MODE = "SET_RECALL_MODE";
        public const string ADJUST_CAMERA_FOV = "ADJUST_CAMERA_FOV";
        public const string STOP_ADJUST_CAMERA_FOV = "STOP_ADJUST_CAMERA_FOV";
        public const string ADJUST_CAMERA_ROT = "ADJUST_CAMERA_ROT";
        public const string STOP_ADJUST_CAMERA_ROT = "STOP_ADJUST_CAMERA_ROT";
        public const string CHANGE_QUEST_CAMERA = "CHANGE_QUEST_CAMERA";
        public const string EXIT_QUEST_CAMERA = "EXIT_QUEST_CAMERA";
        public const string ENABLE_DISSOLVE_DETECT = "ENABLE_DISSOLVE_DETECT";

        //public static string ENTER_OBSERVE_MODE = "ENTER_OBSERVE_MODE";
        public static string EXIT_OBSERVE_MODE = "EXIT_OBSERVE_MODE";
        public static string INTERACT_DURING_OBSERVING = "INTERACT_DURING_OBSERVING";
        public static string MOVE_TO_BACKSTAGE = "MOVE_TO_BACKSTAGE";

        //memory profile scene

        //itemCD
        public const string ITEM_CD_UPDATED = "ITEM_CD_UPDATED";

        //Trigger
        public const string TRIGGER_DISABLED = "TRIGGER_DISABLED";

        public const string ON_RESET_SERVER_TIME = "resetServerTime";

        public const string DISABLE_MAIN_CAMERA = "DISABLE_MAIN_CAMERA";

        //智能语音

        //display

        //CutScene
        public const string CUTSCENE_SHOWCAPTURE = "CUTSCENE_SHOWCAPTURE";

        //nearModel
        public const string ENTER_NEAR_MODEL = "ENTER_NEAR_MODEL";
        public const string OUT_NEAR_MODEL = "OUT_NEAR_MODEL";
        public const string ENTER_APPEARANCE_NEAR_MODEL = "ENTER_APPEARANCE_NEAR_MODEL";

        //Capture Screen
        public const string CAPTURE_SCREEN_CALLBACK = "CAPTURE_SCREEN_CALLBACK";
        public const string CAPTURE_SCREEN_SAVE_ALBUM = "CAPTURE_SCREEN_SAVE_ALBUM";

        public const string DISABLE_BLOOM = "DISABLE_BLOOM";

        public const string RESTORE_WEAPON_POSITION = "RESTORE_WEAPON_POSITION";

        public const string PLAYER_CHANGE_RIDE_STATE = "PLAYER_CHANGE_RIDE_STATE";
        public const string PLAYER_CHANGE_RIDE_MODEL = "PLAYER_CHANGE_RIDE_MODEL";

        public const string NAVI_END = "NAVI_END";
        public const string CHANGE_SWIM_STATE = "CHANGE_SWIM_STATE";
        public const string SET_PLAYER_MOVE_BY_KEYBOARD = "SET_PLAYER_MOVE_BY_KEYBOARD";

        // uniSdk
        public const string SDK_INIT_FINISH = "SDK_INIT_FINISH";
        public const string SDK_LOGIN_DONE = "SDK_LOGIN_DONE";

        public const string KEYBOARD_INPUT_SHIFT = "KEYBOARD_INPUT_SHIFT";
        public const string KEYBOARD_INPUT_SPACE = "KEYBOARD_INPUT_SPACE";
        public const string KEYBOARD_INPUT_ENTER = "KEYBOARD_INPUT_ENTER";
        public const string MODEL_LOADED = "MODEL_LOADED";

        // timeline
        public const string PLAY_TIMELINE = "PLAY_TIMELINE";
        public const string STOP_TIMELINE = "STOP_TIMELINE";
    }
}
