//����ö�ٱ�����д������ű��������
public enum ItemType 
{   //���ӣ���Ʒ���Ҿ�
    Seed,Commodity,Furniture,
    //����
    HoeTool,//�ڵصĹ���
    ChopTool,//�����Ĺ���
    BreakTool,//��ʯͷ�Ĺ���
    ReapTool,//��ݵĹ���
    WaterTool,//��ˮ
    CollectTool,//�ո�Ĺ���
    ReapableScenery//�Ӳ�
}
//���ӵ�����
public enum SlotType 
{
    Bag,//����
    Box,//����
    Shop//�̵�
}
//����������
public enum InventoryLocation 
{
    Player,//������ϵı���
    Box,//���ӵı���
}
//�������Ͳ���
public enum PartType 
{
    None,//ʲôҲû�ã�Ĭ�ϣ�
    Carry,//������Ʒ�����𶯻���
    Hoe,//����
    Water,//��ˮ
    Collect,//�������ռ�
    Chop,//��ͷ
    Break,//��ʯͷ
    Reap//����
}
//��������ֵ�����
public enum PartName 
{
    Body,
    Hair,
    Arm,
    Tool,
}
//���ڵ�ö��
public enum Season 
{
    ����,����,����,����
}
//���������
public enum GridType 
{
    Diggable,//���ڿ�
    DropItem,//�ɶ���
    PlaceFurniture,//�ɷ��üҾ�
    NPCObstacle//NPC�ϰ�
}
//������Ч��ö��
public enum ParticaleEffectType 
{
    None,//��
    LeaveFalling01,//��һ������������Ч
    LeaveFalling02,//�ڶ�������������Ч
    Rock,//ʯͷ����Ч
    ReapableScenery//��ݵ���Ч
}
//��Ϸ״̬
public enum GameState 
{
    GamePlay,//����������Ϸ
    Pause//��ͣ
}
//�ƹ��л�
public enum LightShift 
{
    Morning,Night//���Ϻ�����
}
//��������(ֱ�ӽ�����������Ϊö��ֵѡ��)
public enum SoundName 
{
    none,FootStepSoft,FootStepHard,//�գ���·����Ч
    Axe,Pickaxe,Hoe,Reap,Water,Basket,Chop,//��Ӧʹ�ù��ߵ���Ч
    Pickup,Plant,TreeFalling,Rustle,//��ժ����ֲ����������ݵ���Ч
    AmbientCountryside1, AmbientCountryside2,//������
    MusicCalm1, MusicCalm2,MusicCalm3,MusicCalm4, MusicCalm5,MusicCalm6, AmbientIndoor1//��������
} 