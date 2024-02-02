using Cinemachine;//调用虚拟摄像机的命名空间
using Unity.VisualScripting;
using UnityEngine;
public class SwitchBounds : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.AfterSceneUnloadEvent += SwitchConfinerShape;//将设置边界的方法注册进切换场景后的事件
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneUnloadEvent -= SwitchConfinerShape;
    }
    //设置边界方法
    private void SwitchConfinerShape() 
    {
        //通过查找标签的方法，来找到有边界碰撞器的物体,拿到边界碰撞器组件
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();
        //获取自身虚拟摄像机上的边界功能组件
        CinemachineConfiner confiner=GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = confinerShape;//将边界赋值上去
        //清除缓存的方法,每次设置完边界不清除缓存的话，在切换场景后再一次设置边界时就会失败
        confiner.InvalidatePathCache();
    }
}
