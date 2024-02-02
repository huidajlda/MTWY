using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using MFarm.Inventory;
using System.Collections;

public class AnimatorOverride : MonoBehaviour
{
    private Animator[] animators;//子物体身上的动画控制器
    public SpriteRenderer holdItem;//举起图片的渲染器
    [Header("各部分动画列表")]
    public List<AnimatorType> animatorsTypes;//动画播放类的列表
    //动画名称和动画控制器的字典
    private Dictionary<string,Animator> animatorNameDict=new Dictionary<string,Animator>();
    private void Awake()
    {
        Animator[] animators = GetComponentsInChildren<Animator>();//获取子物体的动画控制器
        foreach (var anim in animators) 
        {
            animatorNameDict.Add(anim.name, anim);//把名字和控制器一一对应添加进字典
        }
    }
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;//注册事件
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;//收割物品后在人物头顶显示物品的方法
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;//注销事件
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
    }
    //收割物品后在人物头顶显示物品的方法
    private void OnHarvestAtPlayerPosition(int ID)
    {
        Sprite itemSprite=InventoryManager.Instance.GetItemDetails(ID).itemOnWorldSprite;//获取物品图片
        if (holdItem.enabled == false)
            StartCoroutine(ShowItem(itemSprite));
    }
    //收获后显示1s物品的协程
    private IEnumerator ShowItem(Sprite itemSprite) 
    {
        holdItem.sprite = itemSprite;
        holdItem.enabled = true;
        yield return new WaitForSeconds(1f);//等待1s
        holdItem.enabled = false;//关闭图片
    }
    //切换场景前
    private void OnBeforeSceneUnloadEvent()
    {
        holdItem.enabled = false;
        SwitchAnimator(PartType.None);//恢复默认动画
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        PartType currentType = itemDetails.itemType switch//根据不同物品来获取其对应的动画类型
        {
            //不同工具返回以后补全
            ItemType.Seed => PartType.Carry,
            ItemType.Commodity=>PartType.Carry,
            ItemType.HoeTool=>PartType.Hoe,//挖坑工具类型
            ItemType.WaterTool=>PartType.Water,//浇水
            ItemType.CollectTool=>PartType.Collect,//收集工具类型
            ItemType.ChopTool=>PartType.Chop,//斧头
            ItemType.BreakTool=>PartType.Break,//十字镐
            ItemType.ReapTool=>PartType.Reap,
            ItemType.Furniture=> PartType.Carry,//建造物品
            _ =>PartType.None,
        };
        if (isSelected == false)//没有呗选中
        {
            currentType = PartType.None;//回到默认动画
            holdItem.enabled = false;//隐藏举起物品的渲染器
        }
        else 
        {
            if (currentType == PartType.Carry) //如果动画类型是举起的
            {
                holdItem.sprite=itemDetails.itemOnWorldSprite;
                holdItem.enabled = true;
            }
            else holdItem.enabled=false;
        }
        SwitchAnimator(currentType);//传入动画类型切换动画
    }
    //切换动画的方法
    private void SwitchAnimator(PartType partType) 
    {
        foreach (var item in animatorsTypes) 
        {
            if (item.partType == partType) 
            {//切换动画
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.OverrideController;
            }
            else if (item.partType == PartType.None)
            {
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.OverrideController;
            }
        }
    }
}
