using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;//������Ч�Ķ����б�
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();//������б�
    private Queue<GameObject> soundQueue=new Queue<GameObject>();//��Ч����
    private void Start()
    {
        CreatePool();//���ɶ����
    }
    private void OnEnable()
    {
        EventHandler.ParticleEffectEvent += OnParticleEffectEvent;//����������Ч���¼�
        EventHandler.InitSoundEffect += InitSoundEffect;//�����������¼�
    }
    private void OnDisable()
    {
        EventHandler.ParticleEffectEvent -= OnParticleEffectEvent;
        EventHandler.InitSoundEffect -= InitSoundEffect;
    }
    //��������صķ���
    private void CreatePool()
    {
        foreach (GameObject item in poolPrefabs) //Ϊÿһ�����崴��һ�������
        {
            Transform parent=new GameObject(item.name).transform;//����һ�������壨��unity�б���������ô�ң�
            parent.SetParent(transform);//����ΪPoolManager��������
            //��������ص�ÿһ������
            //���������Ҫ���Ĺ���(ִ�еķ���)
            //�Ӷ�����ö�����ʱ��ִ�еķ���
            //�ͷŶ���ص�ʱ��ִ�еķ���
            //���ٶ���ص�ʱ��ִ�еķ���
            //�Ƿ������ص��б�
            //����Ĭ����10
            //�������
            var newPool = new ObjectPool<GameObject>(//���������
                ()=>Instantiate(item,parent),
                e => { e.SetActive(true); },//e��ʾ����������ÿ������
                e => { e.SetActive(false); },
                e => { Destroy(e); }
            );
            poolEffectList.Add(newPool);//���������ӵ�������б�
        }
    }
    //����������Ч�ķ���
    private void OnParticleEffectEvent(ParticaleEffectType effectType, Vector3 pos)
    {
        //WORKFLOW:������Ч��ȫ
        ObjectPool<GameObject> objPool = effectType switch
        {
            ParticaleEffectType.LeaveFalling01 => poolEffectList[0],
            ParticaleEffectType.LeaveFalling02 => poolEffectList[1],
            ParticaleEffectType.Rock => poolEffectList[2],
            ParticaleEffectType.ReapableScenery=> poolEffectList[3],
            _ => null
        };
        GameObject obj=objPool.Get();//ȡ��������еĶ���
        obj.transform.position = pos;
        StartCoroutine(ReleaseRoutine(objPool, obj));
    }
    //���ն����ȡ���Ķ����Э��(����أ�ȡ���Ķ���)
    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> pool,GameObject obj) 
    {
        yield return new WaitForSeconds(1.5f);//�ȴ�1.5s
        pool.Release(obj);//����ȡ���Ķ���
    }
    //������Ч�����
    private void CreateSoundPool() 
    {
        var parent = new GameObject(poolPrefabs[4].name).transform;
        parent.SetParent(transform);
        //Ԥ������20����ЧԤ����
        for (int i = 0; i < 20; i++) 
        {
            GameObject newObj = Instantiate(poolPrefabs[4], parent);
            newObj.SetActive(false);//ʧ��
            soundQueue.Enqueue(newObj);//����������Чѹ������
        }
    }
    //����Ч�����ȡ����Ч
    private GameObject GetPoolObject()
    {
        if (soundQueue.Count < 2)//��������Ч����С��2
            CreateSoundPool();//�ٴ���һ�ζ����
        return soundQueue.Dequeue();//ȡ�������еĵ�һ��
    }
    //��ʼ����Ч
    private void InitSoundEffect(SoundDetails soundDetails) 
    {
        var obj=GetPoolObject();//�ó���Ч����ص�һ��
        obj.GetComponent<Sound>().SetSound(soundDetails);//������Ч
        obj.SetActive(true);//����
        StartCoroutine(DisableSound(obj, soundDetails.soundClip.length));
    }
    //������Ч��Э��
    private IEnumerator DisableSound(GameObject obj,float duration) 
    {
        yield return new WaitForSeconds(duration);//�ȴ��������
        obj.SetActive(false);//ʧ��
        soundQueue.Enqueue(obj);//ѹ�ض���    
    }
}
