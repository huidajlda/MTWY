using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
namespace MFarm.Save 
{
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        private List<ISaveable> saveableList=new List<ISaveable>();//�������ݵĽӿ��б�
        public List<DataSlot> dataSlots = new List<DataSlot>(new DataSlot[3]);//����3���ս�����
        private string jsonFalder;//�����ļ�·��
        private int currentDataIndex;//��ʱ����
        protected override void Awake()
        {
            base.Awake();
            jsonFalder = Application.persistentDataPath + "/SAVE DATA/";//�����·���´���SAVE DATA�ļ�
            ReadSaveData();
        }
        private void OnEnable()
        {
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;//��ʼ����Ϸ���¼�
            EventHandler.EndGameEvent += OnEndGameEvent;//������Ϸ���¼�
        }
        private void OnDisable()
        {
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;//��ʼ����Ϸ���¼�
            EventHandler.EndGameEvent -= OnEndGameEvent;//������Ϸ���¼�
        }

        private void OnEndGameEvent()
        {
            Save(currentDataIndex);
        }

        //��ʼ����Ϸ�ķ���
        private void OnStartNewGameEvent(int index)
        {
            currentDataIndex = index;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))//����I���棬O��ȡ
                Save(currentDataIndex);
            if (Input.GetKeyDown(KeyCode.O))//����I���棬O��ȡ
                Load(currentDataIndex);

        }
        //ע��ӿڵķ���
        public void RegisterSaveable(ISaveable saveable) 
        {
            if (!saveableList.Contains(saveable)) //�б��в�����
                saveableList.Add(saveable);//��ӽ��б�
        }
        //��ȡ��Ϸ���ȵķ���
        private void ReadSaveData() 
        {
            if (Directory.Exists(jsonFalder)) //����·��
            {
                for (int i = 0; i < dataSlots.Count; i++) 
                {
                    var resultPth = jsonFalder + "data" + i + ".json";
                    if (File.Exists(resultPth)) 
                    {
                        var stringData=File.ReadAllText(resultPth);
                        var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);
                        dataSlots[i]=jsonData;
                    }
                }
            }
        }
        //����(index����UI�ж�Ӧ��3����Ϸ���Ȱ�ť�����)
        private void Save(int index) 
        {
            DataSlot data=new DataSlot();
            //ѭ��ÿһ�������������ӵ���ǰ���������ֵ䵱��(����GUID��ֵ:�ӿڷ��صı�������)
            foreach (var saveable in saveableList) 
            {
                data.dataDict.Add(saveable.GUID, saveable.GenerateSaveData());
            }
            dataSlots[index]=data;
            //д���ļ�
            var resultPath = jsonFalder + "data" + index + ".json";//����·��
            //Formatting.Indented���ļ��з��б�׼���Ű�(�ɶ���ǿ)��û�о���һ�����ݶ���һ��(ʵ�ʿ���)
            var jsonData = JsonConvert.SerializeObject(dataSlots[index],Formatting.Indented);//���л�
            if (!File.Exists(resultPath)) //��ǰ·��������
            {
                Directory.CreateDirectory(jsonFalder);
            }
            File.WriteAllText(resultPath, jsonData);//д���ļ�
        }
        //��ȡ����
        public void Load(int index) 
        {
            currentDataIndex = index;
            var resultPath= jsonFalder + "data" + index + ".json";//�õ�����·����һ��Ҫ�ͱ����һ����
            var stringData=File.ReadAllText(resultPath);//��ȡ�ļ�����
            var jsonData=JsonConvert.DeserializeObject<DataSlot>(stringData);//�����л�ΪDataSlot����
            //ѭ��ÿһ������,�����ȡ��Ӧ������
            foreach (var saveable in saveableList) 
            {
                saveable.RestoreData(jsonData.dataDict[saveable.GUID]);
            }
        }
    }
}
