using MFarm.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    //更新背包UI的事件(背包类型，背包数据)
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    //调用事件的方法
    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        //判断事件是否为空，不为空就会调用所有注册的方法
        UpdateInventoryUI?.Invoke(location, list);
    }
    //场景上生成物品的事件(物品ID，生成位置)
    public static event Action<int, Vector3> InstantiateItemInScene;
    //调用事件的方法
    public static void CallInstantiateItemInScene(int ID, Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(ID, pos);
    }
    //人物从背包中扔出物品的事件
    public static event Action<int, Vector3,ItemType> DropItemEvent;
    public static void CallDropItemEvent(int ID, Vector3 pos, ItemType itemType) 
    {
        DropItemEvent?.Invoke(ID, pos,itemType);
    }
    //根据选中物品切换动画的事件
    public static event Action<ItemDetails, bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    }
    //时间UI的事件
    //分钟和小时的事件(分钟，小时,天，季节)
    public static event Action<int, int,int,Season> GameMinuteEvent;
    public static void CallGameMinuteEvent(int minute, int hour,int day,Season season)
    {
        GameMinuteEvent?.Invoke(minute, hour,day,season);
    }
    //每过一天调用的事件(天数，季节)
    public static event Action<int, Season> GameDayEvent;
    public static void CallGameDayEvent(int day, Season season) 
    {
        GameDayEvent?.Invoke(day, season);
    }

    //年月日季节的事件(小时，日，月，年，季节)
    public static event Action<int, int, int, int, Season> GameDateEvent;
    public static void CallGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        GameDateEvent?.Invoke(hour, day, month, year, season);
    }
    //切换场景的事件
    public static event Action<string, Vector3> TransitionEvent;
    public static void CallTransitionEvent(string sceneName, Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }
    //卸载场景前的调用所需方法事件
    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent() 
    {
        BeforeSceneUnloadEvent?.Invoke();
    }
    //加载场景后的调用所需方法事件
    public static event Action AfterSceneUnloadEvent;
    public static void CallAfterSceneUnloadEvent() 
    {
        AfterSceneUnloadEvent?.Invoke();
    }
    //场景加载后人物移动到指定位置的事件
    public static event Action<Vector3> MovePosition;
    public static void CallMovePosition(Vector3 targetPosition) 
    {
        MovePosition?.Invoke(targetPosition);
    }
    //鼠标点击事件(点按的坐标,当前的物品)
    public static event Action<Vector3, ItemDetails> MouseClickedEvent;
    public static void CallMouseClickedEvent(Vector3 pos, ItemDetails itemDetails) 
    {
        MouseClickedEvent?.Invoke(pos, itemDetails);
    }
    //鼠标点击播放动画，动画播放后实际执行的事件
    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimation;
    public static void CallExecuteActionAfterAnimation(Vector3 pos,ItemDetails itemDetails) 
    {
        ExecuteActionAfterAnimation?.Invoke(pos, itemDetails);
    }
    //种子种植的事件(种子ID,瓦片信息)
    public static event Action<int, TileDetails> PlantSeedEvent;
    public static void CallPlantSeedEvent(int ID, TileDetails tile) 
    {
        PlantSeedEvent?.Invoke(ID, tile);
    }
    //在玩家坐标生成收获物品添加进背包的事件（生成物品的ID）
    public static event Action<int> HarvestAtPlayerPosition;
    public static void CallHarvestAtPlayerPosition(int ID) 
    {
        HarvestAtPlayerPosition?.Invoke(ID);
    }
    //刷新地图的事件
    public static event Action RefreshCurrentMap;
    public static void CallRefreshCurrentMap() 
    {
        RefreshCurrentMap?.Invoke();
    }
    //播放粒子特效的事件(粒子特效的类型，生成位置)
    public static event Action<ParticaleEffectType,Vector3> ParticleEffectEvent;
    public static void CallParticleEffectEvent(ParticaleEffectType effectType,Vector3 pos) 
    {
        ParticleEffectEvent?.Invoke(effectType,pos);
    }
    //一开始在场景中生成农作物的事件
    public static event Action GenerateCropEvent;
    public static void CallGenerateCropEvent() 
    {
        GenerateCropEvent?.Invoke();
    }
    //显示对话的事件
    public static event Action<DialoguePiece> ShowDialogueEvent;
    public static void CallShowDialogueEvent(DialoguePiece piece) 
    {
        ShowDialogueEvent?.Invoke(piece);
    }
    //打开商店的事件(打开其他背包也可以用)
    public static event Action<SlotType, InventoryBag_SO> BaseBagOpenEvent;
    public static void CallBaseBagOpenEvent(SlotType slotType,InventoryBag_SO bag_SO)
    {
        BaseBagOpenEvent?.Invoke(slotType, bag_SO);
    }
    //关闭商店的事件
    public static event Action<SlotType, InventoryBag_SO> BaseBagCloseEvent;
    public static void CallBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bag_SO)
    {
        BaseBagCloseEvent?.Invoke(slotType, bag_SO);
    }
    //更新游戏状态的事件
    public static event Action<GameState> UpdateGameStateEvent;
    public static void CallUpdateGameStateEvent(GameState gameState) 
    {
        UpdateGameStateEvent?.Invoke(gameState);
    }
    //显示交易UI的事件
    public static event Action<ItemDetails, bool> ShowTradeUI;
    public static void CallShowTradeUI(ItemDetails item, bool isSell) 
    {
        ShowTradeUI?.Invoke(item, isSell);
    }
    //建造(图纸ID，坐标)
    public static event Action<int,Vector3> BuildFurnitureEvent;
    public static void CallBuildFurnitureEvent(int ID,Vector3 pos) 
    {
        BuildFurnitureEvent?.Invoke(ID,pos);
    }
    //切换灯光的事件(季节，切换的灯光，时间差)
    public static event Action<Season, LightShift, float> LightShiftChangeEvent;
    public static void CallLightShiftChangeEvent(Season season,LightShift lightShift, float timeDifference) 
    {
        LightShiftChangeEvent?.Invoke(season, lightShift, timeDifference);
    }
    //音效
    public static event Action<SoundDetails> InitSoundEffect;
    public static void CallInitSoundEffect(SoundDetails soundDetails) 
    {
        InitSoundEffect?.Invoke(soundDetails);
    }
    //播放音效的事件
    public static event Action<SoundName> PlaySoundEvent;
    public static void CallPlaySoundEvent(SoundName soundName) 
    {
        PlaySoundEvent?.Invoke(soundName);
    }
    //开启新游戏的事件
    public static event Action<int> StartNewGameEvent;
    public static void CallStartNewGameEvent(int index)
    {
        StartNewGameEvent?.Invoke(index);
    }
    //结束游戏的事件
    public static event Action EndGameEvent;
    public static void CallEndGameEvent() 
    {
        EndGameEvent?.Invoke();
    }
}
