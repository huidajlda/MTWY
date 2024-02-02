using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.AStar 
{
    public class GridNodes
    {
        private int width;//�����ͼ��
        private int height;//�����ͼ��
        private Node[,] gridNode;//�洢�ڵ������
        //��ʼ���ڵ㷶Χ����
        public GridNodes(int width, int height)
        {
            this.width = width;
            this.height = height;
            gridNode = new Node[width, height];//��������
            //���챣��ÿ���ڵ�
            for (int x=0; x < width; x++) 
            {
                for (int y = 0; y < height; y++) 
                {
                    gridNode[x,y]=new Node(new Vector2Int(x,y));
                }
            }
        }
        //�������귵�ظ�λ�õĽڵ���Ϣ
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