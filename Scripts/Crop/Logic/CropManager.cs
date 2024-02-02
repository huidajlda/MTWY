using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.CropPlant 
{
    public class CropManager : Singleton<CropManager>
    {
        public CropDetails_SO cropData;//种植种子详情数据库变量
        public Transform cropParent;//种子的父物体(让检查器层次没那么乱)
        private Grid currentGrid;//当前的网格
        private Season currentSeason;//当前的季节
        private void OnEnable()
        {
            EventHandler.PlantSeedEvent += OnPlantSeedEvent;//注册种子种植实际
            EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//切换场景后的事件
            EventHandler.GameDayEvent += OnGameDayEvent;//每个一天调用的事件
        }
        private void OnDisable()
        {
            EventHandler.PlantSeedEvent -= OnPlantSeedEvent;
            EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
        }
        //每天事件调用的方法
        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;//拿到当前的季节
        }

        //加载场景后的方法
        private void OnAfterSceneUnloadEvent()
        {
            currentGrid = FindObjectOfType<Grid>();//拿到当前场景的网格
            cropParent = GameObject.FindWithTag("CropParent").transform;//拿到种子父物体对象
        }
        //种植事件调用的方法
        private void OnPlantSeedEvent(int ID, TileDetails tileDetails)
        {
            CropDetails currentCrop = GetCropDetails(ID);//拿到当前种子的种植信息
            //判断种植信息不为空且当前季节可种植且还没种植
            if (currentCrop != null && SeasonAvailable(currentCrop) && tileDetails.seedItemID == -1)
            {
                tileDetails.seedItemID = ID;//当前瓦片种植的种子ID等于这个种子的ID
                tileDetails.growthDays = 0;//生长日期从0开始计算
                //显示农作物
                DisplayCropPlant(tileDetails, currentCrop);
            }
            else if (tileDetails.seedItemID != -1) //用于种植之后，地图刷新种植的种子还在
            {
                //显示农作物
                DisplayCropPlant(tileDetails, currentCrop);
            }
        }
        //显示不同阶段的作物(瓦片信息，种子信息)
        //记得调整锚点位置，生成图片的位置都是根据来计算的，想要在合适的位置生成，锚点位置也要调整好
        private void DisplayCropPlant(TileDetails tileDetails, CropDetails cropDetails) 
        {
            //成长阶段
            int growthStages = cropDetails.growthDays.Length;//获取种子有多少个生长阶段
            int currentStage = 0;//当前的成长阶段
            int dayCounter = cropDetails.TotalGrowthDays;//所有生长阶段所需天数的总和
            //倒叙计算当前的成长阶段(减1是因为从0开始)
            for (int i = growthStages - 1; i >= 0; i--) 
            {
                if (tileDetails.growthDays >= dayCounter)
                {
                    currentStage = i;//当前阶段
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];//减去当前阶段的天数
            }
            //获取当前阶段的预设体和精灵兔
            GameObject cropPrefab = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthSprites[currentStage];
            //生成位置(不加0.5位置就在网格左下角为起点，+0.5就居中了)
            Vector3 pos = new Vector3(tileDetails.gridX + 0.5f, tileDetails.gridY + 0.5f, 0);
            //生成作物
            GameObject cropInstance=Instantiate(cropPrefab,pos,Quaternion.identity,cropParent);
            //注意精灵渲染器在预设体中是在子物体身上
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite=cropSprite;//赋值精灵图
            cropInstance.GetComponent<Crop>().cropDetails = cropDetails;//种植信息
            cropInstance.GetComponent<Crop>().tileDetails = tileDetails;//瓦片地图信息
        }
        //通过ID来查找种子种植信息
        public CropDetails GetCropDetails(int ID) 
        {
            return cropData.cropDetailsList.Find(c => c.seedItemID == ID);
        }
        //种植季节的判断
        private bool SeasonAvailable(CropDetails crop) 
        {
            for (int i = 0; i < crop.seasons.Length; i++) //种植季节任意一个和当前季节一样即可种植
            {
                if (crop.seasons[i]==currentSeason)
                    return true;
            }
            return false;
        }
    }
}
