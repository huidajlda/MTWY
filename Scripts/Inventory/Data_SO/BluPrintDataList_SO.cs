using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="BluPrintDataList_SO",menuName ="Inventory/BluPrintDataList_SO")]
public class BluPrintDataList_SO : ScriptableObject
{
    public List<BluePrintDetails> bluePrintDataList;//��ͼ�����б�
    //������ƷID�����б�����ͼ��Ϣ�ķ���
    public BluePrintDetails GetBluePrintDetails(int itemID) 
    {
        return bluePrintDataList.Find(b=>b.ID==itemID);
    }
}
//��ͼ����������
[System.Serializable]
public class BluePrintDetails 
{
    public int ID;//��ƷID
    public InventoryItem[] resourceItem=new InventoryItem[4];//��ͼ����Ҫ����Դ��Ʒ
    public GameObject buildPrefab;//������Ʒ��Ԥ����
}
