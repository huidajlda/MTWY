using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//������Ϣ���飨���������׵ȣ�
[CreateAssetMenu(fileName = "SoundDetailsList_SO", menuName = "Sound/SoundDetailsList")]
public class SoundDetailsList_SO : ScriptableObject
{
    public List<SoundDetails> soundDetailsList;//����������б�
    //�����������������Ҷ�Ӧ��������
    public SoundDetails GetSoundDetails(SoundName name) 
    {
        return soundDetailsList.Find(s => s.soundName == name);
    }
}
//����������
[System.Serializable]
public class SoundDetails 
{
    public SoundName soundName;//���ֵ�����
    public AudioClip soundClip;//����Ƭ��
    [Range(0.1f,1.5f)]//�ڷ�Χ�����
    public float soundPitchMin;//�������ֵ
    [Range(0.1f, 1.5f)]//�ڷ�Χ�����
    public float soundPitchMax;//�������ֵ
    [Range(0.1f, 1f)]//�ڷ�Χ�����
    public float soundVolume;//������С
}
