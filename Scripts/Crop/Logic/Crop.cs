using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{//执行果实收割的逻辑
    public CropDetails cropDetails;
    public TileDetails tileDetails;//地图瓦片信息
    private int harvestActionCount;//收集工具的使用次数(计数器)
    private Animator anim;//种子挂载的动画
    public bool CanHarvest => tileDetails.growthDays >= cropDetails.TotalGrowthDays;//种子是否成熟
    private Transform PlayerTransform=>FindObjectOfType<Player>().transform;//玩家位置
    //执行收集工具的行为
    public void ProcessToolAction(ItemDetails tool,TileDetails tile)
    {
        tileDetails = tile;
        int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemId);//收集工具需要使用的次数
        if (requireActionCount == -1) return;//工具不可用
        anim=GetComponentInChildren<Animator>();//拿到动画
        //点击计数器
        if (harvestActionCount < requireActionCount) //工具使用次数不足
        {
            harvestActionCount++;
            //判断是否有动画(树木的摇晃)
            if (anim != null && cropDetails.hasAimation) 
            {
                if (PlayerTransform.position.x < transform.position.x)//判断是在左边还是右边
                    anim.SetTrigger("RotateRight");//触发摇晃的动画
                else
                    anim.SetTrigger("RotateLeft");
            }
            //播放粒子特效
            if(cropDetails.hasParticalEffect)
                EventHandler.CallParticleEffectEvent(cropDetails.effectType, transform.position + cropDetails.effectPos);
            //播放声音
            if (cropDetails.soundEffect != SoundName.none) //有音效
            {
                //获取音效详情
                var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(cropDetails.soundEffect);
                EventHandler.CallInitSoundEffect(soundDetails);
            }
        }
        if (harvestActionCount >= requireActionCount) //足够
        {
            if (cropDetails.generateAtPlayerPosition||!cropDetails.hasAimation) //在人物头顶生成
            {
                //生成农作物
                SpawnHarvestItems();
            }
            else if (cropDetails.hasAimation) //判断有没有动画
            {
                if (PlayerTransform.position.x < transform.position.x)
                    anim.SetTrigger("FallingRight");//触发树木倒下的动画
                else
                    anim.SetTrigger("FallingLeft");
                EventHandler.CallPlaySoundEvent(SoundName.TreeFalling);//播放树到的音效
                StartCoroutine(HarvestAfterAnimation());//执行完动画后生成果实的协程
            }
        }
    }
    //执行完动画后生成果实的协程
    private IEnumerator HarvestAfterAnimation() 
    {
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("End")) //判断默认图层中的End名称的动画是否在播放
        {
            yield return null;//播放后跳出循环
        }
        SpawnHarvestItems();//收获果实
        //转换物体
        if (cropDetails.transferItemID > 0)
            CreateTransferCrop();
    }
    //生成转换的物体
    private void CreateTransferCrop() 
    {
        tileDetails.seedItemID = cropDetails.transferItemID;//地图里种子的ID改变成转换物体的ID
        tileDetails.daysSinceLastHarvest = -1;//不能收获了
        tileDetails.growthDays = 0;//重新成长
        EventHandler.CallRefreshCurrentMap();//刷新地图
    }
    //生成农作物
    public void SpawnHarvestItems() 
    {
        //生成农作物的循环
        for (int i = 0; i < cropDetails.producedItemID.Length; i++) //循环该作物可生成物品的列表
        {
            int amountToProduce;//生成作物的数量
            if (cropDetails.producedMaxAmount[i] == cropDetails.producedMinAmount[i]) //如果生成的最大最小相等
            {//代表该作物只生成指定数量
                amountToProduce = cropDetails.producedMaxAmount[i];
            }
            else //生成最大最小范围内的随机数量
            {
                amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i]+1);
            }
            //执行生成指定数量的物品
            for (int j = 0; j < amountToProduce; j++) 
            {
                if (cropDetails.generateAtPlayerPosition)//是否在玩家背包里位置生成
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                else//在世界地图生成
                {
                    //判断是在左侧还是右侧（生成物品的方向）
                    var dirX = transform.position.x > PlayerTransform.position.x ? 1 : -1;
                    //在地图上生成物品的位置（一定范围内随机）
                    var spawnPos = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                            transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 0);
                    EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);//生成物品
                }
                    

            }
        }
        if (tileDetails != null) //瓦片信息不为空
        {
            tileDetails.daysSinceLastHarvest++;//收获次数
            //是否可用重复生长且可重生几次
            if (cropDetails.daysToRegrow > 0 && tileDetails.daysSinceLastHarvest < cropDetails.regrowTime)
            {
                tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;//回到成长阶段
                //刷新地图
                EventHandler.CallRefreshCurrentMap();
            }
            else //不可重复生长
            {
                tileDetails.daysSinceLastHarvest = -1;
                tileDetails.seedItemID = -1;//没有种子
                //看功能需求
                //tileDetails.daysSinceDug = -1;//坑也一起消失
            }
            Destroy(gameObject);
        }
    }
}
