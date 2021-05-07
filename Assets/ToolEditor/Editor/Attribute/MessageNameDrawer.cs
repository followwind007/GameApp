#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MessageNameAttribute), true)]
public class MessageNameDrawer : PropertyDrawer
{
    public const string MESSAGE_NAME = "Message name";

    public readonly float propHeight = GUI.skin.textField.CalcSize(new GUIContent()).y;

    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
    {
        Color bfbgColor = GUI.backgroundColor;
        EditorGUI.BeginProperty(pos, label, property);

        GUI.backgroundColor = ColorUtil.lightBlue;
        pos = new Rect(pos.x, pos.y, pos.width, propHeight);
        pos.y += 2;

        var messagePos = new Rect(pos.x, pos.y, pos.width - 250, propHeight);
        property.stringValue = EditorGUI.TextField(messagePos, MESSAGE_NAME, property.stringValue);

        string predict = GetMessageName(property.stringValue);
        var selectBtnPos = new Rect(pos.x + pos.width - 230, pos.y, 30, propHeight);
        if (GUI.Button(selectBtnPos, "->"))
        {
            property.stringValue = predict;
        }

        var dropPos = new Rect(pos.x + pos.width - 250, pos.y, 250, propHeight);
        EditorGUI.SelectableLabel(dropPos, predict);

        GUI.backgroundColor = bfbgColor;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return propHeight * 1 + 4;
    }

    private string GetMessageName(string predict)
    {
        string prefer = "No Predict";
        var messageType = typeof(Pangu.Const.MessageName);
        var fields = messageType.GetFields();
        foreach (var field in fields)
        {
            if (field.Name.ToLower().Contains(predict.ToLower()))
            {
                prefer = field.Name;
                break;
            }
        }
        return prefer;
    }
}

#endif