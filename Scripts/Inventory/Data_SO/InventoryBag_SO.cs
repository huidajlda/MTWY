using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="InventoryBag_SO",menuName ="Inventory/InventoryBag_SO")]
public class InventoryBag_SO : ScriptableObject
{
    public List<InventoryItem> itemList;//�����洢���ݵ��б�
    //������ƷID������Ʒ����
    public InventoryItem GetInventoryItem(int ID) 
    {
        return itemList.Find(i => i.itemID == ID);
    }
}
