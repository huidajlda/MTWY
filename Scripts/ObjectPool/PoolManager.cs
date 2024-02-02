using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;//粒子特效的对象列表
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();//对象池列表
    private Queue<GameObject> soundQueue=new Queue<GameObject>();//音效队列
    private void Start()
    {
        CreatePool();//生成对象池
    }
    private void OnEnable()
    {
        EventHandler.ParticleEffectEvent += OnParticleEffectEvent;//播放粒子特效的事件
        EventHandler.InitSoundEffect += InitSoundEffect;//播放声音的事件
    }
    private void OnDisable()
    {
        EventHandler.ParticleEffectEvent -= OnParticleEffectEvent;
        EventHandler.InitSoundEffect -= InitSoundEffect;
    }
    //创建对象池的方法
    private void CreatePool()
    {
        foreach (GameObject item in poolPrefabs) //为每一个物体创建一个对象池
        {
            Transform parent=new GameObject(item.name).transform;//创建一个父物体（让unity列表不看起来这么乱）
            parent.SetParent(transform);//设置为PoolManager的子物体
            //创建对象池的每一个参数
            //创建对象池要做的工作(执行的方法)
            //从对象池拿东西的时候执行的方法
            //释放对象池的时候执行的方法
            //销毁对象池的时候执行的方法
            //是否检测对象池的列表
            //容量默认是10
            //最大容量
            var newPool = new ObjectPool<GameObject>(//创建对象池
                ()=>Instantiate(item,parent),
                e => { e.SetActive(true); },//e表示对象池里面的每个物体
                e => { e.SetActive(false); },
                e => { Destroy(e); }
            );
            poolEffectList.Add(newPool);//将对象池添加到对象池列表
        }
    }
    //播放粒子特效的方法
    private void OnParticleEffectEvent(ParticaleEffectType effectType, Vector3 pos)
    {
        //WORKFLOW:根据特效补全
        ObjectPool<GameObject> objPool = effectType switch
        {
            ParticaleEffectType.LeaveFalling01 => poolEffectList[0],
            ParticaleEffectType.LeaveFalling02 => poolEffectList[1],
            ParticaleEffectType.Rock => poolEffectList[2],
            ParticaleEffectType.ReapableScenery=> poolEffectList[3],
            _ => null
        };
        GameObject obj=objPool.Get();//取出对象池中的对象
        obj.transform.position = pos;
        StartCoroutine(ReleaseRoutine(objPool, obj));
    }
    //回收对象池取出的对象的协程(对象池，取出的对象)
    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> pool,GameObject obj) 
    {
        yield return new WaitForSeconds(1.5f);//等待1.5s
        pool.Release(obj);//回收取出的对象
    }
    //创建音效对象池
    private void CreateSoundPool() 
    {
        var parent = new GameObject(poolPrefabs[4].name).transform;
        parent.SetParent(transform);
        //预先生成20个音效预设体
        for (int i = 0; i < 20; i++) 
        {
            GameObject newObj = Instantiate(poolPrefabs[4], parent);
            newObj.SetActive(false);//失活
            soundQueue.Enqueue(newObj);//将创建的音效压进队列
        }
    }
    //从音效对象池取出音效
    private GameObject GetPoolObject()
    {
        if (soundQueue.Count < 2)//队列中音效数量小于2
            CreateSoundPool();//再创建一次对象池
        return soundQueue.Dequeue();//取出队列中的第一个
    }
    //初始化音效
    private void InitSoundEffect(SoundDetails soundDetails) 
    {
        var obj=GetPoolObject();//拿出音效对象池的一个
        obj.GetComponent<Sound>().SetSound(soundDetails);//设置音效
        obj.SetActive(true);//激活
        StartCoroutine(DisableSound(obj, soundDetails.soundClip.length));
    }
    //回收音效的协程
    private IEnumerator DisableSound(GameObject obj,float duration) 
    {
        yield return new WaitForSeconds(duration);//等待播放完成
        obj.SetActive(false);//失活
        soundQueue.Enqueue(obj);//压回队列    
    }
}
