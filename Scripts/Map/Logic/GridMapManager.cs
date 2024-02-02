using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using MFarm.CropPlant;
using MFarm.Save;
namespace MFarm.Map 
{//管理地图瓦片地图
    public class GridMapManager : Singleton<GridMapManager>,ISaveable
    {
        [Header("地图信息")]
        public List<MapData_SO> mapDataList;//地图瓦片数据库的列表
        [Header("种地瓦片切换信息")]
        public RuleTile digTile;//挖坑的瓦片规则
        public RuleTile waterTile;//湿润土地的瓦片规则
        private Tilemap digTilemap;//挖坑的瓦片地图
        private Tilemap waterTilemap;//湿润土地的瓦片地图
        //瓦片中保存的详情的字典,(键:场景名称+坐标,值:瓦片中保存的详情)
        private Dictionary<string,TileDetails> tileDetailsDict=new Dictionary<string,TileDetails>();
        //场景是否为第一次加载的字典
        private Dictionary<string,bool> firstLoadDict=new Dictionary<string,bool>();
        //网格信息
        private Grid currentGrid;
        private Season currentSeason;//当前季节
        //杂草列表
        private List<ReapItem> itemsInRadius;

        public string GUID => GetComponent<DataGUID>().guid;

        private void OnEnable()
        {
            EventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;//使用物品时执行的事件
            EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//切换场景后执行的事件
            EventHandler.GameDayEvent += OnGameDayEvent;//每过一天执行的事件
            EventHandler.RefreshCurrentMap += RefreshMap;//将刷新地图的方法添加进事件里面

        }
        private void OnDisable()
        {
            EventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
            EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.RefreshCurrentMap -= RefreshMap;
        }

