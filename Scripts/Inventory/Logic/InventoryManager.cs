using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Save;
//把所有跟背包、物品数据、使用的物品有关的内容都放到这个命名空间下
//在别的脚本使用时，引用此命名空间即可，这样也可以直到哪些脚本需要和背包关联
namespace MFarm.Inventory 
{
    public class InventoryManager : Singleton<InventoryManager>,ISaveable
    {
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;//物品数据详情
        [Header("建造蓝图")]
        public BluPrintDataList_SO bluPrintData;//建造蓝图详情
        [Header("背包数据")]
        public InventoryBag_SO playerBagTemp;//背包模板数据
        public InventoryBag_SO playerBag;//玩家背包
        private InventoryBag_SO currentBoxBag;//当前箱子背包
        [Header("交易")]
        public int PlayerMoney;//玩家的金钱
        //储存箱子数据的字典
        private Dictionary<string,List<InventoryItem>> boxDataDict=new Dictionary<string,List<InventoryItem>>();
        public int BoxDataAmount => boxDataDict.Count;//字典数量

        public string GUID => GetComponent<DataGUID>().guid;

        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;//扔出物品的事件
            EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;//在玩家身上生成收割物品的事件
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;//注册建造事件扣除相应物品
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;//注册打开通用背包的事件
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;//开始新游戏的事件
        }
        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        }
        //开始游戏调用的方法
        private void OnStartNewGameEvent(int obj)
        {
            playerBag = Instantiate(playerBagTemp);
            PlayerMoney = Settings.playerStartMoney;
            boxDataDict.Clear();
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();//添加进接口列表
        }
        //打开通用背包的事件
        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bag_SO)
        {
            currentBoxBag=bag_SO;//拿到当前打开箱子的数据
        }
        //建造扣除相应物品
        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            RemoveItem(ID,1);//扣除建造图纸
            BluePrintDetails bluePrint=bluPrintData.GetBluePrintDetails(ID);//拿到蓝图数据
            //循环蓝图资源列表
            foreach (var item in bluePrint.resourceItem) 
            {
                RemoveItem(item.itemID, item.itemAmount);//移除对应数量的物品
            }
        }
        //扔出物品的方法
        private void OnDropItemEvent(int ID, Vector3 pos, ItemType itemType)
        {
            RemoveItem(ID, 1);//每次扔出一个背包物品
        }
        //生成收割的物品添加到玩家身上的方法
        private void OnHarvestAtPlayerPosition(int ID)
        {
            //是否已经有该物品
            var index = GetItemIndexInBag(ID);//找到给物品在列表中的位置
            AddItemAtIndex(ID, index, 1);//添加数量为1的物品
            //更新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        //通过ID来找到物品详情的方法
        public ItemDetails GetItemDetails(int ID) 
        {
            //Find有则返回，无则返回空
            return itemDataList_SO.itemDetailsList.Find(i => i.itemId == ID);
        }
        //添加物品的方法
        public void AddItem(Item item,bool toDestory) //一个参数是物品，一个是是否删除
        {
            //是否已经有该物品
            var index = GetItemIndexInBag(item.itemID);//找到给物品在列表中的位置
            AddItemAtIndex(item.itemID, index, 1);//数量为1，地图上拾取的只添加1个
            if (toDestory) 
            {
                Destroy(item.gameObject);//销毁物品
            }
            //更新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        //检测背包是否有空位
        private bool CheckBagCapacity() 
        {
            for (int i = 0; i < playerBag.itemList.Count; i++) 
            {
                if (playerBag.itemList[i].itemID == 0) //itemList保存的是ID和数量的结构体，默认为0
                {//所以当id为0时说明这个地方还没有东西
                    return true;
                }
            }
            return false;
        }
        //通过物品ID找到其在列表中的位置
        private int GetItemIndexInBag(int ID) 
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID) //说明已经有该物品了
                {
                    return i;//返回物品的下标
                }
            }
            return -1;//没有找到该物品
        }
        //在背包指定位置添加物品
        private void AddItemAtIndex(int ID, int index, int amount) 
        {
            if (index == -1&& CheckBagCapacity()) //背包里没有此物品且背包中有空位
            {
                var item=new InventoryItem { itemID = ID, itemAmount=amount};//创建物品并初始化
                for (int i = 0; i < playerBag.itemList.Count; i++)//找到一个空位
                {
                    if (playerBag.itemList[i].itemID == 0) //itemList保存的是ID和数量的结构体，默认为0
                    {//所以当id为0时说明这个地方还没有东西
                        playerBag.itemList[i] = item;//将创建的物品信息赋值给这个空格子
                        break;//跳出循环
                    }
                }
            }
            else//背包有这个物品
            {
                int currentAmount = playerBag.itemList[index].itemAmount+amount;//现在的物品数量
                var item=new InventoryItem { itemID = ID,itemAmount=currentAmount};//更改列表中的数据
                playerBag.itemList[index]=item;
            }
        }
        //在背包中交换物品
        public void SwapItem(int fromIndex, int targetIndex) 
        {
            InventoryItem currentItem = playerBag.itemList[fromIndex];//当前物品数据
            InventoryItem targetItem = playerBag.itemList[targetIndex];//目标物品数据
            if (targetItem.itemID != 0) //目标格子不为空
            {//交换物品
                playerBag.itemList[fromIndex] = targetItem;
                playerBag.itemList[targetIndex] = currentItem;
            }
            else //目标格子为空
            {
                playerBag.itemList[targetIndex] = currentItem;
                playerBag.itemList[fromIndex]=new InventoryItem();//原格子变为空
            }
            //刷新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        //跨背包交换物品
        public void SwapItem(InventoryLocation locationFrom, int fromIndex, InventoryLocation locationTarget, int targetIndex)
        {
            var currentList = GetItemList(locationFrom);//从哪里拖出来的物品数据
            var targetList = GetItemList(locationTarget);//拖拽到的目标列表
            InventoryItem currentItem = currentList[fromIndex];//当前物品
            if (targetIndex < targetList.Count) //确保在范围内
            {
                InventoryItem targetItem = targetList[targetIndex];
                //目标位置不为空且不是同一个物品
                if (targetItem.itemID != 0 && currentItem.itemID != targetItem.itemID)
                {
                    //交换物品
                    currentList[fromIndex] = targetItem;
                    targetList[targetIndex] = currentItem;
                }
                else if (currentItem.itemID == targetItem.itemID) //相同物品
                {
                    targetItem.itemAmount += currentItem.itemAmount;//数量相加
                    targetList[targetIndex] = targetItem;//更新数据
                    currentList[fromIndex] = new InventoryItem();//原来位置等于空
                }
                else //目标是空格子
                {
                    targetList[targetIndex] = currentItem;//赋值到空格子
                    currentList[fromIndex]=new InventoryItem();//原来位置等于空
                }
                //更新UI
                EventHandler.CallUpdateInventoryUI(locationFrom, currentList);
                EventHandler.CallUpdateInventoryUI(locationTarget, targetList);
            }
        }
        //根据位置返回背包的数据列表
        private List<InventoryItem> GetItemList(InventoryLocation location) 
        {
            return location switch
            {
                InventoryLocation.Player => playerBag.itemList,//返回玩家背包数据
                InventoryLocation.Box => currentBoxBag.itemList,//返回箱子数据
                _ => null
            };
        }
        //移除指定数量背包物品的方法
        private void RemoveItem(int ID,int removeAmount) 
        {
            var index = GetItemIndexInBag(ID);//物品被背包列表中的索引
            if (playerBag.itemList[index].itemAmount > removeAmount) //背包中物品的数量要大于移除物品的数量(小于不执行)
            {
                var amount = playerBag.itemList[index].itemAmount-removeAmount;//剩余物品数量
                var item = new InventoryItem { itemID = ID, itemAmount = amount };//生成新的物品信息
                playerBag.itemList[index] = item;//更新物品信息
            }
            else if (playerBag.itemList[index].itemAmount == removeAmount) 
            {
                var item = new InventoryItem();//一个空物体(清空)
                playerBag.itemList[index] = item;//更新信息
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);//更新玩家的背包UI
        }
        //交易物品
        public void TradeItem(ItemDetails itemDetails,int amount,bool isSellTrade) 
        {
            int cost = itemDetails.itemPrice * amount;//总金额
            //获得物品背包位置
            int index = GetItemIndexInBag(itemDetails.itemId);
            if (isSellTrade) //卖
            {
                if (playerBag.itemList[index].itemAmount >= amount) //背包数量足够
                {
                    RemoveItem(itemDetails.itemId, amount);//移除背包中相应的数量
                    cost = (int)(cost * itemDetails.sellPercentage);//卖出去的价钱
                    PlayerMoney += cost;//加上金钱
                }
            }
            else if (PlayerMoney - cost >= 0) //买（足够钱）
            {
                if (CheckBagCapacity()) //检测背包容量
                {
                    AddItemAtIndex(itemDetails.itemId, index, amount);//添加进背包
                }
                PlayerMoney -= cost;//减去金钱
            }
            //刷新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        //检测建造资源所需库存
        public bool CheckStock(int ID) 
        {
            var bluePrintDetails=bluPrintData.GetBluePrintDetails(ID);//拿到蓝图详情
            //循环蓝图里所需要的资源
            foreach (var resourceItem in bluePrintDetails.resourceItem) 
            {
                var itemStock = playerBag.GetInventoryItem(resourceItem.itemID);//拿到背包中物品的数量
                if (itemStock.itemAmount >= resourceItem.itemAmount)//库存大于所需资源
                    continue;//判断下一个
                else return false;
            }
            return true;
        }
        //根据key来查找箱子数据
        public List<InventoryItem> GetBoxDataList(string key) 
        {
            if(boxDataDict.ContainsKey(key))
                return boxDataDict[key];
            return null;
        }
        //箱子数据添加进字典
        public void AddBoxDataDic(Box box) 
        {
            var key = box.name + box.index;
            if (!boxDataDict.ContainsKey(key))//不包含这个键
                boxDataDict.Add(key,box.boxBagData.itemList);//添加进去
        }
        //保存数据
        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData=new GameSaveData();
            saveData.playerMoney = this.PlayerMoney;//保存金钱
            saveData.inventoryDict=new Dictionary<string, List<InventoryItem>>();
            saveData.inventoryDict.Add(playerBag.name, playerBag.itemList);//保存背包数据
            foreach (var item in boxDataDict) //保存所有箱子数据
            {
                saveData.inventoryDict.Add(item.Key, item.Value);
            }
            return saveData;
        }
        //读取数据
        public void RestoreData(GameSaveData saveData)
        {
            this.PlayerMoney = saveData.playerMoney;//拿到金钱
            playerBag = Instantiate(playerBagTemp);
            playerBag.itemList = saveData.inventoryDict[playerBag.name];//拿到背包数据
            foreach (var item in saveData.inventoryDict) //拿到箱子的数据
            {
                if (boxDataDict.ContainsKey(item.Key)) 
                {
                    boxDataDict[item.Key]=item.Value;
                }
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);//刷新背包UI
        }
    }
}
