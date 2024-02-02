using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Save;
//�����и���������Ʒ���ݡ�ʹ�õ���Ʒ�йص����ݶ��ŵ���������ռ���
//�ڱ�Ľű�ʹ��ʱ�����ô������ռ伴�ɣ�����Ҳ����ֱ����Щ�ű���Ҫ�ͱ�������
namespace MFarm.Inventory 
{
    public class InventoryManager : Singleton<InventoryManager>,ISaveable
    {
        [Header("��Ʒ����")]
        public ItemDataList_SO itemDataList_SO;//��Ʒ��������
        [Header("������ͼ")]
        public BluPrintDataList_SO bluPrintData;//������ͼ����
        [Header("��������")]
        public InventoryBag_SO playerBagTemp;//����ģ������
        public InventoryBag_SO playerBag;//��ұ���
        private InventoryBag_SO currentBoxBag;//��ǰ���ӱ���
        [Header("����")]
        public int PlayerMoney;//��ҵĽ�Ǯ
        //�����������ݵ��ֵ�
        private Dictionary<string,List<InventoryItem>> boxDataDict=new Dictionary<string,List<InventoryItem>>();
        public int BoxDataAmount => boxDataDict.Count;//�ֵ�����

        public string GUID => GetComponent<DataGUID>().guid;

        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;//�ӳ���Ʒ���¼�
            EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;//��������������ո���Ʒ���¼�
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;//ע�Ὠ���¼��۳���Ӧ��Ʒ
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;//ע���ͨ�ñ������¼�
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;//��ʼ����Ϸ���¼�
        }
        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        }
        //��ʼ��Ϸ���õķ���
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
            saveable.RegisterSaveable();//��ӽ��ӿ��б�
        }
        //��ͨ�ñ������¼�
        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bag_SO)
        {
            currentBoxBag=bag_SO;//�õ���ǰ�����ӵ�����
        }
        //����۳���Ӧ��Ʒ
        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            RemoveItem(ID,1);//�۳�����ͼֽ
            BluePrintDetails bluePrint=bluPrintData.GetBluePrintDetails(ID);//�õ���ͼ����
            //ѭ����ͼ��Դ�б�
            foreach (var item in bluePrint.resourceItem) 
            {
                RemoveItem(item.itemID, item.itemAmount);//�Ƴ���Ӧ��������Ʒ
            }
        }
        //�ӳ���Ʒ�ķ���
        private void OnDropItemEvent(int ID, Vector3 pos, ItemType itemType)
        {
            RemoveItem(ID, 1);//ÿ���ӳ�һ��������Ʒ
        }
        //�����ո����Ʒ��ӵ�������ϵķ���
        private void OnHarvestAtPlayerPosition(int ID)
        {
            //�Ƿ��Ѿ��и���Ʒ
            var index = GetItemIndexInBag(ID);//�ҵ�����Ʒ���б��е�λ��
            AddItemAtIndex(ID, index, 1);//�������Ϊ1����Ʒ
            //����UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        //ͨ��ID���ҵ���Ʒ����ķ���
        public ItemDetails GetItemDetails(int ID) 
        {
            //Find���򷵻أ����򷵻ؿ�
            return itemDataList_SO.itemDetailsList.Find(i => i.itemId == ID);
        }
        //�����Ʒ�ķ���
        public void AddItem(Item item,bool toDestory) //һ����������Ʒ��һ�����Ƿ�ɾ��
        {
            //�Ƿ��Ѿ��и���Ʒ
            var index = GetItemIndexInBag(item.itemID);//�ҵ�����Ʒ���б��е�λ��
            AddItemAtIndex(item.itemID, index, 1);//����Ϊ1����ͼ��ʰȡ��ֻ���1��
            if (toDestory) 
            {
                Destroy(item.gameObject);//������Ʒ
            }
            //����UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        //��ⱳ���Ƿ��п�λ
        private bool CheckBagCapacity() 
        {
            for (int i = 0; i < playerBag.itemList.Count; i++) 
            {
                if (playerBag.itemList[i].itemID == 0) //itemList�������ID�������Ľṹ�壬Ĭ��Ϊ0
                {//���Ե�idΪ0ʱ˵������ط���û�ж���
                    return true;
                }
            }
            return false;
        }
        //ͨ����ƷID�ҵ������б��е�λ��
        private int GetItemIndexInBag(int ID) 
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID) //˵���Ѿ��и���Ʒ��
                {
                    return i;//������Ʒ���±�
                }
            }
            return -1;//û���ҵ�����Ʒ
        }
        //�ڱ���ָ��λ�������Ʒ
        private void AddItemAtIndex(int ID, int index, int amount) 
        {
            if (index == -1&& CheckBagCapacity()) //������û�д���Ʒ�ұ������п�λ
            {
                var item=new InventoryItem { itemID = ID, itemAmount=amount};//������Ʒ����ʼ��
                for (int i = 0; i < playerBag.itemList.Count; i++)//�ҵ�һ����λ
                {
                    if (playerBag.itemList[i].itemID == 0) //itemList�������ID�������Ľṹ�壬Ĭ��Ϊ0
                    {//���Ե�idΪ0ʱ˵������ط���û�ж���
                        playerBag.itemList[i] = item;//����������Ʒ��Ϣ��ֵ������ո���
                        break;//����ѭ��
                    }
                }
            }
            else//�����������Ʒ
            {
                int currentAmount = playerBag.itemList[index].itemAmount+amount;//���ڵ���Ʒ����
                var item=new InventoryItem { itemID = ID,itemAmount=currentAmount};//�����б��е�����
                playerBag.itemList[index]=item;
            }
        }
        //�ڱ����н�����Ʒ
        public void SwapItem(int fromIndex, int targetIndex) 
        {
            InventoryItem currentItem = playerBag.itemList[fromIndex];//��ǰ��Ʒ����
            InventoryItem targetItem = playerBag.itemList[targetIndex];//Ŀ����Ʒ����
            if (targetItem.itemID != 0) //Ŀ����Ӳ�Ϊ��
            {//������Ʒ
                playerBag.itemList[fromIndex] = targetItem;
                playerBag.itemList[targetIndex] = currentItem;
            }
            else //Ŀ�����Ϊ��
            {
                playerBag.itemList[targetIndex] = currentItem;
                playerBag.itemList[fromIndex]=new InventoryItem();//ԭ���ӱ�Ϊ��
            }
            //ˢ��UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        //�米��������Ʒ
        public void SwapItem(InventoryLocation locationFrom, int fromIndex, InventoryLocation locationTarget, int targetIndex)
        {
            var currentList = GetItemList(locationFrom);//�������ϳ�������Ʒ����
            var targetList = GetItemList(locationTarget);//��ק����Ŀ���б�
            InventoryItem currentItem = currentList[fromIndex];//��ǰ��Ʒ
            if (targetIndex < targetList.Count) //ȷ���ڷ�Χ��
            {
                InventoryItem targetItem = targetList[targetIndex];
                //Ŀ��λ�ò�Ϊ���Ҳ���ͬһ����Ʒ
                if (targetItem.itemID != 0 && currentItem.itemID != targetItem.itemID)
                {
                    //������Ʒ
                    currentList[fromIndex] = targetItem;
                    targetList[targetIndex] = currentItem;
                }
                else if (currentItem.itemID == targetItem.itemID) //��ͬ��Ʒ
                {
                    targetItem.itemAmount += currentItem.itemAmount;//�������
                    targetList[targetIndex] = targetItem;//��������
                    currentList[fromIndex] = new InventoryItem();//ԭ��λ�õ��ڿ�
                }
                else //Ŀ���ǿո���
                {
                    targetList[targetIndex] = currentItem;//��ֵ���ո���
                    currentList[fromIndex]=new InventoryItem();//ԭ��λ�õ��ڿ�
                }
                //����UI
                EventHandler.CallUpdateInventoryUI(locationFrom, currentList);
                EventHandler.CallUpdateInventoryUI(locationTarget, targetList);
            }
        }
        //����λ�÷��ر����������б�
        private List<InventoryItem> GetItemList(InventoryLocation location) 
        {
            return location switch
            {
                InventoryLocation.Player => playerBag.itemList,//������ұ�������
                InventoryLocation.Box => currentBoxBag.itemList,//������������
                _ => null
            };
        }
        //�Ƴ�ָ������������Ʒ�ķ���
        private void RemoveItem(int ID,int removeAmount) 
        {
            var index = GetItemIndexInBag(ID);//��Ʒ�������б��е�����
            if (playerBag.itemList[index].itemAmount > removeAmount) //��������Ʒ������Ҫ�����Ƴ���Ʒ������(С�ڲ�ִ��)
            {
                var amount = playerBag.itemList[index].itemAmount-removeAmount;//ʣ����Ʒ����
                var item = new InventoryItem { itemID = ID, itemAmount = amount };//�����µ���Ʒ��Ϣ
                playerBag.itemList[index] = item;//������Ʒ��Ϣ
            }
            else if (playerBag.itemList[index].itemAmount == removeAmount) 
            {
                var item = new InventoryItem();//һ��������(���)
                playerBag.itemList[index] = item;//������Ϣ
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);//������ҵı���UI
        }
        //������Ʒ
        public void TradeItem(ItemDetails itemDetails,int amount,bool isSellTrade) 
        {
            int cost = itemDetails.itemPrice * amount;//�ܽ��
            //�����Ʒ����λ��
            int index = GetItemIndexInBag(itemDetails.itemId);
            if (isSellTrade) //��
            {
                if (playerBag.itemList[index].itemAmount >= amount) //���������㹻
                {
                    RemoveItem(itemDetails.itemId, amount);//�Ƴ���������Ӧ������
                    cost = (int)(cost * itemDetails.sellPercentage);//����ȥ�ļ�Ǯ
                    PlayerMoney += cost;//���Ͻ�Ǯ
                }
            }
            else if (PlayerMoney - cost >= 0) //���㹻Ǯ��
            {
                if (CheckBagCapacity()) //��ⱳ������
                {
                    AddItemAtIndex(itemDetails.itemId, index, amount);//��ӽ�����
                }
                PlayerMoney -= cost;//��ȥ��Ǯ
            }
            //ˢ��UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        //��⽨����Դ������
        public bool CheckStock(int ID) 
        {
            var bluePrintDetails=bluPrintData.GetBluePrintDetails(ID);//�õ���ͼ����
            //ѭ����ͼ������Ҫ����Դ
            foreach (var resourceItem in bluePrintDetails.resourceItem) 
            {
                var itemStock = playerBag.GetInventoryItem(resourceItem.itemID);//�õ���������Ʒ������
                if (itemStock.itemAmount >= resourceItem.itemAmount)//������������Դ
                    continue;//�ж���һ��
                else return false;
            }
            return true;
        }
        //����key��������������
        public List<InventoryItem> GetBoxDataList(string key) 
        {
            if(boxDataDict.ContainsKey(key))
                return boxDataDict[key];
            return null;
        }
        //����������ӽ��ֵ�
        public void AddBoxDataDic(Box box) 
        {
            var key = box.name + box.index;
            if (!boxDataDict.ContainsKey(key))//�����������
                boxDataDict.Add(key,box.boxBagData.itemList);//��ӽ�ȥ
        }
        //��������
        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData=new GameSaveData();
            saveData.playerMoney = this.PlayerMoney;//�����Ǯ
            saveData.inventoryDict=new Dictionary<string, List<InventoryItem>>();
            saveData.inventoryDict.Add(playerBag.name, playerBag.itemList);//���汳������
            foreach (var item in boxDataDict) //����������������
            {
                saveData.inventoryDict.Add(item.Key, item.Value);
            }
            return saveData;
        }
        //��ȡ����
        public void RestoreData(GameSaveData saveData)
        {
            this.PlayerMoney = saveData.playerMoney;//�õ���Ǯ
            playerBag = Instantiate(playerBagTemp);
            playerBag.itemList = saveData.inventoryDict[playerBag.name];//�õ���������
            foreach (var item in saveData.inventoryDict) //�õ����ӵ�����
            {
                if (boxDataDict.ContainsKey(item.Key)) 
                {
                    boxDataDict[item.Key]=item.Value;
                }
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);//ˢ�±���UI
        }
    }
}
