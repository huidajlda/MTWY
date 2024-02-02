using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TimeUI : MonoBehaviour
{
    public RectTransform dayNightImage;//日夜转换时间图片ui
    public RectTransform clockParent;//时间刻度的父物体(每个子物体表示一个时刻)
    public Image seasonImage;//季节的图片UI
    public TextMeshProUGUI dateText;//日期文本
    public TextMeshProUGUI timeText;//时间文本
    public Sprite[] seasonSprites;//季节的精灵图
    private List<GameObject> clockBlocks=new List<GameObject>();//存放时刻的列表
    private void Awake()
    {
        for (int i = 0; i < clockParent.childCount; i++) 
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);//获取每一个时刻
            clockParent.GetChild(i).gameObject.SetActive(false);//将时刻隐藏
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
    //设置时间的方法
    //设置分钟小时
    private void OnGameMinuteEvent(int minute, int hour,int day,Season season)
    {
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");//以00：00的方式显示时间
    }
    //设置年月日
    private void OnGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        dateText.text = year + "年" + month.ToString("00") + "月" + day.ToString("00") + "日";//设置年月日
        seasonImage.sprite = seasonSprites[(int)season];//切换季节的精灵图
        SwitchHourImage(hour);//切换时刻
        DayNightImageRotate(hour);//旋转日夜图片
    }
    //切换时刻图片的方法
    private void SwitchHourImage(int hour) 
    {
        int index = hour / 4;//有6张表示时刻的图片，每4小时却换一张
        if (index == 0) 
        {
            foreach (var item in clockBlocks) 
            {
                item.SetActive(false);//将所有时刻图片隐藏
            }
        }
        else 
        {
            for (int i = 0; i < clockBlocks.Count; i++) 
            {
                if (i < index+1)
                    clockBlocks[i].SetActive(true);//显示时刻
                else
                    clockBlocks[i].SetActive(false);//隐藏时刻
            }
        }
    }
    //旋转日月图片的方法
    private void DayNightImageRotate(int hour) 
    {
        var target = new Vector3(0, 0, hour * 15-90);//360/24=15,每小时转15度，-90是-90度时是图片黑夜，从黑夜开始旋转
        //DORotate(旋转到的度数,需要的时间,模式一般用Fast)
        dayNightImage.DORotate(target, 1f, RotateMode.Fast);
    }
}
