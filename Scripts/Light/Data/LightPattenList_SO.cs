using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LightPattenList_SO", menuName ="Light/Light Patten")]
public class LightPattenList_SO : ScriptableObject
{
    public List<LightDetails> lightPattenList;//灯光详情的列表
    //通过季节和早晚来返回灯光详情
    public LightDetails GetLightDetails(Season season, LightShift lightShift) 
    {
        return lightPattenList.Find(l => l.season == season && l.lightShift == lightShift);
    }
}
[System.Serializable]
//灯光详情
public class LightDetails 
{
    public Season season;//季节
    public LightShift lightShift;//灯光随时间切换
    public Color lightColor;//颜色
    public float lightAmount;//强度
}
