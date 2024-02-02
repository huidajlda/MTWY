using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MFarm.Inventory 
{
                                        //�㰴�¼��Ľӿ�       ��ʼ��ק          ��ק����      ������ק
    public class SlotUI : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        [Header("����Ļ�ȡ")]
        //������Կ��������ڼ�����и�ֵ
        [SerializeField] private Image slotImage;//��ƷͼƬ
        [SerializeField] private TextMeshProUGUI amountText;//�����ı�
        public Image slotHeightlight;//��ѡ�еĸ���ͼƬ(public��Ϊ��InventoryUI�ĸ����������Ե���)
        [SerializeField] private Button button;//�Լ����ϵ�button���
        [Header("��������")]
        public SlotType slotType;//�������͵�ö��
        public bool isSelected;//�Ƿ�ѡ��
        [Header("��Ʒ��Ϣ")]
        public ItemDetails itemDetails;//��Ʒ����
        public int itemAmount;//��Ʒ����
        public int slotIndex;//�������
        public InventoryLocation Location //�������ͷ���λ�õ�����
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
        public InventoryUI inventoryUI=>GetComponentInParent<InventoryUI>();//�õ��������ϵ�InventoryUI
        private void Start()
        {
            //ItemDetails��һ���࣬����ı�����Ĭ���и���ʼ�������Բ����ÿ����ж�
            if (itemDetails ==null) //˵���ǿո���
            {
                UpdateEmptySlot();//��ʼ���ո���
            }
        }
        //��ʼ��Ϊ�ո���
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;//ѡ���˿ո��ӣ�ȡ��ѡ��
                inventoryUI.UpdateSlotHightlight(-1);//ȡ������
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
            itemDetails = null;//����Ϊ��
            slotImage.enabled = false;//�ر�ͼƬ
            amountText.text = string.Empty;//�����ı�Ϊ��
            button.interactable = false;//�ո��Ӳ��ܱ��㰴
        }
        //���¸���UI����Ϣ
        public void UpdateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;//��¼��Ʒ��Ϣ
            slotImage.sprite = item.itemIcon;//����ͼƬ
            itemAmount = amount;//��¼����
            amountText.text = amount.ToString();//��������
            slotImage.enabled = true;//����ͼƬ
            button.interactable = true;//��ť���Կ��Ե㰴
        }
        //ʵ�ֵ㰴�¼��ӿڵķ���
        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails==null)//����Ϊ�ղ�ִ��
                return;
            isSelected=!isSelected;//ѡ��״̬���л�
            inventoryUI.UpdateSlotHightlight(slotIndex);//���ø�����ʾ����
            if (slotType == SlotType.Bag) //�����Ʒ�ڱ���������
            {
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
        }
        //��ʼ��ק
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemAmount != 0) 
            {
                inventoryUI.dragItem.enabled = true;//��ʾ��קͼƬ
                inventoryUI.dragItem.sprite = slotImage.sprite;//���þ���ͼ
                inventoryUI.dragItem.SetNativeSize();//������Ȼ��С
                isSelected = true;
                inventoryUI.UpdateSlotHightlight(slotIndex);
            }
        }
        //��ק����
        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.transform.position=Input.mousePosition;//ͼƬλ�õ������λ��
        }
        //������ק
        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.enabled=false;//�ر�ͼƬ
            if (eventData.pointerCurrentRaycast.gameObject != null) //��ײ������UI����Ʒ(�����)�᷵�ؿ�
            {//��ײ�����Ǹ��ӵ�UI�Ͳ�ִ��
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                    return;
                //�õ�Ŀ�����ϵ�SlotUI
                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targetIndex = targetSlot.slotIndex;//�õ�Ŀ���ڱ���������
                //��Player������Χ�ڽ���
                if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                }
                else if (slotType == SlotType.Shop && targetSlot.slotType == SlotType.Bag) //���̵�����
                {
                    EventHandler.CallShowTradeUI(itemDetails, false);//��
                }
                else if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Shop) //�������̵�(��)
                {
                    EventHandler.CallShowTradeUI(itemDetails, true);//��
                }
                else if (slotType != SlotType.Shop && targetSlot.slotType != SlotType.Shop && slotType != targetSlot.slotType) 
                {//�������Ͳ����̵꣬��Ŀ�����Ҳ�����̵꣬�Һ��Լ����Ͳ�ͬ(��ͬ���Ǳ����ڻ���)
                    //���������жϴ����ӵ������������������������
                    InventoryManager.Instance.SwapItem(Location, slotIndex, targetSlot.Location, targetSlot.slotIndex);
                }
                inventoryUI.UpdateSlotHightlight(-1);//�ر����и��ӵĸ�����ʾ
            }
            else //��������ͼ��
            {
                if (itemDetails.canDropped) 
                {
                    //������Ļ����ת��������(�õ��ɿ�����Ӧ���ϵ�����)
                    //Ĭ����������z�������ľ���Ϊ-10������Ҫ�Ѿ��벹����ȥ
                    var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                    EventHandler.CallInstantiateItemInScene(itemDetails.itemId, pos);//��ͼ�ϴ�����Ʒ���¼�
                }
            }
        }
    }
}
