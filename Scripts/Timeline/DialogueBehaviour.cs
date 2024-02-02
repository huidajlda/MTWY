using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using MFarm.Dialogue;
//�����Զ����Timeline��Track����Ҫ�̳�PlayableBehaviour
[System.Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    private PlayableDirector director;//�õ�PlayableDirector���
    public DialoguePiece dialoguePiece;//�Ի�����
    public override void OnPlayableCreate(Playable playable)
    {
        //ͨ����ǰ���ŵĶ��������õ�PlayableDirector
        director = playable.GetGraph().GetResolver() as PlayableDirector;
    }
    //һ����ʼ���ŶԻ�Ƭ�Σ��ͻ�����������
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //���������Ի�UI
        EventHandler.CallShowDialogueEvent(dialoguePiece);
        if (Application.isPlaying) //���ڲ��ŵ������
        {
            if (dialoguePiece.hasToPause) //�Ի��������ͣ��������(����һ���Ի�)
            {
                //��ͣTimeline
                TimelineManager.Instance.PauseTimeline(director);
            }
            else 
            {
                EventHandler.CallShowDialogueEvent(null);//�رնԻ�
            }
        }
    }
    //��Timeline�����ڼ�ÿִ֡��
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (Application.isPlaying)
        {
            TimelineManager.Instance.IsDone = dialoguePiece.isDone;//�Ƿ񲥷���Ի�
        }
    }
    //�ڶԻ�Ƭ�β������ִ��
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        EventHandler.CallShowDialogueEvent(null);//�رնԻ�UI
    }
    //��ʼ����ʱִ��
    public override void OnGraphStart(Playable playable)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.Pause);//��ͣ�ƶ���ʱ��
    }
    //����ʱִ��
    public override void OnGraphStop(Playable playable)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);//�ָ��ƶ���ʱ��
    }
}
