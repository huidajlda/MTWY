using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.CropPlant;
namespace MFarm.Inventory //包含在背包的命名空间中
{
    public class Item : MonoBehaviour
    {
        public int itemID;//物品的ID
        private SpriteRenderer spriteRenderer;//精灵渲染器
        public ItemDetails itemDetails;//存储物品详细信息的变量
        private BoxCollider2D coll;//碰撞体
        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();//获取子物体上的精灵渲染器
            coll = GetComponent<BoxCollider2D>();//获取父物体上的碰撞体
        }
        private void Start()
        {
            if (itemID != 0) 
            {
                Init(itemID);
            }
        }
        //初始化物品(生成物品)
        public void Init(int ID) 
        {
            itemID = ID;
            //使用InventoryManager里面的通过ID获取物品详情的方法
            itemDetails = InventoryManager.Instance.GetItemDetails(itemID);
            if (itemDetails != null) //获取不为空
            {
                //赋值地图上显示的图片(用三元运算符是防止忘记设计地图上的图片的物体也能正确显示)
                //有地图上的图片则显示，没有这显示物体本身的图片
                spriteRenderer.sprite = itemDetails.itemOnWorldSprite!=null?itemDetails.itemOnWorldSprite:itemDetails.itemIcon;
                //修改碰撞体的尺寸和位置(因为图片锚点在图片下方中点，不是图片中心)，让其能跟随图片大小变化
                //获取图片的大小
                Vector2 newSize=new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
                coll.size = newSize;//修改碰撞体大小
                coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
            }
            if (itemDetails.itemType == ItemType.ReapableScenery) 
            {
                gameObject.AddComponent<ReapItem>();//添加杂草脚本
                gameObject.GetComponent<ReapItem>().InitCropData(itemDetails.itemId);
                gameObject.AddComponent<Iteminteractive>();//添加人物走过摇晃的脚本
            }  
        }
    }

}
