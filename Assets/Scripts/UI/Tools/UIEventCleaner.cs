
using GameApp.DataBinder;
using GameApp.Util;

namespace UnityEngine.UI
{
    public class UIEventCleaner : MonoBehaviour
    {
        private void OnDestroy()
        {
            CleanEvent(gameObject);
        }

        public static void CleanEvent(GameObject go)
        {
            RemoveButton(go);
            RemoveInputField(go);
            RemoveToggle(go);
        }

        public static void RemoveButton(GameObject go)
        {
            var btns = go.GetComponentsInChildren<Button>(true);
            foreach (var btn in btns)
            {
                btn.onClick.RemoveAllListeners();
            }

            var helpers = go.GetComponentsInChildren<ButtonHelper>(true);
            foreach (var helper in helpers)
            {
                helper.onLongPress = null;
                helper.onLongPressExit = null;
            }
        }

        public static void RemoveInputField(GameObject go)
        {
            var inputs = go.GetComponentsInChildren<InputField>(true);
            foreach (var input in inputs)
            {
                input.onValueChanged.RemoveAllListeners();
                input.onEndEdit.RemoveAllListeners();
            }
        }

        public static void RemoveToggle(GameObject go)
        {
            var tgls = go.GetComponentsInChildren<Toggle>(true);
            foreach (var tgl in tgls)
            {
                tgl.onValueChanged.RemoveAllListeners();
            }
        }

        public static void ClearBind(BindableValues binds)
        {
            if (!Application.isPlaying) return;
            foreach (var v in binds.valDict.Values)
            {
                RemoveHoldedObject(v);

                if (v is Button btn)
                {
                    btn.onClick = null;
                }
                else if (v is Toggle tgl)
                {
                    tgl.onValueChanged = null;
                }
                else if (v is InputField input)
                {
                    input.onValueChanged = null;
                    input.onEndEdit = null;
                }
                else if (v is Dropdown drop)
                {
                    drop.onValueChanged = null;
                }
            }
        }
        
        private static void RemoveHoldedObject(object o)
        {
            #if TEMPLATE_MODE
            var state = Framework.Manager.LuaManager.Instance.State;
            #else
            var state = LuaHelper.GetLuaState();
            #endif
            if (state == null || o == null) return;

            if (o is Sprite || o is Texture || o is AnimationClip) return;
            if (o is GameObject go)
            {
                if (go && go.scene.rootCount == 0) return;
            }

            var translator = state.translator;
            if (translator.Getudata(o, out var idx))
            {
                translator.RemoveObject(idx);
            }
        }
        
        
    }
}