        //在场景切换后拿到当前场景的Grid网格
        private void OnAfterSceneUnloadEvent()
        {
            currentGrid=FindObjectOfType<Grid>();//拿到当前场景的Grid;
            digTilemap=GameObject.FindWithTag("Dig").GetComponent<Tilemap>();//拿到挖坑的瓦片地图
            waterTilemap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();//拿到湿润土地的瓦片地图
            if (firstLoadDict[SceneManager.GetActiveScene().name]) //判断当前场景是否为第一次加载
            {
                //呼叫预先生成农作物的事件
                EventHandler.CallGenerateCropEvent();
                firstLoadDict[SceneManager.GetActiveScene().name] = false;//不是第一次加载了
            }
            //DisplayMap(SceneManager.GetActiveScene().name);//显示当前创建中的瓦片信息
            RefreshMap();//直接刷新（上面注释的也能用）
        }
        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();//添加进接口列表
            foreach (var mapData in mapDataList) //循环地图数据列表
            {
                firstLoadDict.Add(mapData.sceneName, true);//所有地图默认第一次加载
                InitTileDetailsDict(mapData);//初始化信息字典
            }
        }
        //每过一天执行的方法
        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;//记录当前的季节
            //刷新地图信息
            foreach (var tile in tileDetailsDict) 
            {
                if(tile.Value.daysSinceWatered>-1)//说明昨天浇过水了
                    tile.Value.daysSinceWatered = -1;//今天没浇水
                if (tile.Value.daysSinceDug > -1)//已经挖坑了
                    tile.Value.daysSinceDug++;//挖坑的事件加1
                //超期消除挖坑(挖坑超过5天且没有播种就让坑小时)
                if (tile.Value.daysSinceDug > 5 && tile.Value.seedItemID == -1) 
                {
                    tile.Value.daysSinceDug = -1;
                    tile.Value.canDig = true;//恢复可挖坑的状态
                    tile.Value.growthDays=-1;
                }
                if (tile.Value.seedItemID != -1) //如果种了东西了
                {
                    tile.Value.growthDays++;
                }
            }
            RefreshMap();//刷新瓦片地图
        }
        //初始化瓦片信息字典
        private void InitTileDetailsDict(MapData_SO mapData) 
        {
            foreach (TileProperty tileProperty in mapData.tileProperties) 
            {
                TileDetails tileDetails = new TileDetails 
                {
                    gridX=tileProperty.tileCordinate.x,//坐标赋值
                    gridY=tileProperty.tileCordinate.y,
                };
                //字典的键值(瓦片坐标+场景名称)
                string key=tileDetails.gridX+"x"+tileDetails.gridY+"y"+mapData.sceneName;
                if (GetTileDetails(key) != null) 
                {
                    tileDetails=GetTileDetails(key);//如果不为空，则使用原来的值
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
                    tileDetailsDict[key] = tileDetails;//如果有，就重新赋值
                else
                    tileDetailsDict.Add(key, tileDetails);//没有就添加到字典
            }
        }
        //根据键值查找字典信息
        public TileDetails GetTileDetails(string key) 
        {
            if (tileDetailsDict.ContainsKey(key))//有就返回，无就返回空
                return tileDetailsDict[key];
            else return null;
        }
        //通过坐标返回信息
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos) 
        {
            string key=mouseGridPos.x+"x"+mouseGridPos.y+"y"+SceneManager.GetActiveScene().name;
            return GetTileDetails(key);//返回信息
        }
        //执行实际物品或工具的功能
        private void OnExecuteActionAfterAnimation(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            var mouseGridPos=currentGrid.WorldToCell(mouseWorldPos);//获取鼠标的网格坐标
            var currentTile = GetTileDetailsOnMousePosition(mouseGridPos);//通过坐标返回当前瓦片信息
            if (currentTile != null) 
            {
                Crop currentCrop = GetCropObject(mouseWorldPos);
                //WORKFLOW:物品使用实际功能
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed://种子
                        EventHandler.CallPlantSeedEvent(itemDetails.itemId, currentTile);//调用种植的事件
                        //调用仍东西的事件(但种子只减数量不生成)
                        EventHandler.CallDropItemEvent(itemDetails.itemId, mouseGridPos, itemDetails.itemType);
                        EventHandler.CallPlaySoundEvent(SoundName.Plant);//播放种植音效
                        break;
                    case ItemType.Commodity://是商品
                        EventHandler.CallDropItemEvent(itemDetails.itemId, mouseGridPos,itemDetails.itemType);//扔出物品的事件
                        break;
                    case ItemType.HoeTool://挖坑工具
                        SetDigGround(currentTile);
                        currentTile.daysSinceDug = 0;//挖坑天数
                        currentTile.canDig = false;//不可以继续挖坑
                        currentTile.canDropItem = false;//不可以在坑上扔东西
                        EventHandler.CallPlaySoundEvent(SoundName.Hoe);
                        break;
                    case ItemType.WaterTool://浇水工具
                        SetWaterGround(currentTile);
                        currentTile.daysSinceWatered = 0;//浇水的天数
                        EventHandler.CallPlaySoundEvent(SoundName.Water);
                        break;
                    case ItemType.CollectTool://收集工具
                        if (currentCrop != null)
                            currentCrop.ProcessToolAction(itemDetails,currentTile);//执行收集工具的行为
                        break;
                    case ItemType.BreakTool:
                    case ItemType.ChopTool://斧头
                        currentCrop?.ProcessToolAction(itemDetails, currentCrop.tileDetails);
                        break;
                    case ItemType.ReapTool:
                        var reapCount = 0;//计数器
                        for (int i = 0; i < itemsInRadius.Count; i++) //循环杂草列表
                        {
                            //播放割草特效
                            EventHandler.CallParticleEffectEvent(ParticaleEffectType.ReapableScenery, 
                                                    itemsInRadius[i].transform.position + Vector3.up);
                            itemsInRadius[i].SpawnHarvestItems();//收获方法
                            Destroy(itemsInRadius[i].gameObject);//删除杂草物体
                            reapCount++;
                            if(reapCount>=Settings.reapAmount)//大于规定数量这次就不继续清空了
                                break;
                        }
                        EventHandler.CallPlaySoundEvent(SoundName.Reap);
                        break;
                    case ItemType.Furniture:
                        EventHandler.CallBuildFurnitureEvent(itemDetails.itemId,mouseWorldPos);
                        break;
                }
                UpdateTileDetails(currentTile);//更新瓦片字典中的信息
            }
        }
        //获取鼠标位置的种子种植
        public Crop GetCropObject(Vector3 mouseWorldPos) 
        {
            //Physics2D.OverlapAreaAll返回2D某个点附近的所有碰撞体
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);
            Crop currentCrop = null;//当前种子种植信息
            for (int i = 0; i < colliders.Length; i++) 
            {
                if (colliders[i].GetComponent<Crop>())
                    currentCrop = colliders[i].GetComponent<Crop>();//获取当前种子种植信息
            }
            return currentCrop;
        }
        //鼠标位置工具范围内有没有杂草信息
        public bool HaveReapableItemsInRadius(Vector3 mouseWolrdPos, ItemDetails tool) 
        {
            itemsInRadius = new List<ReapItem>();
            Collider2D[] colliders=new Collider2D[20];
            //返回范围内检测到的碰撞体，这个方法在经常检测时对比其他来说可以提高性能
            //该方法的四个参数分别是中心点，范围，返回的数组,图层
            Physics2D.OverlapCircleNonAlloc(mouseWolrdPos, tool.itemUseRadius, colliders);
            if (colliders.Length > 0) //数组里面有东西
            {
                for (int i = 0; i < colliders.Length; i++) 
                {
                    if (colliders[i] != null) 
                    {
                        if (colliders[i].GetComponent<ReapItem>()) //身上有杂草的脚本
                        {
                            var item = colliders[i].GetComponent<ReapItem>();
                            itemsInRadius.Add(item);//添加到杂草列表
                        }
                    }
                }
            }
            return itemsInRadius.Count > 0;
        }
        //设置挖坑的瓦片
        private void SetDigGround(TileDetails tile) 
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);//生成瓦片的位置坐标
            if (digTilemap != null) //瓦片地图不为空
                digTilemap.SetTile(pos, digTile);//设置瓦片
        }
        //设置浇水后湿润土地的瓦片
        private void SetWaterGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);//生成瓦片的位置坐标
            if (waterTilemap != null) //瓦片地图不为空
                waterTilemap.SetTile(pos, waterTile);//设置瓦片
        }
        //更新字典中的地图信息
        public void UpdateTileDetails(TileDetails tileDetails) 
        {
            //拿到当前瓦片的key
            string key=tileDetails.gridX+"x"+tileDetails.gridY+"y"+SceneManager.GetActiveScene().name;
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;//找到字典中的瓦片信息进行替换
            }
            else 
            {
                tileDetailsDict.Add(key, tileDetails);//加入字典
            }
        }
        //刷新瓦片地图
        private void RefreshMap() 
        {
            if(digTilemap!=null)//挖坑瓦片地图不为空
                digTilemap.ClearAllTiles();//清空挖坑瓦片地图
            if(waterTilemap!=null)//湿润瓦片地图不为空
                waterTilemap.ClearAllTiles();//清空湿润瓦片地图
            foreach (var crop in FindObjectsOfType<Crop>()) //循环种植了的种子
            {
                Destroy(crop.gameObject);//删除种子
            }
            DisplayMap(SceneManager.GetActiveScene().name);//重新显示瓦片地图信息
        }
        //显示当前场景中的地图信息
        private void DisplayMap(string sceneName) 
        {
            foreach (var tile in tileDetailsDict) //找到瓦片地图
            {
                var key = tile.Key;
                var tileDetails = tile.Value;
                if (key.Contains(sceneName)) //瓦片是否包含场景的名称
                {
                    if (tileDetails.daysSinceDug > -1)//已经被挖坑了
                        SetDigGround(tileDetails);//将该地替换成挖坑的瓦片地图
                    if (tileDetails.daysSinceWatered > -1) //已经浇过水了
                        SetWaterGround(tileDetails);//将该地替换成湿润的瓦片地图
                    if (tileDetails.seedItemID > -1)//瓦片有没有种子的信息
                        EventHandler.CallPlantSeedEvent(tileDetails.seedItemID, tileDetails);
                }
            }
        }
        //根据地图名称构建网格范围（地图名称，输出网格范围和原点坐标）
        public bool GetGridDimensions(string sceneName,out Vector2Int gridDimensions,out Vector2Int gridOrigin) 
        {
            gridDimensions = Vector2Int.zero;
            gridOrigin=Vector2Int.zero;
            foreach (var mapData in mapDataList) //循环每一个地图信息
            {
                if (mapData.sceneName == sceneName) 
                {
                    gridDimensions.x = mapData.gridWidth;//地图宽度
                    gridDimensions.y = mapData.gridHeight;//地图高度
                    gridOrigin.x = mapData.originX;//原点x
                    gridOrigin.y = mapData.originY;//原点y
                    return true;//有这个地图信息
                }
            }
            return false;//没有这个地图信息
        }
        //保存数据
        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.tileDetailsDict=this.tileDetailsDict;
            saveData.firstLoadDict=this.firstLoadDict;
            return saveData;
        }
        //读取数据
        public void RestoreData(GameSaveData saveData)
        {
            this.tileDetailsDict = saveData.tileDetailsDict;
            this.firstLoadDict = saveData.firstLoadDict;
        }
    }
}