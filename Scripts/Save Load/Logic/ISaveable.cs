namespace MFarm.Save 
{
    public interface ISaveable
    {
        string GUID { get; }//存储标识
        void RegisterSaveable() 
        {
            SaveLoadManager.Instance.RegisterSaveable(this);//添加进接口列表
        }
        GameSaveData GenerateSaveData();//储存数据
        void RestoreData(GameSaveData saveData);//读取数据
    }
}
