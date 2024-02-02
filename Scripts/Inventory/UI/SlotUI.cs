using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MFarm.Inventory 
{
                                        //点按事件的接口       开始拖拽          拖拽过程      结束拖拽
    public class SlotUI : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        [Header("组件的获取")]
        //这个特性可以让其在检查器中赋值
        [SerializeField] private Image slotImage;//物品图片
        [SerializeField] private TextMeshProUGUI amountText;//数量文本
        public Image slotHeightlight;//被选中的高亮图片(public是为了InventoryUI的高亮方法可以调用)
        [SerializeField] private Button button;//自己身上的button组件
        [Header("格子类型")]
        public SlotType slotType;//格子类型的枚举
        public bool isSelected;//是否被选中
        [Header("物品信息")]
        public ItemDetails itemDetails;//物品详情
        public int itemAmount;//物品数量
        public int slotIndex;//背包序号
        public InventoryLocation Location //根据类型返回位置的属性
        {
            get 
            {
                return slotType switch
                {
                    SlotType.Bag => InventoryLocation.Player,
                    SlotType.Box => InventoryLocation.Box,
                    _=>InventoryLocation.Player,
                };
            }
        }
        public InventoryUI inventoryUI=>GetComponentInParent<InventoryUI>();//拿到父物体上的InventoryUI
        private void Start()
        {
            //ItemDetails是一个类，上面的变量会默认有个初始化，所以不能用空来判断
            if (itemDetails ==null) //说明是空格子
            {
                UpdateEmptySlot();//初始化空格子
            }
        }
        //初始化为空格子
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;//选中了空格子，取消选着
                inventoryUI.UpdateSlotHightlight(-1);//取消高亮
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
            itemDetails = null;//数据为空
            slotImage.enabled = false;//关闭图片
            amountText.text = string.Empty;//数量文本为空
            button.interactable = false;//空格子不能被点按
        }
        //更新格子UI和信息
        public void UpdateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;//记录物品信息
            slotImage.sprite = item.itemIcon;//更新图片
            itemAmount = amount;//记录数量
            amountText.text = amount.ToString();//更新数量
            slotImage.enabled = true;//开启图片
            button.interactable = true;//按钮可以可以点按
        }
        //实现点按事件接口的方法
        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails==null)//格子为空不执行
                return;
            isSelected=!isSelected;//选中状态的切换
            inventoryUI.UpdateSlotHightlight(slotIndex);//调用高亮显示方法
            if (slotType == SlotType.Bag) //如果物品在背包格子里
            {
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
        }
        //开始拖拽
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemAmount != 0) 
            {
                inventoryUI.dragItem.enabled = true;//显示拖拽图片
                inventoryUI.dragItem.sprite = slotImage.sprite;//设置精灵图
                inventoryUI.dragItem.SetNativeSize();//设置自然大小
                isSelected = true;
                inventoryUI.UpdateSlotHightlight(slotIndex);
            }
        }
        //拖拽过程
        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.transform.position=Input.mousePosition;//图片位置等于鼠标位置
        }
        //结束拖拽
        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.enabled=false;//关闭图片
            if (eventData.pointerCurrentRaycast.gameObject != null) //碰撞到不是UI的物品(地面等)会返回空
            {//碰撞到不是格子的UI就不执行
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                    return;
                //拿到目标身上的SlotUI
                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targetIndex = targetSlot.slotIndex;//拿到目标在背包里的序号
                //在Player背包范围内交换
                if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                }
                else if (slotType == SlotType.Shop && targetSlot.slotType == SlotType.Bag) //从商店买东西
                {
                    EventHandler.CallShowTradeUI(itemDetails, false);//买
                }
                else if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Shop) //背包到商店(卖)
                {
                    EventHandler.CallShowTradeUI(itemDetails, true);//卖
                }
                else if (slotType != SlotType.Shop && targetSlot.slotType != SlotType.Shop && slotType != targetSlot.slotType) 
                {//格子类型不是商店，且目标格子也不是商店，且和自己类型不同(相同就是背包内互换)
                    //这样可以判断从箱子到背包，背包到箱子两者情况
                    InventoryManager.Instance.SwapItem(Location, slotIndex, targetSlot.Location, targetSlot.slotIndex);
                }
                inventoryUI.UpdateSlotHightlight(-1);//关闭所有格子的高亮显示
            }
            else //丢弃到地图上
            {
                if (itemDetails.canDropped) 
                {
                    //鼠标的屏幕坐标转世界坐标(拿到松开鼠标对应地上的坐标)
                    //默认情况摄像机z轴里地面的距离为-10，所以要把距离补偿回去
                    var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                    EventHandler.CallInstantiateItemInScene(itemDetails.itemId, pos);//地图上创建物品的事件
                }
            }
        }
    }
}
