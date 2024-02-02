using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerItemFader : MonoBehaviour
{
    //���봥����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //��׮����Ҷ�������˽���Ľű����������������ǵĸ���������
        ItemFader[] faders=collision.GetComponentsInChildren<ItemFader>();
        if (faders.Length > 0) 
        {//ѭ����׮����Ҷ
            foreach (var item in faders) 
            {
                item.FadeOut();//��͸��
            }
        }
    }
    //�뿪������
    private void OnTriggerExit2D(Collider2D collision)
    {
        ItemFader[] faders = collision.GetComponentsInChildren<ItemFader>();
        if (faders.Length > 0)
        {
            foreach (var item in faders)
            {
                item.FadeIn();
            }
        }
    }
}
