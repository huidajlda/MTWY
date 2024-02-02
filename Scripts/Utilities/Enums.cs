//所有枚举变量都写在这个脚本里面管理
public enum ItemType 
{   //种子，商品，家具
    Seed,Commodity,Furniture,
    //工具
    HoeTool,//挖地的工具
    ChopTool,//砍树的工具
    BreakTool,//砸石头的工具
    ReapTool,//割草的工具
    WaterTool,//浇水
    CollectTool,//收割的工具
    ReapableScenery//杂草
}
//格子的类型
public enum SlotType 
{
    Bag,//背包
    Box,//盒子
    Shop//商店
}
//背包的类型
public enum InventoryLocation 
{
    Player,//玩家身上的背包
    Box,//箱子的背包
}
//动画类型部分
public enum PartType 
{
    None,//什么也没拿（默认）
    Carry,//拿着物品（举起动画）
    Hoe,//工具
    Water,//浇水
    Collect,//菜篮子收集
    Chop,//斧头
    Break,//敲石头
    Reap//除草
}
//身体各部分的名称
public enum PartName 
{
    Body,
    Hair,
    Arm,
    Tool,
}
//季节的枚举
public enum Season 
{
    春天,夏天,秋天,冬天
}
//网格的类型
public enum GridType 
{
    Diggable,//可挖坑
    DropItem,//可丢弃
    PlaceFurniture,//可放置家具
    NPCObstacle//NPC障碍
}
//粒子特效的枚举
public enum ParticaleEffectType 
{
    None,//空
    LeaveFalling01,//第一种树的粒子特效
    LeaveFalling02,//第二种树的粒子特效
    Rock,//石头的特效
    ReapableScenery//割草的特效
}
//游戏状态
public enum GameState 
{
    GamePlay,//正常运行游戏
    Pause//暂停
}
//灯光切换
public enum LightShift 
{
    Morning,Night//早上和晚上
}
//音乐名称(直接将音乐名称作为枚举值选择)
public enum SoundName 
{
    none,FootStepSoft,FootStepHard,//空，走路的音效
    Axe,Pickaxe,Hoe,Reap,Water,Basket,Chop,//对应使用工具的音效
    Pickup,Plant,TreeFalling,Rustle,//采摘，种植，树倒，割草的音效
    AmbientCountryside1, AmbientCountryside2,//环境音
    MusicCalm1, MusicCalm2,MusicCalm3,MusicCalm4, MusicCalm5,MusicCalm6, AmbientIndoor1//背景音乐
} 