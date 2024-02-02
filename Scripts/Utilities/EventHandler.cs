using MFarm.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    //���±���UI���¼�(�������ͣ���������)
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    //�����¼��ķ���
    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        //�ж��¼��Ƿ�Ϊ�գ���Ϊ�վͻ��������ע��ķ���
        UpdateInventoryUI?.Invoke(location, list);
    }
    //������������Ʒ���¼�(��ƷID������λ��)
    public static event Action<int, Vector3> InstantiateItemInScene;
    //�����¼��ķ���
    public static void CallInstantiateItemInScene(int ID, Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(ID, pos);
    }
    //����ӱ������ӳ���Ʒ���¼�
    public static event Action<int, Vector3,ItemType> DropItemEvent;
    public static void CallDropItemEvent(int ID, Vector3 pos, ItemType itemType) 
    {
        DropItemEvent?.Invoke(ID, pos,itemType);
    }
    //����ѡ����Ʒ�л��������¼�
    public static event Action<ItemDetails, bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    }
    //ʱ��UI���¼�
    //���Ӻ�Сʱ���¼�(���ӣ�Сʱ,�죬����)
    public static event Action<int, int,int,Season> GameMinuteEvent;
    public static void CallGameMinuteEvent(int minute, int hour,int day,Season season)
    {
        GameMinuteEvent?.Invoke(minute, hour,day,season);
    }
    //ÿ��һ����õ��¼�(����������)
    public static event Action<int, Season> GameDayEvent;
    public static void CallGameDayEvent(int day, Season season) 
    {
        GameDayEvent?.Invoke(day, season);
    }

    //�����ռ��ڵ��¼�(Сʱ���գ��£��꣬����)
    public static event Action<int, int, int, int, Season> GameDateEvent;
    public static void CallGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        GameDateEvent?.Invoke(hour, day, month, year, season);
    }
    //�л��������¼�
    public static event Action<string, Vector3> TransitionEvent;
    public static void CallTransitionEvent(string sceneName, Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }
    //ж�س���ǰ�ĵ������跽���¼�
    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent() 
    {
        BeforeSceneUnloadEvent?.Invoke();
    }
    //���س�����ĵ������跽���¼�
    public static event Action AfterSceneUnloadEvent;
    public static void CallAfterSceneUnloadEvent() 
    {
        AfterSceneUnloadEvent?.Invoke();
    }
    //�������غ������ƶ���ָ��λ�õ��¼�
    public static event Action<Vector3> MovePosition;
    public static void CallMovePosition(Vector3 targetPosition) 
    {
        MovePosition?.Invoke(targetPosition);
    }
    //������¼�(�㰴������,��ǰ����Ʒ)
    public static event Action<Vector3, ItemDetails> MouseClickedEvent;
    public static void CallMouseClickedEvent(Vector3 pos, ItemDetails itemDetails) 
    {
        MouseClickedEvent?.Invoke(pos, itemDetails);
    }
    //��������Ŷ������������ź�ʵ��ִ�е��¼�
    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimation;
    public static void CallExecuteActionAfterAnimation(Vector3 pos,ItemDetails itemDetails) 
    {
        ExecuteActionAfterAnimation?.Invoke(pos, itemDetails);
    }
    //������ֲ���¼�(����ID,��Ƭ��Ϣ)
    public static event Action<int, TileDetails> PlantSeedEvent;
    public static void CallPlantSeedEvent(int ID, TileDetails tile) 
    {
        PlantSeedEvent?.Invoke(ID, tile);
    }
    //��������������ջ���Ʒ��ӽ��������¼���������Ʒ��ID��
    public static event Action<int> HarvestAtPlayerPosition;
    public static void CallHarvestAtPlayerPosition(int ID) 
    {
        HarvestAtPlayerPosition?.Invoke(ID);
    }
    //ˢ�µ�ͼ���¼�
    public static event Action RefreshCurrentMap;
    public static void CallRefreshCurrentMap() 
    {
        RefreshCurrentMap?.Invoke();
    }
    //����������Ч���¼�(������Ч�����ͣ�����λ��)
    public static event Action<ParticaleEffectType,Vector3> ParticleEffectEvent;
    public static void CallParticleEffectEvent(ParticaleEffectType effectType,Vector3 pos) 
    {
        ParticleEffectEvent?.Invoke(effectType,pos);
    }
    //һ��ʼ�ڳ���������ũ������¼�
    public static event Action GenerateCropEvent;
    public static void CallGenerateCropEvent() 
    {
        GenerateCropEvent?.Invoke();
    }
    //��ʾ�Ի����¼�
    public static event Action<DialoguePiece> ShowDialogueEvent;
    public static void CallShowDialogueEvent(DialoguePiece piece) 
    {
        ShowDialogueEvent?.Invoke(piece);
    }
    //���̵���¼�(����������Ҳ������)
    public static event Action<SlotType, InventoryBag_SO> BaseBagOpenEvent;
    public static void CallBaseBagOpenEvent(SlotType slotType,InventoryBag_SO bag_SO)
    {
        BaseBagOpenEvent?.Invoke(slotType, bag_SO);
    }
    //�ر��̵���¼�
    public static event Action<SlotType, InventoryBag_SO> BaseBagCloseEvent;
    public static void CallBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bag_SO)
    {
        BaseBagCloseEvent?.Invoke(slotType, bag_SO);
    }
    //������Ϸ״̬���¼�
    public static event Action<GameState> UpdateGameStateEvent;
    public static void CallUpdateGameStateEvent(GameState gameState) 
    {
        UpdateGameStateEvent?.Invoke(gameState);
    }
    //��ʾ����UI���¼�
    public static event Action<ItemDetails, bool> ShowTradeUI;
    public static void CallShowTradeUI(ItemDetails item, bool isSell) 
    {
        ShowTradeUI?.Invoke(item, isSell);
    }
    //����(ͼֽID������)
    public static event Action<int,Vector3> BuildFurnitureEvent;
    public static void CallBuildFurnitureEvent(int ID,Vector3 pos) 
    {
        BuildFurnitureEvent?.Invoke(ID,pos);
    }
    //�л��ƹ���¼�(���ڣ��л��ĵƹ⣬ʱ���)
    public static event Action<Season, LightShift, float> LightShiftChangeEvent;
    public static void CallLightShiftChangeEvent(Season season,LightShift lightShift, float timeDifference) 
    {
        LightShiftChangeEvent?.Invoke(season, lightShift, timeDifference);
    }
    //��Ч
    public static event Action<SoundDetails> InitSoundEffect;
    public static void CallInitSoundEffect(SoundDetails soundDetails) 
    {
        InitSoundEffect?.Invoke(soundDetails);
    }
    //������Ч���¼�
    public static event Action<SoundName> PlaySoundEvent;
    public static void CallPlaySoundEvent(SoundName soundName) 
    {
        PlaySoundEvent?.Invoke(soundName);
    }
    //��������Ϸ���¼�
    public static event Action<int> StartNewGameEvent;
    public static void CallStartNewGameEvent(int index)
    {
        StartNewGameEvent?.Invoke(index);
    }
    //������Ϸ���¼�
    public static event Action EndGameEvent;
    public static void CallEndGameEvent() 
    {
        EndGameEvent?.Invoke();
    }
}
