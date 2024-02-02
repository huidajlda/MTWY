using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Dialogue 
{
    [System.Serializable]
    public class DialoguePiece
    {
        [Header("�Ի�����")]
        public Sprite faceImage;//����ͼƬ
        public bool onLeft;//�Ƿ������
        public string name;//��������
        [TextArea]
        public string dialogueText;//�Ի�����
        public bool hasToPause;//�Ƿ���ͣ(���ո����)
        [HideInInspector]public bool isDone;//�����Ի�
    }
}
