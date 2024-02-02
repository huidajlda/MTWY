using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SceneSoundList_SO", menuName = "Sound/SceneSoundList")]
public class SceneSoundList_SO : ScriptableObject
{
    public List<SceneSoundItem> sceneSoundList;//场景音乐的列表
    //根据场景名称返回该场景音乐
    public SceneSoundItem GetSceneSoundItem(string name) 
    {
        return sceneSoundList.Find(s => s.sceneName == name);
    }
}
//场景音乐类
[System.Serializable]
public class SceneSoundItem
{
    [SceneName]public string sceneName;//场景名称
    public SoundName ambient;//环境音
    public SoundName music;//音乐
}
