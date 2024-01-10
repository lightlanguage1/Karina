using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    // ��ɫ����
    [SerializeField] protected float hp;        // ��ǰ����ֵ
    [SerializeField] protected float maxhp;     // �������ֵ
    [SerializeField] protected float mp;        // ��ǰħ��ֵ
    [SerializeField] protected float maxmp;     // ���ħ��ֵ
    [SerializeField] private ProgressBar progressBar;  // ���������
    [SerializeField] protected float atk;       // ������
    [SerializeField] protected float def;       // ������
    [SerializeField] protected float sex;       // �Թ�����

    // ʹ��ί��ʵ�ֽű�֮��ͨ��
    public delegate void taskCompletedDelegate();

    public static event taskCompletedDelegate onTaskCompleted;

    // ����ʼ������ָ���ǿ�ʼ�л�������̬ͼ������
    public delegate void taskBeginDelegate();

    public static event taskBeginDelegate onTaskBegin;

    // ����������ִ�е��߼�
    public delegate void enemyDie();

    public static event enemyDie onEnemyDie;

    private bool isBlink;   // ��˸�Ƿ����
    private bool isDie;

    protected virtual void OnEnable()
    {
        // ��ʼ����ɫ����
        hp = maxhp;
        isBlink = false;
        isDie = false;
    }

    protected virtual void initHpAndMp(float hp, float mp, float maxhp, float maxmp)
    {
        // ��ʼ�����������
        progressBar.initializeHpAndMp(hp, mp, maxhp, maxmp);
    }

    public virtual void takeDamage(float damage)
    {
        // �۳�����ֵ
        hp -= damage;
        if (hp <= 0f)
        {
            hp = 0;
            isDie = true;
            StartCoroutine(die());
        }
        // ���½�����
        progressBar.updateHp(hp, maxhp);
        // ������˸����
        StartCoroutine(blinkAnimation());
    }

    private IEnumerator blinkAnimation()
    {
        yield return new WaitForSeconds(0.15f);  // �ֶ���������Ѫ��ʱ������˸
        float blinkInterval = 0.1f;
        int blinkCount = 5;
        Image image = transform.GetChild(0).GetComponent<Image>();

        // ��������ʼ�¼�
        onTaskBegin?.Invoke();

        for (int i = 0; i < blinkCount; i++)
        {
            // ����͸����
            Color newColor1 = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
            image.color = newColor1;

            // �ȴ�һ��ʱ��
            yield return new WaitForSeconds(blinkInterval);

            // �ָ�͸����
            Color newColor2 = new Color(image.color.r, image.color.g, image.color.b, 1f);
            image.color = newColor2;

            yield return new WaitForSeconds(blinkInterval);
        }
        if (isDie)
        {
            isBlink = true;
            yield break;    // ��ֹ die Э�̻�δ�����˴��б����Ƴ�ʱ��ս��Э�̹��类���ѣ���һЩ�Ѷȣ��ú�˼����
        }
        // ������������¼�
        onTaskCompleted?.Invoke();
    }

    public virtual void subtractMagic(float subtractMp)
    {
        // �۳�ħ��ֵ
        mp -= subtractMp;
        // ����ħ��ֵ������
        progressBar.updateMp(mp, maxmp);
        if (mp <= 0f)
        {
            // �޷�ʹ�ü�����
        }
    }

    private IEnumerator die()
    {
        yield return new WaitUntil(() => isBlink);
        isBlink = false;
        // �������������¼�����������¼�
        onEnemyDie?.Invoke();
        onTaskCompleted?.Invoke();
    }

    public virtual void RestoreHp(float restoreHp)
    {
        if (restoreHp == hp) return;

        // �ָ�����ֵ����ȷ���� 0 �� maxhp ֮��
        hp = Mathf.Clamp(restoreHp + hp, 0, maxhp);
    }

    public virtual void RestoreMp(float restoreMp)
    {
        if (restoreMp == hp) return;

        // �ָ�ħ��ֵ����ȷ���� 0 �� maxmp ֮��
        hp = Mathf.Clamp(restoreMp + hp, 0, maxmp);
    }

    protected IEnumerator hpRestoreCoroutine(WaitForSeconds waitTime, float percent)
    {
        while (hp < maxhp)
        {
            yield return waitTime;
            // ���ٷֱȻָ�����ֵ
            RestoreHp(maxhp * percent);
        }
    }

    protected IEnumerator mpRestoreCoroutine(WaitForSeconds waitTime, float percent)
    {
        while (mp < maxmp)
        {
            yield return waitTime;
            // ���ٷֱȻָ�ħ��ֵ
            RestoreHp(maxhp * percent);
        }
    }
}
