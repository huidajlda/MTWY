using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFunction : MonoBehaviour
{
    public InventoryBag_SO shopData;//�̵걳������
    private bool isOpen;//�Ƿ��
    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape)) //�������Ұ�����ESC
        {
            //�رձ���
            CloseShop();
        }
    }
    //���̵�ķ���(NPCҪ���̵꣬�͵������)
    public void OpenShop() 
    {
        isOpen = true;//�̵��
        EventHandler.CallBaseBagOpenEvent(SlotType.Shop, shopData);//�����̵���¼�
        EventHandler.CallUpdateGameStateEvent(GameState.Pause);//���̵����ͣ��Ϸ(��Ҳ����ƶ�)
    }
    //�ر��̵�ķ���
    public void CloseShop() 
    {
        isOpen = false;
        EventHandler.CallBaseBagCloseEvent(SlotType.Shop, shopData);//���йر�
        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);//�ر��̵��������Ϸ
    }
}
