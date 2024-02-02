using System;
using UnityEngine;
namespace MFarm.AStar 
{//A*算法中的节点类
    public class Node : IComparable<Node>//这个是System里面专门用来比较的接口
    {
        public Vector2Int gridPosition;//节点的网格坐标
        public int gCost=0;//距离起点格子的距离
        public int hCost=0;//距离目标格子的距离
        public int FCost => gCost + hCost;//该格子的权重（值）
        public bool isObstacle=false;//当前格子是否是障碍
        public Node parentNode;//父节点
        //构造函数
        public Node(Vector2Int pos) 
        {
            gridPosition = pos;
            parentNode = null;//一开始没有父节点，只有选中后才会具有
        }
        //比较接口的实现（大于返回1，等于返回0，小于返回-1）
        //比较两个格子的方法
        public int CompareTo(Node other)
        {
            //比较选出最低的FCost的格子
            int result = FCost.CompareTo(other.FCost);
            if (result == 0) //当FCost相同时
            {
                result=hCost.CompareTo(other.hCost);//选离终点近的格子
            }
            return result;//返回两个格子的比较结果
        }
    }
}