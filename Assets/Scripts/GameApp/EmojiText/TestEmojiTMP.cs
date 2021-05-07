using UnityEngine;

namespace GameApp.EmojiText
{
    public class TestEmojiTMP : MonoBehaviour
    {
        public void OnClickTag(TagDataTMP tagData)
        {
            Debug.Log($"click {tagData.matchText} => {tagData.populateText}");
        }
    }
}