using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    //ItemFader��ʹ�õı���
    public const float fabeDuration= 0.35f;//��Ʒ��ɫ�仯��ʱ��
    public const float targetAlpha = 0.45f;//��ɫ�仯����͸����

    //ʱ����ص���ֵ��������
    public const float secondThreshold = 0.01f;//������ʱ�䵽����ʱ�ͼ�1s����ֵԽСʱ��Խ�죩
    public const int secondHold=59;//���ӵĽ���
    public const int minuteHold = 59;//���ӵĽ���
    public const int hourHold = 23;//ʱ�ӵĽ���
    public const int dayHold = 10;//��Ľ���(10��Ϊ1��)
    public const int seasonHold = 3;//ÿ��3������Ϊ1�꣨0~3��
    //�����л��ı���
    public const float sceneFadeDuration = 0.8f;//�������뵭���仯��ʱ��
    //ÿ�θ��������ٵ�����
    public const int reapAmount = 2;
    //NPC�����ƶ���С
    public const float gridCellSize = 1;//�����ƶ�1��
    public const float gridCellDiagonalSize = 1.41f;//б�����ƶ�����
    public const int maxGridSize = 9999;//�������ߴ�(���ж��õ�)
    //����ƹ��ʱ���
    public static TimeSpan morningTime = new TimeSpan(5, 0, 0);//����5��(����)
    public static TimeSpan nightTime = new TimeSpan(19, 0, 0);//�������(���)
    public const float lightChangeDuration=25f;//�л��ƹ��ʱ��25s
    public static Vector3 PlayerStartPos=new Vector3(14,-6,0);//��ҳ�ʼ����λ��
    public const int playerStartMoney = 100;//��ҳ�ʼ��Ǯ
}
