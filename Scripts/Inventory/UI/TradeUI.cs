using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MFarm.Inventory 
{
    public class TradeUI : MonoBehaviour
    {
        public Image itemIcon;//物品图片
        public Text itemName;//物品名称
        public InputField tradeAmount;//交易数量
        public Button submitButton;//确认按钮
        public Button cancelButton;//取消按钮
        private ItemDetails item;//物品详情
        private bool isSellTrade;//交易状态(买还是卖)
        private void Awake()
        {
            cancelButton.onClick.AddListener(CancelTrade);//将取消交易的方法添加到取消按钮的点击事件
            submitButton.onClick.AddListener(TradeItem);//将交易的方法添加到确认按钮的点击事件
        }
        //设置交易UI界面
        public void SetupTradeUI(ItemDetails item, bool isSell)
        {
            this.item = item;//物品信息
            itemIcon.sprite = item.itemIcon;//更新图片
            itemName.text = item.itemName;//更新名称
            isSellTrade = isSell;//交易状态
            tradeAmount.text = string.Empty;//清空数量
        }
        //交易物品
        private void TradeItem() 
        {
            var amount = Convert.ToInt32(tradeAmount.text);//文本转数字
            InventoryManager.Instance.TradeItem(item, amount, isSellTrade);//交易物品
            CancelTrade();//交易完成关闭窗口
        }
        //取消交易
        private void CancelTrade()
        {
            this.gameObject.SetActive(false);//关闭交易UI
        }
    }

}