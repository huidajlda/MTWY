using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;//audiosource���
    //������Ч�ķ���
    public void SetSound(SoundDetails soundDetails) 
    {
        audioSource.clip = soundDetails.soundClip;//���ò�����Ч
        audioSource.volume = soundDetails.soundVolume;//��������
        audioSource.pitch = Random.Range(soundDetails.soundPitchMin, soundDetails.soundPitchMax);//��������
    }

}
