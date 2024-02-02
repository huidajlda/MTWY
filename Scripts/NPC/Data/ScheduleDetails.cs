using System;
using UnityEngine;
[Serializable]
public class ScheduleDetails:IComparable<ScheduleDetails>
{
    public int hour, minute, day;//时间点
    public int priority;//优先级,越小越先执行
    public Season season;//季节
    public string targetScene;//场景
    public Vector2Int targetGridPosition;//坐标
    public AnimationClip clipAtStop;//播放的动画片段
    public bool interactable;//是否可以和玩家对话
    public int Time => (hour * 100) + minute;
    //构造函数
    public ScheduleDetails(int hour, int minute, int day, int priority, Season season, string targetScene, Vector2Int targetGridPosition, AnimationClip clipAtStop, bool interactable)
    {
        this.hour = hour;
        this.minute = minute;
        this.day = day;
        this.priority = priority;
        this.season = season;
        this.targetScene = targetScene;
        this.targetGridPosition = targetGridPosition;
        this.clipAtStop = clipAtStop;
        this.interactable = interactable;
    }
    //比较的方法
    public int CompareTo(ScheduleDetails other)
    {
        if (Time == other.Time)
        {
            if (priority > other.priority)
                return 1;
            else
                return -1;
        }
        else if (Time > other.Time)
        {
            return 1;
        }
        else if (Time < other.Time) 
        {
            return -1;
        }
        return 0;
    }
}
