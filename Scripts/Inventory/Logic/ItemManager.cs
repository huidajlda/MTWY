using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MFarm.Save;

namespace MFarm.Inventory 
{
    public class ItemManager : MonoBehaviour,ISaveable
    {
        public Item itemPrefab;//��ʼ����Ʒ�Ľű�
        public Item bounceItemPrefab;//�ӳ�ȥ������Ԥ����
        private Transform itemParent;//��Ʒ�����ڸö���(��������)
        private Transform PlayerTransform => FindObjectOfType<Player>().transform;//���λ��

        public string GUID => GetComponent<DataGUID>().guid;

        //���泡���������Ʒ(�������ƣ����л���Ʒ�б�)
        private Dictionary<string,List<SceneItem>> sceneItemDict=new Dictionary<string, List<SceneItem>>();
        //���泡������Ľ�����Ʒ
        private Dictionary<string, List<SceneFurniture>> sceneFurnitureDict = new Dictionary<string, List<SceneFurniture>>();
        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;//ע���¼�(������Ʒ)
            EventHandler.DropItemEvent += OnDropItemEvent;//�ӳ���Ʒ���¼�
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//��ȡ��Ʒ������ķ���
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;//�����¼�����
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;//��ʼ����Ϸ���¼�
        }
        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();//��ӽ��ӿ��б�
        }
        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;//ע���¼�
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        }
        //��ʼ����Ϸ�ķ���
        private void OnStartNewGameEvent(int obj)
        {
            sceneItemDict.Clear();
            sceneFurnitureDict.Clear();
        }

        //���췽��
        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            //�õ���ͼ����
            BluePrintDetails bluePrint=InventoryManager.Instance.bluPrintData.GetBluePrintDetails(ID);
            //��������
            var buildItem = Instantiate(bluePrint.buildPrefab, mousePos, Quaternion.identity, itemParent);
            if (buildItem.GetComponent<Box>()) //����Ǻ��ӣ��������һ�����
            {
                buildItem.GetComponent<Box>().index = InventoryManager.Instance.BoxDataAmount;
                buildItem.GetComponent<Box>().InitBox(buildItem.GetComponent<Box>().index);//���ӳ�ʼ���ķ���
            }
        }

        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItem();//���˷������볡���л�ǰ���¼����ڳ����л�ǰ���泡����Ʒ������
            GetAllSceneFurniture();//�����л�ǰ��������
        }
        private void OnAfterSceneUnloadEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;//��ȡ������
            RecreateAllItem();//�������غ���¼������������ɳ�����Ʒ�ķ���
            RebuildFurniture();//�������غ��������ɽ�����Ʒ
        }
        //������Ʒ�ķ���
        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
            var item = Instantiate(bounceItemPrefab, pos, Quaternion.identity, itemParent);//������Ʒ
            item.itemID = ID;
            item.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);//��Ʒ����
        }
        //�ӳ���Ʒ�ķ���
        private void OnDropItemEvent(int ID, Vector3 mousePos,ItemType itemType)
        {
            if (itemType == ItemType.Seed) return;
            var item = Instantiate(bounceItemPrefab, PlayerTransform.position, Quaternion.identity, itemParent);//������Ʒ
            item.itemID = ID;
            var dir = (mousePos - PlayerTransform.position).normalized;//����
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos, dir);//�����ӳ�ȥ������
        }
        //���浱ǰ��������Ʒ����
        private void GetAllSceneItem() 
        {
            List<SceneItem> currentSceneItems=new List<SceneItem>();//��ʱ����Ʒ�б�
            foreach (var item in FindObjectsOfType<Item>()) //ѭ����ǰ�������е���Ʒ
            {
                SceneItem  sceneItem =new SceneItem //��ʼ��������Ʒ����
                { 
                    itemID=item.itemID,//��ƷID
                    position=new SerializableVector3(item.transform.position)//���л�������
                };
                currentSceneItems.Add(sceneItem);//��ӵ���Ʒ�б���
            }
            //�ж��ֵ�����û�иĳ���������
            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItems;//����ǰ��Ʒ�б�ֵ������
            }
            else //�ֵ���û�иĳ�������Ϣ
            {
                //���������ƺ͵�ǰ��Ʒ�б���Ϣ��ӽ�ȥ
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItems);
            }
        }
        //�������ɵ�ǰ��������Ʒ
        private void RecreateAllItem()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();//��ʱ��Ʒ�б�
            //TryGetValue���Բ��Ҷ�Ӧ�������Ƶ����ݣ��оͻ᷵�ص���ʱ������
            if (sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItems)) 
            {
                if (currentSceneItems != null) //���ص���Ʒ���ݲ�Ϊ��
                {
                    foreach (var item in FindObjectsOfType<Item>()) //����ճ����������������
                    {
                        Destroy(item.gameObject);
                    }
                    foreach (var item in currentSceneItems) 
                    {
                        //������Ʒ
                        Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                        newItem.Init(item.itemID);//��ʼ����Ʒ
                    }
                }
            }
        }
        //���浱ǰ�����Ľ�����Ʒ����
        private void GetAllSceneFurniture() 
        {
            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();//��ʱ�Ľ�����Ʒ�б�
            foreach (var item in FindObjectsOfType<Furniture>()) //ѭ����ǰ�������е���Ʒ
            {
                SceneFurniture sceneFurniture= new SceneFurniture //��ʼ��������Ʒ����
                {
                    itemID = item.itemID,//��ƷID
                    position = new SerializableVector3(item.transform.position)//���л�������
                };
                if(item.GetComponent<Box>())//����Ǻ��ӣ�����������
                    sceneFurniture.boxIndex=item.GetComponent<Box>().index;
                currentSceneFurniture.Add(sceneFurniture);//��ӵ���Ʒ�б���
            }
            //�ж��ֵ�����û�иĳ���������
            if (sceneFurnitureDict.ContainsKey(SceneManager.GetActiveScene().name))
                sceneFurnitureDict[SceneManager.GetActiveScene().name] = currentSceneFurniture;//����ǰ��Ʒ�б�ֵ������
            else //�ֵ���û�иĳ�������Ϣ
            {
                //���������ƺ͵�ǰ��Ʒ�б���Ϣ��ӽ�ȥ
                sceneFurnitureDict.Add(SceneManager.GetActiveScene().name, currentSceneFurniture);
            }
        }
        //�������ɵ�ǰ�����Ľ�����Ʒ
        private void RebuildFurniture() 
        {
            List<SceneFurniture> currentSceneFurniture=new List<SceneFurniture>();//��ʱ��Ʒ�б�
            //TryGetValue���Բ��Ҷ�Ӧ�������Ƶ����ݣ��оͻ᷵�ص���ʱ������
            if (sceneFurnitureDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneFurniture))
            {
                if (currentSceneFurniture != null) //���ص���Ʒ���ݲ�Ϊ��
                {   //ѭ����������
                    foreach (SceneFurniture sceneFurniture in currentSceneFurniture) 
                    {   //������Ʒ
                        //�õ���ͼ����
                        BluePrintDetails bluePrint = InventoryManager.Instance.bluPrintData.GetBluePrintDetails(sceneFurniture.itemID);
                        //��������
                        var buildItem = Instantiate(bluePrint.buildPrefab, sceneFurniture.position.ToVector3(), Quaternion.identity, itemParent);
                        if (buildItem.GetComponent<Box>()) //����Ǻ��ӣ��������һ�����
                        {
                            buildItem.GetComponent<Box>().InitBox(sceneFurniture.boxIndex);
                        }
                    }
                }
            }
        }
        //��������
        public GameSaveData GenerateSaveData()
        {   //�ڳ���û��ж��ʱ�����û�е��ã���û�취�õ����ݣ�����Ҫ���õ�����
            GetAllSceneItem();//���г�������Ʒ
            GetAllSceneFurniture();//���г����ļҾ�,ִ������Ӧ�������ֵ�����ɺ���
            GameSaveData saveData = new GameSaveData();
            saveData.sceneItemDict=this.sceneItemDict;
            saveData.sceneFurnitureDict=this.sceneFurnitureDict;
            return saveData;
        }
        //��ȡ����
        public void RestoreData(GameSaveData saveData)
        {
            this.sceneItemDict=saveData.sceneItemDict;
            this.sceneFurnitureDict=saveData.sceneFurnitureDict;
            //ˢ�³�������Ʒ�Ҿ�
            RecreateAllItem();
            RebuildFurniture();
        }
    }
}
