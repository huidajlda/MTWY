using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Inventory 
{
    public class ItemBounce : MonoBehaviour
    {
        private Transform spriteTrans;//物品的位置
        private BoxCollider2D coll;//物体的碰撞体
        public float gravity = -3.5f;//下降速度
        private bool isGround;//是否已经落地
        private float distance;//飞多远的距离
        private Vector2 direction;//飞行方向
        private Vector3 targetPos;//目的坐标
        private void Awake()
        {
            spriteTrans = transform.GetChild(0);//获取物体的位置
            coll = GetComponent<BoxCollider2D>();//获取物体的碰撞体
            coll.enabled = false;//扔出去过程中关闭碰撞体
        }
        private void Update()
        {
            Bounce();//飞出去的方法
        }
        //生成扔出物品的方法(初始化)
        public void InitBounceItem(Vector3 target, Vector2 dir) 
        {
            coll.enabled = false;//确保碰撞体关闭了
            direction = dir;//方向
            targetPos=target;//目标
            distance=Vector3.Distance(target, transform.position);//距离
            spriteTrans.position += Vector3.up*1.5f;//1.5f是因为生成位置在Playr头上，刚好是1.5左右
        }
        //抛出物品的方法
        private void Bounce ()
        {
            isGround=spriteTrans.position.y<=transform.position.y;//y<=0就代表在地面了
            if (Vector3.Distance(transform.position, targetPos) > 0.1f) //代表还没有飞到
            {
                transform.position += (Vector3)direction*distance*-gravity*Time.deltaTime;//不断靠近目标点
            }
            if (!isGround)
            {
                spriteTrans.position += Vector3.up * gravity * Time.deltaTime;//不断下降
            }
            else //到了
            {
                spriteTrans.position=transform.position;//让位置重合
                coll.enabled = true;//打开碰撞体
            }
        }
    }

}
