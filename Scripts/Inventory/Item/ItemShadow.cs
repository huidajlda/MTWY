using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Inventory 
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemShadow : MonoBehaviour
    {
        public SpriteRenderer itemSprite;//物体的精灵渲染器(直接在窗口赋值)
        private SpriteRenderer shadowSprite;//物体阴影的精灵渲染器
        private void Awake()
        {
            shadowSprite = GetComponent<SpriteRenderer>();
        }
        private void Start()
        {
            shadowSprite.sprite = itemSprite.sprite;
            shadowSprite.color = new Color(0, 0, 0, 0.3f);//将物体图片改成全黑半透明就作为阴影图片
        }
    }
}