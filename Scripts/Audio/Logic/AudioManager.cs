using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [Header("�������ݿ�")]
    public SoundDetailsList_SO soundDetailsData;//�������ݿ�
    public SceneSoundList_SO sceneSoundData;//�����������ݿ�
    [Header("�������")]
    public AudioSource ambientSource;
    public AudioSource gameSource;
    private Coroutine soundRoutine;//�������ֵ�Э�̱���
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;//audioMixer����
    [Header("�������")]
    public AudioMixerSnapshot normalSnapShot;//�����Ŀ���
    public AudioMixerSnapshot ambientSnapShot;//�������Ŀ���
    public AudioMixerSnapshot muteSnapShot;//�����Ŀ���
    private float musicTransitionSecond = 8f;//����ת��ʱ��
    public float MusicStartSecond => Random.Range(5f, 10f);//���5~10s��ʼ���ű�������
    private void OnEnable()
    {
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//ע����س�������¼�
        EventHandler.PlaySoundEvent += OnPlaySoundEvent;//������Ч���¼�
        EventHandler.EndGameEvent += OnEndGameEvent;//������Ϸ���¼�
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
        EventHandler.PlaySoundEvent -= OnPlaySoundEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }

    private void OnEndGameEvent()
    {
        if(soundRoutine!=null)
            StopCoroutine(soundRoutine);
        muteSnapShot.TransitionTo(1f);//����
    }

    private void OnPlaySoundEvent(SoundName soundName)
    {
        var soundDetails=soundDetailsData.GetSoundDetails(soundName);
        if (soundDetails != null) 
            EventHandler.CallInitSoundEffect(soundDetails);
    }

    //���س����󲥷����ֵķ���
    private void OnAfterSceneUnloadEvent()
    {
        string currentScene=SceneManager.GetActiveScene().name;//�õ���ǰ��������
        SceneSoundItem sceneSound=sceneSoundData.GetSceneSoundItem(currentScene);//�õ�������������Ϣ
        if (sceneSound == null)
            return;
        SoundDetails ambient = soundDetailsData.GetSoundDetails(sceneSound.ambient);//�õ�����������������
        SoundDetails music = soundDetailsData.GetSoundDetails(sceneSound.music);//�õ���������������
        if (soundRoutine != null)
            StopCoroutine(soundRoutine);//ֹͣ�������ֵ�Э��
        soundRoutine = StartCoroutine(PlaySoundRoutine(music, ambient));//����Э��
    }
    //���ű������ֺͻ�������Э��
    private IEnumerator PlaySoundRoutine(SoundDetails music, SoundDetails ambient) 
    {
        if (music != null && ambient != null) 
        {
            PlayAmbientClip(ambient,1f);//�Ȳ��Ż�����
            yield return new WaitForSeconds(MusicStartSecond);//�ȴ�5~10s�󲥷�����
            PlayMusicClip(music,musicTransitionSecond);//���ű�������
        }
    }
    //���ų������ֵķ���
    private void PlayMusicClip(SoundDetails soundDetails,float transitionTime) 
    {
        audioMixer.SetFloat("MusicVolume", ConvertSoundVolume(soundDetails.soundVolume));//����ֵ
        gameSource.clip = soundDetails.soundClip;//��ֵ����Ƭ��
        if(gameSource.isActiveAndEnabled)//�Ƿ񼤻�
            gameSource.Play();//��������
        normalSnapShot.TransitionTo(transitionTime);//8sʱ��ת�����������
    }
    //���ų���������
    private void PlayAmbientClip(SoundDetails soundDetails, float transitionTime)
    {
        audioMixer.SetFloat("AmbientVolume", ConvertSoundVolume(soundDetails.soundVolume));//����ֵ
        ambientSource.clip = soundDetails.soundClip;//��ֵ����Ƭ��
        if (ambientSource.isActiveAndEnabled)//�Ƿ񼤻�
            ambientSource.Play();//��������
        ambientSnapShot.TransitionTo(transitionTime);//1sת�����������
    }
    //ת��������ֵ�����������õ���0.1~1.5����Mixer������-80~20db��
    private float ConvertSoundVolume(float amount) 
    {
        return (amount * 100 - 80);
    }
    //�޸�������������
    public void SetMasterVolume(float value) 
    {
        audioMixer.SetFloat("MasterVolume", (value * 100 - 80));
    }
}
