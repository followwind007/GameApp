using System;
using System.Collections.Generic;
using UnityEngine;
using GameApp.ScenePlayable;

namespace GameApp.Timeline
{
    [Serializable]
    public class ShaderPlayableCommand: ShaderCommand
    {
        public void DoCommand(Material material)
        {
            if (!CheckParam(material)) return;
            switch (type)
            {
                case CommandType.SetFloat:
                    material.SetFloat(paramName, valueFloat1);
                    break;
                case CommandType.SetInt:
                    material.SetInt(paramName, valueInt1);
                    break;
                case CommandType.SetColor:
                    material.SetColor(paramName, valueColor1);
                    break;
                case CommandType.SetKeyword:
                    if (valueFloat1 > 0)
                    {
                        material.EnableKeyword(paramName);
                    }
                    else
                    {
                        material.DisableKeyword(paramName);
                    }
                    //material.SetFloat(paramName, valueFloat1);
                    break;
            }
        }
    }

    [Serializable]
    public class MaterialElement
    {
        public bool selected = true;
        public bool resetMaterial;
        [PathRef(typeof(Material))]
        public string materialPath;
    }


    [Serializable]
    public class ShaderPlayableTweenCommand: ShaderCommand
    {
        private Dictionary<Material, int> _startIntDict = new Dictionary<Material, int>();
        private Dictionary<Material, float> _startFloatDict = new Dictionary<Material, float>();
        private Dictionary<Material, Color> _startColorDict = new Dictionary<Material, Color>();

        public void Init(List<Material> mats)
        {
            _startIntDict.Clear();
            _startFloatDict.Clear();
            _startColorDict.Clear();
            foreach (var mat in mats)
            {
                if (!CheckParam(mat)) return;
                switch (type)
                {
                    case CommandType.SetFloat:
                        var startFloatVal = mat.GetFloat(paramName);
                        _startFloatDict[mat] = startFloatVal;
                        break;
                    case CommandType.SetInt:
                        var startIntVal = mat.GetInt(paramName);
                        _startIntDict[mat] = startIntVal;
                        break;
                    case CommandType.SetColor:
                        var startColorVal = mat.GetColor(paramName);
                        _startColorDict[mat] = startColorVal;
                        break;
                }
            }
        }

        public void DoCommand(Material material, float proc)
        {
            if (!CheckParam(material)) return;
            switch (type)
            {
                case CommandType.SetFloat:
                    DoSetFloatCommand(material, proc);
                    break;
                case CommandType.SetColor:
                    DoSetColorCommand(material, proc);
                    break;
                case CommandType.SetInt:
                    DoSetIntCommand(material, proc);
                    break;
                default:
                    break;
            }
        }

        private void DoSetFloatCommand(Material material, float proc)
        {
            float valFloat;
            if (useCurve)
            {
                valFloat = ScenePlayableUtil.GetCurvedFloat(valueFactor, valueCurve0, proc);
            }
            else
            {
                var startFloat = valueFloat0;
                if (!useStartValue && _startFloatDict.ContainsKey(material))
                    startFloat = _startFloatDict[material];
                valFloat = startFloat + (valueFloat1 - startFloat) * proc;
            }
            material.SetFloat(paramName, valFloat);
        }

        private void DoSetKeywordCommand(Material material, float proc)
        {
            DoSetFloatCommand(material, proc);
            if (valueFloat1 > 0)
            {
                material.EnableKeyword(paramName);
            }
            else
            {
                material.DisableKeyword(paramName);
            }
            
        }

        private void DoSetColorCommand(Material material, float proc)
        {
            Color valColor;
            if (useCurve)
            {
                AnimationCurve[] curves = { valueCurve0, valueCurve1, valueCurve2, valueCurve3 };
                valColor = ScenePlayableUtil.GetCurvedColor(curves, proc);
            }
            else
            {
                var startColor = valueColor0;
                if (!useStartValue && _startColorDict.ContainsKey(material))
                    startColor = _startColorDict[material];
                valColor = Color.Lerp(startColor, valueColor1, proc);
            }
            material.SetColor(paramName, valColor);
        }

        private void DoSetIntCommand(Material material, float proc)
        {
            int valInt;
            if (useCurve)
            {
                valInt = ScenePlayableUtil.GetCurvedInt(valueFactor, valueCurve0, proc);
            }
            else
            {
                var startInt = valueInt0;
                if (!useStartValue && _startIntDict.ContainsKey(material))
                    startInt = _startIntDict[material];
                valInt = startInt + (int)((valueInt1 - startInt) * proc);
            }
            material.SetInt(paramName, valInt);
        }


    }

    [Serializable]
    public class ShaderCommand
    {
        public enum CommandType
        {
            SetFloat = 0,
            SetInt = 1,
            SetColor = 2,
            SetKeyword = 3,
        }

        public string paramName;
        public CommandType type;
        
        public string paramValue;

        public bool useStartValue;

        public bool useCurve;
        public AnimationCurve valueCurve0 = new AnimationCurve();
        public AnimationCurve valueCurve1 = new AnimationCurve();
        public AnimationCurve valueCurve2 = new AnimationCurve();
        public AnimationCurve valueCurve3 = new AnimationCurve();
        public float valueFactor = 1f;

        public int valueInt0;
        public int valueInt1;

        public float valueFloat0;
        public float valueFloat1;

        public Color valueColor0;
        public Color valueColor1;

        public bool CheckParam(Material material)
        {
            if (type == CommandType.SetKeyword)
            {
                return true;
            }
            return !string.IsNullOrEmpty(paramName) && material && material.HasProperty(paramName);
        }
    }

}