using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.CropPlant 
{
    public class ReapItem : MonoBehaviour
    {
        private CropDetails cropDetails;//杂草生长的相关信息
        private Transform PlayerTransform => FindObjectOfType<Player>().transform;
        public void InitCropData(int ID) 
        {
            cropDetails=CropManager.Instance.GetCropDetails(ID);
        }
        //收割杂草
        public void SpawnHarvestItems()
        {
            //生成农作物的循环
            for (int i = 0; i < cropDetails.producedItemID.Length; i++) //循环该作物可生成物品的列表
            {
                int amountToProduce;//生成作物的数量
                if (cropDetails.producedMaxAmount[i] == cropDetails.producedMinAmount[i]) //如果生成的最大最小相等
                {//代表该作物只生成指定数量
                    amountToProduce = cropDetails.producedMaxAmount[i];
                }
                else //生成最大最小范围内的随机数量
                {
                    amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i] + 1);
                }
                //执行生成指定数量的物品
                for (int j = 0; j < amountToProduce; j++)
                {
                    if (cropDetails.generateAtPlayerPosition)//是否在玩家背包里位置生成
                        EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                    else//在世界地图生成
                    {
                        //判断是在左侧还是右侧（生成物品的方向）
                        var dirX = transform.position.x > PlayerTransform.position.x ? 1 : -1;
                        //在地图上生成物品的位置（一定范围内随机）
                        var spawnPos = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                                transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 0);
                        EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);//生成物品
                    }
                }
            }
        }
    }
}
