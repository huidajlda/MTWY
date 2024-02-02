using System.Collections.Generic;
using UnityEngine;
//�����Լ�д�����ṹ�嶼д������ű����棬��������޸ģ�����
[System.Serializable]
//��Ʒ��Ϣ��
public class ItemDetails
{
    public int itemId;//��ƷID
    public string itemName;//��Ʒ����
    public ItemType itemType;//��Ʒ������
    public Sprite itemIcon;//��Ʒ�ľ���ͼ
    public Sprite itemOnWorldSprite;//�ڵ�ͼ�ϲ���ʱ��ʾ��ͼƬ
    public string itemDescription;//��Ʒ����
    public int itemUseRadius;//��ʹ�õķ�Χ
    public bool canPickedup;//�Ƿ����ʰȡ
    public bool canDropped;//�Ƿ���Զ���
    public bool canCarried;//�Ƿ���Ծ���
    public int itemPrice;//��Ʒ�Ĺ���ʱ�ļ�ֵ
    [Range(0,1)]
    public float sellPercentage;//��Ʒ����ʱ��ֵ������ۿ۰ٷֱ�
}
//�������ŵ���Ʒ���ݵĽṹ��
//class�Ļ����ж�����Ҫ�жϱ���λ���Ƿ�Ϊ��
[System.Serializable]
public struct InventoryItem 
{
    public int itemID;//��Ʒ��ID
    public int itemAmount;//��Ʒ������
}
//���Ŷ������͵���
[System.Serializable]
public class AnimatorType 
{
    public PartType partType;//����
    public PartName partName;//����
    public AnimatorOverrideController OverrideController;//����������
}
//���������л�����
[System.Serializable]
public class SerializableVector3 
{
    public float x,y,z;//Vector3���л�����ܱ���
    public SerializableVector3(Vector3 pos) 
    {
        this.x=pos.x; this.y =pos.y; this.z=pos.z;
    }
    public Vector3 ToVector3() 
    {
        return new Vector3(x,y,z);
    }
    public Vector2Int ToVector2Int() 
    {
        return new Vector2Int((int)x, (int)y);
    }
}
//�����������Ʒ��
public class SceneItem
{
    public int itemID;//��ƷID
    public SerializableVector3 position;//��Ʒ�����л�����
}
//��������Ľ�����Ʒ��
[System.Serializable]
public class SceneFurniture 
{
    public int itemID;//��ƷID
    public SerializableVector3 position;//��Ʒ�����л�����
    public int boxIndex;//�������
}
[System.Serializable]
//��Ƭ���ӵ�����
public class TileProperty 
{
    public Vector2Int tileCordinate;//��Ƭ����
    public GridType gridType;//��������
    public bool boolTypeValue;//�Ƿ񱻱��
}

//��Ƭ����ϸ��Ϣ��
public class TileDetails 
{
    public int gridX, gridY;//��Ƭ��XY����
    public bool canDig;//�Ƿ�����ڿ�
    public bool canDropItem;//�Ƿ���Զ���
    public bool canPlaceFurniture;//�Ƿ���Է���
    public bool isNPCObstacle;//�Ƿ���NPC���ϰ�
    public int daysSinceDug = -1;//����ӱ����˶�������
    public int daysSinceWatered = -1;//��ˮ��������
    public int seedItemID = -1;//��ֲ���ӵ�ID
    public int growthDays= -1;//���ӳɳ���������
    public int daysSinceLastHarvest = -1;//����һ���ջ���˼���(��Щ���ӿ��Է����ջ�)
}
//NPCλ����
[System.Serializable]
public class NPCPosition 
{
    public Transform npc;
    public string startScene;//��ʼ�ĳ���
    public Vector3 position;//����
}
//����������·���ļ�����

[System.Serializable]
public class SceneRoute 
{
    public string fromSceneName;//��ʲô����
    public string gotoSceneName;//��ʲô����
    public List<ScenePath> scenePathList;//·���б�
}
//����·����
[System.Serializable]
public class ScenePath
{
    public string sceneName;//����������
    public Vector2Int fromGridCell;//��������
    public Vector2Int gotoGridCell;//ȥ������
}