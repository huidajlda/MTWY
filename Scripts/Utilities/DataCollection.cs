using System.Collections.Generic;
using UnityEngine;
//所有自己写的类或结构体都写在这个脚本里面，方便管理，修改，查找
[System.Serializable]
//物品信息类
public class ItemDetails
{
    public int itemId;//物品ID
    public string itemName;//物品名称
    public ItemType itemType;//物品的类型
    public Sprite itemIcon;//物品的精灵图
    public Sprite itemOnWorldSprite;//在地图上产生时显示的图片
    public string itemDescription;//物品详情
    public int itemUseRadius;//可使用的范围
    public bool canPickedup;//是否可以拾取
    public bool canDropped;//是否可以丢弃
    public bool canCarried;//是否可以举起
    public int itemPrice;//物品的购买时的价值
    [Range(0,1)]
    public float sellPercentage;//物品出售时价值所打的折扣百分比
}
//背包里存放的物品数据的结构体
//class的话在判断是需要判断背包位置是否为空
[System.Serializable]
public struct InventoryItem 
{
    public int itemID;//物品的ID
    public int itemAmount;//物品的数量
}
//播放动画类型的类
[System.Serializable]
public class AnimatorType 
{
    public PartType partType;//类型
    public PartName partName;//名称
    public AnimatorOverrideController OverrideController;//动画控制器
}
//将坐标序列化的类
[System.Serializable]
public class SerializableVector3 
{
    public float x,y,z;//Vector3序列化后才能保存
    public SerializableVector3(Vector3 pos) 
    {
        this.x=pos.x; this.y =pos.y; this.z=pos.z;
    }
    public Vector3 ToVector3() 
    {
        return new Vector3(x,y,z);
    }
    public Vector2Int ToVector2Int() 
    {
        return new Vector2Int((int)x, (int)y);
    }
}
//场景里面的物品类
public class SceneItem
{
    public int itemID;//物品ID
    public SerializableVector3 position;//物品的序列化坐标
}
//场景里面的建造物品类
[System.Serializable]
public class SceneFurniture 
{
    public int itemID;//物品ID
    public SerializableVector3 position;//物品的序列化坐标
    public int boxIndex;//盒子序号
}
[System.Serializable]
//瓦片格子的属性
public class TileProperty 
{
    public Vector2Int tileCordinate;//瓦片坐标
    public GridType gridType;//网格类型
    public bool boolTypeValue;//是否被标记
}

//瓦片的详细信息类
public class TileDetails 
{
    public int gridX, gridY;//瓦片的XY坐标
    public bool canDig;//是否可以挖坑
    public bool canDropItem;//是否可以丢弃
    public bool canPlaceFurniture;//是否可以放置
    public bool isNPCObstacle;//是否是NPC的障碍
    public int daysSinceDug = -1;//这个坑被挖了多少天了
    public int daysSinceWatered = -1;//浇水多少天了
    public int seedItemID = -1;//种植种子的ID
    public int growthDays= -1;//种子成长多少天了
    public int daysSinceLastHarvest = -1;//离上一次收获过了几天(有些种子可以反复收获)
}
//NPC位置类
[System.Serializable]
public class NPCPosition 
{
    public Transform npc;
    public string startScene;//开始的场景
    public Vector3 position;//坐标
}
//两个场景间路径的集合类

[System.Serializable]
public class SceneRoute 
{
    public string fromSceneName;//从什么场景
    public string gotoSceneName;//到什么场景
    public List<ScenePath> scenePathList;//路径列表
}
//场景路径类
[System.Serializable]
public class ScenePath
{
    public string sceneName;//场景的名称
    public Vector2Int fromGridCell;//来的坐标
    public Vector2Int gotoGridCell;//去的坐标
}