using UnityEngine;
using UnityEditor;
using ToolEditor.EvePro.Editor.Excel;

public class TestEditor : ScriptableObject
{
    //[MenuItem("Test/TestExcel")]
    static void TestExcel()
    {
        ExcelHelper.UpdateTables();
        Debug.Log(ExcelHelper.SpawnPoint);
        Debug.Log(ExcelHelper.SpaceItem);
        Debug.Log(ExcelHelper.Monster);
    }
}