using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using MFarm.CropPlant;
using MFarm.Save;
namespace MFarm.Map 
{//�����ͼ��Ƭ��ͼ
    public class GridMapManager : Singleton<GridMapManager>,ISaveable
    {
        [Header("��ͼ��Ϣ")]
        public List<MapData_SO> mapDataList;//��ͼ��Ƭ���ݿ���б�
        [Header("�ֵ���Ƭ�л���Ϣ")]
        public RuleTile digTile;//�ڿӵ���Ƭ����
        public RuleTile waterTile;//ʪ�����ص���Ƭ����
        private Tilemap digTilemap;//�ڿӵ���Ƭ��ͼ
        private Tilemap waterTilemap;//ʪ�����ص���Ƭ��ͼ
        //��Ƭ�б����������ֵ�,(��:��������+����,ֵ:��Ƭ�б��������)
        private Dictionary<string,TileDetails> tileDetailsDict=new Dictionary<string,TileDetails>();
        //�����Ƿ�Ϊ��һ�μ��ص��ֵ�
        private Dictionary<string,bool> firstLoadDict=new Dictionary<string,bool>();
        //������Ϣ
        private Grid currentGrid;
        private Season currentSeason;//��ǰ����
        //�Ӳ��б�
        private List<ReapItem> itemsInRadius;

        public string GUID => GetComponent<DataGUID>().guid;

        private void OnEnable()
        {
            EventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;//ʹ����Ʒʱִ�е��¼�
            EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//�л�������ִ�е��¼�
            EventHandler.GameDayEvent += OnGameDayEvent;//ÿ��һ��ִ�е��¼�
            EventHandler.RefreshCurrentMap += RefreshMap;//��ˢ�µ�ͼ�ķ�����ӽ��¼�����

        }
        private void OnDisable()
        {
            EventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.RefreshCurrentMap -= RefreshMap;
        }

