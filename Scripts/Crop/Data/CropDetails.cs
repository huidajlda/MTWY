using UnityEngine;
[System.Serializable]
public class CropDetails
{
    public int seedItemID;//���ӵ�ID
    [Header("��ͬ�׶���Ҫ�ɳ�������")]
    public int[] growthDays;//��ͬ�׶�����ɳ�ʱ�������
    public int TotalGrowthDays//�ɳ�����Ҫ��������
    {
        get 
        {
            int amount = 0;
            foreach (var days in growthDays) 
            {
                amount += days;
            }
            return amount;
        }
    }
    [Header("��ͬ�����׶ε�Ԥ����")]//��ľ�ĳɳ�������Ҫ
    public GameObject[] growthPrefabs;//��ͬ�׶�Ԥ���������
    [Header("��ͬ�׶ε�ͼƬ")]//���ӵĳɳ�������Ҫ
    public Sprite[] growthSprites;
    [Header("����ֲ�ļ���")]
    public Season[] seasons;//�����ڶ�����ڲ���
    [Space]//�������Ŀո񣬸���һ��
    [Header("�ո��")]
    public int[] harvestToolItemID;//�����ո������Ĺ���
    [Header("ʹ�õĹ����ո�ʱ��Ҫʹ�õĴ���")]
    public int[] requireActionCount;//������Ҫ������Ĺ���IDһһ��Ӧ
    [Header("ת����ID(���屻�ո���ɵĶ�����ID)")]
    public int transferItemID;//���ﱻ�ո���ɵĶ�����ID
    [Space]
    [Header("�ո��ʵ����Ϣ")]
    public int[] producedItemID;//�ո��ʵ��ID
    [Header("���ɹ�ʵ����С�������")]
    public int[] producedMinAmount;//�ո��ʵ��������Сֵ��Ҫ��IDһһ��Ӧ��
    public int[] producedMaxAmount;//�ո��ʵ���������ֵ��Ҫ��IDһһ��Ӧ��
    [Header("���ɹ�ʵ�ķ�Χ")]
    public Vector2 spawnRadius;//���ɹ�ʵ�ķ�Χ
    [Header("�ٴ�������ʱ��")]
    public int daysToRegrow;//�ٴ�������ʱ��
    [Header("���ظ���������")]
    public int regrowTime;//���ظ������Ĵ���
    [Header("��ѡ���")]
    public bool generateAtPlayerPosition;//�Ƿ��������������
    public bool hasAimation;//��û�ж���
    public bool hasParticalEffect;//��û��������Ч
    public ParticaleEffectType effectType;//������Ч
    public Vector3 effectPos;//������Ч����
    public SoundName soundEffect;
    //�жϵ�ǰ�ո���Ƿ���ã�����ID��
    public bool CheckToolAvailabele(int toolID) 
    {
        foreach (var tool in harvestToolItemID) //ѭ�������ո�ߵ�����
        {
            if (tool == toolID) 
            {
                return true;//�˹��߿���
            }
        }
        return false;//�˹��߲�����
    }
    //������Ҫʹ�õĴ���
    public int GetTotalRequireCount(int toolID) 
    {
        for (int i = 0; i < harvestToolItemID.Length; i++) //ѭ�������б�
        {
            if (harvestToolItemID[i]==toolID)//�ڿ�ʹ�õĹ����б��ҵ��ù���
                return requireActionCount[i];//���ظù�����Ҫʹ�õĴ���
        }
        return -1;//�ù��߲���ʹ��
    }
}
