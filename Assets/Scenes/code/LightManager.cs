using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(10)]
public class LightManager : Singleton<LightManager>
{
    [SerializeField] private List<LightControl> sceneLights; //���泡���е����еƹ�
    private LightShift lightShift = LightShift.morning;

    private void OnEnable()
    {
        Global.instance.onLightChange += switchLight;
    }

    private void OnDisable()
    {
        Global.instance.onLightChange -= switchLight;
    }

    private void switchLight(LightShift lightShift)
    {
        //���֮ǰ����ĵƹ�ؼ�
        sceneLights.Clear();
        //�ҳ��������еƿؼ�
        sceneLights.AddRange(FindObjectsByType<LightControl>(FindObjectsSortMode.None));
        //����
        foreach (var light in sceneLights)
        {
            light.changeLight(lightShift);
        }
    }
}