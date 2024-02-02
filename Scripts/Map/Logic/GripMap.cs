using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
[ExecuteInEditMode]//�ڱ༭ģʽ�½������е�����
//���ص�ÿ����Ƭ��ͼ�ϣ�ͬ���洢��Ƭ��ͼ����Ϣ
public class GripMap : MonoBehaviour
{
    public MapData_SO mapData;//��ͼ��Ϣ�����ݿ�
    public GridType gridType;//��Ƭ����
    private Tilemap currentTilemap;//��Ƭ��ͼ
    private void OnEnable()//�༭�������ͼʱ(˵��Ҫ�����޸�)
    {
        if (!Application.IsPlaying(this)) //�ж���������Ƿ�������
        {//��Ϊ���ڱ༭ģʽ����ִ�У�������û������ʱ
            currentTilemap=GetComponent<Tilemap>();
            if (mapData != null)
                mapData.tileProperties.Clear();//�������
        }
    }
    private void OnDisable()//�༭��ʧ���ͼʱ˵����Ƭ�޸����
    {
        if (!Application.IsPlaying(this)) //�ж���������Ƿ�������
        {//��Ϊ���ڱ༭ģʽ����ִ�У�������û������ʱ
            currentTilemap = GetComponent<Tilemap>();
            UpdateTileProperties();
#if UNITY_EDITOR//ȷ��ֻ�ڱ༭����ִ�����沿�ִ��룬�����ִ��
            if (mapData != null)
                EditorUtility.SetDirty(mapData);//ʵʱ����SpriteObject���ݣ���Ȼ�˳�Unity�����ݾ�û��
#endif 
        }
    }
    //���µ�ͼ���ݿ�����Ƭ�б����Ϣ
    private void UpdateTileProperties() 
    {
        currentTilemap.CompressBounds();//������Ի�ȡ����ʵ����Ƭ��ͼ��С(�����˵ĵط�)
        if (!Application.IsPlaying(this)) 
        {
            if (mapData != null) 
            {
                Vector3Int starPos = currentTilemap.cellBounds.min;//�ѻ��Ƶ�ͼ���½ǵ�����
                Vector3Int endPos=currentTilemap.cellBounds.max;//�ѻ��Ƶ�ͼ���Ͻǵ�����
                for (int x = starPos.x; x < endPos.x; x++) //ѭ����Χ�����е���Ƭ
                {
                    for (int y = starPos.y; y < endPos.y; y++) 
                    {
                        TileBase tile = currentTilemap.GetTile(new Vector3Int(x, y, 0));//�õ���λ�õ���Ƭ
                        if (tile != null) 
                        {//��ʼ����Ƭ���������Ƭ��Ϣ
                            TileProperty newTile = new TileProperty
                            {
                                tileCordinate = new Vector2Int(x, y),//��������
                                gridType = this.gridType,//��������
                                boolTypeValue = true//�Ѿ������
                            };
                            mapData.tileProperties.Add(newTile);//����Ƭ������Ϣ��ӵ��б���
                        }
                    }
                }
            }
        }
    }
}
