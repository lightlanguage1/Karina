using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightControl : MonoBehaviour
{
    public LightSwitch lightSwitch; //�ű�����
    private Light2D curLight;
    private LightDetails curlightDetails;

    private void Start()
    {
        curLight = GetComponent<Light2D>();
    }

    //ʵ�ֵƹ��л����߼�

    public void changeLight(LightShift lightShift)
    {
        curlightDetails = lightSwitch.getLightDetails(lightShift);
        //ͨ������������ʵ�ְ���/��ҹ�Ĺ���Ч��
        DOTween.To(() => curLight.color, c => curLight.color = c, curlightDetails.lightColor, Global.instance.duration);
        DOTween.To(() => curLight.intensity, c => curLight.intensity = c, curlightDetails.lightAmount, Global.instance.duration);
    }
}