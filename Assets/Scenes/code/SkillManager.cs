using System.Collections.Generic;
using UnityEngine;


public class SkillManager : Singleton<SkillManager>
{
    
    private Dictionary<int, Skill> skillCache; // 技能缓存
    [SerializeField] private List<string> skillList;

    private void Start()    //一定要确保SkillDatabase 在当前脚本之前调用 否则为空
    {
        skillCache = GetComponent<SkillDatabase>().getSkillCache();
#if UNITY_EDITOR
        foreach (KeyValuePair<int,Skill> skill in skillCache)
        {
            skillList.Add(skill.Value.skillName);
        }
#endif
    }

    public Dictionary<int, Skill> getSkillCache() //做个封装提供公开的接口，防止程序员修改技能缓存，接口隔离原则，只要不是设计框架无所谓，这里举个例子
    {
        if (skillCache == null) Debug.LogError("技能缓存未初始化！");
        return skillCache;
    }

    public Skill GetSkillByID(int skillID)
    {
        if (skillCache.TryGetValue(skillID, out Skill skill))   //out 输出参数 表示该变量在函数内部被赋值
        {
            return skill;
        }
        else
        {
            Debug.LogWarning("技能ID " + skillID + " 不存在于缓存中！");
            return null;
        }
    }

    public bool UseCachedSkill()
    {
        // 使用技能
        Debug.Log("使用技能："  );
        // 在这里执行技能的逻辑

        return true; // 使用技能成功
    }
}