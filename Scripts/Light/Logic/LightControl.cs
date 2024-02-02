using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class LightControl : MonoBehaviour
{
    public LightPattenList_SO lightData;//灯光数据
    private Light2D currentLight;//当前灯光
    private LightDetails currentLightDetails;//当前灯光详情
    private void Awake()
    {
        currentLight = GetComponent<Light2D>();//获取灯光组件
    }
    //实际切换灯光
    public void ChangeLightShift(Season season, LightShift lightShift, float timeDifference) 
    {
        currentLightDetails = lightData.GetLightDetails(season, lightShift);//获取灯光详情
        if (timeDifference < Settings.lightChangeDuration) //时间差小于灯光切换时间
        {
            //计算灯光颜色的差值
            var colorOffst = (currentLightDetails.lightColor - currentLight.color)/Settings.lightChangeDuration*timeDifference;
            currentLight.color += colorOffst;//加上颜色差值
            //参数(要设置改变的内容,要设置成的颜色,目标值，时间)
            //设置灯光颜色
            DOTween.To(() => currentLight.color, c => currentLight.color = c, currentLightDetails.lightColor, Settings.lightChangeDuration - timeDifference);
            //设置灯光强度
            DOTween.To(() => currentLight.intensity, i => currentLight.intensity = i, currentLightDetails.lightAmount, Settings.lightChangeDuration - timeDifference);
        }
        if (timeDifference > Settings.lightChangeDuration) //时间差大于灯光切换时间
        {   //直接赋值，不变化了
            currentLight.color = currentLightDetails.lightColor;
            currentLight.intensity = currentLightDetails.lightAmount;
        }
    }
}
