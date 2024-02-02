using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.AStar 
{
    public class GridNodes
    {
        private int width;//网格地图宽
        private int height;//网格地图高
        private Node[,] gridNode;//存储节点的数组
        //初始化节点范围数据
        public GridNodes(int width, int height)
        {
            this.width = width;
            this.height = height;
            gridNode = new Node[width, height];//创建数组
            //构造保存每个节点
            for (int x=0; x < width; x++) 
            {
                for (int y = 0; y < height; y++) 
                {
                    gridNode[x,y]=new Node(new Vector2Int(x,y));
                }
            }
        }
        //根据坐标返回该位置的节点信息
        public Node GetGridNode(int xPos, int yPos) 
        {
            if (xPos < width && yPos < height) 
            {
                return gridNode[xPos, yPos];
            }
            return null;
        }
    }
}