using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using MFarm.Dialogue;
//创建自定义的Timeline的Track就需要继承PlayableBehaviour
[System.Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    private PlayableDirector director;//拿到PlayableDirector组件
    public DialoguePiece dialoguePiece;//对话详情
    public override void OnPlayableCreate(Playable playable)
    {
        //通过当前播放的动画反向拿到PlayableDirector
        director = playable.GetGraph().GetResolver() as PlayableDirector;
    }
    //一旦开始播放对话片段，就会调用这个方法
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //呼叫启动对话UI
        EventHandler.CallShowDialogueEvent(dialoguePiece);
        if (Application.isPlaying) //正在播放的情况下
        {
            if (dialoguePiece.hasToPause) //对话详情的暂停被勾上了(有下一条对话)
            {
                //暂停Timeline
                TimelineManager.Instance.PauseTimeline(director);
            }
            else 
            {
                EventHandler.CallShowDialogueEvent(null);//关闭对话
            }
        }
    }
    //在Timeline播放期间每帧执行
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (Application.isPlaying)
        {
            TimelineManager.Instance.IsDone = dialoguePiece.isDone;//是否播放完对话
        }
    }
    //在对话片段播放完后执行
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        EventHandler.CallShowDialogueEvent(null);//关闭对话UI
    }
    //开始播放时执行
    public override void OnGraphStart(Playable playable)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.Pause);//暂停移动和时间
    }
    //结束时执行
    public override void OnGraphStop(Playable playable)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);//恢复移动和时间
    }
}
