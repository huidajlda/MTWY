using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;//audiosource组件
    //设置音效的方法
    public void SetSound(SoundDetails soundDetails) 
    {
        audioSource.clip = soundDetails.soundClip;//设置播放音效
        audioSource.volume = soundDetails.soundVolume;//设置音量
        audioSource.pitch = Random.Range(soundDetails.soundPitchMin, soundDetails.soundPitchMax);//设置音阶
    }

}
