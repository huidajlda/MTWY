using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Map;
namespace MFarm.CropPlant 
{
    //Ԥ�ȼ��ص���ͼ�ϵ�ũ����Ľű�
    public class CropGenerator : MonoBehaviour
    {
        private Grid currentGrid;//��ǰ������Ϣ
        public int seedItemID;//����ID
        public int growthDays;//�ɳ���ʲô�׶�
        private void Awake()
        {
            currentGrid=FindObjectOfType<Grid>();//�õ�������Ϣ
        }
        private void OnEnable()
        {
            EventHandler.GenerateCropEvent+= GenerateCrop;//�������غ��ͼ�����ɳ�ʼũ������¼�
        }
        private void OnDisable()
        {
            EventHandler.GenerateCropEvent -= GenerateCrop;
        }
        //����ũ����
        private void GenerateCrop() 
        {
            Vector3Int cropGridPos = currentGrid.WorldToCell(transform.position);//�õ���ǰ����������
            //���µ�ͼ����
            if (seedItemID != 0) 
            {
                var tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(cropGridPos);//�ܴﵽ��Ƭ��Ϣ
                if (tile == null) 
                {
                    tile = new TileDetails();//����һ����Ϣ��Ƭ��Ϣ
                    tile.gridX = cropGridPos.x;
                    tile.gridY = cropGridPos.y;
                }
                //������Ƭ��Ϣ
                tile.daysSinceWatered = -1;
                tile.seedItemID = seedItemID;
                tile.growthDays = growthDays;
                GridMapManager.Instance.UpdateTileDetails(tile);//ˢ�µ�ͼ
            }
        }
    }
}
