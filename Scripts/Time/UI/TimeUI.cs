using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TimeUI : MonoBehaviour
{
    public RectTransform dayNightImage;//��ҹת��ʱ��ͼƬui
    public RectTransform clockParent;//ʱ��̶ȵĸ�����(ÿ���������ʾһ��ʱ��)
    public Image seasonImage;//���ڵ�ͼƬUI
    public TextMeshProUGUI dateText;//�����ı�
    public TextMeshProUGUI timeText;//ʱ���ı�
    public Sprite[] seasonSprites;//���ڵľ���ͼ
    private List<GameObject> clockBlocks=new List<GameObject>();//���ʱ�̵��б�
    private void Awake()
    {
        for (int i = 0; i < clockParent.childCount; i++) 
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);//��ȡÿһ��ʱ��
            clockParent.GetChild(i).gameObject.SetActive(false);//��ʱ������
        }
    }
    private void OnEnable()
    {
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
        EventHandler.GameDateEvent += OnGameDateEvent;
    }
    private void OnDisable()
    {
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        EventHandler.GameDateEvent -= OnGameDateEvent;
    }
    //����ʱ��ķ���
    //���÷���Сʱ
    private void OnGameMinuteEvent(int minute, int hour,int day,Season season)
    {
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");//��00��00�ķ�ʽ��ʾʱ��
    }
    //����������
    private void OnGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        dateText.text = year + "��" + month.ToString("00") + "��" + day.ToString("00") + "��";//����������
        seasonImage.sprite = seasonSprites[(int)season];//�л����ڵľ���ͼ
        SwitchHourImage(hour);//�л�ʱ��
        DayNightImageRotate(hour);//��ת��ҹͼƬ
    }
    //�л�ʱ��ͼƬ�ķ���
    private void SwitchHourImage(int hour) 
    {
        int index = hour / 4;//��6�ű�ʾʱ�̵�ͼƬ��ÿ4Сʱȴ��һ��
        if (index == 0) 
        {
            foreach (var item in clockBlocks) 
            {
                item.SetActive(false);//������ʱ��ͼƬ����
            }
        }
        else 
        {
            for (int i = 0; i < clockBlocks.Count; i++) 
            {
                if (i < index+1)
                    clockBlocks[i].SetActive(true);//��ʾʱ��
                else
                    clockBlocks[i].SetActive(false);//����ʱ��
            }
        }
    }
    //��ת����ͼƬ�ķ���
    private void DayNightImageRotate(int hour) 
    {
        var target = new Vector3(0, 0, hour * 15-90);//360/24=15,ÿСʱת15�ȣ�-90��-90��ʱ��ͼƬ��ҹ���Ӻ�ҹ��ʼ��ת
        //DORotate(��ת���Ķ���,��Ҫ��ʱ��,ģʽһ����Fast)
        dayNightImage.DORotate(target, 1f, RotateMode.Fast);
    }
}
