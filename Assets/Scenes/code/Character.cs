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
    [SerializeField] protected float atk;   //攻击力
    [SerializeField] protected float def;   //防御力
    [SerializeField] protected float sex;   //性攻击力

    //使用委托实现脚本之间通信
    public delegate void taskCompletedDelegate();

    public static event taskCompletedDelegate onTaskCompleted;

    //任务开始 （任务指的是开始切换卡琳姿态图的任务）
    public delegate void taskBeginDelegate();

    public static event taskBeginDelegate onTaskBegin;

    //敌人死后，执行的逻辑
    public delegate void enemyDie();

    public static event enemyDie onEnemyDie;

    private bool isBlink;   //闪烁是否结束
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
        yield return new WaitForSeconds(0.15f);  //手动调整，扣血的时候再闪烁
        float blinkInterval = 0.1f;
        int blinkCount = 5;
        Image image = transform.GetChild(0).GetComponent<Image>();

        onTaskBegin?.Invoke();

        for (int i = 0; i < blinkCount; i++)
        {
            //降低透明度
            Color newColor1 = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
            image.color = newColor1;

            //等待一段时间
            yield return new WaitForSeconds(blinkInterval);

            //恢复
            Color newColor2 = new Color(image.color.r, image.color.g, image.color.b, 1f);
            image.color = newColor2;

            yield return new WaitForSeconds(blinkInterval);
        }
        if (isDie)
        {
            isBlink = true;
            yield break;    //防止die协程还未将敌人从列表中移除时，战斗协程过早被唤醒 有点难度好好思考下
        }
        //触发定义的事件
        onTaskCompleted?.Invoke();
    }

    public virtual void subtractMagic(float subtractMp)
    {
        mp -= subtractMp;
        progressBar.updateMp(mp, maxmp);
        if (mp <= 0f)
        {
            //无法使用技能了
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