using DG.Tweening;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]//ȷ��һ����������
public class ItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;//������Ⱦ���
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();//��ȡ������Ⱦ���
    }
    //��ɫ�𽥻ָ�
    public void FadeIn() 
    {
        Color targetColor = new Color(1, 1, 1, 1);//0~1��ʾrgba��0~255
        //һ���Ǳ仯����ɫ��Ŀ��ֵ��һ���Ǳ仯ʱ��
        spriteRenderer.DOColor(targetColor, Settings.fabeDuration);
    }
    //��ɫ��͸��
    public void FadeOut()
    {
        Color targetColor = new Color(1, 1, 1, Settings.targetAlpha);
        //һ���Ǳ仯����ɫ��Ŀ��ֵ��һ���Ǳ仯ʱ��
        spriteRenderer.DOColor(targetColor, Settings.fabeDuration);
    }
}
