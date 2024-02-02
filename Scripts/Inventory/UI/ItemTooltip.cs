using MFarm.Inventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;//��Ʒ�����ı�
    [SerializeField] private TextMeshProUGUI typeText;//��Ʒ����
    [SerializeField] private TextMeshProUGUI descriptionText;//��Ʒ����
    [SerializeField] private Text valueText;//��Ʒ�۸�
    [SerializeField] private GameObject bottomPart;//������ť
    [Header("����")]
    public GameObject resourcePanel;//��Դ���
    [SerializeField] private Image[] resourceItem;//��ԴͼƬ
    //������Ʒ��ʾ�ķ���
    public void SetupTooltip(ItemDetails itemDetails,SlotType slotType) 
    {
        nameText.text = itemDetails.itemName;
        typeText.text = GetItemType(itemDetails.itemType);
        descriptionText.text=itemDetails.itemDescription;
        //ֻ�����ӣ���Ʒ���Ҿ߿��Խ��ף�����ֻ����Щ��ʾ��Ǯ
        if (itemDetails.itemType == ItemType.Seed || itemDetails.itemType == ItemType.Commodity || itemDetails.itemType == ItemType.Furniture)
        {
            bottomPart.SetActive(true);//��ʾ��ť
            //��ʾ�۸�
            var price = itemDetails.itemPrice;
            if (slotType == SlotType.Bag) //��Ʒ�ڱ�����
            {
                price = (int)(price * itemDetails.sellPercentage);//��ʾ���������۸�
            }
            valueText.text = price.ToString();//��ֵ���ı�
        }
        else bottomPart.SetActive(false);//����ʾ�۸�
        //�ڵ������ֱ�ɶ���ʱ�����ӳ٣���Щ��Ʒ�����Ƚ϶ࣩ�������������»��ƾͲ������ӳ���
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());//ǿ�����»���UI
    }
    private string GetItemType(ItemType itemType) 
    {
        return itemType switch//switch�����﷨��
        {
            ItemType.Seed => "����",
            ItemType.Commodity => "��Ʒ",
            ItemType.Furniture => "�Ҿ�",
            ItemType.BreakTool => "����",
            ItemType.ChopTool => "����",
            ItemType.CollectTool => "����",
            ItemType.HoeTool => "����",
            ItemType.ReapTool => "����",
            ItemType.WaterTool => "����",
            _ => "��"
        };
    }
    //��ʾ��ͼ������Դ
    public void SetupResourcePanel(int ID) 
    {
        var bluePrintDetails=InventoryManager.Instance.bluPrintData.GetBluePrintDetails(ID);//��ȡ��ͼ����
        for (int i = 0; i < resourceItem.Length; i++) //���ֻ��ʾ3��������Դ
        {
            if (i < bluePrintDetails.resourceItem.Length)
            {
                var item = bluePrintDetails.resourceItem[i];
                resourceItem[i].gameObject.SetActive(true);//��ʾUI
                resourceItem[i].sprite = InventoryManager.Instance.GetItemDetails(item.itemID).itemIcon;//�õ���ƷͼƬ
                resourceItem[i].transform.GetChild(0).GetComponent<Text>().text = item.itemAmount.ToString();//��������
            }
            else 
            {
                resourceItem[i].gameObject.SetActive(false);//�ر�UI
            }
        }
    }
}
