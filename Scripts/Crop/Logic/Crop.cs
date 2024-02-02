using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{//ִ�й�ʵ�ո���߼�
    public CropDetails cropDetails;
    public TileDetails tileDetails;//��ͼ��Ƭ��Ϣ
    private int harvestActionCount;//�ռ����ߵ�ʹ�ô���(������)
    private Animator anim;//���ӹ��صĶ���
    public bool CanHarvest => tileDetails.growthDays >= cropDetails.TotalGrowthDays;//�����Ƿ����
    private Transform PlayerTransform=>FindObjectOfType<Player>().transform;//���λ��
    //ִ���ռ����ߵ���Ϊ
    public void ProcessToolAction(ItemDetails tool,TileDetails tile)
    {
        tileDetails = tile;
        int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemId);//�ռ�������Ҫʹ�õĴ���
        if (requireActionCount == -1) return;//���߲�����
        anim=GetComponentInChildren<Animator>();//�õ�����
        //���������
        if (harvestActionCount < requireActionCount) //����ʹ�ô�������
        {
            harvestActionCount++;
            //�ж��Ƿ��ж���(��ľ��ҡ��)
            if (anim != null && cropDetails.hasAimation) 
            {
                if (PlayerTransform.position.x < transform.position.x)//�ж�������߻����ұ�
                    anim.SetTrigger("RotateRight");//����ҡ�εĶ���
                else
                    anim.SetTrigger("RotateLeft");
            }
            //����������Ч
            if(cropDetails.hasParticalEffect)
                EventHandler.CallParticleEffectEvent(cropDetails.effectType, transform.position + cropDetails.effectPos);
            //��������
            if (cropDetails.soundEffect != SoundName.none) //����Ч
            {
                //��ȡ��Ч����
                var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(cropDetails.soundEffect);
                EventHandler.CallInitSoundEffect(soundDetails);
            }
        }
        if (harvestActionCount >= requireActionCount) //�㹻
        {
            if (cropDetails.generateAtPlayerPosition||!cropDetails.hasAimation) //������ͷ������
            {
                //����ũ����
                SpawnHarvestItems();
            }
            else if (cropDetails.hasAimation) //�ж���û�ж���
            {
                if (PlayerTransform.position.x < transform.position.x)
                    anim.SetTrigger("FallingRight");//������ľ���µĶ���
                else
                    anim.SetTrigger("FallingLeft");
                EventHandler.CallPlaySoundEvent(SoundName.TreeFalling);//������������Ч
                StartCoroutine(HarvestAfterAnimation());//ִ���궯�������ɹ�ʵ��Э��
            }
        }
    }
    //ִ���궯�������ɹ�ʵ��Э��
    private IEnumerator HarvestAfterAnimation() 
    {
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("End")) //�ж�Ĭ��ͼ���е�End���ƵĶ����Ƿ��ڲ���
        {
            yield return null;//���ź�����ѭ��
        }
        SpawnHarvestItems();//�ջ��ʵ
        //ת������
        if (cropDetails.transferItemID > 0)
            CreateTransferCrop();
    }
    //����ת��������
    private void CreateTransferCrop() 
    {
        tileDetails.seedItemID = cropDetails.transferItemID;//��ͼ�����ӵ�ID�ı��ת�������ID
        tileDetails.daysSinceLastHarvest = -1;//�����ջ���
        tileDetails.growthDays = 0;//���³ɳ�
        EventHandler.CallRefreshCurrentMap();//ˢ�µ�ͼ
    }
    //����ũ����
    public void SpawnHarvestItems() 
    {
        //����ũ�����ѭ��
        for (int i = 0; i < cropDetails.producedItemID.Length; i++) //ѭ���������������Ʒ���б�
        {
            int amountToProduce;//�������������
            if (cropDetails.producedMaxAmount[i] == cropDetails.producedMinAmount[i]) //������ɵ������С���
            {//���������ֻ����ָ������
                amountToProduce = cropDetails.producedMaxAmount[i];
            }
            else //���������С��Χ�ڵ��������
            {
                amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i]+1);
            }
            //ִ������ָ����������Ʒ
            for (int j = 0; j < amountToProduce; j++) 
            {
                if (cropDetails.generateAtPlayerPosition)//�Ƿ�����ұ�����λ������
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                else//�������ͼ����
                {
                    //�ж�������໹���Ҳࣨ������Ʒ�ķ���
                    var dirX = transform.position.x > PlayerTransform.position.x ? 1 : -1;
                    //�ڵ�ͼ��������Ʒ��λ�ã�һ����Χ�������
                    var spawnPos = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                            transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 0);
                    EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);//������Ʒ
                }
                    

            }
        }
        if (tileDetails != null) //��Ƭ��Ϣ��Ϊ��
        {
            tileDetails.daysSinceLastHarvest++;//�ջ����
            //�Ƿ�����ظ������ҿ���������
            if (cropDetails.daysToRegrow > 0 && tileDetails.daysSinceLastHarvest < cropDetails.regrowTime)
            {
                tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;//�ص��ɳ��׶�
                //ˢ�µ�ͼ
                EventHandler.CallRefreshCurrentMap();
            }
            else //�����ظ�����
            {
                tileDetails.daysSinceLastHarvest = -1;
                tileDetails.seedItemID = -1;//û������
                //����������
                //tileDetails.daysSinceDug = -1;//��Ҳһ����ʧ
            }
            Destroy(gameObject);
        }
    }
}
