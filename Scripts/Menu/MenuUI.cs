using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject[] panels;//拿到Menu下面所有的Panel
    //切换面板
    public void SwitchPanel(int index) 
    {
        for (int i = 0; i < panels.Length; i++) 
        {
            if (i == index) 
            {
                panels[i].transform.SetAsLastSibling();//这样面板就回去到最下面了
            }
        }
    }
    //退出游戏
    public void ExitGame() 
    {
        Application.Quit();
    }
}
