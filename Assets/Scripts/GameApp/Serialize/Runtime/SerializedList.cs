using System.Collections.Generic;

namespace GameApp.Serialize
{
    [System.Serializable]
    public class SerlizedList
    {
        public List<SerializedJsonObject> list = new List<SerializedJsonObject>();
    }
}