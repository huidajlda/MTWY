using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ScheduleDetails_SO",menuName ="NPC Schedule/ScheduleDataList")]
public class ScheduleDetails_SO : ScriptableObject
{
    public List<ScheduleDetails> scheduleList;//行程列表
}
