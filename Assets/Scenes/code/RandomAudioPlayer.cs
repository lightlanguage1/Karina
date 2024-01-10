using System.Collections;
using UnityEngine;

public class RandomAudioPlayer : MonoBehaviour
{
    [Header("��ƵԴ")]
    public AudioSource audioSource;

    [Header("��ƵƬ������")]
    public AudioClip[] audioClips;

    [Header("���ż������Сֵ�����ֵ")]
    public float minInterval = 5f;
    public float maxInterval = 10f;

    [Header("�л���Ƶ��Ķ�����ʱ��")]
    public float additionalIntervalAfterSwitch = 2f;

    [Header("��ȴʱ�䣬��ֹ����Ƶ������Ƶ�л�")]
    public float cooldownTime = 5f;

    [Header("��һ�β�����Ƶ��ʱ��")]
    private float lastPlayTime;

    void Start()
    {
        // �����ƵԴ����ƵƬ�������Ƿ�������
        if (audioSource == null || audioClips.Length == 0)
        {
            Debug.LogError("������Audio Source������һ��Audio Clip��");
            return;
        }

        // ��ʼ��Ϊ������ȴʱ�䣬ȷ����һ�β��Ų�������
        lastPlayTime = -cooldownTime;

        // ��ʼ���������Ƶ��Э��
        StartCoroutine(PlayRandomAudio());
    }

    // Э�̣����������Ƶ
    IEnumerator PlayRandomAudio()
    {
        while (true)
        {
            // ����Ƿ�����л�������Ƶ
            if (Time.time - lastPlayTime > cooldownTime)
            {
                // ���ѡ��һ����ƵƬ��
                AudioClip randomClip = audioClips[Random.Range(0, audioClips.Length)];

                // ������ƵԴ��Ƭ��
                audioSource.clip = randomClip;

                // ������Ƶ
                audioSource.Play();

                // ��¼���һ�β���ʱ��
                lastPlayTime = Time.time;

                // �ȴ�һ�����ʱ����ٴβ���
                float interval = Random.Range(minInterval, maxInterval) + additionalIntervalAfterSwitch;
                yield return new WaitForSeconds(interval);
            }
            else
            {
                // ���������ȴʱ���ڣ���ȴ�һС��ʱ����ټ���Ƿ���Բ���
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
