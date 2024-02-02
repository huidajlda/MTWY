using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class LightControl : MonoBehaviour
{
    public LightPattenList_SO lightData;//�ƹ�����
    private Light2D currentLight;//��ǰ�ƹ�
    private LightDetails currentLightDetails;//��ǰ�ƹ�����
    private void Awake()
    {
        currentLight = GetComponent<Light2D>();//��ȡ�ƹ����
    }
    //ʵ���л��ƹ�
    public void ChangeLightShift(Season season, LightShift lightShift, float timeDifference) 
    {
        currentLightDetails = lightData.GetLightDetails(season, lightShift);//��ȡ�ƹ�����
        if (timeDifference < Settings.lightChangeDuration) //ʱ���С�ڵƹ��л�ʱ��
        {
            //����ƹ���ɫ�Ĳ�ֵ
            var colorOffst = (currentLightDetails.lightColor - currentLight.color)/Settings.lightChangeDuration*timeDifference;
            currentLight.color += colorOffst;//������ɫ��ֵ
            //����(Ҫ���øı������,Ҫ���óɵ���ɫ,Ŀ��ֵ��ʱ��)
            //���õƹ���ɫ
            DOTween.To(() => currentLight.color, c => currentLight.color = c, currentLightDetails.lightColor, Settings.lightChangeDuration - timeDifference);
            //���õƹ�ǿ��
            DOTween.To(() => currentLight.intensity, i => currentLight.intensity = i, currentLightDetails.lightAmount, Settings.lightChangeDuration - timeDifference);
        }
        if (timeDifference > Settings.lightChangeDuration) //ʱ�����ڵƹ��л�ʱ��
        {   //ֱ�Ӹ�ֵ�����仯��
            currentLight.color = currentLightDetails.lightColor;
            currentLight.intensity = currentLightDetails.lightAmount;
        }
    }
}
