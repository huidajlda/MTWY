using System;
using UnityEngine;
using UnityEngine.Playables;
using MFarm.Save;
public class TimelineManager : Singleton<TimelineManager>,ISaveable
{
    public PlayableDirector startDirector;//��ʼ��Ϸ��PlayableDirector
    private PlayableDirector currentDirector;//��ǰ���ڲ��ŵ�PlayableDirector
    private bool isDone;//�Ի�Ƭ���Ƿ񲥷����
    public bool IsDone { set => isDone = value; }

    public string GUID => GetComponent<DataGUID>().guid;

    private bool isPause;//�Ƿ�����ͣ
    private bool isFirst=true;//��һ�β���
    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;//һ��ʱ��Ϸ��Ĭ��ΪstartDirector
        //�л�������Ȼ���ж����ٻ�ȥ��ǰ�����ľͿ��ԣ�����Ͳ�����
    }
    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }
    private void OnEnable()
    {
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//���س�������õķ������¼�
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
    }
    private void Update()
    {
        if (isPause && Input.GetKeyDown(KeyCode.Space)&&isDone) //Timeline��ͣ���Ұ��¿ո���
        {   //��������Timeline
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);//��������
        }
    }
    //ֹͣ����Timeline
    public void PauseTimeline(PlayableDirector director) 
    {
        currentDirector = director;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);//��ͣ���ŵ�ǰdirector
        isPause = true;
    }
    //���س�������õķ���
    private void OnAfterSceneUnloadEvent()
    {
        currentDirector=FindObjectOfType<PlayableDirector>();//�ҵ���ǰ������Timeline
        if (currentDirector != null && isFirst) 
        {
            currentDirector.Play();//����
            isFirst = false;
        }
    }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData savedata = new GameSaveData();
        savedata.isFirst =this.isFirst; 
        return savedata;
    }

    public void RestoreData(GameSaveData saveData)
    {
        this.isFirst = saveData.isFirst;
    }
}
