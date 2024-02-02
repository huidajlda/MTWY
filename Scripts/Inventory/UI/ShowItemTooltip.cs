using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MFarm.Inventory 
{
    [RequireComponent(typeof(SlotUI))]
    //鼠标滑进和离开的接口
    public class ShowItemTooltip : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        private SlotUI slotUI;//格子脚本
        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();//拿到父物体上的InventoryUI

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();//获取组件
        }
        //鼠标滑进接口方法的实现
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotUI.itemDetails != null)
            {
                inventoryUI.itemTooltip.gameObject.SetActive(true);//显示提示
                inventoryUI.itemTooltip.SetupTooltip(slotUI.itemDetails, slotUI.slotType);//设置文本
                inventoryUI.itemTooltip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);//设置锚点位置
                inventoryUI.itemTooltip.transform.position=transform.position+Vector3.up*60;//调整位置
                if (slotUI.itemDetails.itemType == ItemType.Furniture)
                {
                    inventoryUI.itemTooltip.resourcePanel.SetActive(true);//显示资源面板
                    inventoryUI.itemTooltip.SetupResourcePanel(slotUI.itemDetails.itemId);//设置面板UI
                }
                else 
                {
                    inventoryUI.itemTooltip.resourcePanel.SetActive(false);//关闭资源面板
                }
            }
            else 
            {
                inventoryUI.itemTooltip.gameObject.SetActive(false);//关闭提示
            }
        }
        //鼠标离开时
        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.itemTooltip.gameObject.SetActive(false);
        }
    }

}