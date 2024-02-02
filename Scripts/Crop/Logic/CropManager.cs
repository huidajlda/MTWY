using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.CropPlant 
{
    public class CropManager : Singleton<CropManager>
    {
        public CropDetails_SO cropData;//��ֲ�����������ݿ����
        public Transform cropParent;//���ӵĸ�����(�ü�������û��ô��)
        private Grid currentGrid;//��ǰ������
        private Season currentSeason;//��ǰ�ļ���
        private void OnEnable()
        {
            EventHandler.PlantSeedEvent += OnPlantSeedEvent;//ע��������ֲʵ��
            EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//�л���������¼�
            EventHandler.GameDayEvent += OnGameDayEvent;//ÿ��һ����õ��¼�
        }
        private void OnDisable()
        {
            EventHandler.PlantSeedEvent -= OnPlantSeedEvent;
            EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
        }
        //ÿ���¼����õķ���
        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;//�õ���ǰ�ļ���
        }

        //���س�����ķ���
        private void OnAfterSceneUnloadEvent()
        {
            currentGrid = FindObjectOfType<Grid>();//�õ���ǰ����������
            cropParent = GameObject.FindWithTag("CropParent").transform;//�õ����Ӹ��������
        }
        //��ֲ�¼����õķ���
        private void OnPlantSeedEvent(int ID, TileDetails tileDetails)
        {
            CropDetails currentCrop = GetCropDetails(ID);//�õ���ǰ���ӵ���ֲ��Ϣ
            //�ж���ֲ��Ϣ��Ϊ���ҵ�ǰ���ڿ���ֲ�һ�û��ֲ
            if (currentCrop != null && SeasonAvailable(currentCrop) && tileDetails.seedItemID == -1)
            {
                tileDetails.seedItemID = ID;//��ǰ��Ƭ��ֲ������ID����������ӵ�ID
                tileDetails.growthDays = 0;//�������ڴ�0��ʼ����
                //��ʾũ����
                DisplayCropPlant(tileDetails, currentCrop);
            }
            else if (tileDetails.seedItemID != -1) //������ֲ֮�󣬵�ͼˢ����ֲ�����ӻ���
            {
                //��ʾũ����
                DisplayCropPlant(tileDetails, currentCrop);
            }
        }
        //��ʾ��ͬ�׶ε�����(��Ƭ��Ϣ��������Ϣ)
        //�ǵõ���ê��λ�ã�����ͼƬ��λ�ö��Ǹ���������ģ���Ҫ�ں��ʵ�λ�����ɣ�ê��λ��ҲҪ������
        private void DisplayCropPlant(TileDetails tileDetails, CropDetails cropDetails) 
        {
            //�ɳ��׶�
            int growthStages = cropDetails.growthDays.Length;//��ȡ�����ж��ٸ������׶�
            int currentStage = 0;//��ǰ�ĳɳ��׶�
            int dayCounter = cropDetails.TotalGrowthDays;//���������׶������������ܺ�
            //������㵱ǰ�ĳɳ��׶�(��1����Ϊ��0��ʼ)
            for (int i = growthStages - 1; i >= 0; i--) 
            {
                if (tileDetails.growthDays >= dayCounter)
                {
                    currentStage = i;//��ǰ�׶�
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];//��ȥ��ǰ�׶ε�����
            }
            //��ȡ��ǰ�׶ε�Ԥ����;�����
            GameObject cropPrefab = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthSprites[currentStage];
            //����λ��(����0.5λ�þ����������½�Ϊ��㣬+0.5�;�����)
            Vector3 pos = new Vector3(tileDetails.gridX + 0.5f, tileDetails.gridY + 0.5f, 0);
            //��������
            GameObject cropInstance=Instantiate(cropPrefab,pos,Quaternion.identity,cropParent);
            //ע�⾫����Ⱦ����Ԥ��������������������
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite=cropSprite;//��ֵ����ͼ
            cropInstance.GetComponent<Crop>().cropDetails = cropDetails;//��ֲ��Ϣ
            cropInstance.GetComponent<Crop>().tileDetails = tileDetails;//��Ƭ��ͼ��Ϣ
        }
        //ͨ��ID������������ֲ��Ϣ
        public CropDetails GetCropDetails(int ID) 
        {
            return cropData.cropDetailsList.Find(c => c.seedItemID == ID);
        }
        //��ֲ���ڵ��ж�
        private bool SeasonAvailable(CropDetails crop) 
        {
            for (int i = 0; i < crop.seasons.Length; i++) //��ֲ��������һ���͵�ǰ����һ��������ֲ
            {
                if (crop.seasons[i]==currentSeason)
                    return true;
            }
            return false;
        }
    }
}
