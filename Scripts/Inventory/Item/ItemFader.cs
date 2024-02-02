using DG.Tweening;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]//确保一定有这个组件
public class ItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;//精灵渲染组件
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();//获取精灵渲染组件
    }
    //颜色逐渐恢复
    public void FadeIn() 
    {
        Color targetColor = new Color(1, 1, 1, 1);//0~1表示rgba的0~255
        //一个是变化到颜色的目标值，一个是变化时间
        spriteRenderer.DOColor(targetColor, Settings.fabeDuration);
    }
    //颜色逐渐透明
    public void FadeOut()
    {
        Color targetColor = new Color(1, 1, 1, Settings.targetAlpha);
        //一个是变化到颜色的目标值，一个是变化时间
        spriteRenderer.DOColor(targetColor, Settings.fabeDuration);
    }
}
