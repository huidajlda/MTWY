using Cinemachine;//��������������������ռ�
using Unity.VisualScripting;
using UnityEngine;
public class SwitchBounds : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.AfterSceneUnloadEvent += SwitchConfinerShape;//�����ñ߽�ķ���ע����л���������¼�
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneUnloadEvent -= SwitchConfinerShape;
    }
    //���ñ߽緽��
    private void SwitchConfinerShape() 
    {
        //ͨ�����ұ�ǩ�ķ��������ҵ��б߽���ײ��������,�õ��߽���ײ�����
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();
        //��ȡ��������������ϵı߽繦�����
        CinemachineConfiner confiner=GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = confinerShape;//���߽縳ֵ��ȥ
        //�������ķ���,ÿ��������߽粻�������Ļ������л���������һ�����ñ߽�ʱ�ͻ�ʧ��
        confiner.InvalidatePathCache();
    }
}
