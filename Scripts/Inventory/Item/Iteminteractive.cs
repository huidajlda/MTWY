using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iteminteractive : MonoBehaviour
{
    private bool isAnimating;//�����Ƿ����ڲ���
    private WaitForSeconds pause=new WaitForSeconds(0.04f);//�������ʱ��
    private void OnTriggerEnter2D(Collider2D other)//���봥����
    {
        if (!isAnimating) 
        {
            if (other.transform.position.x < transform.position.x)
            {
                //�Է�����࣬����ҡ��
                StartCoroutine(RotateRight());
            }
            else 
            {
                //�Է����Ҳ࣬����ҡ��
                StartCoroutine(RotateLeft());
            }
            EventHandler.CallPlaySoundEvent(SoundName.Rustle);//����ҡ�ε���Ч
        }
    }
    private void OnTriggerExit2D(Collider2D other)//�뿪������
    {
        if (!isAnimating)
        {
            if (other.transform.position.x > transform.position.x)
            {
                //�Է�����࣬����ҡ��
                StartCoroutine(RotateRight());
            }
            else
            {
                //�Է����Ҳ࣬����ҡ��
                StartCoroutine(RotateLeft());
            }
            EventHandler.CallPlaySoundEvent(SoundName.Rustle);//����ҡ�ε���Ч
        }
    }
    //����ҡ�ε�Э��
    private IEnumerator RotateLeft()
    {
        isAnimating = true;//���ڲ��Ŷ���
        for (int i = 0; i < 4; i++) //ѭ��4��(ҡ��4��)
        {
            transform.GetChild(0).Rotate(0, 0, 2);//��ͼƬ��ת2�ȣ�ҡ�Σ�
            yield return pause;//�ȴ�һ��
        }
        for (int i = 0; i < 5; i++) //������ҡ�λ���
        {
            transform.GetChild(0).Rotate(0, 0, -2);//��ͼƬ��ת2�ȣ�ҡ�Σ�
            yield return pause;//�ȴ�һ��
        }
        transform.GetChild(0).Rotate(0, 0, 2);//����ȱʧ��2��
        yield return pause;//�ȴ�һ��
        isAnimating = false;
    }
    //����ҡ�ε�Э��(��������ҡ�ε�Э������-2��2��2��-2)
    private IEnumerator RotateRight()
    {
        isAnimating = true;//���ڲ��Ŷ���
        for (int i = 0; i < 4; i++) //ѭ��4��(ҡ��4��)
        {
            transform.GetChild(0).Rotate(0, 0, -2);//��ͼƬ��ת2�ȣ�ҡ�Σ�
            yield return pause;//�ȴ�һ��
        }
        for (int i = 0; i < 5; i++) //������ҡ�λ���
        {
            transform.GetChild(0).Rotate(0, 0, 2);//��ͼƬ��ת2�ȣ�ҡ�Σ�
            yield return pause;//�ȴ�һ��
        }
        transform.GetChild(0).Rotate(0, 0, -2);//����ȱʧ��2��
        yield return pause;//�ȴ�һ��
        isAnimating = false;
    }
}
