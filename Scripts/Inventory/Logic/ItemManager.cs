using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MFarm.Save;

namespace MFarm.Inventory 
{
    public class ItemManager : MonoBehaviour,ISaveable
    {
        public Item itemPrefab;//初始化物品的脚本
        public Item bounceItemPrefab;//扔出去的物体预制体
        private Transform itemParent;//物品生成在该对象(保持整洁)
        private Transform PlayerTransform => FindObjectOfType<Player>().transform;//玩家位置

        public string GUID => GetComponent<DataGUID>().guid;

        //储存场景里面的物品(场景名称，序列化物品列表)
        private Dictionary<string,List<SceneItem>> sceneItemDict=new Dictionary<string, List<SceneItem>>();
        //储存场景里面的建造物品
        private Dictionary<string, List<SceneFurniture>> sceneFurnitureDict = new Dictionary<string, List<SceneFurniture>>();
        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;//注册事件(生成物品)
            EventHandler.DropItemEvent += OnDropItemEvent;//扔出物品的事件
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//获取物品父物体的方法
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;//建造事件方法
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;//开始新游戏的事件
        }
        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();//添加进接口列表
        }
        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;//注销事件
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        }
        //开始新游戏的方法
        private void OnStartNewGameEvent(int obj)
        {
            sceneItemDict.Clear();
            sceneFurnitureDict.Clear();
        }

        //建造方法
        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            //拿到蓝图数据
            BluePrintDetails bluePrint=InventoryManager.Instance.bluPrintData.GetBluePrintDetails(ID);
            //创建物体
            var buildItem = Instantiate(bluePrint.buildPrefab, mousePos, Quaternion.identity, itemParent);
            if (buildItem.GetComponent<Box>()) //如果是盒子，则给盒子一个序号
            {
                buildItem.GetComponent<Box>().index = InventoryManager.Instance.BoxDataAmount;
                buildItem.GetComponent<Box>().InitBox(buildItem.GetComponent<Box>().index);//箱子初始化的方法
            }
        }

        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItem();//将此方法加入场景切换前的事件，在场景切换前保存场景物品的数据
            GetAllSceneFurniture();//场景切换前保存数据
        }
        private void OnAfterSceneUnloadEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;//获取父物体
            RecreateAllItem();//场景加载后的事件调用重新生成场景物品的方法
            RebuildFurniture();//场景加载后重新生成建造物品
        }
        //生成物品的方法
        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
            var item = Instantiate(bounceItemPrefab, pos, Quaternion.identity, itemParent);//生成物品
            item.itemID = ID;
            item.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);//物品掉落
        }
        //扔出物品的方法
        private void OnDropItemEvent(int ID, Vector3 mousePos,ItemType itemType)
        {
            if (itemType == ItemType.Seed) return;
            var item = Instantiate(bounceItemPrefab, PlayerTransform.position, Quaternion.identity, itemParent);//生成物品
            item.itemID = ID;
            var dir = (mousePos - PlayerTransform.position).normalized;//方向
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos, dir);//生成扔出去的物体
        }
        //保存当前场景的物品数据
        private void GetAllSceneItem() 
        {
            List<SceneItem> currentSceneItems=new List<SceneItem>();//临时的物品列表
            foreach (var item in FindObjectsOfType<Item>()) //循环当前场景所有的物品
            {
                SceneItem  sceneItem =new SceneItem //初始化场景物品数据
                { 
                    itemID=item.itemID,//物品ID
                    position=new SerializableVector3(item.transform.position)//序列化的坐标
                };
                currentSceneItems.Add(sceneItem);//添加到物品列表中
            }
            //判断字典中有没有改场景的名称
            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItems;//将当前物品列表赋值给场景
            }
            else //字典中没有改场景的信息
            {
                //将场景名称和当前物品列表信息添加进去
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItems);
            }
        }
        //重新生成当前场景的物品
        private void RecreateAllItem()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();//临时物品列表
            //TryGetValue尝试查找对应场景名称的数据，有就会返回到临时变量里
            if (sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItems)) 
            {
                if (currentSceneItems != null) //返回的物品数据不为空
                {
                    foreach (var item in FindObjectsOfType<Item>()) //先清空场景里面的所有数据
                    {
                        Destroy(item.gameObject);
                    }
                    foreach (var item in currentSceneItems) 
                    {
                        //创建物品
                        Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                        newItem.Init(item.itemID);//初始化物品
                    }
                }
            }
        }
        //保存当前场景的建造物品数据
        private void GetAllSceneFurniture() 
        {
            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();//临时的建造物品列表
            foreach (var item in FindObjectsOfType<Furniture>()) //循环当前场景所有的物品
            {
                SceneFurniture sceneFurniture= new SceneFurniture //初始化场景物品数据
                {
                    itemID = item.itemID,//物品ID
                    position = new SerializableVector3(item.transform.position)//序列化的坐标
                };
                if(item.GetComponent<Box>())//如果是盒子，保存盒子序号
                    sceneFurniture.boxIndex=item.GetComponent<Box>().index;
                currentSceneFurniture.Add(sceneFurniture);//添加到物品列表中
            }
            //判断字典中有没有改场景的名称
            if (sceneFurnitureDict.ContainsKey(SceneManager.GetActiveScene().name))
                sceneFurnitureDict[SceneManager.GetActiveScene().name] = currentSceneFurniture;//将当前物品列表赋值给场景
            else //字典中没有改场景的信息
            {
                //将场景名称和当前物品列表信息添加进去
                sceneFurnitureDict.Add(SceneManager.GetActiveScene().name, currentSceneFurniture);
            }
        }
        //重新生成当前场景的建造物品
        private void RebuildFurniture() 
        {
            List<SceneFurniture> currentSceneFurniture=new List<SceneFurniture>();//临时物品列表
            //TryGetValue尝试查找对应场景名称的数据，有就会返回到临时变量里
            if (sceneFurnitureDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneFurniture))
            {
                if (currentSceneFurniture != null) //返回的物品数据不为空
                {   //循环所有数据
                    foreach (SceneFurniture sceneFurniture in currentSceneFurniture) 
                    {   //建造物品
                        //拿到蓝图数据
                        BluePrintDetails bluePrint = InventoryManager.Instance.bluPrintData.GetBluePrintDetails(sceneFurniture.itemID);
                        //创建物体
                        var buildItem = Instantiate(bluePrint.buildPrefab, sceneFurniture.position.ToVector3(), Quaternion.identity, itemParent);
                        if (buildItem.GetComponent<Box>()) //如果是盒子，则给盒子一个序号
                        {
                            buildItem.GetComponent<Box>().InitBox(sceneFurniture.boxIndex);
                        }
                    }
                }
            }
        }
        //保存数据
        public GameSaveData GenerateSaveData()
        {   //在场景没有卸载时这里就没有调用，就没办法拿到数据，所有要先拿到数据
            GetAllSceneItem();//所有场景的物品
            GetAllSceneFurniture();//所有场景的家具,执行完后对应的两个字典就生成好了
            GameSaveData saveData = new GameSaveData();
            saveData.sceneItemDict=this.sceneItemDict;
            saveData.sceneFurnitureDict=this.sceneFurnitureDict;
            return saveData;
        }
        //读取数据
        public void RestoreData(GameSaveData saveData)
        {
            this.sceneItemDict=saveData.sceneItemDict;
            this.sceneFurnitureDict=saveData.sceneFurnitureDict;
            //刷新场景的物品家具
            RecreateAllItem();
            RebuildFurniture();
        }
    }
}
