using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerItemFader : MonoBehaviour
{
    //进入触发器
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //树桩和树叶都挂载了渐变的脚本，而触发器在它们的父物体上面
        ItemFader[] faders=collision.GetComponentsInChildren<ItemFader>();
        if (faders.Length > 0) 
        {//循环树桩和树叶
            foreach (var item in faders) 
            {
                item.FadeOut();//变透明
            }
        }
    }
    //离开触发器
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
