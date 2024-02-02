using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.CropPlant 
{
    public class ReapItem : MonoBehaviour
    {
        private CropDetails cropDetails;//�Ӳ������������Ϣ
        private Transform PlayerTransform => FindObjectOfType<Player>().transform;
        public void InitCropData(int ID) 
        {
            cropDetails=CropManager.Instance.GetCropDetails(ID);
        }
        //�ո��Ӳ�
        public void SpawnHarvestItems()
        {
            //����ũ�����ѭ��
            for (int i = 0; i < cropDetails.producedItemID.Length; i++) //ѭ���������������Ʒ���б�
            {
                int amountToProduce;//�������������
                if (cropDetails.producedMaxAmount[i] == cropDetails.producedMinAmount[i]) //������ɵ������С���
                {//���������ֻ����ָ������
                    amountToProduce = cropDetails.producedMaxAmount[i];
                }
                else //���������С��Χ�ڵ��������
                {
                    amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i] + 1);
                }
                //ִ������ָ����������Ʒ
                for (int j = 0; j < amountToProduce; j++)
                {
                    if (cropDetails.generateAtPlayerPosition)//�Ƿ�����ұ�����λ������
                        EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                    else//�������ͼ����
                    {
                        //�ж�������໹���Ҳࣨ������Ʒ�ķ���
                        var dirX = transform.position.x > PlayerTransform.position.x ? 1 : -1;
                        //�ڵ�ͼ��������Ʒ��λ�ã�һ����Χ�������
                        var spawnPos = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                                transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 0);
                        EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);//������Ʒ
                    }
                }
            }
        }
    }
}
