using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MFarm.Save;
public class SaveSlotUI : MonoBehaviour
{
    public Text dataTime, dataScene;//游戏时间和游戏场景
    private Button currentButton;//当前按钮
    private DataSlot currentData;//当前游戏进度
    private int Index=>transform.GetSiblingIndex();//按钮的序号
    private void Awake()
    {
        currentButton= GetComponent<Button>();
        currentButton.onClick.AddListener(LoadGameData);
    }
    private void OnEnable()
    {
        SetupSlotUI();
    }
    //设置文本内容
    private void SetupSlotUI() 
    {
        currentData = SaveLoadManager.Instance.dataSlots[Index];
        if (currentData != null)
        {
            dataTime.text = currentData.DataTime;
            dataScene.text = currentData.DataScene;
        }
        else 
        {
            dataTime.text = "这个世界还没开始";
            dataScene.text = "旅途还未开始";
        }
    }
    //加载游戏数据
    private void LoadGameData() 
    {
        if (currentData != null)
        {
            SaveLoadManager.Instance.Load(Index);
        }
        else 
        {
            EventHandler.CallStartNewGameEvent(Index);
        }
    }
}
