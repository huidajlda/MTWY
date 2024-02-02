using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Inventory 
{
    public class ItemBounce : MonoBehaviour
    {
        private Transform spriteTrans;//��Ʒ��λ��
        private BoxCollider2D coll;//�������ײ��
        public float gravity = -3.5f;//�½��ٶ�
        private bool isGround;//�Ƿ��Ѿ����
        private float distance;//�ɶ�Զ�ľ���
        private Vector2 direction;//���з���
        private Vector3 targetPos;//Ŀ������
        private void Awake()
        {
            spriteTrans = transform.GetChild(0);//��ȡ�����λ��
            coll = GetComponent<BoxCollider2D>();//��ȡ�������ײ��
            coll.enabled = false;//�ӳ�ȥ�����йر���ײ��
        }
        private void Update()
        {
            Bounce();//�ɳ�ȥ�ķ���
        }
        //�����ӳ���Ʒ�ķ���(��ʼ��)
        public void InitBounceItem(Vector3 target, Vector2 dir) 
        {
            coll.enabled = false;//ȷ����ײ��ر���
            direction = dir;//����
            targetPos=target;//Ŀ��
            distance=Vector3.Distance(target, transform.position);//����
            spriteTrans.position += Vector3.up*1.5f;//1.5f����Ϊ����λ����Playrͷ�ϣ��պ���1.5����
        }
        //�׳���Ʒ�ķ���
        private void Bounce ()
        {
            isGround=spriteTrans.position.y<=transform.position.y;//y<=0�ʹ����ڵ�����
            if (Vector3.Distance(transform.position, targetPos) > 0.1f) //����û�зɵ�
            {
                transform.position += (Vector3)direction*distance*-gravity*Time.deltaTime;//���Ͽ���Ŀ���
            }
            if (!isGround)
            {
                spriteTrans.position += Vector3.up * gravity * Time.deltaTime;//�����½�
            }
            else //����
            {
                spriteTrans.position=transform.position;//��λ���غ�
                coll.enabled = true;//����ײ��
            }
        }
    }

}
