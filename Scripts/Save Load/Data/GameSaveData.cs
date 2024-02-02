using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Save 
{   //���������
    [System.Serializable]
    public class GameSaveData
    {
        public string dataSceneName;//��Ϸ��������
        //��������������ֵ䣬string����������
        public Dictionary<string, SerializableVector3> characterPosDict;
        //���浱ǰ������������
        public Dictionary<string, List<SceneItem>> sceneItemDict;
        //���浱ǰ�����ļҾ�
        public Dictionary<string, List<SceneFurniture>> sceneFurnitureDict;
        //������Ƭ��Ϣ
        public Dictionary<string, TileDetails> tileDetailsDict;
        //�����Ƿ��һ�μ���
        public Dictionary<string, bool> firstLoadDict;
        //��������
        public Dictionary<string, List<InventoryItem>> inventoryDict;
        //ʱ������
        public Dictionary<string, int> timeDict;
        //��ҽ�Ǯ
        public int playerMoney;
        //NPC������
        public string targetScene;//�г̵�Ŀ�곡��
        public bool interactable;//�Ƿ���Ի���
        public int animationInstanceID;//����
        //��������
        public bool isFirst;//�Ƿ��һ�β���
    }
}
