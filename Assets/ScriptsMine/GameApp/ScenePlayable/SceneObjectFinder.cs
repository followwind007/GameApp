using UnityEngine;

namespace GameApp.ScenePlayable
{
    public enum FindType
    {
        None = 0,
        Tag = 1,
        Path = 2,
    }

    [System.Serializable]
    public class SceneObjectFinder
    {
        public FindType findType = FindType.None;
        public string valueString0;

        public GameObject Target
        {
            get
            {
                GameObject target = null;
                switch (findType)
                {
                    case FindType.None:
                        break;
                    case FindType.Tag:
                        target = GameObject.FindGameObjectWithTag(valueString0);
                        break;
                    case FindType.Path:
                        target = GameObject.Find(valueString0);
                        break;
                    default:
                        break;
                }
                return target;
            }
        }


    }

}