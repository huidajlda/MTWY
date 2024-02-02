using UnityEngine;
namespace MFarm.Inventory 
{
    [RequireComponent(typeof(SlotUI))]
    public class ActionBarButton : MonoBehaviour
    {
        public KeyCode key;//��������
        private SlotUI slotUI;//����UI
        private bool canUse;      
        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();//�õ�SlotUI
        }
        private void OnEnable()
        {
            EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;//ע�������Ϸ״̬�ķ���
        }
        private void OnDisable()
        {
            EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;//ע�������Ϸ״̬�ķ���
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
                    slotUI.isSelected = !slotUI.isSelected;//���ӵ�ѡ��״̬
                    if (slotUI.isSelected)
                        slotUI.inventoryUI.UpdateSlotHightlight(slotUI.slotIndex);//���ø���
                    else
                        slotUI.inventoryUI.UpdateSlotHightlight(-1);//ȡ������
                    EventHandler.CallItemSelectedEvent(slotUI.itemDetails, slotUI.isSelected);//����ѡ���¼�
                }
            }
        }
    }
}

