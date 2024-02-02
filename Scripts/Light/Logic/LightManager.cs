using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private LightControl[] sceneLights;//�����ƹ�����
    private LightShift currentLightShift;//��ǰ�ƹ���л�
    private Season currentSeason;//��ǰ����
    private float timeDifference=Settings.lightChangeDuration;//�¼���
    private void OnEnable()
    {
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//ע�᳡�����غ�ִ�е��¼�����
        EventHandler.LightShiftChangeEvent += OnLightShiftChangeEvent;//ע���л��ƹ��¼��ķ���
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;//��ʼ����Ϸ���¼�
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

    //�������غ��ȡ���еƹ�
    private void OnAfterSceneUnloadEvent()
    {
        sceneLights = FindObjectsOfType<LightControl>();
        foreach (LightControl light in sceneLights) 
        {
            //lightcontrol�ı�ƹ�ķ���
            light.ChangeLightShift(currentSeason, currentLightShift, timeDifference);
        }
    }
    //�л��ƹ�ķ���
    private void OnLightShiftChangeEvent(Season season, LightShift lightShift, float timeDifference)
    {
        //��������
        currentSeason = season;
        this.timeDifference = timeDifference;
        if (currentLightShift != lightShift) 
        {
            currentLightShift = lightShift;
            foreach (LightControl light in sceneLights)
            {
                //lightcontrol�ı�ƹ�ķ���
                light.ChangeLightShift(currentSeason, currentLightShift, timeDifference);
            }
        }
    }
}
