using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using MFarm.Inventory;
using System.Collections;

public class AnimatorOverride : MonoBehaviour
{
    private Animator[] animators;//���������ϵĶ���������
    public SpriteRenderer holdItem;//����ͼƬ����Ⱦ��
    [Header("�����ֶ����б�")]
    public List<AnimatorType> animatorsTypes;//������������б�
    //�������ƺͶ������������ֵ�
    private Dictionary<string,Animator> animatorNameDict=new Dictionary<string,Animator>();
    private void Awake()
    {
        Animator[] animators = GetComponentsInChildren<Animator>();//��ȡ������Ķ���������
        foreach (var anim in animators) 
        {
            animatorNameDict.Add(anim.name, anim);//�����ֺͿ�����һһ��Ӧ��ӽ��ֵ�
        }
    }
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;//ע���¼�
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;//�ո���Ʒ��������ͷ����ʾ��Ʒ�ķ���
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;//ע���¼�
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
    }
    //�ո���Ʒ��������ͷ����ʾ��Ʒ�ķ���
    private void OnHarvestAtPlayerPosition(int ID)
    {
        Sprite itemSprite=InventoryManager.Instance.GetItemDetails(ID).itemOnWorldSprite;//��ȡ��ƷͼƬ
        if (holdItem.enabled == false)
            StartCoroutine(ShowItem(itemSprite));
    }
    //�ջ����ʾ1s��Ʒ��Э��
    private IEnumerator ShowItem(Sprite itemSprite) 
    {
        holdItem.sprite = itemSprite;
        holdItem.enabled = true;
        yield return new WaitForSeconds(1f);//�ȴ�1s
        holdItem.enabled = false;//�ر�ͼƬ
    }
    //�л�����ǰ
    private void OnBeforeSceneUnloadEvent()
    {
        holdItem.enabled = false;
        SwitchAnimator(PartType.None);//�ָ�Ĭ�϶���
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        PartType currentType = itemDetails.itemType switch//���ݲ�ͬ��Ʒ����ȡ���Ӧ�Ķ�������
        {
            //��ͬ���߷����Ժ�ȫ
            ItemType.Seed => PartType.Carry,
            ItemType.Commodity=>PartType.Carry,
            ItemType.HoeTool=>PartType.Hoe,//�ڿӹ�������
            ItemType.WaterTool=>PartType.Water,//��ˮ
            ItemType.CollectTool=>PartType.Collect,//�ռ���������
            ItemType.ChopTool=>PartType.Chop,//��ͷ
            ItemType.BreakTool=>PartType.Break,//ʮ�ָ�
            ItemType.ReapTool=>PartType.Reap,
            ItemType.Furniture=> PartType.Carry,//������Ʒ
            _ =>PartType.None,
        };
        if (isSelected == false)//û����ѡ��
        {
            currentType = PartType.None;//�ص�Ĭ�϶���
            holdItem.enabled = false;//���ؾ�����Ʒ����Ⱦ��
        }
        else 
        {
            if (currentType == PartType.Carry) //������������Ǿ����
            {
                holdItem.sprite=itemDetails.itemOnWorldSprite;
                holdItem.enabled = true;
            }
            else holdItem.enabled=false;
        }
        SwitchAnimator(currentType);//���붯�������л�����
    }
    //�л������ķ���
    private void SwitchAnimator(PartType partType) 
    {
        foreach (var item in animatorsTypes) 
        {
            if (item.partType == partType) 
            {//�л�����
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.OverrideController;
            }
            else if (item.partType == PartType.None)
            {
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.OverrideController;
            }
        }
    }
}
