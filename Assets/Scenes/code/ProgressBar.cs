using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [Header("Ѫ��and����")]
    public float curHpFillProgress;

    public float targetHpFillProgress;
    public float curMpFillProgress;
    public float targetMpFillProgress;
    public float fillSpeed; 
    private float t;
    private bool isHp;
    private Coroutine hpCoroutine;
    private Coroutine mpCoroutine;

    [SerializeField] private Image hpFillImageBack;
    [SerializeField] private Image hpFillImageFront;
    [SerializeField] private Image mpFillImageBack;
    [SerializeField] private Image mpFillImageFront;

    private WaitForSeconds delayTime;   //��ʱʱ��

    private void Awake()
    {
        delayTime = new WaitForSeconds(0.35f);
        isHp = false;
        //���hp��ͼ�� ǰ��
        hpFillImageBack = transform.Find("HpBack").GetComponent<Image>(); //����ֱ���Ӷ���
        hpFillImageFront = transform.Find("HpFront").GetComponent<Image>();
        //���mp��ͼ�� ǰ��
        mpFillImageBack = transform.Find("MpBack").GetComponent<Image>();
        mpFillImageFront = transform.Find("MpFront").GetComponent<Image>();
    }

    public void initializeHpAndMp(float hpCurVal, float mpCurval, float hpMaxVal, float mpMaxVal)
    {
        curHpFillProgress = hpCurVal / hpMaxVal;
        targetHpFillProgress = curHpFillProgress;
        curMpFillProgress = mpCurval / mpMaxVal;
        targetMpFillProgress = curMpFillProgress;
        //��hp mp ǰ��2�Ž���ͼ������ʾ
        hpFillImageFront.fillAmount = curHpFillProgress;
        hpFillImageBack.fillAmount = curHpFillProgress;
        mpFillImageBack.fillAmount = curMpFillProgress;
        mpFillImageFront.fillAmount = curMpFillProgress;
    }

    public void updateHp(float hpCurVal, float maxVal)
    {
        targetHpFillProgress = hpCurVal / maxVal;

        //��ֹ�ú������������ã����½������������ּ��ٳ�BUG
        if (hpCoroutine != null)
        {
            StopCoroutine(hpCoroutine);
            hpCoroutine = null;
        }

        if (curHpFillProgress < targetHpFillProgress)  //���ӽ���
        {
            //�����ͼ�����ӣ�Ȼ������ǰ���ͼ
            isHp = true;
            hpFillImageBack.fillAmount = targetHpFillProgress;
            hpCoroutine = StartCoroutine(changeImgFillPorgress(hpFillImageFront));
        }
        else if (curHpFillProgress > targetHpFillProgress) //���ٽ���
        {
            //�������෴
            isHp = true;
            hpFillImageFront.fillAmount = targetHpFillProgress;
            StartCoroutine(changeImgFillPorgress(hpFillImageBack));
        }
    }

    public void updateMp(float mpCurval, float maxVal)
    {
        targetMpFillProgress = mpCurval / maxVal;
        if (targetMpFillProgress <= 0) targetMpFillProgress = 0;

        if (mpCoroutine != null)
        {
            StopCoroutine(mpCoroutine);
            mpCoroutine= null;
        }

        if (curMpFillProgress < targetMpFillProgress)
        {
            isHp = false;
            mpFillImageBack.fillAmount = targetMpFillProgress;
            StartCoroutine(changeImgFillPorgress(mpFillImageFront));
        }
        else if (curMpFillProgress > targetMpFillProgress)
        {
            isHp = false;
            mpFillImageFront.fillAmount = targetMpFillProgress;
            StartCoroutine(changeImgFillPorgress(mpFillImageBack));
        }
    }

    private IEnumerator changeImgFillPorgress(Image img)
    {
        yield return delayTime;
        t = 0f;

        if (isHp)
        {
            while (true)
            {
                t += Time.deltaTime * fillSpeed;
                t = Mathf.Clamp01(t);
                curHpFillProgress = Mathf.Lerp(curHpFillProgress, targetHpFillProgress, t); //���Բ�ֵ������a + (b - a) * t ��ֵ��ʽ
                if (curHpFillProgress <= targetHpFillProgress)
                {
                    curHpFillProgress = targetHpFillProgress;
                    img.fillAmount = curHpFillProgress;
                    break;
                }
                img.fillAmount = curHpFillProgress;
                yield return null;
            }
        }
        else
        {
            while (true)
            {
                t += Time.deltaTime * fillSpeed;
                t = Mathf.Clamp01(t);
                curMpFillProgress = Mathf.Lerp(curMpFillProgress, targetMpFillProgress, t); //���Բ�ֵ������a + (b - a) * t ��ֵ��ʽ
                if (curMpFillProgress <= targetMpFillProgress)
                {
                    curMpFillProgress = targetMpFillProgress;
                    img.fillAmount = curMpFillProgress;
                    break;
                }
                img.fillAmount = curMpFillProgress;
                yield return null;
            }
        }
    }
}