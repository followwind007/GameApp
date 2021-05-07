using System.Text;
using GameApp.Pesistence;
using UnityEngine;

namespace ScriptsMine.Test
{
    public class TestPersist : MonoBehaviour
    {
        public string writeText = "local a = 5\nprint(a)";

        private void OnGUI()
        {
            writeText = GUILayout.TextArea(writeText, GUILayout.Width(500), GUILayout.Height(300));
            if (GUILayout.Button("Write To File"))
            {
                var _ = PersistFile.AsyncWriteToFile(Encoding.UTF8.GetBytes(writeText), "Lua/test.lua", PersistPipeline.RawDataPipeline);
            }
        }
    }
}