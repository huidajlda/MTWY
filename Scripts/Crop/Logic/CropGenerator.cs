using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Map;
namespace MFarm.CropPlant 
{
    //预先加载到地图上的农作物的脚本
    public class CropGenerator : MonoBehaviour
    {
        private Grid currentGrid;//当前网格信息
        public int seedItemID;//种子ID
        public int growthDays;//成长到什么阶段
        private void Awake()
        {
            currentGrid=FindObjectOfType<Grid>();//拿到网格信息
        }
        private void OnEnable()
        {
            EventHandler.GenerateCropEvent+= GenerateCrop;//场景加载后地图上生成初始农作物的事件
        }
        private void OnDisable()
        {
            EventHandler.GenerateCropEvent -= GenerateCrop;
        }
        //生成农作物
        private void GenerateCrop() 
        {
            Vector3Int cropGridPos = currentGrid.WorldToCell(transform.position);//拿到当前的网格坐标
            //更新地图内容
            if (seedItemID != 0) 
            {
                var tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(cropGridPos);//能达到瓦片信息
                if (tile == null) 
                {
                    tile = new TileDetails();//生成一个信息瓦片信息
                    tile.gridX = cropGridPos.x;
                    tile.gridY = cropGridPos.y;
                }
                //设置瓦片信息
                tile.daysSinceWatered = -1;
                tile.seedItemID = seedItemID;
                tile.growthDays = growthDays;
                GridMapManager.Instance.UpdateTileDetails(tile);//刷新地图
            }
        }
    }
}
