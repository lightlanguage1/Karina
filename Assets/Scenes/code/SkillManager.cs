using System.Collections.Generic;
using UnityEngine;


public class SkillManager : Singleton<SkillManager>
{
    
    private Dictionary<int, Skill> skillCache; // ���ܻ���
    [SerializeField] private List<string> skillList;

    private void Start()    //һ��Ҫȷ��SkillDatabase �ڵ�ǰ�ű�֮ǰ���� ����Ϊ��
    {
        skillCache = GetComponent<SkillDatabase>().getSkillCache();
#if UNITY_EDITOR
        foreach (KeyValuePair<int,Skill> skill in skillCache)
        {
            skillList.Add(skill.Value.skillName);
        }
#endif
    }

    public Dictionary<int, Skill> getSkillCache() //������װ�ṩ�����Ľӿڣ���ֹ����Ա�޸ļ��ܻ��棬�ӿڸ���ԭ��ֻҪ������ƿ������ν������ٸ�����
    {
        if (skillCache == null) Debug.LogError("���ܻ���δ��ʼ����");
        return skillCache;
    }

    public Skill GetSkillByID(int skillID)
    {
        if (skillCache.TryGetValue(skillID, out Skill skill))   //out ������� ��ʾ�ñ����ں����ڲ�����ֵ
        {
            return skill;
        }
        else
        {
            Debug.LogWarning("����ID " + skillID + " �������ڻ����У�");
            return null;
        }
    }

    public bool UseCachedSkill()
    {
        // ʹ�ü���
        Debug.Log("ʹ�ü��ܣ�"  );
        // ������ִ�м��ܵ��߼�

        return true; // ʹ�ü��ܳɹ�
    }
}