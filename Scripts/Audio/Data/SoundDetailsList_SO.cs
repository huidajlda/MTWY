using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//声音信息详情（音量，音阶等）
[CreateAssetMenu(fileName = "SoundDetailsList_SO", menuName = "Sound/SoundDetailsList")]
public class SoundDetailsList_SO : ScriptableObject
{
    public List<SoundDetails> soundDetailsList;//声音详情的列表
    //根据声音名称来查找对应声音详情
    public SoundDetails GetSoundDetails(SoundName name) 
    {
        return soundDetailsList.Find(s => s.soundName == name);
    }
}
//声音详情类
[System.Serializable]
public class SoundDetails 
{
    public SoundName soundName;//音乐的名称
    public AudioClip soundClip;//声音片段
    [Range(0.1f,1.5f)]//在范围内随机
    public float soundPitchMin;//音阶最低值
    [Range(0.1f, 1.5f)]//在范围内随机
    public float soundPitchMax;//音阶最高值
    [Range(0.1f, 1f)]//在范围内随机
    public float soundVolume;//音量大小
}
