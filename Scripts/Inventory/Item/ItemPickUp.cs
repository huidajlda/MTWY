using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Inventory //拾取也要使用物品数据，所以也放在这个命名空间里
{
    public class ItemPickUp : MonoBehaviour
    {
        //进入触发器（碰撞拾取物品）
        private void OnTriggerEnter2D(Collider2D other)
        {
            Item item=other.GetComponent<Item>();
            if (item != null) //碰撞物品身上有没有Item脚本
            {
                if (item.itemDetails.canPickedup) //该物品可以拾取
                {
                    //因为拾取的是地图上的物品，所以第二个参数是true，需要销毁物体
                    InventoryManager.Instance.AddItem(item, true);//调用背包中添加物品的方法
                    //播放音效
                    EventHandler.CallPlaySoundEvent(SoundName.Pickup);
                }
            }
        }
    }
}
