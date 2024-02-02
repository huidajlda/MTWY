using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.CropPlant;
namespace MFarm.Inventory //�����ڱ����������ռ���
{
    public class Item : MonoBehaviour
    {
        public int itemID;//��Ʒ��ID
        private SpriteRenderer spriteRenderer;//������Ⱦ��
        public ItemDetails itemDetails;//�洢��Ʒ��ϸ��Ϣ�ı���
        private BoxCollider2D coll;//��ײ��
        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();//��ȡ�������ϵľ�����Ⱦ��
            coll = GetComponent<BoxCollider2D>();//��ȡ�������ϵ���ײ��
        }
        private void Start()
        {
            if (itemID != 0) 
            {
                Init(itemID);
            }
        }
        //��ʼ����Ʒ(������Ʒ)
        public void Init(int ID) 
        {
            itemID = ID;
            //ʹ��InventoryManager�����ͨ��ID��ȡ��Ʒ����ķ���
            itemDetails = InventoryManager.Instance.GetItemDetails(itemID);
            if (itemDetails != null) //��ȡ��Ϊ��
            {
                //��ֵ��ͼ����ʾ��ͼƬ(����Ԫ������Ƿ�ֹ������Ƶ�ͼ�ϵ�ͼƬ������Ҳ����ȷ��ʾ)
                //�е�ͼ�ϵ�ͼƬ����ʾ��û������ʾ���屾���ͼƬ
                spriteRenderer.sprite = itemDetails.itemOnWorldSprite!=null?itemDetails.itemOnWorldSprite:itemDetails.itemIcon;
                //�޸���ײ��ĳߴ��λ��(��ΪͼƬê����ͼƬ�·��е㣬����ͼƬ����)�������ܸ���ͼƬ��С�仯
                //��ȡͼƬ�Ĵ�С
                Vector2 newSize=new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
                coll.size = newSize;//�޸���ײ���С
                coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
            }
            if (itemDetails.itemType == ItemType.ReapableScenery) 
            {
                gameObject.AddComponent<ReapItem>();//����Ӳݽű�
                gameObject.GetComponent<ReapItem>().InitCropData(itemDetails.itemId);
                gameObject.AddComponent<Iteminteractive>();//��������߹�ҡ�εĽű�
            }  
        }
    }

}
