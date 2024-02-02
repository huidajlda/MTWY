using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
//�Ի�Ƭ��(Ҫ��ӽ�Timeline���)
public class DialogueClip : PlayableAsset,ITimelineClipAsset
{
    //ClipCaps��������������Ϊʵ�ִ˽ӿڿɲ��ŵļ���������ʵ�ֲſ�����Timeline���ţ�
    public ClipCaps clipCaps=> ClipCaps.None;//��ʼ���ؿ�
    public DialogueBehaviour dialogue=new DialogueBehaviour();
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        //��playable�ﴴ��һ���ɱ༭Ƭ�Σ���dialogueΪģ�壩
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph, dialogue);
        return playable;
    }

}
