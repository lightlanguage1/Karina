using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField] protected float hp;
    [SerializeField] protected float maxhp;
    [SerializeField] protected float mp;
    [SerializeField] protected float maxmp;
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] protected float atk;   //������
    [SerializeField] protected float def;   //������
    [SerializeField] protected float sex;   //�Թ�����

    //ʹ��ί��ʵ�ֽű�֮��ͨ��
    public delegate void taskCompletedDelegate();

    public static event taskCompletedDelegate onTaskCompleted;

    //����ʼ ������ָ���ǿ�ʼ�л�������̬ͼ������
    public delegate void taskBeginDelegate();

    public static event taskBeginDelegate onTaskBegin;

    //��������ִ�е��߼�
    public delegate void enemyDie();

    public static event enemyDie onEnemyDie;

    private bool isBlink;   //��˸�Ƿ����
    private bool isDie;

    protected virtual void OnEnable()
    {
        hp = maxhp;
        isBlink = false;
        isDie = false;
    }

    protected virtual void initHpAndMp(float hp, float mp, float maxhp, float maxmp)
    {
        progressBar.initializeHpAndMp(hp, mp, maxhp, maxmp);
    }

    public virtual void takeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0f)
        {
            hp = 0;
            isDie = true;
            StartCoroutine(die());
        }
        progressBar.updateHp(hp, maxhp);
        StartCoroutine(blinkAnimation());
    }

    private IEnumerator blinkAnimation()
    {
        yield return new WaitForSeconds(0.15f);  //�ֶ���������Ѫ��ʱ������˸
        float blinkInterval = 0.1f;
        int blinkCount = 5;
        Image image = transform.GetChild(0).GetComponent<Image>();

        onTaskBegin?.Invoke();

        for (int i = 0; i < blinkCount; i++)
        {
            //����͸����
            Color newColor1 = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
            image.color = newColor1;

            //�ȴ�һ��ʱ��
            yield return new WaitForSeconds(blinkInterval);

            //�ָ�
            Color newColor2 = new Color(image.color.r, image.color.g, image.color.b, 1f);
            image.color = newColor2;

            yield return new WaitForSeconds(blinkInterval);
        }
        if (isDie)
        {
            isBlink = true;
            yield break;    //��ֹdieЭ�̻�δ�����˴��б����Ƴ�ʱ��ս��Э�̹��类���� �е��ѶȺú�˼����
        }
        //����������¼�
        onTaskCompleted?.Invoke();
    }

    public virtual void subtractMagic(float subtractMp)
    {
        mp -= subtractMp;
        progressBar.updateMp(mp, maxmp);
        if (mp <= 0f)
        {
            //�޷�ʹ�ü�����
        }
    }

    private IEnumerator die()
    {
        yield return new WaitUntil(() => isBlink);
        isBlink = false;
        onEnemyDie?.Invoke();
        onTaskCompleted?.Invoke();
    }

    public virtual void RestoreHp(float restroeHp)
    {
        if (restroeHp == hp) return;

        hp = Mathf.Clamp(restroeHp + hp, 0, maxhp);
    }

    public virtual void RestoreMp(float restroeMp)
    {
        if (restroeMp == hp) return;

        hp = Mathf.Clamp(restroeMp + hp, 0, maxmp);
    }

    protected IEnumerator hpRestoreCoroutine(WaitForSeconds waitTime, float percent)
    {
        while (hp < maxhp)
        {
            yield return waitTime;
            RestoreHp(maxhp * percent);
        }
    }

    protected IEnumerator mpRestoreCoroutine(WaitForSeconds waitTime, float percent)
    {
        while (mp < maxmp)
        {
            yield return waitTime;
            RestoreHp(maxhp * percent);
        }
    }
}