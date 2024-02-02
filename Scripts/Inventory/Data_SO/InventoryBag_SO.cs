using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="InventoryBag_SO",menuName ="Inventory/InventoryBag_SO")]
public class InventoryBag_SO : ScriptableObject
{
    public List<InventoryItem> itemList;//背包存储数据的列表
    //根据物品ID返回物品数据
    public InventoryItem GetInventoryItem(int ID) 
    {
        return itemList.Find(i => i.itemID == ID);
    }
}
