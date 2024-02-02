using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="BluPrintDataList_SO",menuName ="Inventory/BluPrintDataList_SO")]
public class BluPrintDataList_SO : ScriptableObject
{
    public List<BluePrintDetails> bluePrintDataList;//蓝图数据列表
    //根据物品ID查找列表中蓝图信息的方法
    public BluePrintDetails GetBluePrintDetails(int itemID) 
    {
        return bluePrintDataList.Find(b=>b.ID==itemID);
    }
}
//蓝图数据详情类
[System.Serializable]
public class BluePrintDetails 
{
    public int ID;//物品ID
    public InventoryItem[] resourceItem=new InventoryItem[4];//蓝图所需要的资源物品
    public GameObject buildPrefab;//建造物品的预设体
}
