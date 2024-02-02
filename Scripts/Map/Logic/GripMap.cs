using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
[ExecuteInEditMode]//在编辑模式下进行运行的特性
//挂载到每个瓦片地图上，同来存储瓦片地图的信息
public class GripMap : MonoBehaviour
{
    public MapData_SO mapData;//地图信息的数据库
    public GridType gridType;//瓦片类型
    private Tilemap currentTilemap;//瓦片地图
    private void OnEnable()//编辑器激活地图时(说明要进行修改)
    {
        if (!Application.IsPlaying(this)) //判断这个代码是否在运行
        {//因为是在编辑模式下来执行，所以在没有运行时
            currentTilemap=GetComponent<Tilemap>();
            if (mapData != null)
                mapData.tileProperties.Clear();//清空数据
        }
    }
    private void OnDisable()//编辑器失活地图时说明瓦片修改完毕
    {
        if (!Application.IsPlaying(this)) //判断这个代码是否在运行
        {//因为是在编辑模式下来执行，所以在没有运行时
            currentTilemap = GetComponent<Tilemap>();
            UpdateTileProperties();
#if UNITY_EDITOR//确保只在编辑器中执行下面部分代码，打包后不执行
            if (mapData != null)
                EditorUtility.SetDirty(mapData);//实时保存SpriteObject数据，不然退出Unity后数据就没了
#endif 
        }
    }
    //更新地图数据库中瓦片列表的信息
    private void UpdateTileProperties() 
    {
        currentTilemap.CompressBounds();//这个可以获取到真实的瓦片地图大小(绘制了的地方)
        if (!Application.IsPlaying(this)) 
        {
            if (mapData != null) 
            {
                Vector3Int starPos = currentTilemap.cellBounds.min;//已绘制地图左下角的坐标
                Vector3Int endPos=currentTilemap.cellBounds.max;//已绘制地图右上角的坐标
                for (int x = starPos.x; x < endPos.x; x++) //循环范围内所有的瓦片
                {
                    for (int y = starPos.y; y < endPos.y; y++) 
                    {
                        TileBase tile = currentTilemap.GetTile(new Vector3Int(x, y, 0));//拿到该位置的瓦片
                        if (tile != null) 
                        {//初始化瓦片格子类的瓦片信息
                            TileProperty newTile = new TileProperty
                            {
                                tileCordinate = new Vector2Int(x, y),//网格坐标
                                gridType = this.gridType,//格子类型
                                boolTypeValue = true//已经被标记
                            };
                            mapData.tileProperties.Add(newTile);//将瓦片格子信息添加到列表里
                        }
                    }
                }
            }
        }
    }
}
