using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;

using UnityEngine;
using UnityEditor;
public class UIInfoProfiler
{
    public ProfilerProperty property = new ProfilerProperty();

    [@MenuItem("Tools/UIAssistant prop")]
    public static void GetProperty()
    {
        int firstFrameIndex = ProfilerDriver.firstFrameIndex;
        int lastFrameIndex = ProfilerDriver.lastFrameIndex;
        
        Debug.Log(firstFrameIndex + ":" + lastFrameIndex);
        for (int frameIndex = firstFrameIndex; frameIndex <= lastFrameIndex; frameIndex++)
        {
            ProfilerProperty property = new ProfilerProperty();
            
            property.onlyShowGPUSamples = false;
            bool enterChildren = true;
            while (property.Next(enterChildren))
            {
                
                
            }
            property.Cleanup();
        }
        ProfilerProperty prop = new ProfilerProperty();
        if (prop == null || !prop.frameDataReady)
        {
            Debug.Log("not ready to get property " + prop.frameFPS);
            return;
        }
        UISystemProfilerInfo[] uiInfos = prop.GetUISystemProfilerInfo();
        Debug.Log(uiInfos == null);
        foreach (UISystemProfilerInfo info in uiInfos)
        {
            Debug.Log(info);
        }
    }
}