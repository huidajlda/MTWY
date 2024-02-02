using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
namespace MFarm.Save 
{
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        private List<ISaveable> saveableList=new List<ISaveable>();//储存数据的接口列表
        public List<DataSlot> dataSlots = new List<DataSlot>(new DataSlot[3]);//创建3个空进度条
        private string jsonFalder;//保存文件路径
        private int currentDataIndex;//临时变量
        protected override void Awake()
        {
            base.Awake();
            jsonFalder = Application.persistentDataPath + "/SAVE DATA/";//在这个路径下创建SAVE DATA文件
            ReadSaveData();
        }
        private void OnEnable()
        {
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;//开始新游戏的事件
            EventHandler.EndGameEvent += OnEndGameEvent;//结束游戏的事件
        }
        private void OnDisable()
        {
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;//开始新游戏的事件
            EventHandler.EndGameEvent -= OnEndGameEvent;//结束游戏的事件
        }

        private void OnEndGameEvent()
        {
            Save(currentDataIndex);
        }

        //开始新游戏的方法
        private void OnStartNewGameEvent(int index)
        {
            currentDataIndex = index;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))//测试I保存，O读取
                Save(currentDataIndex);
            if (Input.GetKeyDown(KeyCode.O))//测试I保存，O读取
                Load(currentDataIndex);

        }
        //注册接口的方法
        public void RegisterSaveable(ISaveable saveable) 
        {
            if (!saveableList.Contains(saveable)) //列表中不包含
                saveableList.Add(saveable);//添加进列表
        }
        //读取游戏进度的方法
        private void ReadSaveData() 
        {
            if (Directory.Exists(jsonFalder)) //存在路径
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
        //保存(index就是UI中对应的3个游戏进度按钮的序号)
        private void Save(int index) 
        {
            DataSlot data=new DataSlot();
            //循环每一个保存的数据添加到当前进度条的字典当中(键：GUID，值:接口返回的保存数据)
            foreach (var saveable in saveableList) 
            {
                data.dataDict.Add(saveable.GUID, saveable.GenerateSaveData());
            }
            dataSlots[index]=data;
            //写入文件
            var resultPath = jsonFalder + "data" + index + ".json";//最终路径
            //Formatting.Indented让文件有分行标准化排版(可读性强)，没有就是一堆数据堆在一起(实际开发)
            var jsonData = JsonConvert.SerializeObject(dataSlots[index],Formatting.Indented);//序列化
            if (!File.Exists(resultPath)) //当前路径不存在
            {
                Directory.CreateDirectory(jsonFalder);
            }
            File.WriteAllText(resultPath, jsonData);//写入文件
        }
        //读取进度
        public void Load(int index) 
        {
            currentDataIndex = index;
            var resultPath= jsonFalder + "data" + index + ".json";//拿到最终路径（一定要和保存的一样）
            var stringData=File.ReadAllText(resultPath);//读取文件数据
            var jsonData=JsonConvert.DeserializeObject<DataSlot>(stringData);//反序列化为DataSlot类型
            //循环每一个数据,让其读取对应的数据
            foreach (var saveable in saveableList) 
            {
                saveable.RestoreData(jsonData.dataDict[saveable.GUID]);
            }
        }
    }
}
