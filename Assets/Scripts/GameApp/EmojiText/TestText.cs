
using System;
using GameApp.Util;
using UnityEngine;
using UnityEngine.UI;
using GameApp.EmojiText;

public class TestText : MonoBehaviour
{
	public EmojiText text;
	// Use this for initialization

	public Text text1;

	public string s1 = "";

	public string s2 = "<color=white><color=red> 服务器将在[%d_delay]分钟后进行维护，请大家合理安排时间，提前做好准备。</color></color>";
	public string len = "7";
	
	private void Start ()
	{
		text.onCreateButton = (go, data) =>
		{
			var img = go.GetComponent<Image>();
			img.color = Color.red;
			var btn = go.GetComponent<Button>();
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(() => Debug.Log("btn clicked"));
			var btnHelper = go.GetComponent<ButtonHelper>();
			btnHelper.onLongPress = () => Debug.Log("long pressed");
			btnHelper.onLongPressExit = () => Debug.Log("long press exit");
		};
		/*text.text = @"
		    <b>加粗</b>
			<i>斜体</i>
			<size=50>字号</size>
			<color=#00ffffff>颜色</color>
			<t=link,text=下划线,type=keyword,id=2025,color=FF7F00,u=true,uheight=2,ucolor=2a5caa>
			<t=link,text=虚空回归,type=card,color=478ef8,id=300108>
			<t=link,text=天梯对战,type=system,color=c10a0e,content=ladder>";*/
		text.text = "<t=link,text=主动,type=keyword,id=6002,color=FF7F00>：使敌方1个装配<color=#33CCFF>九头蛇</color>装备的角色<t=link,text=虚弱,type=keyword,id=7099,color=FF7F00>（1回合1次）";
		//text.text = "<t=link,text=Select One,type=keyword,id=7153,color=FF7F00>:Shrink:\tPower -20, obtain <t=link,text=Ranged,type=keyword,id=7126,color=FF7F00> or Expand: Power +20, obtain <t=link,text=Pierce,type=keyword,id=7035,color=FF7F00>.";
		//text.text = "<color=#ff0000>主动</color>：装配<color=#33CCFF>雷神之锤</color>时，从手牌选1个<color=#33CCFF>仙宫勇士</color>角色移入战场";
		//text.text = "<t=link,text=我,type=keyword,id=7153,color=FF7F00>";
	}

	private void OnGUI()
	{
		if (GUILayout.Button("Change"))
		{
			text.gameObject.SetActive(true);
			//text.text = "测试专用，说明特别的长，我请您吃蒸羊羔<t=link,text=\"人类\",type=keyword,id=1001><t=link,text=\"生化\",type=keyword,id=1002><t=link,text=\"科技\",type=keyword,id=1003><t=link,text=\"宇宙\",type=keyword,id=1004><t=link,text=\"变异\",type=keyword,id=1007><t=link,text=\"神秘\",type=keyword,id=1005><t=link,text=\"神域\",type=keyword,id=1006><t=link,text=\"变异\",type=keyword,id=1007>我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔测试专用，说明特别的长，我请您吃蒸羊羔";
			text.text = "<t=link,text=战吼,type=keyword,id=6003,color=FF7F00>";
		}
		
		s1 = GUILayout.TextField(s1);

		if (GUILayout.Button("Text Length"))
		{
			text1.text = s1;
			Debug.Log($"{text1.preferredWidth}");
		}

		s2 = GUILayout.TextField(s2);
		len = GUILayout.TextField(len);
		
		if (GUILayout.Button("Set Shrink"))
		{
			Debug.Log(string.IsNullOrEmpty(s2)
				? $"{UIUtils.ShrinkText(text.text, Convert.ToInt32(len))}"
				: $"{UIUtils.ShrinkText(s2, Convert.ToInt32(len))}");
		}

	}

	
	
	
}
