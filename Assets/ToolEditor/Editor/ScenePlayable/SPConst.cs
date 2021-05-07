#if UNITY_EDITOR
namespace Pangu.ScenePlayable
{
    public static class SpConst
    {
        public const string NOT_AVAILABLE = "Not Avalible In Editor Mode...";
        public const string EVENT_NAME = "Event Name";
        public const string TRIGGER_RADIUS = "Trigger Radius";
        public const string USE_FOLLOW = "Followed Target";
        public const string SHOW_SENDER = "Show Trigger Object";
        public const string TRIGGER_ALL = "Tigger All Target";

        public const string MAIN_PLAYER = "MainPlayer";

        public const string REFRESH = "Refresh";
        public const string TRIGGER_EVENT = "Trigger Event";
        public const string BROADCAST_EVENT = "Broadcast Event";

        public const string SENDER_NAME = "_Playable Sender";

        public const int BUTTON_WIDTH = 60;
        public const int BUTTON_HEIGHT = 30;

        public const string TRIGGER_NAME = "Playable Trigger";
    }
}
#endif