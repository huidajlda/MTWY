using System;
using UnityEngine;
using UnityEngine.Playables;
using MFarm.Save;
public class TimelineManager : Singleton<TimelineManager>,ISaveable
{
    public PlayableDirector startDirector;//初始游戏的PlayableDirector
    private PlayableDirector currentDirector;//当前正在播放的PlayableDirector
    private bool isDone;//对话片段是否播放完成
    public bool IsDone { set => isDone = value; }

    public string GUID => GetComponent<DataGUID>().guid;

    private bool isPause;//是否在暂停
    private bool isFirst=true;//第一次播放
    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;//一开时游戏，默认为startDirector
        //切换场景后，然后有动画再回去当前场景的就可以，这里就不做了
    }
    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }
    private void OnEnable()
    {
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//加载场景后调用的方法的事件
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
    }
    private void Update()
    {
        if (isPause && Input.GetKeyDown(KeyCode.Space)&&isDone) //Timeline暂停了且按下空格了
        {   //继续播放Timeline
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);//继续播放
        }
    }
    //停止播放Timeline
    public void PauseTimeline(PlayableDirector director) 
    {
        currentDirector = director;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);//暂停播放当前director
        isPause = true;
    }
    //加载场景后调用的方法
    private void OnAfterSceneUnloadEvent()
    {
        currentDirector=FindObjectOfType<PlayableDirector>();//找到当前场景的Timeline
        if (currentDirector != null && isFirst) 
        {
            currentDirector.Play();//播放
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
