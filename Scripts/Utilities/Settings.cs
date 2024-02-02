using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    //ItemFader中使用的变量
    public const float fabeDuration= 0.35f;//物品颜色变化的时间
    public const float targetAlpha = 0.45f;//颜色变化到的透明度

    //时间相关的阈值变量设置
    public const float secondThreshold = 0.01f;//当运行时间到这里时就加1s（数值越小时间越快）
    public const int secondHold=59;//秒钟的进制
    public const int minuteHold = 59;//分钟的进制
    public const int hourHold = 23;//时钟的进制
    public const int dayHold = 10;//天的进制(10天为1月)
    public const int seasonHold = 3;//每过3个季节为1年（0~3）
    //场景切换的变量
    public const float sceneFadeDuration = 0.8f;//场景淡入淡出变化的时间
    //每次割草最多销毁的数量
    public const int reapAmount = 2;
    //NPC网格移动大小
    public const float gridCellSize = 1;//正常移动1格
    public const float gridCellDiagonalSize = 1.41f;//斜方向移动距离
    public const int maxGridSize = 9999;//最大网格尺寸(做判断用的)
    //早晚灯光的时间戳
    public static TimeSpan morningTime = new TimeSpan(5, 0, 0);//早上5点(天亮)
    public static TimeSpan nightTime = new TimeSpan(19, 0, 0);//晚上起点(天黑)
    public const float lightChangeDuration=25f;//切换灯光的时间25s
    public static Vector3 PlayerStartPos=new Vector3(14,-6,0);//玩家初始坐标位置
    public const int playerStartMoney = 100;//玩家初始金钱
}
