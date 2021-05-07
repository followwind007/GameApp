using GameApp.Util;
using Unity.Collections;
using UnityEngine;

namespace GameApp.DynResolver.Test
{
    public class TestDyn : MonoBehaviour
    {
        private const string Example = @"
strict struct MessageA 0
{
    int field0 0;//int field
    int[] field1 1;
    float field2 2;
    string field3 3;
}

default struct MessageB 1
{
    int field0 0;
    MessageA field1 1;
}";
        
        private void Start()
        {
            var watch = "TestDyn";
            Utils.StartWatch(watch);
            DescriptorUtil.Parse(Example);
            Debug.Log($"decrypt: {Utils.ReadResetWatch(watch)}");
            TestEncode();
            Debug.Log($"encode: {Utils.ReadResetWatch(watch)}");
        }

        private float _times = 1f;
        private void OnGUI()
        {
            if (GUILayout.Button("Start Parse", GUILayout.Width(100), GUILayout.Height(30)))
            {
                DescriptorUtil.Parse(Example);
            }
            if (GUILayout.Button("Start Test", GUILayout.Width(100), GUILayout.Height(30)))
            {
                TestEncode();
            }

            GUILayout.Space(10);
            GUILayout.Label($"times: {_times}");
            _times = GUILayout.HorizontalSlider(_times, 0, 10000, GUILayout.Width(1000));
            
        }

        public struct A
        {
            public string str;
            
        }
        private void Update()
        {
            for (var i = 0; i < 1000; i++)
            {
                
            }
        }

        private void TestEncode()
        {
            for (var i = 0; i < _times; i++)
            {
                var obj1 = new DynObject("MessageB");
            
                var obj = obj1.GetField("field1").Object;
                var arr = obj.GetList("field1");
                arr.Add().Int = 2;
            
                var bytes = obj1.ToBytes();
                //Debug.Log(ByteUtil.TransToString(bytes));

                var dObj1 = new DynObject(bytes);
                var dObj = dObj1.GetField("field1").Object;
                var dArr = dObj.GetList("field1");
                var _ = dArr.list[0].Int;
            }
            
        }
    }
}