        //�ڳ����л����õ���ǰ������Grid����
        private void OnAfterSceneUnloadEvent()
        {
            currentGrid=FindObjectOfType<Grid>();//�õ���ǰ������Grid;
            digTilemap=GameObject.FindWithTag("Dig").GetComponent<Tilemap>();//�õ��ڿӵ���Ƭ��ͼ
            waterTilemap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();//�õ�ʪ�����ص���Ƭ��ͼ
            if (firstLoadDict[SceneManager.GetActiveScene().name]) //�жϵ�ǰ�����Ƿ�Ϊ��һ�μ���
            {
                //����Ԥ������ũ������¼�
                EventHandler.CallGenerateCropEvent();
                firstLoadDict[SceneManager.GetActiveScene().name] = false;//���ǵ�һ�μ�����
            }
            //DisplayMap(SceneManager.GetActiveScene().name);//��ʾ��ǰ�����е���Ƭ��Ϣ
            RefreshMap();//ֱ��ˢ�£�����ע�͵�Ҳ���ã�
        }
        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();//��ӽ��ӿ��б�
            foreach (var mapData in mapDataList) //ѭ����ͼ�����б�
            {
                firstLoadDict.Add(mapData.sceneName, true);//���е�ͼĬ�ϵ�һ�μ���
                InitTileDetailsDict(mapData);//��ʼ����Ϣ�ֵ�
            }
        }
        //ÿ��һ��ִ�еķ���
        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;//��¼��ǰ�ļ���
            //ˢ�µ�ͼ��Ϣ
            foreach (var tile in tileDetailsDict) 
            {
                if(tile.Value.daysSinceWatered>-1)//˵�����콽��ˮ��
                    tile.Value.daysSinceWatered = -1;//����û��ˮ
                if (tile.Value.daysSinceDug > -1)//�Ѿ��ڿ���
                    tile.Value.daysSinceDug++;//�ڿӵ��¼���1
                //���������ڿ�(�ڿӳ���5����û�в��־��ÿ�Сʱ)
                if (tile.Value.daysSinceDug > 5 && tile.Value.seedItemID == -1) 
                {
                    tile.Value.daysSinceDug = -1;
                    tile.Value.canDig = true;//�ָ����ڿӵ�״̬
                    tile.Value.growthDays=-1;
                }
                if (tile.Value.seedItemID != -1) //������˶�����
                {
                    tile.Value.growthDays++;
                }
            }
            RefreshMap();//ˢ����Ƭ��ͼ
        }
        //��ʼ����Ƭ��Ϣ�ֵ�
        private void InitTileDetailsDict(MapData_SO mapData) 
        {
            foreach (TileProperty tileProperty in mapData.tileProperties) 
            {
                TileDetails tileDetails = new TileDetails 
                {
                    gridX=tileProperty.tileCordinate.x,//���긳ֵ
                    gridY=tileProperty.tileCordinate.y,
                };
                //�ֵ�ļ�ֵ(��Ƭ����+��������)
                string key=tileDetails.gridX+"x"+tileDetails.gridY+"y"+mapData.sceneName;
                if (GetTileDetails(key) != null) 
                {
                    tileDetails=GetTileDetails(key);//�����Ϊ�գ���ʹ��ԭ����ֵ
                }
                switch (tileProperty.gridType) 
                {
                    case GridType.Diggable:
                        tileDetails.canDig = tileProperty.boolTypeValue;
                        break;
                    case GridType.DropItem:
                        tileDetails.canDropItem = tileProperty.boolTypeValue;
                        break;
                    case GridType.PlaceFurniture:
                        tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                        break;
                    case GridType.NPCObstacle:
                        tileDetails.isNPCObstacle = tileProperty.boolTypeValue;
                        break;
                }
                if (GetTileDetails(key) != null)
                    tileDetailsDict[key] = tileDetails;//����У������¸�ֵ
                else
                    tileDetailsDict.Add(key, tileDetails);//û�о���ӵ��ֵ�
            }
        }
        //���ݼ�ֵ�����ֵ���Ϣ
        public TileDetails GetTileDetails(string key) 
        {
            if (tileDetailsDict.ContainsKey(key))//�оͷ��أ��޾ͷ��ؿ�
                return tileDetailsDict[key];
            else return null;
        }
        //ͨ�����귵����Ϣ
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos) 
        {
            string key=mouseGridPos.x+"x"+mouseGridPos.y+"y"+SceneManager.GetActiveScene().name;
            return GetTileDetails(key);//������Ϣ
        }
        //ִ��ʵ����Ʒ�򹤾ߵĹ���
        private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            var mouseGridPos=currentGrid.WorldToCell(mouseWorldPos);//��ȡ������������
            var currentTile = GetTileDetailsOnMousePosition(mouseGridPos);//ͨ�����귵�ص�ǰ��Ƭ��Ϣ
            if (currentTile != null) 
            {
                Crop currentCrop = GetCropObject(mouseWorldPos);
                //WORKFLOW:��Ʒʹ��ʵ�ʹ���
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed://����
                        EventHandler.CallPlantSeedEvent(itemDetails.itemId, currentTile);//������ֲ���¼�
                        //�����Զ������¼�(������ֻ������������)
                        EventHandler.CallDropItemEvent(itemDetails.itemId, mouseGridPos, itemDetails.itemType);
                        EventHandler.CallPlaySoundEvent(SoundName.Plant);//������ֲ��Ч
                        break;
                    case ItemType.Commodity://����Ʒ
                        EventHandler.CallDropItemEvent(itemDetails.itemId, mouseGridPos,itemDetails.itemType);//�ӳ���Ʒ���¼�
                        break;
                    case ItemType.HoeTool://�ڿӹ���
                        SetDigGround(currentTile);
                        currentTile.daysSinceDug = 0;//�ڿ�����
                        currentTile.canDig = false;//�����Լ����ڿ�
                        currentTile.canDropItem = false;//�������ڿ����Ӷ���
                        EventHandler.CallPlaySoundEvent(SoundName.Hoe);
                        break;
                    case ItemType.WaterTool://��ˮ����
                        SetWaterGround(currentTile);
                        currentTile.daysSinceWatered = 0;//��ˮ������
                        EventHandler.CallPlaySoundEvent(SoundName.Water);
                        break;
                    case ItemType.CollectTool://�ռ�����
                        if (currentCrop != null)
                            currentCrop.ProcessToolAction(itemDetails,currentTile);//ִ���ռ����ߵ���Ϊ
                        break;
                    case ItemType.BreakTool:
                    case ItemType.ChopTool://��ͷ
                        currentCrop?.ProcessToolAction(itemDetails, currentCrop.tileDetails);
                        break;
                    case ItemType.ReapTool:
                        var reapCount = 0;//������
                        for (int i = 0; i < itemsInRadius.Count; i++) //ѭ���Ӳ��б�
                        {
                            //���Ÿ����Ч
                            EventHandler.CallParticleEffectEvent(ParticaleEffectType.ReapableScenery, 
                                                    itemsInRadius[i].transform.position + Vector3.up);
                            itemsInRadius[i].SpawnHarvestItems();//�ջ񷽷�
                            Destroy(itemsInRadius[i].gameObject);//ɾ���Ӳ�����
                            reapCount++;
                            if(reapCount>=Settings.reapAmount)//���ڹ涨������ξͲ����������
                                break;
                        }
                        EventHandler.CallPlaySoundEvent(SoundName.Reap);
                        break;
                    case ItemType.Furniture:
                        EventHandler.CallBuildFurnitureEvent(itemDetails.itemId,mouseWorldPos);
                        break;
                }
                UpdateTileDetails(currentTile);//������Ƭ�ֵ��е���Ϣ
            }
        }
        //��ȡ���λ�õ�������ֲ
        public Crop GetCropObject(Vector3 mouseWorldPos) 
        {
            //Physics2D.OverlapAreaAll����2Dĳ���㸽����������ײ��
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);
            Crop currentCrop = null;//��ǰ������ֲ��Ϣ
            for (int i = 0; i < colliders.Length; i++) 
            {
                if (colliders[i].GetComponent<Crop>())
                    currentCrop = colliders[i].GetComponent<Crop>();//��ȡ��ǰ������ֲ��Ϣ
            }
            return currentCrop;
        }
        //���λ�ù��߷�Χ����û���Ӳ���Ϣ
        public bool HaveReapableItemsInRadius(Vector3 mouseWolrdPos, ItemDetails tool) 
        {
            itemsInRadius = new List<ReapItem>();
            Collider2D[] colliders=new Collider2D[20];
            //���ط�Χ�ڼ�⵽����ײ�壬��������ھ������ʱ�Ա�������˵�����������
            //�÷������ĸ������ֱ������ĵ㣬��Χ�����ص�����,ͼ��
            Physics2D.OverlapCircleNonAlloc(mouseWolrdPos, tool.itemUseRadius, colliders);
            if (colliders.Length > 0) //���������ж���
            {
                for (int i = 0; i < colliders.Length; i++) 
                {
                    if (colliders[i] != null) 
                    {
                        if (colliders[i].GetComponent<ReapItem>()) //�������ӲݵĽű�
                        {
                            var item = colliders[i].GetComponent<ReapItem>();
                            itemsInRadius.Add(item);//��ӵ��Ӳ��б�
                        }
                    }
                }
            }
            return itemsInRadius.Count > 0;
        }
        //�����ڿӵ���Ƭ
        private void SetDigGround(TileDetails tile) 
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);//������Ƭ��λ������
            if (digTilemap != null) //��Ƭ��ͼ��Ϊ��
                digTilemap.SetTile(pos, digTile);//������Ƭ
        }
        //���ý�ˮ��ʪ�����ص���Ƭ
        private void SetWaterGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);//������Ƭ��λ������
            if (waterTilemap != null) //��Ƭ��ͼ��Ϊ��
                waterTilemap.SetTile(pos, waterTile);//������Ƭ
        }
        //�����ֵ��еĵ�ͼ��Ϣ
        public void UpdateTileDetails(TileDetails tileDetails) 
        {
            //�õ���ǰ��Ƭ��key
            string key=tileDetails.gridX+"x"+tileDetails.gridY+"y"+SceneManager.GetActiveScene().name;
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;//�ҵ��ֵ��е���Ƭ��Ϣ�����滻
            }
            else 
            {
                tileDetailsDict.Add(key, tileDetails);//�����ֵ�
            }
        }
        //ˢ����Ƭ��ͼ
        private void RefreshMap() 
        {
            if(digTilemap!=null)//�ڿ���Ƭ��ͼ��Ϊ��
                digTilemap.ClearAllTiles();//����ڿ���Ƭ��ͼ
            if(waterTilemap!=null)//ʪ����Ƭ��ͼ��Ϊ��
                waterTilemap.ClearAllTiles();//���ʪ����Ƭ��ͼ
            foreach (var crop in FindObjectsOfType<Crop>()) //ѭ����ֲ�˵�����
            {
                Destroy(crop.gameObject);//ɾ������
            }
            DisplayMap(SceneManager.GetActiveScene().name);//������ʾ��Ƭ��ͼ��Ϣ
        }
        //��ʾ��ǰ�����еĵ�ͼ��Ϣ
        private void DisplayMap(string sceneName) 
        {
            foreach (var tile in tileDetailsDict) //�ҵ���Ƭ��ͼ
            {
                var key = tile.Key;
                var tileDetails = tile.Value;
                if (key.Contains(sceneName)) //��Ƭ�Ƿ��������������
                {
                    if (tileDetails.daysSinceDug > -1)//�Ѿ����ڿ���
                        SetDigGround(tileDetails);//���õ��滻���ڿӵ���Ƭ��ͼ
                    if (tileDetails.daysSinceWatered > -1) //�Ѿ�����ˮ��
                        SetWaterGround(tileDetails);//���õ��滻��ʪ�����Ƭ��ͼ
                    if (tileDetails.seedItemID > -1)//��Ƭ��û�����ӵ���Ϣ
                        EventHandler.CallPlantSeedEvent(tileDetails.seedItemID, tileDetails);
                }
            }
        }
        //���ݵ�ͼ���ƹ�������Χ����ͼ���ƣ��������Χ��ԭ�����꣩
        public bool GetGridDimensions(string sceneName,out Vector2Int gridDimensions,out Vector2Int gridOrigin) 
        {
            gridDimensions = Vector2Int.zero;
            gridOrigin=Vector2Int.zero;
            foreach (var mapData in mapDataList) //ѭ��ÿһ����ͼ��Ϣ
            {
                if (mapData.sceneName == sceneName) 
                {
                    gridDimensions.x = mapData.gridWidth;//��ͼ���
                    gridDimensions.y = mapData.gridHeight;//��ͼ�߶�
                    gridOrigin.x = mapData.originX;//ԭ��x
                    gridOrigin.y = mapData.originY;//ԭ��y
                    return true;//�������ͼ��Ϣ
                }
            }
            return false;//û�������ͼ��Ϣ
        }
        //��������
        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.tileDetailsDict=this.tileDetailsDict;
            saveData.firstLoadDict=this.firstLoadDict;
            return saveData;
        }
        //��ȡ����
        public void RestoreData(GameSaveData saveData)
        {
            this.tileDetailsDict = saveData.tileDetailsDict;
            this.firstLoadDict = saveData.firstLoadDict;
        }
    }
}