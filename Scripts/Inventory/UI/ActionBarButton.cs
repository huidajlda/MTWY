using UnityEngine;
namespace MFarm.Inventory 
{
    [RequireComponent(typeof(SlotUI))]
    public class ActionBarButton : MonoBehaviour
    {
        public KeyCode key;//键盘输入
        private SlotUI slotUI;//格子UI
        private bool canUse;      
        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();//拿到SlotUI
        }
        private void OnEnable()
        {
            EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;//注册更新游戏状态的方法
        }
        private void OnDisable()
        {
            EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;//注册更新游戏状态的方法
        }

        private void OnUpdateGameStateEvent(GameState gamestate)
        {
            canUse = gamestate == GameState.GamePlay;
        }

        private void Update()
        {
            if (Input.GetKeyDown(key)&&canUse) 
            {
                if (slotUI.itemDetails != null) 
                {
                    slotUI.isSelected = !slotUI.isSelected;//格子的选中状态
                    if (slotUI.isSelected)
                        slotUI.inventoryUI.UpdateSlotHightlight(slotUI.slotIndex);//设置高亮
                    else
                        slotUI.inventoryUI.UpdateSlotHightlight(-1);//取消高亮
                    EventHandler.CallItemSelectedEvent(slotUI.itemDetails, slotUI.isSelected);//触发选择事件
                }
            }
        }
    }
}

