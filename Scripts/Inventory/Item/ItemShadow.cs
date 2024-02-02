using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Inventory 
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemShadow : MonoBehaviour
    {
        public SpriteRenderer itemSprite;//����ľ�����Ⱦ��(ֱ���ڴ��ڸ�ֵ)
        private SpriteRenderer shadowSprite;//������Ӱ�ľ�����Ⱦ��
        private void Awake()
        {
            shadowSprite = GetComponent<SpriteRenderer>();
        }
        private void Start()
        {
            shadowSprite.sprite = itemSprite.sprite;
            shadowSprite.color = new Color(0, 0, 0, 0.3f);//������ͼƬ�ĳ�ȫ�ڰ�͸������Ϊ��ӰͼƬ
        }
    }
}