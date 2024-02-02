using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Inventory //ʰȡҲҪʹ����Ʒ���ݣ�����Ҳ������������ռ���
{
    public class ItemPickUp : MonoBehaviour
    {
        //���봥��������ײʰȡ��Ʒ��
        private void OnTriggerEnter2D(Collider2D other)
        {
            Item item=other.GetComponent<Item>();
            if (item != null) //��ײ��Ʒ������û��Item�ű�
            {
                if (item.itemDetails.canPickedup) //����Ʒ����ʰȡ
                {
                    //��Ϊʰȡ���ǵ�ͼ�ϵ���Ʒ�����Եڶ���������true����Ҫ��������
                    InventoryManager.Instance.AddItem(item, true);//���ñ����������Ʒ�ķ���
                    //������Ч
                    EventHandler.CallPlaySoundEvent(SoundName.Pickup);
                }
            }
        }
    }
}
