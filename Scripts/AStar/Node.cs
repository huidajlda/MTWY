using System;
using UnityEngine;
namespace MFarm.AStar 
{//A*�㷨�еĽڵ���
    public class Node : IComparable<Node>//�����System����ר�������ȽϵĽӿ�
    {
        public Vector2Int gridPosition;//�ڵ����������
        public int gCost=0;//���������ӵľ���
        public int hCost=0;//����Ŀ����ӵľ���
        public int FCost => gCost + hCost;//�ø��ӵ�Ȩ�أ�ֵ��
        public bool isObstacle=false;//��ǰ�����Ƿ����ϰ�
        public Node parentNode;//���ڵ�
        //���캯��
        public Node(Vector2Int pos) 
        {
            gridPosition = pos;
            parentNode = null;//һ��ʼû�и��ڵ㣬ֻ��ѡ�к�Ż����
        }
        //�ȽϽӿڵ�ʵ�֣����ڷ���1�����ڷ���0��С�ڷ���-1��
        //�Ƚ��������ӵķ���
        public int CompareTo(Node other)
        {
            //�Ƚ�ѡ����͵�FCost�ĸ���
            int result = FCost.CompareTo(other.FCost);
            if (result == 0) //��FCost��ͬʱ
            {
                result=hCost.CompareTo(other.hCost);//ѡ���յ���ĸ���
            }
            return result;//�����������ӵıȽϽ��
        }
    }
}