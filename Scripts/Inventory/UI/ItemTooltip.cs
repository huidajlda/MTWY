using MFarm.Inventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;//物品名字文本
    [SerializeField] private TextMeshProUGUI typeText;//物品类型
    [SerializeField] private TextMeshProUGUI descriptionText;//物品详情
    [SerializeField] private Text valueText;//物品价格
    [SerializeField] private GameObject bottomPart;//售卖按钮
    [Header("建造")]
    public GameObject resourcePanel;//资源面板
    [SerializeField] private Image[] resourceItem;//资源图片
    //设置物品提示的方法
    public void SetupTooltip(ItemDetails itemDetails,SlotType slotType) 
    {
        nameText.text = itemDetails.itemName;
        typeText.text = GetItemType(itemDetails.itemType);
        descriptionText.text=itemDetails.itemDescription;
        //只有种子，商品，家具可以交易，所以只给这些显示金钱
        if (itemDetails.itemType == ItemType.Seed || itemDetails.itemType == ItemType.Commodity || itemDetails.itemType == ItemType.Furniture)
        {
            bottomPart.SetActive(true);//显示按钮
            //显示价格
            var price = itemDetails.itemPrice;
            if (slotType == SlotType.Bag) //物品在背包里
            {
                price = (int)(price * itemDetails.sellPercentage);//显示的是售卖价格
            }
            valueText.text = price.ToString();//赋值给文本
        }
        else bottomPart.SetActive(false);//不显示价格
        //在单行文字变成多行时会有延迟（有些物品描述比较多），所以让其重新绘制就不会有延迟了
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());//强制重新绘制UI
    }
    private string GetItemType(ItemType itemType) 
    {
        return itemType switch//switch语句的语法糖
        {
            ItemType.Seed => "种子",
            ItemType.Commodity => "商品",
            ItemType.Furniture => "家具",
            ItemType.BreakTool => "工具",
            ItemType.ChopTool => "工具",
            ItemType.CollectTool => "工具",
            ItemType.HoeTool => "工具",
            ItemType.ReapTool => "工具",
            ItemType.WaterTool => "工具",
            _ => "无"
        };
    }
    //显示蓝图所需资源
    public void SetupResourcePanel(int ID) 
    {
        var bluePrintDetails=InventoryManager.Instance.bluPrintData.GetBluePrintDetails(ID);//获取蓝图数据
        for (int i = 0; i < resourceItem.Length; i++) //最多只显示3个所需资源
        {
            if (i < bluePrintDetails.resourceItem.Length)
            {
                var item = bluePrintDetails.resourceItem[i];
                resourceItem[i].gameObject.SetActive(true);//显示UI
                resourceItem[i].sprite = InventoryManager.Instance.GetItemDetails(item.itemID).itemIcon;//拿到物品图片
                resourceItem[i].transform.GetChild(0).GetComponent<Text>().text = item.itemAmount.ToString();//所需数量
            }
            else 
            {
                resourceItem[i].gameObject.SetActive(false);//关闭UI
            }
        }
    }
}
