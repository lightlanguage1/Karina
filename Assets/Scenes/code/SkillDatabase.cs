using Mono.Data.Sqlite;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class SkillDatabase : MonoBehaviour
{
    private const string databasePath = "Data Source=Assets/StreamingAssets/Karryn"; // SQLite数据库文件路径
    private Dictionary<int, Skill> skillCache; // 技能缓存

    private void Awake()
    {
        // 加载所有技能数据并缓存
        LoadAllSkills();
    }

    public Dictionary<int, Skill> getSkillCache()
    {
        return skillCache;
    }

    private void LoadAllSkills()
    {
        skillCache = new Dictionary<int, Skill>();

        SqliteConnection connection = new SqliteConnection(databasePath);

        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Skills";

        SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())   //读每一行
        {
            Skill skill = new Skill();
            skill.ID = reader.GetInt32(0);  //0代表一个字段 然后以此类推
            skill.skillName = reader.GetString(1);
            skill.damage = reader.GetInt32(2);
            skill.addSexVal = reader.GetInt32(3);
            skill.reduceSexVal = reader.GetInt32(4);
            skill.cooldown = reader.GetFloat(5);
            skill.hpCost = reader.GetInt32(6);
            skill.mpCost = reader.GetInt32(7);
            skill.voliCost = reader.GetInt32(8);

            skillCache.Add(skill.ID, skill);
        }

        int counter = 0;
        if (connection != null)
        {
            connection.Close();
            counter++;
        }
        if (command != null)
        {
            command.Dispose();
            counter++;
        }
        if (reader != null)
        {
            reader.Close();
            counter++;
        }
#if UNITY_EDITOR
        if (counter == 3) Debug.Log("SQLite数据库关闭成功！");
        else Debug.Log("SQLite数据库关闭失败，请检查问题！");
#endif
    }
}