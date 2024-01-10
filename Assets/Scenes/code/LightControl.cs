using DG.Tweening;
using UnityEngine.Rendering.Universal;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    public LightSwitch lightSwitch; // �ƹ��л��ű�����
    private Light2D curLight;       // ��ǰ�ƹ����
    private LightDetails curlightDetails; // ��ǰ�ƹ�ϸ����Ϣ

    private void Start()
    {
        // ��ȡ��ǰ�����ϵ� Light2D ���
        curLight = GetComponent<Light2D>();
    }

    // ʵ�ֵƹ��л����߼�
    public void ChangeLight(LightShift lightShift)
    {
        // ��ȡָ���ƹ�ת�����͵ĵƹ�ϸ����Ϣ
        curlightDetails = lightSwitch.GetLightDetails(lightShift);


        // ͨ������������ʵ�ֵƹ���ɫ��ǿ�ȵ�ƽ������Ч��
        DG.Tweening.Core.TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> tweenerCore = DOTween.To(() => curLight.color, c => curLight.color = c, curlightDetails.lightColor, GameTimeManager.instance.duration);
        DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> tweenerCore1 = DOTween.To(() => curLight.intensity, c => curLight.intensity = c, curlightDetails.lightAmount, GameTimeManager.instance.duration);
    }

}