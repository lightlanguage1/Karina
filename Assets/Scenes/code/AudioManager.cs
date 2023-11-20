using AudioStrategy;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using static Global;

public class AudioManager : Singleton<AudioManager>
{
    [Header("音乐库")]
    public SceneSoundList_SO AudioItemList;

    public SoundList_SO soundList;

    [Header("Audio Source")]
    public AudioSource ambientSource;   //环境音源

    public AudioSource bgSource;    //背景音源
    public AudioSource efffectSource;   //效果音

    //作为主轨道方便控制子轨道以及可以使用一些专业高级的音频效果
    [Header("snapshots")]
    public AudioMixerSnapshot normalSnapShot;

    public AudioMixerSnapshot muteSnapShot;
    public AudioMixerSnapshot onlyBgSnapShot;

    //策略字典
    private Dictionary<SoundType, IAudioStrategy> audioStrategyDic;

    protected override void Awake()
    {
        base.Awake();
        audioStrategyDic = new Dictionary<SoundType, IAudioStrategy>
        {
            {SoundType.Walk,new WalkAudioStrategy()},
            {SoundType.MoveCursor,new MoveCusorAudioStrategy()},
            {SoundType.Click,new ButtonClickedAudioStrategy()},
            {SoundType.Daji,new DajiSkillAudioStrategy()},
            {SoundType.Xuli,new XuliSkillAudioStrategy()}
        };
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += playBgAudio;
        SceneManager.sceneUnloaded += stopAudio;
        EventHandler.onSoundChange += playCustomAudio;
    }

    public void playCustomAudio(SoundType type)    //播放自定义音效
    {
        if (audioStrategyDic.TryGetValue(type, out var audioStrategy))
        {
            SoundDetails soundDetails = soundList.getSoundDetails(type);
            audioStrategy.PlayAudio(GetAudioSource(type), soundDetails);
        }
    }

    private AudioSource GetAudioSource(SoundType type)
    {
        switch (type)
        {
            case SoundType.None: return null;
            case SoundType.Walk: return efffectSource;
            case SoundType.MoveCursor: return efffectSource;
            case SoundType.Click: return efffectSource;
            case SoundType.Daji: return efffectSource;
            case SoundType.Xuli: return efffectSource;
            case SoundType.StartMenu: return bgSource;
            case SoundType.Main1L: return bgSource;
            case SoundType.Battle: return bgSource;
        }
        return null;
    }

    private void playBgAudio(Scene scene, LoadSceneMode mode)
    {
        SceneSoundItem item = AudioItemList.getSceneSoundItem(scene.name);
        if (item == null) return;

        //SoundDetails ambient = soundList.getSoundDetails(item.ambientSoundType); 添加环境音例如风声雨声 目前还没有
        SoundDetails bg = soundList.getSoundDetails(item.bgSoundType);

        playBgMusicClip(bg);    //播放背景音乐
    }

    private void stopAudio(Scene scene)
    {
        bgSource.Stop();    //音频将停止播放，并且当前播放的位置将被重置为音频的开头。
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="soundDetails"></param>
    private void playBgMusicClip(SoundDetails soundDetails)
    {
        bgSource.volume = soundDetails.volumeSize;
        bgSource.clip = soundDetails.soundClip;
        if (bgSource.isActiveAndEnabled) bgSource.Play();
    }

    /// <summary>
    /// 播放环境音
    /// </summary>
    /// <param name="soundDetails"></param>
    private void playAmbientMusicClip(SoundDetails soundDetails)
    {
        ambientSource.clip = soundDetails.soundClip;
        if (ambientSource.isActiveAndEnabled) ambientSource.Play();
    }
}