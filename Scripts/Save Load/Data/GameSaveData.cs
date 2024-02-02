using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Save 
{   //保存的数据
    [System.Serializable]
    public class GameSaveData
    {
        public string dataSceneName;//游戏场景名称
        //保存人物坐标的字典，string是人物名称
        public Dictionary<string, SerializableVector3> characterPosDict;
        //保存当前场景所有物体
        public Dictionary<string, List<SceneItem>> sceneItemDict;
        //保存当前场景的家具
        public Dictionary<string, List<SceneFurniture>> sceneFurnitureDict;
        //场景瓦片信息
        public Dictionary<string, TileDetails> tileDetailsDict;
        //场景是否第一次加载
        public Dictionary<string, bool> firstLoadDict;
        //背包数据
        public Dictionary<string, List<InventoryItem>> inventoryDict;
        //时间数据
        public Dictionary<string, int> timeDict;
        //玩家金钱
        public int playerMoney;
        //NPC的数据
        public string targetScene;//行程的目标场景
        public bool interactable;//是否可以互动
        public int animationInstanceID;//动画
        //过场动画
        public bool isFirst;//是否第一次播放
    }
}
