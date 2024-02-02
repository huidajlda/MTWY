using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private LightControl[] sceneLights;//场景灯光数组
    private LightShift currentLightShift;//当前灯光的切换
    private Season currentSeason;//当前季节
    private float timeDifference=Settings.lightChangeDuration;//事件差
    private void OnEnable()
    {
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//注册场景加载后执行的事件方法
        EventHandler.LightShiftChangeEvent += OnLightShiftChangeEvent;//注册切换灯光事件的方法
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;//开始新游戏的事件
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
        EventHandler.LightShiftChangeEvent -= OnLightShiftChangeEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    private void OnStartNewGameEvent(int obj)
    {
        currentLightShift = LightShift.Morning;
    }

    //场景加载后获取所有灯光
    private void OnAfterSceneUnloadEvent()
    {
        sceneLights = FindObjectsOfType<LightControl>();
        foreach (LightControl light in sceneLights) 
        {
            //lightcontrol改变灯光的方法
            light.ChangeLightShift(currentSeason, currentLightShift, timeDifference);
        }
    }
    //切换灯光的方法
    private void OnLightShiftChangeEvent(Season season, LightShift lightShift, float timeDifference)
    {
        //保存数据
        currentSeason = season;
        this.timeDifference = timeDifference;
        if (currentLightShift != lightShift) 
        {
            currentLightShift = lightShift;
            foreach (LightControl light in sceneLights)
            {
                //lightcontrol改变灯光的方法
                light.ChangeLightShift(currentSeason, currentLightShift, timeDifference);
            }
        }
    }
}
