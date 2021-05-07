using UnityEngine;
using System.Collections.Generic;

namespace GameApp.DataBinder
{
    [System.Serializable]
    public class BindableExpose
    {
        public string key;
        public ExposedReference<Object> value;
    }

    [System.Serializable]
    public class BindableExposes
    {
        [SerializeField]
        public List<BindableExpose> _list;

        public List<BindableExpose> List { get { return _list; } }

    }

}