using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightPattren", menuName = "Light/LightPattren")]
public class LightSwitch : ScriptableObject
{
    public List<LightDetails> lightDetailsl;

    /// <summary>
    ///
    /// </summary>
    /// <param name="lightShift">灯光切换</param>
    /// <returns></returns>
    public LightDetails getLightDetails(LightShift lightShift)
    {
        return lightDetailsl.Find(l => l.lightShift == lightShift);    //找到脚本对象列表中符合该条件的元素
    }
}

[System.Serializable]  //将该类标记为可以序列化or反序列化的
public class LightDetails
{
    public LightShift lightShift;
    public Color lightColor;
    public float lightAmount;
}

public enum LightShift
{
    morning,
    night
}