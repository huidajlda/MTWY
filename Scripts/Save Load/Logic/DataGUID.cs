using UnityEngine;
[ExecuteAlways]//һֱ���е�����
//����ű�������������Ҫ�洢�Ĵ�����
public class DataGUID : MonoBehaviour
{
    public string guid;//��ʶ��������
    private void Awake()
    {
        if (guid == string.Empty) 
        {
            guid=System.Guid.NewGuid().ToString();//����GUID
        }
    }
}
