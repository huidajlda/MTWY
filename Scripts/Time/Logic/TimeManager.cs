using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Save;

public class TimeManager : Singleton<TimeManager>,ISaveable
{
    //秒，分，时，天，月，年
    private int gameSecond, gameMinute,gameHour,gameDay,gameMonth,gameYear;
    private Season gameSeason = Season.春天;//季节的枚举(初始赋值春天)
    private int monthInSeason = 3;//3个月为一个季节(可以更改)
    public bool gameClockPause;//时间暂停
    private float tikTime;//计时器
    //灯光时间差
    private float timeDifference;
    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);//返回游戏的时间戳

    public string GUID => GetComponent<DataGUID>().guid;

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;//加载场景前的事件
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//加载场景后的事件
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;//注册更新游戏状态的事件
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;//开始新游戏的事件
        EventHandler.EndGameEvent += OnEndGameEvent;//结束游戏的事件
    }
    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }

    private void OnEndGameEvent()
    {
        gameClockPause = true;//暂停时间
    }

    private void OnStartNewGameEvent(int obj)
    {
        NewGameTime();
        gameClockPause = false;
    }

    //更新游戏状态事件调用的方法
    private void OnUpdateGameStateEvent(GameState gameState)
    {
        gameClockPause = gameState == GameState.Pause;//暂停游戏时间
    }

    private void OnAfterSceneUnloadEvent()
    {
        gameClockPause = false;//时间继续
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);//设置时间
        EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);//设置灯光
    }

    private void OnBeforeSceneUnloadEvent()
    {
        gameClockPause = true;//暂停时间
    }

    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();//添加进接口列表
        gameClockPause = true;
    }
    private void Update()
    {
        if (!gameClockPause) //时间没有暂停
        {
            tikTime += Time.deltaTime;
            if (tikTime >= Settings.secondThreshold) //计时器大于设定的1秒时间
            {
                UpdateGameTime();//就可以调用时间更新的方法了
                tikTime -= Settings.secondThreshold;//减去设定的1秒，重新开始计算时间
            }
        }
        if (Input.GetKey(KeyCode.T)) //时间加速（测试使用）
        {
            for (int i = 0; i < 60; i++) 
            {
                UpdateGameTime();
            }
        }
        if (Input.GetKeyDown(KeyCode.G)) //测试
        {
            gameDay++;
            EventHandler.CallGameDayEvent(gameDay, gameSeason);
            EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        }
    }
    //给时间初始化赋值的方法
    private void NewGameTime() 
    {//游戏时间从2023年的1月1日7时0分0秒开始
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 7;
        gameDay=1; 
        gameMonth = 1; 
        gameYear = 2023;
        gameSeason = Season.春天;
    }
    //更新时间的方法
    private void UpdateGameTime()
    {
        gameSecond++;//秒钟+1
        if ((gameSecond > Settings.secondHold)) //秒钟大于进制，分钟加1以此类推
        {
            gameMinute++;//分钟+1
            gameSecond=0;//秒钟重新开始
            if (gameMinute > Settings.minuteHold) 
            {
                gameHour++;//时钟+1
                gameMinute=0;//分钟重新开始
                if (gameHour > Settings.hourHold) 
                {
                    gameDay++;//天数+1
                    gameHour=0;//时钟重新开始
                    if (gameDay > Settings.dayHold) 
                    {
                        gameDay = 1;//天数从1开始
                        gameMonth++;//月份+1
                        if (gameMonth > 12) 
                        {
                            gameMonth = 1;
                        }
                        monthInSeason--;//季度月份减少1
                        if (monthInSeason == 0) //更换季度
                        {
                            monthInSeason = 3;//嫉妒月份更新
                            int seasonNumber = (int)gameSeason;//季节的枚举值(0~3)
                            seasonNumber++;//到下一个季节的枚举值
                            if (seasonNumber > Settings.seasonHold) 
                            {
                                seasonNumber = 0;//重新从春天开始
                                gameYear++;//年份+1
                            }
                            gameSeason = (Season)seasonNumber;//将对应的季节枚举转换回去赋值给当前季节
                            if (gameYear > 9999) //可以不写，让时间一直下去
                            {
                                gameYear = 2023;
                            }
                        }
                        //每天刷新一次地图和农作物生长的事件
                        EventHandler.CallGameDayEvent(gameDay, gameSeason);
                    }
                }
                EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);//时钟变化时
            }
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour,gameDay,gameSeason);//分钟变化时
            //切换灯光
            EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
        }
    }
    //返回切换的灯光同时计算时间差
    private LightShift GetCurrentLightShift() 
    {
        if (GameTime >= Settings.morningTime && GameTime <= Settings.nightTime) //白天的时间段
        {
            timeDifference = (float)(GameTime - Settings.morningTime).TotalMinutes;//计算时间差
            return LightShift.Morning;//返回早上的灯光
        }
        if (GameTime < Settings.morningTime || GameTime >= Settings.nightTime) //夜晚时间段
        {   //晚上的时间差可能会出现负数，所以加Abs
            timeDifference = Mathf.Abs((float)(GameTime - Settings.nightTime).TotalMinutes);//计算时间差
            return LightShift.Night;//返回晚上的灯光
        }
        return LightShift.Morning;//默认返回早上灯光
    }
    //保存数据
    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.timeDict = new Dictionary<string, int>();
        saveData.timeDict.Add("gameYear", gameYear);
        saveData.timeDict.Add("gameSeason", (int)gameSeason);
        saveData.timeDict.Add("gameMonth", gameMonth);
        saveData.timeDict.Add("gameDay", gameDay);
        saveData.timeDict.Add("gameHour", gameHour);
        saveData.timeDict.Add("gameMinute", gameMinute);
        saveData.timeDict.Add("gameSecond", gameSecond);
        return saveData;
    }
    //读取数据
    public void RestoreData(GameSaveData saveData)
    {
        gameYear = saveData.timeDict["gameYear"];
        gameSeason = (Season)saveData.timeDict["gameSeason"];
        gameMonth = saveData.timeDict["gameMonth"];
        gameDay = saveData.timeDict["gameDay"];
        gameHour = saveData.timeDict["gameHour"];
        gameMinute = saveData.timeDict["gameMinute"];
        gameSecond = saveData.timeDict["gameSecond"];
    }
}
