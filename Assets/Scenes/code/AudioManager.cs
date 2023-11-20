using AudioStrategy;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using static Global;

public class AudioManager : Singleton<AudioManager>
{
    [Header("���ֿ�")]
    public SceneSoundList_SO AudioItemList;

    public SoundList_SO soundList;

    [Header("Audio Source")]
    public AudioSource ambientSource;   //������Դ

    public AudioSource bgSource;    //������Դ
    public AudioSource efffectSource;   //Ч����

    //��Ϊ�������������ӹ���Լ�����ʹ��һЩרҵ�߼�����ƵЧ��
    [Header("snapshots")]
    public AudioMixerSnapshot normalSnapShot;

    public AudioMixerSnapshot muteSnapShot;
    public AudioMixerSnapshot onlyBgSnapShot;

    //�����ֵ�
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

    public void playCustomAudio(SoundType type)    //�����Զ�����Ч
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

        //SoundDetails ambient = soundList.getSoundDetails(item.ambientSoundType); ��ӻ���������������� Ŀǰ��û��
        SoundDetails bg = soundList.getSoundDetails(item.bgSoundType);

        playBgMusicClip(bg);    //���ű�������
    }

    private void stopAudio(Scene scene)
    {
        bgSource.Stop();    //��Ƶ��ֹͣ���ţ����ҵ�ǰ���ŵ�λ�ý�������Ϊ��Ƶ�Ŀ�ͷ��
    }

    /// <summary>
    /// ���ű�������
    /// </summary>
    /// <param name="soundDetails"></param>
    private void playBgMusicClip(SoundDetails soundDetails)
    {
        bgSource.volume = soundDetails.volumeSize;
        bgSource.clip = soundDetails.soundClip;
        if (bgSource.isActiveAndEnabled) bgSource.Play();
    }

    /// <summary>
    /// ���Ż�����
    /// </summary>
    /// <param name="soundDetails"></param>
    private void playAmbientMusicClip(SoundDetails soundDetails)
    {
        ambientSource.clip = soundDetails.soundClip;
        if (ambientSource.isActiveAndEnabled) ambientSource.Play();
    }
}