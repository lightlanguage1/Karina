using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LightPattern", menuName = "Light/LightPattern")]
public class LightSwitch : ScriptableObject
{
    public List<LightDetails> lightDetails;

    /// <summary>
    /// ���ݵƹ�ת�����ͻ�ȡ��Ӧ�ĵƹ�ϸ����Ϣ
    /// </summary>
    /// <param name="lightShift">�ƹ��л�����</param>
    /// <returns>��Ӧ�ĵƹ�ϸ����Ϣ</returns>
    public LightDetails GetLightDetails(LightShift lightShift)
    {
        
        LightDetails foundDetails = lightDetails.FirstOrDefault(l => l.lightShift.ToString().ToLower() == lightShift.ToString().ToLower());

        return foundDetails;
    }


}


// ���Ϊ�����л��ĵƹ�ϸ����
[System.Serializable]
public class LightDetails
{
    // �ƹ�ת������
    public LightShift lightShift;

    // �ƹ���ɫ
    public Color lightColor;

    // �ƹ�ǿ��
    public float lightAmount;

}


// ����ƹ�ת�����͵�ö��
public enum LightShift
{
    morning,
    night,
    midnight,
    afternoon,
    noon,
    forenoon
}
