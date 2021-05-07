#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Framework.Manager;

namespace Pangu.ScenePlayable
{
    public class SpPreviewWindow : EditorWindow
    {
        private string _eventName = "";
        private float _radius = 5f;

        private string[] _eventOptions = { };
        private int _selectedEvent;
        private int _lastSelectedEvent = -1;

        private GameObject _sender;

        private GameObject _follow;
        private bool _useFollow = true;

        private bool _showSender = true;

        private bool _triggerAllFlag = true;

        private int _checkCount;
        private readonly int _checkInterval = 100;
        private static readonly int Mode = Shader.PropertyToID("_Mode");
        private static readonly int Color = Shader.PropertyToID("_Color");
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

        private void OnGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                DrawNotAvailable();
                return;
            }
            //Event Name
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            _selectedEvent = EditorGUILayout.Popup(SpConst.EVENT_NAME, _selectedEvent, _eventOptions, GUILayout.Width(300));
            if (_lastSelectedEvent != _selectedEvent && _selectedEvent < _eventOptions.Length)
            {
                _eventName = _eventOptions[_selectedEvent];
                _lastSelectedEvent = _selectedEvent;
            }
            _eventName = EditorGUILayout.TextField(_eventName);
            GUILayout.EndHorizontal();
            //Trigger Radius
            GUILayout.Space(5);
            _radius = EditorGUILayout.Slider(SpConst.TRIGGER_RADIUS, _radius, 0f, 10f);
            //Follow Object
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            _follow = EditorGUILayout.ObjectField(SpConst.USE_FOLLOW, _follow, typeof(GameObject), true) as GameObject;
            GUILayout.Space(20);
            _useFollow = EditorGUILayout.Toggle(_useFollow, GUILayout.Width(50));
            GUILayout.EndHorizontal();
            //Show Trigger Object
            GUILayout.Space(5);
            _showSender = EditorGUILayout.Toggle(SpConst.SHOW_SENDER, _showSender);
            //Trigger All
            GUILayout.Space(5);
            _triggerAllFlag = EditorGUILayout.Toggle(SpConst.TRIGGER_ALL, _triggerAllFlag);
            //Button
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(SpConst.TRIGGER_EVENT, GUILayout.Height(20), GUILayout.Width(120)))
                TriggerEvent();
            if (GUILayout.Button(SpConst.BROADCAST_EVENT, GUILayout.Height(20), GUILayout.Width(120)))
                ScenePlayableManager.Instance.OnReceiveEventPlayable(_eventName);
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        private void DrawNotAvailable()
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(SpConst.NOT_AVAILABLE);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void Awake()
        {
            EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;
        }

        private void OnDestroy()
        {
            if (_sender)
            {
                DestorySender();
            }
            SceneView.RepaintAll();
        }

        private void HandleOnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                DestorySender();
            }
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                if (_sender)
                {
                    if (_showSender)
                        _sender.transform.localScale = new Vector3(_radius, _radius, _radius) * 2;
                    else
                        _sender.transform.localScale = Vector3.zero;
                    
                    if (_useFollow && _follow != null)
                        _sender.transform.position = _follow.transform.position;
                }
                if (focusedWindow == this) SceneView.RepaintAll();
                if (Input.GetMouseButton(1))
                {
                    TriggerEvent();
                }
            }

            if (++_checkCount > _checkInterval)
            {
                _checkCount = 0;
                if (Application.isPlaying)
                {
                    CreateSender();
                    RefreshEventList();
                }
                else DestorySender();
            }
        }

        private void RefreshEventList()
        {
            GetEventOptions();
            if (_selectedEvent < 0 || _selectedEvent >= _eventOptions.Length)
            {
                _selectedEvent = _eventOptions.Length > 0 ? 0 : -1;
            }
           
        }

        private void TriggerEvent()
        {
            ScenePlayableManager.Instance.TriggerEventNearby(_sender, _eventName, _radius, _triggerAllFlag);
        }

        private void GetEventOptions()
        {
            List<string> eventList = new List<string>();
            foreach (var kv in ScenePlayableManager.Instance.PlayableDict)
            {
                eventList.Add(kv.Key);
            }
            _eventOptions = eventList.ToArray();
        }

        private void CreateSender()
        {
            if (_sender != null) return;

            Material mat = new Material(Shader.Find("Standard"));
            mat.SetInt(Mode, 3);
            mat.SetColor(Color, new Color(0.55f, 0.18f, 0.9f, 0.1f));
            
            mat.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt(ZWrite, 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
            mat.name = "sender mat";

            _sender = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _sender.name = SpConst.SENDER_NAME;
            _sender.hideFlags = HideFlags.DontSave;
            _sender.transform.SetAsFirstSibling();
            DestroyImmediate(_sender.GetComponent<Collider>());

            Renderer render = _sender.GetComponent<Renderer>();
            render.material = mat;

            var mainPlayer = GameObject.Find(SpConst.MAIN_PLAYER);
            if (mainPlayer != null)
                _follow = mainPlayer;
            
        }

        private void DestorySender()
        {
            if (_sender == null) return;
            _sender = null;
            DestroyImmediate(GameObject.Find(SpConst.SENDER_NAME));
        }

    }
}
#endif