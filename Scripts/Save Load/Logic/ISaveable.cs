namespace MFarm.Save 
{
    public interface ISaveable
    {
        string GUID { get; }//�洢��ʶ
        void RegisterSaveable() 
        {
            SaveLoadManager.Instance.RegisterSaveable(this);//��ӽ��ӿ��б�
        }
        GameSaveData GenerateSaveData();//��������
        void RestoreData(GameSaveData saveData);//��ȡ����
    }
}
