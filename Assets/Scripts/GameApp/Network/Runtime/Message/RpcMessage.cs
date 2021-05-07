namespace GameApp.Network
{
    public struct RpcMessage
    {
        public uint receiver;
        public string method;
        public string values;
    }
}