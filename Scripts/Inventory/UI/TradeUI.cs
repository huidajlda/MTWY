using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace MFarm.Inventory 
{
    public class TradeUI : MonoBehaviour
    {
        public Image itemIcon;//��ƷͼƬ
        public Text itemName;//��Ʒ����
        public InputField tradeAmount;//��������
        public Button submitButton;//ȷ�ϰ�ť
        public Button cancelButton;//ȡ����ť
        private ItemDetails item;//��Ʒ����
        private bool isSellTrade;//����״̬(������)
        private void Awake()
        {
            cancelButton.onClick.AddListener(CancelTrade);//��ȡ�����׵ķ�����ӵ�ȡ����ť�ĵ���¼�
            submitButton.onClick.AddListener(TradeItem);//�����׵ķ�����ӵ�ȷ�ϰ�ť�ĵ���¼�
        }
        //���ý���UI����
        public void SetupTradeUI(ItemDetails item, bool isSell)
        {
            this.item = item;//��Ʒ��Ϣ
            itemIcon.sprite = item.itemIcon;//����ͼƬ
            itemName.text = item.itemName;//��������
            isSellTrade = isSell;//����״̬
            tradeAmount.text = string.Empty;//�������
        }
        //������Ʒ
        private void TradeItem() 
        {
            var amount = Convert.ToInt32(tradeAmount.text);//�ı�ת����
            InventoryManager.Instance.TradeItem(item, amount, isSellTrade);//������Ʒ
            CancelTrade();//������ɹرմ���
        }
        //ȡ������
        private void CancelTrade()
        {
            this.gameObject.SetActive(false);//�رս���UI
        }
    }

}