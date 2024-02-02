using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
//对话片段(要添加进Timeline轨道)
public class DialogueClip : PlayableAsset,ITimelineClipAsset
{
    //ClipCaps返回描述，内容为实现此接口可播放的剪辑（必须实现才可以在Timeline播放）
    public ClipCaps clipCaps=> ClipCaps.None;//初始返回空
    public DialogueBehaviour dialogue=new DialogueBehaviour();
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        //在playable里创建一个可编辑片段（以dialogue为模板）
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph, dialogue);
        return playable;
    }

}
