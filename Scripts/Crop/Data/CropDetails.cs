using UnityEngine;
[System.Serializable]
public class CropDetails
{
    public int seedItemID;//种子的ID
    [Header("不同阶段需要成长的天数")]
    public int[] growthDays;//不同阶段所需成长时间的数组
    public int TotalGrowthDays//成长所需要的总天数
    {
        get 
        {
            int amount = 0;
            foreach (var days in growthDays) 
            {
                amount += days;
            }
            return amount;
        }
    }
    [Header("不同生长阶段的预设体")]//树木的成长可能需要
    public GameObject[] growthPrefabs;//不同阶段预设体的数组
    [Header("不同阶段的图片")]//种子的成长可能需要
    public Sprite[] growthSprites;
    [Header("可种植的季节")]
    public Season[] seasons;//可以在多个季节播种
    [Space]//检查器里的空格，隔开一点
    [Header("收割工具")]
    public int[] harvestToolItemID;//可以收割该作物的工具
    [Header("使用的工具收割时需要使用的次数")]
    public int[] requireActionCount;//次数需要和上面的工具ID一一对应
    [Header("转换的ID(物体被收割后变成的东西的ID)")]
    public int transferItemID;//作物被收割后变成的东西的ID
    [Space]
    [Header("收割果实的信息")]
    public int[] producedItemID;//收割果实的ID
    [Header("生成果实的最小最大数量")]
    public int[] producedMinAmount;//收割果实数量的最小值（要与ID一一对应）
    public int[] producedMaxAmount;//收割果实数量的最大值（要与ID一一对应）
    [Header("生成果实的范围")]
    public Vector2 spawnRadius;//生成果实的范围
    [Header("再次生长的时间")]
    public int daysToRegrow;//再次生长的时间
    [Header("可重复生长几次")]
    public int regrowTime;//可重复生长的次数
    [Header("可选择的")]
    public bool generateAtPlayerPosition;//是否在玩家身上生成
    public bool hasAimation;//有没有动画
    public bool hasParticalEffect;//有没有例子特效
    public ParticaleEffectType effectType;//粒子特效
    public Vector3 effectPos;//生成特效坐标
    public SoundName soundEffect;
    //判断当前收割工具是否可用（工具ID）
    public bool CheckToolAvailabele(int toolID) 
    {
        foreach (var tool in harvestToolItemID) //循环种子收割工具的数组
        {
            if (tool == toolID) 
            {
                return true;//此工具可用
            }
        }
        return false;//此工具不可用
    }
    //工具需要使用的次数
    public int GetTotalRequireCount(int toolID) 
    {
        for (int i = 0; i < harvestToolItemID.Length; i++) //循环工具列表
        {
            if (harvestToolItemID[i]==toolID)//在可使用的工具列表找到该工具
                return requireActionCount[i];//返回该工具需要使用的次数
        }
        return -1;//该工具不可使用
    }
}
