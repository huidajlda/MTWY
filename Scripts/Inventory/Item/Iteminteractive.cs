using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iteminteractive : MonoBehaviour
{
    private bool isAnimating;//动画是否正在播放
    private WaitForSeconds pause=new WaitForSeconds(0.04f);//动画间隔时间
    private void OnTriggerEnter2D(Collider2D other)//进入触发器
    {
        if (!isAnimating) 
        {
            if (other.transform.position.x < transform.position.x)
            {
                //对方在左侧，向右摇晃
                StartCoroutine(RotateRight());
            }
            else 
            {
                //对方在右侧，向左摇晃
                StartCoroutine(RotateLeft());
            }
            EventHandler.CallPlaySoundEvent(SoundName.Rustle);//播放摇晃的音效
        }
    }
    private void OnTriggerExit2D(Collider2D other)//离开触发器
    {
        if (!isAnimating)
        {
            if (other.transform.position.x > transform.position.x)
            {
                //对方在左侧，向右摇晃
                StartCoroutine(RotateRight());
            }
            else
            {
                //对方在右侧，向左摇晃
                StartCoroutine(RotateLeft());
            }
            EventHandler.CallPlaySoundEvent(SoundName.Rustle);//播放摇晃的音效
        }
    }
    //向左摇晃的协程
    private IEnumerator RotateLeft()
    {
        isAnimating = true;//正在播放动画
        for (int i = 0; i < 4; i++) //循环4次(摇晃4次)
        {
            transform.GetChild(0).Rotate(0, 0, 2);//让图片旋转2度（摇晃）
            yield return pause;//等待一下
        }
        for (int i = 0; i < 5; i++) //将度数摇晃回来
        {
            transform.GetChild(0).Rotate(0, 0, -2);//让图片旋转2度（摇晃）
            yield return pause;//等待一下
        }
        transform.GetChild(0).Rotate(0, 0, 2);//补上缺失的2度
        yield return pause;//等待一下
        isAnimating = false;
    }
    //向右摇晃的协程(就是向左摇晃的协程里面-2改2，2改-2)
    private IEnumerator RotateRight()
    {
        isAnimating = true;//正在播放动画
        for (int i = 0; i < 4; i++) //循环4次(摇晃4次)
        {
            transform.GetChild(0).Rotate(0, 0, -2);//让图片旋转2度（摇晃）
            yield return pause;//等待一下
        }
        for (int i = 0; i < 5; i++) //将度数摇晃回来
        {
            transform.GetChild(0).Rotate(0, 0, 2);//让图片旋转2度（摇晃）
            yield return pause;//等待一下
        }
        transform.GetChild(0).Rotate(0, 0, -2);//补上缺失的2度
        yield return pause;//等待一下
        isAnimating = false;
    }
}
