using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MFarm.Inventory 
{
    [RequireComponent(typeof(SlotUI))]
    //��껬�����뿪�Ľӿ�
    public class ShowItemTooltip : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        private SlotUI slotUI;//���ӽű�
        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();//�õ��������ϵ�InventoryUI

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();//��ȡ���
        }
        //��껬���ӿڷ�����ʵ��
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotUI.itemDetails != null)
            {
                inventoryUI.itemTooltip.gameObject.SetActive(true);//��ʾ��ʾ
                inventoryUI.itemTooltip.SetupTooltip(slotUI.itemDetails, slotUI.slotType);//�����ı�
                inventoryUI.itemTooltip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);//����ê��λ��
                inventoryUI.itemTooltip.transform.position=transform.position+Vector3.up*60;//����λ��
                if (slotUI.itemDetails.itemType == ItemType.Furniture)
                {
                    inventoryUI.itemTooltip.resourcePanel.SetActive(true);//��ʾ��Դ���
                    inventoryUI.itemTooltip.SetupResourcePanel(slotUI.itemDetails.itemId);//�������UI
                }
                else 
                {
                    inventoryUI.itemTooltip.resourcePanel.SetActive(false);//�ر���Դ���
                }
            }
            else 
            {
                inventoryUI.itemTooltip.gameObject.SetActive(false);//�ر���ʾ
            }
        }
        //����뿪ʱ
        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.itemTooltip.gameObject.SetActive(false);
        }
    }

}