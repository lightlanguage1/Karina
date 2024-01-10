using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(10)]
public class LightManager : Singleton<LightManager>
{
    [SerializeField] private List<LightControl> sceneLights; //保存场景中的所有灯光
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
        //清空之前保存的灯光控件
        sceneLights.Clear();
        //找场景中所有灯控件
        sceneLights.AddRange(FindObjectsByType<LightControl>(FindObjectsSortMode.None));
        //遍历
        foreach (var light in sceneLights)
        {
            light.changeLight(lightShift);
        }
    }
}