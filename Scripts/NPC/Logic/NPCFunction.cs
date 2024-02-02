using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFunction : MonoBehaviour
{
    public InventoryBag_SO shopData;//商店背包数据
    private bool isOpen;//是否打开
    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape)) //背包打开且按下了ESC
        {
            //关闭背包
            CloseShop();
        }
    }
    //打开商店的方法(NPC要打开商店，就调用这个)
    public void OpenShop() 
    {
        isOpen = true;//商店打开
        EventHandler.CallBaseBagOpenEvent(SlotType.Shop, shopData);//呼叫商店打开事件
        EventHandler.CallUpdateGameStateEvent(GameState.Pause);//打开商店后，暂停游戏(玩家不能移动)
    }
    //关闭商店的方法
    public void CloseShop() 
    {
        isOpen = false;
        EventHandler.CallBaseBagCloseEvent(SlotType.Shop, shopData);//呼叫关闭
        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);//关闭商店后，正常游戏
    }
}
