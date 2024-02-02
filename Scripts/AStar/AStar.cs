using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Map;

namespace MFarm.AStar 
{
    public class AStar : Singleton<AStar>
    {
        private GridNodes gridNodes;//ÿһ�ŵ�ͼ������ڵ���Ϣ
        private Node startNode;//���
        private Node targetNode;//Ŀ��㣨�յ㣩��Щ����������ʱ�洢�õ�
        private int gridWidth;//�����
        private int gridHeight;//�����
        private int originX;//���½�ԭ��x����
        private int originY;//���½�ԭ��y����
        private List<Node> openNodeList;//��ǰѡ�е�Node�ڵ���Χ��8���ڵ�
        private HashSet<Node> closeNodeList;//���б�ѡ�е���б�hashset��֤�����ظ������Ҳ���ʱ��List�죩
        private bool pathFound;//�Ƿ��ҵ�·��
        //����·��,����Stack��ÿһ��
        public void BuildPath(string sceneName,Vector2Int startPos,Vector2Int endPos,Stack<MovementStep> npcMovementStack) 
        {
            pathFound = false;
            if (GenerateGridNodes(sceneName, startPos, endPos)) //������Ϣ��������
            {
                //�������·��
                if (FindShortestPath()) 
                {
                    //����Npc���ƶ�·��
                    UpdatePathOnMovementStepStack(sceneName, npcMovementStack);
                }
            }
        }
        //��������ڵ���Ϣ,��ʼ���б�
        private bool GenerateGridNodes(string sceneName,Vector2Int startPos,Vector2Int endPos) 
        {//�ж���û�иõ�ͼ������Χ��Ϣ
            if (GridMapManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin)) 
            {//��ʼ������Χ����
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);//��������Χ�Ľڵ�����
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;
                openNodeList = new List<Node>();
                closeNodeList = new HashSet<Node>();
            }
            else
                return false;
            //gridNodes�ķ�Χ�Ǵ�0��0��ʼ��������Ҫ��ȥԭ������õ�ʵ��λ��
            startNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);//��ʼ�ڵ�
            targetNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);//Ŀ���
            for (int x = 0; x < gridWidth; x++) 
            {
                for (int y = 0; y < gridHeight; y++) 
                {
                    Vector3Int tilePos=new Vector3Int(x + originX, y + originY,0);//����λ�þ�Ҫ��������
                    var key = tilePos.x + "x" + tilePos.y + "y" + sceneName;//����key
                    TileDetails tile=GridMapManager.Instance.GetTileDetails(key);//�õ���Ƭ��Ϣ
                    if (tile != null) 
                    {
                        Node node = gridNodes.GetGridNode(x, y);//�õ���λ�õĽڵ���Ϣ
                        if(tile.isNPCObstacle)
                            node.isObstacle = true;
                    }
                }
            }
            return true;
        }
        //�������·��
        private bool FindShortestPath()
        {
            openNodeList.Add(startNode);//������
            while (openNodeList.Count > 0) 
            {
                openNodeList.Sort();//�ڵ�����Node�ں��ȽϺ�����Ϊ�̳е��Ǹ��ӿ�
                Node closeNode = openNodeList[0];//ȡ��Ȩ����С�ĵ�
                openNodeList.RemoveAt(0);
                closeNodeList.Add(closeNode);//���뱻ѡ�е��б�
                if (closeNode == targetNode)
                {
                    pathFound = true;//�ҵ����·����
                    break;
                }
                //������Χ8��node���䵽openlist����
                EvaluateNeighbourNodes(closeNode);
            }
            return pathFound;
        }
        //�����������Χ8���㲢����Ȩ��
        private void EvaluateNeighbourNodes(Node currentNode) 
        {
            Vector2Int currentNodePos = currentNode.gridPosition;
            Node validNeighbourNode;
            //ѭ����Χ8���㣨9����
            for (int x = -1; x <= 1; x++) 
            {
                for (int y = -1; y <= 1; y++) 
                {
                    if (x == 0 && y == 0)//�������������
                        continue;
                    validNeighbourNode = GetValidNieghbourNode(currentNodePos.x+x,currentNodePos.y+y);//��ȡ����ڵ�
                    if (validNeighbourNode != null) 
                    {
                        if (!openNodeList.Contains(validNeighbourNode)) 
                        {//���㿪ʼ�ڵ�ֵ���յ�ڵ�ֵ
                            validNeighbourNode.gCost=currentNode.gCost+GetDistance(currentNode,validNeighbourNode);
                            validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                            validNeighbourNode.parentNode = currentNode;//���Ӹ��ڵ�
                            openNodeList.Add(validNeighbourNode);//��ӵ���Χ8������б���
                        }
                    }
                }
            }
        }
        //���ؿ��õĽڵ�
        private Node GetValidNieghbourNode(int x, int y) 
        {
            if (x >= gridWidth || y >= gridHeight || x < 0 || y < 0)
                return null;//���ڶ�ά������ֱ�ӷ���
            Node neighbourNode=gridNodes.GetGridNode(x, y);//�õ��ڵ�
            if (neighbourNode.isObstacle || closeNodeList.Contains(neighbourNode))
                return null;//���ϰ������Ѿ���ѡ����Ҳֱ�ӷ���
            else
                return neighbourNode;//��������ڵ�
        }
        //�����ڵ��ľ���ֵ(14������б����+10�������������ң�)
        private int GetDistance(Node nodeA, Node nodeB) 
        {
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);//x���ϵľ����
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);//y���ϵľ����
            if (xDistance > yDistance)  
            {
                return 14 * yDistance + 10 * (xDistance - yDistance);
            }
            return 14 * xDistance + 10 * (yDistance - xDistance);
        }
        //����·��ÿһ��������ͳ�������
        private void UpdatePathOnMovementStepStack(string sceneName,Stack<MovementStep> npcMovementStep) 
        {
            Node nextNode = targetNode;
            while (nextNode != null) 
            {
                MovementStep newStep = new MovementStep();
                newStep.sceneName = sceneName;
                newStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);
                //ѹ���ջ
                npcMovementStep.Push(newStep);
                nextNode = nextNode.parentNode;
            }
        }
    }
}
