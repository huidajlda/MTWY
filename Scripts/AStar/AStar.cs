using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Map;

namespace MFarm.AStar 
{
    public class AStar : Singleton<AStar>
    {
        private GridNodes gridNodes;//每一张地图的网格节点信息
        private Node startNode;//起点
        private Node targetNode;//目标点（终点）这些变量都是临时存储用的
        private int gridWidth;//网格宽
        private int gridHeight;//网格高
        private int originX;//左下角原点x坐标
        private int originY;//左下角原点y坐标
        private List<Node> openNodeList;//当前选中的Node节点周围的8个节点
        private HashSet<Node> closeNodeList;//所有被选中点的列表（hashset保证不会重复，而且查找时比List快）
        private bool pathFound;//是否找到路径
        //创建路径,更新Stack的每一步
        public void BuildPath(string sceneName,Vector2Int startPos,Vector2Int endPos,Stack<MovementStep> npcMovementStack) 
        {
            pathFound = false;
            if (GenerateGridNodes(sceneName, startPos, endPos)) //网格信息创建好了
            {
                //查找最短路径
                if (FindShortestPath()) 
                {
                    //构建Npc的移动路径
                    UpdatePathOnMovementStepStack(sceneName, npcMovementStack);
                }
            }
        }
        //构建网格节点信息,初始化列表
        private bool GenerateGridNodes(string sceneName,Vector2Int startPos,Vector2Int endPos) 
        {//判断有没有该地图的网格范围信息
            if (GridMapManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrigin)) 
            {//初始化网格范围数据
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);//创建网格范围的节点数据
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;
                openNodeList = new List<Node>();
                closeNodeList = new HashSet<Node>();
            }
            else
                return false;
            //gridNodes的范围是从0，0开始的所以需要减去原点坐标得到实际位置
            startNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);//起始节点
            targetNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);//目标点
            for (int x = 0; x < gridWidth; x++) 
            {
                for (int y = 0; y < gridHeight; y++) 
                {
                    Vector3Int tilePos=new Vector3Int(x + originX, y + originY,0);//网格位置就要反过来拿
                    var key = tilePos.x + "x" + tilePos.y + "y" + sceneName;//生成key
                    TileDetails tile=GridMapManager.Instance.GetTileDetails(key);//拿到瓦片信息
                    if (tile != null) 
                    {
                        Node node = gridNodes.GetGridNode(x, y);//拿到该位置的节点信息
                        if(tile.isNPCObstacle)
                            node.isObstacle = true;
                    }
                }
            }
            return true;
        }
        //查找最短路径
        private bool FindShortestPath()
        {
            openNodeList.Add(startNode);//添加起点
            while (openNodeList.Count > 0) 
            {
                openNodeList.Sort();//节点排序，Node内涵比较函数因为继承的那个接口
                Node closeNode = openNodeList[0];//取出权重最小的点
                openNodeList.RemoveAt(0);
                closeNodeList.Add(closeNode);//加入被选中的列表
                if (closeNode == targetNode)
                {
                    pathFound = true;//找到最短路径了
                    break;
                }
                //计算周围8个node补充到openlist里面
                EvaluateNeighbourNodes(closeNode);
            }
            return pathFound;
        }
        //计算和评估周围8个点并生成权重
        private void EvaluateNeighbourNodes(Node currentNode) 
        {
            Vector2Int currentNodePos = currentNode.gridPosition;
            Node validNeighbourNode;
            //循环周围8个点（9宫格）
            for (int x = -1; x <= 1; x++) 
            {
                for (int y = -1; y <= 1; y++) 
                {
                    if (x == 0 && y == 0)//忽略自身这个点
                        continue;
                    validNeighbourNode = GetValidNieghbourNode(currentNodePos.x+x,currentNodePos.y+y);//获取这个节点
                    if (validNeighbourNode != null) 
                    {
                        if (!openNodeList.Contains(validNeighbourNode)) 
                        {//计算开始节点值和终点节点值
                            validNeighbourNode.gCost=currentNode.gCost+GetDistance(currentNode,validNeighbourNode);
                            validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                            validNeighbourNode.parentNode = currentNode;//连接父节点
                            openNodeList.Add(validNeighbourNode);//添加到周围8个点的列表中
                        }
                    }
                }
            }
        }
        //返回可用的节点
        private Node GetValidNieghbourNode(int x, int y) 
        {
            if (x >= gridWidth || y >= gridHeight || x < 0 || y < 0)
                return null;//不在二维数组内直接返回
            Node neighbourNode=gridNodes.GetGridNode(x, y);//拿到节点
            if (neighbourNode.isObstacle || closeNodeList.Contains(neighbourNode))
                return null;//是障碍或者已经被选中了也直接返回
            else
                return neighbourNode;//返回这个节点
        }
        //两个节点间的距离值(14倍数（斜方向）+10倍数（上下左右）)
        private int GetDistance(Node nodeA, Node nodeB) 
        {
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);//x轴上的距离差
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);//y轴上的距离差
            if (xDistance > yDistance)  
            {
                return 14 * yDistance + 10 * (xDistance - yDistance);
            }
            return 14 * xDistance + 10 * (yDistance - xDistance);
        }
        //更新路径每一步的坐标和场景名称
        private void UpdatePathOnMovementStepStack(string sceneName,Stack<MovementStep> npcMovementStep) 
        {
            Node nextNode = targetNode;
            while (nextNode != null) 
            {
                MovementStep newStep = new MovementStep();
                newStep.sceneName = sceneName;
                newStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);
                //压入堆栈
                npcMovementStep.Push(newStep);
                nextNode = nextNode.parentNode;
            }
        }
    }
}
