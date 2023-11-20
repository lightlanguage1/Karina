using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightControl : MonoBehaviour
{
    public LightSwitch lightSwitch; //脚本对象
    private Light2D curLight;
    private LightDetails curlightDetails;

    private void Start()
    {
        curLight = GetComponent<Light2D>();
    }

    //实现灯光切换的逻辑

    public void changeLight(LightShift lightShift)
    {
        curlightDetails = lightSwitch.getLightDetails(lightShift);
        //通过动画补间插件实现白天/黑夜的过渡效果
        DOTween.To(() => curLight.color, c => curLight.color = c, curlightDetails.lightColor, Global.instance.duration);
        DOTween.To(() => curLight.intensity, c => curLight.intensity = c, curlightDetails.lightAmount, Global.instance.duration);
    }
}