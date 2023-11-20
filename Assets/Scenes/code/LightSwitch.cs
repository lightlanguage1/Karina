using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightPattren", menuName = "Light/LightPattren")]
public class LightSwitch : ScriptableObject
{
    public List<LightDetails> lightDetailsl;

    /// <summary>
    ///
    /// </summary>
    /// <param name="lightShift">�ƹ��л�</param>
    /// <returns></returns>
    public LightDetails getLightDetails(LightShift lightShift)
    {
        return lightDetailsl.Find(l => l.lightShift == lightShift);    //�ҵ��ű������б��з��ϸ�������Ԫ��
    }
}

[System.Serializable]  //��������Ϊ�������л�or�����л���
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