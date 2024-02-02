using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.AStar 
{
    public class MovementStep//移动步骤
    {
        public string sceneName;//属于那个场景
        //时间戳
        public int hour;//时
        public int minute;//分
        public int second;//秒
        public Vector2Int gridCoordinate;//这步对应网格的坐标
    }
}
