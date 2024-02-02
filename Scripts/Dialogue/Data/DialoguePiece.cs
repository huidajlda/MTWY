using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Dialogue 
{
    [System.Serializable]
    public class DialoguePiece
    {
        [Header("对话详情")]
        public Sprite faceImage;//人物图片
        public bool onLeft;//是否在左边
        public string name;//人物名称
        [TextArea]
        public string dialogueText;//对话内容
        public bool hasToPause;//是否暂停(按空格继续)
        [HideInInspector]public bool isDone;//结束对话
    }
}
