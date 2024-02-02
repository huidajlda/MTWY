using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Save;

public class TimeManager : Singleton<TimeManager>,ISaveable
{
    //�룬�֣�ʱ���죬�£���
    private int gameSecond, gameMinute,gameHour,gameDay,gameMonth,gameYear;
    private Season gameSeason = Season.����;//���ڵ�ö��(��ʼ��ֵ����)
    private int monthInSeason = 3;//3����Ϊһ������(���Ը���)
    public bool gameClockPause;//ʱ����ͣ
    private float tikTime;//��ʱ��
    //�ƹ�ʱ���
    private float timeDifference;
    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);//������Ϸ��ʱ���

    public string GUID => GetComponent<DataGUID>().guid;

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;//���س���ǰ���¼�
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//���س�������¼�
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;//ע�������Ϸ״̬���¼�
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;//��ʼ����Ϸ���¼�
        EventHandler.EndGameEvent += OnEndGameEvent;//������Ϸ���¼�
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
        gameClockPause = true;//��ͣʱ��
    }

    private void OnStartNewGameEvent(int obj)
    {
        NewGameTime();
        gameClockPause = false;
    }

    //������Ϸ״̬�¼����õķ���
    private void OnUpdateGameStateEvent(GameState gameState)
    {
        gameClockPause = gameState == GameState.Pause;//��ͣ��Ϸʱ��
    }

    private void OnAfterSceneUnloadEvent()
    {
        gameClockPause = false;//ʱ�����
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);//����ʱ��
        EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);//���õƹ�
    }

    private void OnBeforeSceneUnloadEvent()
    {
        gameClockPause = true;//��ͣʱ��
    }

    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();//��ӽ��ӿ��б�
        gameClockPause = true;
    }
    private void Update()
    {
        if (!gameClockPause) //ʱ��û����ͣ
        {
            tikTime += Time.deltaTime;
            if (tikTime >= Settings.secondThreshold) //��ʱ�������趨��1��ʱ��
            {
                UpdateGameTime();//�Ϳ��Ե���ʱ����µķ�����
                tikTime -= Settings.secondThreshold;//��ȥ�趨��1�룬���¿�ʼ����ʱ��
            }
        }
        if (Input.GetKey(KeyCode.T)) //ʱ����٣�����ʹ�ã�
        {
            for (int i = 0; i < 60; i++) 
            {
                UpdateGameTime();
            }
        }
        if (Input.GetKeyDown(KeyCode.G)) //����
        {
            gameDay++;
            EventHandler.CallGameDayEvent(gameDay, gameSeason);
            EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        }
    }
    //��ʱ���ʼ����ֵ�ķ���
    private void NewGameTime() 
    {//��Ϸʱ���2023���1��1��7ʱ0��0�뿪ʼ
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 7;
        gameDay=1; 
        gameMonth = 1; 
        gameYear = 2023;
        gameSeason = Season.����;
    }
    //����ʱ��ķ���
    private void UpdateGameTime()
    {
        gameSecond++;//����+1
        if ((gameSecond > Settings.secondHold)) //���Ӵ��ڽ��ƣ����Ӽ�1�Դ�����
        {
            gameMinute++;//����+1
            gameSecond=0;//�������¿�ʼ
            if (gameMinute > Settings.minuteHold) 
            {
                gameHour++;//ʱ��+1
                gameMinute=0;//�������¿�ʼ
                if (gameHour > Settings.hourHold) 
                {
                    gameDay++;//����+1
                    gameHour=0;//ʱ�����¿�ʼ
                    if (gameDay > Settings.dayHold) 
                    {
                        gameDay = 1;//������1��ʼ
                        gameMonth++;//�·�+1
                        if (gameMonth > 12) 
                        {
                            gameMonth = 1;
                        }
                        monthInSeason--;//�����·ݼ���1
                        if (monthInSeason == 0) //��������
                        {
                            monthInSeason = 3;//�����·ݸ���
                            int seasonNumber = (int)gameSeason;//���ڵ�ö��ֵ(0~3)
                            seasonNumber++;//����һ�����ڵ�ö��ֵ
                            if (seasonNumber > Settings.seasonHold) 
                            {
                                seasonNumber = 0;//���´Ӵ��쿪ʼ
                                gameYear++;//���+1
                            }
                            gameSeason = (Season)seasonNumber;//����Ӧ�ļ���ö��ת����ȥ��ֵ����ǰ����
                            if (gameYear > 9999) //���Բ�д����ʱ��һֱ��ȥ
                            {
                                gameYear = 2023;
                            }
                        }
                        //ÿ��ˢ��һ�ε�ͼ��ũ�����������¼�
                        EventHandler.CallGameDayEvent(gameDay, gameSeason);
                    }
                }
                EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);//ʱ�ӱ仯ʱ
            }
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour,gameDay,gameSeason);//���ӱ仯ʱ
            //�л��ƹ�
            EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
        }
    }
    //�����л��ĵƹ�ͬʱ����ʱ���
    private LightShift GetCurrentLightShift() 
    {
        if (GameTime >= Settings.morningTime && GameTime <= Settings.nightTime) //�����ʱ���
        {
            timeDifference = (float)(GameTime - Settings.morningTime).TotalMinutes;//����ʱ���
            return LightShift.Morning;//�������ϵĵƹ�
        }
        if (GameTime < Settings.morningTime || GameTime >= Settings.nightTime) //ҹ��ʱ���
        {   //���ϵ�ʱ�����ܻ���ָ��������Լ�Abs
            timeDifference = Mathf.Abs((float)(GameTime - Settings.nightTime).TotalMinutes);//����ʱ���
            return LightShift.Night;//�������ϵĵƹ�
        }
        return LightShift.Morning;//Ĭ�Ϸ������ϵƹ�
    }
    //��������
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
    //��ȡ����
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
