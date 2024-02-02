using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MFarm.Inventory 
{
    public class InventoryUI : MonoBehaviour
    {
        public ItemTooltip itemTooltip;//��Ʒ��ʾUI
        [Header("��קͼƬ")]
        public Image dragItem;
        [Header("��ұ���")]
        [SerializeField] private GameObject bagUI;//����UI
        private bool bagOpened;//�����Ƿ��
        [Header("ͨ�ñ���")]
        [SerializeField] private GameObject baseBag;//ͨ�ñ�����UI
        public GameObject shopSlotPrefab;//�̵���ӵ�Ԥ����
        public GameObject boxSlotPrefab;//���Ӹ��ӵ�Ԥ����
        [Header("����UI")]
        public TradeUI tradeUI;//����UI
        public TextMeshProUGUI playerMoneyText;//��ҽ�Ǯ�ı�
        //����Ϣ���ͱ�������ĸ��Ӷ���ק�������
        [SerializeField] private SlotUI[] playerSlots;//���ӵ�����
        [SerializeField] private List<SlotUI> baseBagSlots;//ͨ�ñ����ĸ����б�
        private void OnEnable()//����ʱע���¼�
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;//ע����̵���¼�
            EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;//ע��ر��̵���¼�
            EventHandler.ShowTradeUI += OnShowTradeUI;//��ʾ����UI���¼�
        }
        private void OnDisable()//����ʱע���¼�
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI -= OnShowTradeUI;
        }
        //���̵�(ͨ������)�ķ���
        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            GameObject prefab = slotType switch
            {
                SlotType.Shop => shopSlotPrefab,//���̵����;������̵�ĸ���Ԥ����
                SlotType.Box => boxSlotPrefab,//���������������ӵĸ���Ԥ����
                _=>null,
            };
            baseBag.SetActive(true);//��ʾ����UI
            //���ָ�������ĸ���
            baseBagSlots =new List<SlotUI>();
            for (int i = 0; i < bagData.itemList.Count; i++) 
            {
                var slot=Instantiate(prefab,baseBag.transform.GetChild(0)).GetComponent<SlotUI>();
                slot.slotIndex = i;
                baseBagSlots.Add(slot);
            }
            //ǿ��ˢ�±���
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());
            if (slotType == SlotType.Shop) //���̵�����
            {
                bagUI.GetComponent<RectTransform>().pivot=new Vector2(-1,0.5f);//���ñ���ê��λ�ã����䲻����̵��ڵ�
                bagUI.SetActive(true);//��ʾ����UI
                bagOpened = true;
            }
            OnUpdateInventoryUI(InventoryLocation.Box, bagData.itemList);//����UI��ʾ
        }
        //�ر��̵�ķ���
        private void OnBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            baseBag.SetActive(false);//�ر��̵��UI
            itemTooltip.gameObject.SetActive(false);//��Ʒ��ʾҲҪ�ر�
            UpdateSlotHightlight(-1);//�رո���
            //�����б��е�����
            foreach (var slot in baseBagSlots) 
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();//����б�
            if (slotType == SlotType.Shop) //���̵�����
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);//��ê��λ�øĻ�ȥ
                bagUI.SetActive(false);//���ر���UI
                bagOpened = false;
            }
        }
        private void OnBeforeSceneUnloadEvent()
        {
            UpdateSlotHightlight(-1);//ȡ������ѡ��
        }

        private void Start()
        {
            //��ÿһ��SlotUI�ĸ�����Ÿ�ֵ
            for (int i = 0; i < playerSlots.Length; i++) 
            {
                playerSlots[i].slotIndex = i;
            }
            bagOpened = bagUI.activeInHierarchy;//��ȡ��ǰ�����ļ���״̬
            playerMoneyText.text=InventoryManager.Instance.PlayerMoney.ToString();//������ҽ�Ǯ
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B)) 
            {
                OpenBagUI();
            }
        }
        //��ʾ����UI
        private void OnShowTradeUI(ItemDetails item, bool isSell)
        {
            tradeUI.gameObject.SetActive(true);//��ʾ����UI
            tradeUI.SetupTradeUI(item, isSell);//��ʼ������UI
        }
        //���±���UI
        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {
            switch (location) 
            {
                case InventoryLocation.Player://������ϵı���
                    for (int i = 0; i < playerSlots.Length; i++) 
                    {
                        if (list[i].itemAmount > 0)
                        {
                            //����ID�õ���Ʒ��Ϣ
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            playerSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else //û�ж�������ʼ��Ϊ�ո���
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
                case InventoryLocation.Box://���Ӻ��̵���ͬһ����
                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            //����ID�õ���Ʒ��Ϣ
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            baseBagSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else //û�ж�������ʼ��Ϊ�ո���
                        {
                            baseBagSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
            playerMoneyText.text=InventoryManager.Instance.PlayerMoney.ToString();//���½��
        }
        //�򿪹رձ����ķ���
        public void OpenBagUI() 
        {
            bagOpened = !bagOpened;
            bagUI.SetActive(bagOpened);
        }
        //����ѡ�и��Ӹ�����ʾ
        public void UpdateSlotHightlight(int index) 
        {
            foreach (var slot in playerSlots) 
            {
                if (slot.isSelected && slot.slotIndex == index)
                {
                    slot.slotHeightlight.gameObject.SetActive(true);//������ʾ
                }
                else 
                {
                    slot.isSelected = false;//�޸�ѡ��״̬
                    slot.slotHeightlight.gameObject.SetActive(false);//ȡ��������ʾ
                }
            }
        }
    }
}