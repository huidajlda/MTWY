using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [Header("音乐数据库")]
    public SoundDetailsList_SO soundDetailsData;//音乐数据库
    public SceneSoundList_SO sceneSoundData;//场景音乐数据库
    [Header("播放组件")]
    public AudioSource ambientSource;
    public AudioSource gameSource;
    private Coroutine soundRoutine;//播放音乐的协程变量
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;//audioMixer变量
    [Header("轨道快照")]
    public AudioMixerSnapshot normalSnapShot;//正常的快照
    public AudioMixerSnapshot ambientSnapShot;//环境音的快照
    public AudioMixerSnapshot muteSnapShot;//静音的快照
    private float musicTransitionSecond = 8f;//快照转换时间
    public float MusicStartSecond => Random.Range(5f, 10f);//随机5~10s后开始播放背景音乐
    private void OnEnable()
    {
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//注册加载场景后的事件
        EventHandler.PlaySoundEvent += OnPlaySoundEvent;//播放音效的事件
        EventHandler.EndGameEvent += OnEndGameEvent;//结束游戏的事件
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
        muteSnapShot.TransitionTo(1f);//静音
    }

    private void OnPlaySoundEvent(SoundName soundName)
    {
        var soundDetails=soundDetailsData.GetSoundDetails(soundName);
        if (soundDetails != null) 
            EventHandler.CallInitSoundEffect(soundDetails);
    }

    //加载场景后播放音乐的方法
    private void OnAfterSceneUnloadEvent()
    {
        string currentScene=SceneManager.GetActiveScene().name;//拿到当前场景名称
        SceneSoundItem sceneSound=sceneSoundData.GetSceneSoundItem(currentScene);//拿到场景的音乐信息
        if (sceneSound == null)
            return;
        SoundDetails ambient = soundDetailsData.GetSoundDetails(sceneSound.ambient);//拿到场景环境音的详情
        SoundDetails music = soundDetailsData.GetSoundDetails(sceneSound.music);//拿到场景的音乐详情
        if (soundRoutine != null)
            StopCoroutine(soundRoutine);//停止播放音乐的协程
        soundRoutine = StartCoroutine(PlaySoundRoutine(music, ambient));//开启协程
    }
    //播放背景音乐和环境音的协程
    private IEnumerator PlaySoundRoutine(SoundDetails music, SoundDetails ambient) 
    {
        if (music != null && ambient != null) 
        {
            PlayAmbientClip(ambient,1f);//先播放环境音
            yield return new WaitForSeconds(MusicStartSecond);//等待5~10s后播放音乐
            PlayMusicClip(music,musicTransitionSecond);//播放背景音乐
        }
    }
    //播放场景音乐的方法
    private void PlayMusicClip(SoundDetails soundDetails,float transitionTime) 
    {
        audioMixer.SetFloat("MusicVolume", ConvertSoundVolume(soundDetails.soundVolume));//设置值
        gameSource.clip = soundDetails.soundClip;//赋值音乐片段
        if(gameSource.isActiveAndEnabled)//是否激活
            gameSource.Play();//播放音乐
        normalSnapShot.TransitionTo(transitionTime);//8s时间转换到这个快照
    }
    //播放场景环境音
    private void PlayAmbientClip(SoundDetails soundDetails, float transitionTime)
    {
        audioMixer.SetFloat("AmbientVolume", ConvertSoundVolume(soundDetails.soundVolume));//设置值
        ambientSource.clip = soundDetails.soundClip;//赋值音乐片段
        if (ambientSource.isActiveAndEnabled)//是否激活
            ambientSource.Play();//播放音乐
        ambientSnapShot.TransitionTo(transitionTime);//1s转换到这个快照
    }
    //转换音量数值（类里面设置的是0.1~1.5，而Mixer里面是-80~20db）
    private float ConvertSoundVolume(float amount) 
    {
        return (amount * 100 - 80);
    }
    //修改主声道的音量
    public void SetMasterVolume(float value) 
    {
        audioMixer.SetFloat("MasterVolume", (value * 100 - 80));
    }
}
