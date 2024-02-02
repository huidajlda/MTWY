using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MFarm.Save;
public class SaveSlotUI : MonoBehaviour
{
    public Text dataTime, dataScene;//��Ϸʱ�����Ϸ����
    private Button currentButton;//��ǰ��ť
    private DataSlot currentData;//��ǰ��Ϸ����
    private int Index=>transform.GetSiblingIndex();//��ť�����
    private void Awake()
    {
        currentButton= GetComponent<Button>();
        currentButton.onClick.AddListener(LoadGameData);
    }
    private void OnEnable()
    {
        SetupSlotUI();
    }
    //�����ı�����
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
            dataTime.text = "������绹û��ʼ";
            dataScene.text = "��;��δ��ʼ";
        }
    }
    //������Ϸ����
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
