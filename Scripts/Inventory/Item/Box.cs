using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Inventory 
{
    public class Box : MonoBehaviour
    {
        public InventoryBag_SO boxBagTemplate;//���ӱ�����ģ������
        public InventoryBag_SO boxBagData;//���ӱ�������
        public GameObject mouseIcon;//�߽���ʾ�����ͼ��
        private bool canOpen = false;//�����Ƿ���Դ�
        private bool isOpen;//�����Ƿ��Ѿ���
        public int index;//�������
        private void OnEnable()
        {
            if (boxBagData == null) 
            {
                boxBagData=Instantiate(boxBagTemplate);//��ֵģ������
            }
        }
        //���봥����
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) 
            {
                canOpen = true;//���ӿ��Դ�
                mouseIcon.SetActive(true);//��ʾͼ��
            }
        }
        //�뿪������
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = false;//���Ӳ��ܴ�
                mouseIcon.SetActive(false);//�ر�ͼ��
            }
        }
        private void Update()
        {   //����û�д�,�ҿ��Դ򿪲���������Ҽ�
            if (!isOpen && canOpen && Input.GetMouseButtonDown(1)) 
            {
                //������
                EventHandler.CallBaseBagOpenEvent(SlotType.Box, boxBagData);
                isOpen = true;
            }
            if (!canOpen && isOpen) //�����Ѿ����˵����Ӵ���
            {
                //�ر�����
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen=false;
            }
            if (isOpen&&Input.GetKeyDown(KeyCode.Escape)) //��ESC�ر�����
            {
                //�ر�����
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen = false;
            }
        }
        //��ʼ������
        public void InitBox(int boxIndex) 
        {
            index=boxIndex;
            var key = this.name + index;
            if (InventoryManager.Instance.GetBoxDataList(key) != null) //�ֵ����Ѿ���������
            {
                boxBagData.itemList = InventoryManager.Instance.GetBoxDataList(key);
            }
            else //�½�����
            {
                InventoryManager.Instance.AddBoxDataDic(this);
            }
        }
    }
}