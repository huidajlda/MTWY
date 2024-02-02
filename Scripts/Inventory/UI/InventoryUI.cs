using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MFarm.Inventory 
{
    public class InventoryUI : MonoBehaviour
    {
        public ItemTooltip itemTooltip;//物品提示UI
        [Header("拖拽图片")]
        public Image dragItem;
        [Header("玩家背包")]
        [SerializeField] private GameObject bagUI;//背包UI
        private bool bagOpened;//背包是否打开
        [Header("通用背包")]
        [SerializeField] private GameObject baseBag;//通用背包的UI
        public GameObject shopSlotPrefab;//商店格子的预设体
        public GameObject boxSlotPrefab;//箱子格子的预设体
        [Header("交易UI")]
        public TradeUI tradeUI;//交易UI
        public TextMeshProUGUI playerMoneyText;//玩家金钱文本
        //将信息栏和背包里面的格子都拖拽这里管理
        [SerializeField] private SlotUI[] playerSlots;//格子的数组
        [SerializeField] private List<SlotUI> baseBagSlots;//通用背包的格子列表
        private void OnEnable()//激活时注册事件
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;//注册打开商店的事件
            EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;//注册关闭商店的事件
            EventHandler.ShowTradeUI += OnShowTradeUI;//显示交易UI的事件
        }
        private void OnDisable()//销毁时注销事件
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI -= OnShowTradeUI;
        }
        //打开商店(通过背包)的方法
        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            GameObject prefab = slotType switch
            {
                SlotType.Shop => shopSlotPrefab,//是商店类型就用上商店的格子预设体
                SlotType.Box => boxSlotPrefab,//是箱子类型用箱子的格子预设体
                _=>null,
            };
            baseBag.SetActive(true);//显示背包UI
            //添加指定数量的格子
            baseBagSlots =new List<SlotUI>();
            for (int i = 0; i < bagData.itemList.Count; i++) 
            {
                var slot=Instantiate(prefab,baseBag.transform.GetChild(0)).GetComponent<SlotUI>();
                slot.slotIndex = i;
                baseBagSlots.Add(slot);
            }
            //强制刷新背包
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());
            if (slotType == SlotType.Shop) //是商店类型
            {
                bagUI.GetComponent<RectTransform>().pivot=new Vector2(-1,0.5f);//设置背包锚点位置，让其不会和商店遮挡
                bagUI.SetActive(true);//显示背包UI
                bagOpened = true;
            }
            OnUpdateInventoryUI(InventoryLocation.Box, bagData.itemList);//更新UI显示
        }
        //关闭商店的方法
        private void OnBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            baseBag.SetActive(false);//关闭商店的UI
            itemTooltip.gameObject.SetActive(false);//物品提示也要关闭
            UpdateSlotHightlight(-1);//关闭高亮
            //销毁列表中的内容
            foreach (var slot in baseBagSlots) 
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();//清空列表
            if (slotType == SlotType.Shop) //是商店类型
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);//将锚点位置改回去
                bagUI.SetActive(false);//隐藏背包UI
                bagOpened = false;
            }
        }
        private void OnBeforeSceneUnloadEvent()
        {
            UpdateSlotHightlight(-1);//取消高亮选着
        }

        private void Start()
        {
            //给每一个SlotUI的格子序号赋值
            for (int i = 0; i < playerSlots.Length; i++) 
            {
                playerSlots[i].slotIndex = i;
            }
            bagOpened = bagUI.activeInHierarchy;//获取当前背包的激活状态
            playerMoneyText.text=InventoryManager.Instance.PlayerMoney.ToString();//更新玩家金钱
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B)) 
            {
                OpenBagUI();
            }
        }
        //显示交易UI
        private void OnShowTradeUI(ItemDetails item, bool isSell)
        {
            tradeUI.gameObject.SetActive(true);//显示交易UI
            tradeUI.SetupTradeUI(item, isSell);//初始化交易UI
        }
        //更新背包UI
        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {
            switch (location) 
            {
                case InventoryLocation.Player://玩家身上的背包
                    for (int i = 0; i < playerSlots.Length; i++) 
                    {
                        if (list[i].itemAmount > 0)
                        {
                            //根据ID拿到物品信息
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            playerSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else //没有东西，初始化为空格子
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
                case InventoryLocation.Box://盒子和商店用同一个了
                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            //根据ID拿到物品信息
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            baseBagSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else //没有东西，初始化为空格子
                        {
                            baseBagSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
            playerMoneyText.text=InventoryManager.Instance.PlayerMoney.ToString();//更新金币
        }
        //打开关闭背包的方法
        public void OpenBagUI() 
        {
            bagOpened = !bagOpened;
            bagUI.SetActive(bagOpened);
        }
        //更新选中格子高亮显示
        public void UpdateSlotHightlight(int index) 
        {
            foreach (var slot in playerSlots) 
            {
                if (slot.isSelected && slot.slotIndex == index)
                {
                    slot.slotHeightlight.gameObject.SetActive(true);//高亮显示
                }
                else 
                {
                    slot.isSelected = false;//修改选中状态
                    slot.slotHeightlight.gameObject.SetActive(false);//取消高亮显示
                }
            }
        }
    }
}