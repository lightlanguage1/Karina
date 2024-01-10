using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LocalData : MonoBehaviour
{
    private BinaryFormatter binaryFormatter;    //二进制格式化器
    private string saveSlotDir;
    public string[] saveSlotPos;

    private void Awake()
    {
        binaryFormatter = new BinaryFormatter();
        saveSlotDir = Application.persistentDataPath + "/" + "SaveSlot";
        saveSlotPos = new string[3];
        for (int i = 0; i < saveSlotPos.Length; i++)
        {
            saveSlotPos[i] = "save" + i.ToString() + ".save";
        }
    }

    public List<PlayerData> getPlayerLocalData()
    {
        List<PlayerData> playerDataList = new List<PlayerData>();
        for (int j = 0; j < 3; j++)
        {
            if (Load(j))
            {
                Debug.Log("load saveslot  " + Global.instance.playerData.num.ToString() + "  succeed！");
                playerDataList.Add(Global.instance.playerData);
            }
            else
            {
                Debug.Log("load saveslot  " + Global.instance.playerData.num.ToString() + "  failed！");
            }
        }
        return playerDataList;
    }

    public void Save(int num)   //第几个存档位
    {
        if (Directory.Exists(saveSlotDir))
        {
            Debug.Log("查询到存档目录!");
        }
        else
        {
            Debug.Log("未查询到存档目录!");
            Directory.CreateDirectory(saveSlotDir);
            Debug.Log("创建完毕，目录为" + saveSlotDir);
        }
        //记录下当前的日期时间
        //玩家相关 保存到本地
        DateTime curDateTime = DateTime.Now;
        string formatTime = curDateTime.ToString("yyyy-MM-dd hh:mm:ss");
        //Global.instance.playerData.dateTime = formatTime;
        Global.instance.playerData.num = num;
        Global.instance.playerData.level = PlayerManager.instance.level;
        Global.instance.playerData.sexVal = (int)PlayerManager.instance.curHappyVal;
        Global.instance.playerData.x = GameObject.Find("Karryn").transform.localPosition.x;
        Global.instance.playerData.y = GameObject.Find("Karryn").transform.localPosition.y;
        Global.instance.playerData.z = GameObject.Find("Karryn").transform.localPosition.z;

        //创建文件，对象转为json，序列为二进制写入文件中
        string playerSaveSlot = saveSlotDir + "/" + saveSlotPos[num];
        FileStream fStream = File.Create(playerSaveSlot);
        //string js = JsonUtility.ToJson(Global.instance.playerData);
        binaryFormatter.Serialize(fStream, Global.instance.playerData); //将对象序列化为二进制 写入文件中
        Debug.Log("存档完成,存档位于" + playerSaveSlot);
        fStream.Close();

        //敌人相关 保存到数据库
        DbAccess db = GetComponent<DbAccess>();
        db.OpenDB(Global.instance.databasePath);
        string[] clos = { "Name", "PositionX", "PositionY", "PositionZ", "IsLive" };
        //将场景内所有敌人添加到list中 （live/die）
        List<NPC> npcList = new List<NPC>();
        var parent = GameObject.Find("Enemies");
        string[] closVal = new string[parent.transform.childCount * clos.Length];
        npcList.AddRange(FindObjectsByType<NPC>(FindObjectsSortMode.None));
        foreach (var pair in Global.instance.diedDic)
        {
            NPC npc = pair.Value.GetComponent<NPC>();
            npcList.Add(npc);
        }
        for (int i = 0; i < closVal.Length / clos.Length; i++)
        {
            closVal[i * clos.Length] = "'" + npcList[i].gameObject.name + "'";
            closVal[i * clos.Length + 1] = npcList[i].transform.localPosition.x.ToString();
            closVal[i * clos.Length + 2] = npcList[i].transform.localPosition.y.ToString();
            closVal[i * clos.Length + 3] = npcList[i].transform.localPosition.z.ToString();
            closVal[i * clos.Length + 4] = npcList[i].gameObject.activeSelf ? "1" : "0";
            db.UpdateInto("EnemyInfo", clos, closVal, clos.Length, "ID", i.ToString());
        }
        db.CloseSqlConnection();
    }

    private bool Load(int num)
    {
        if (!Directory.Exists(saveSlotDir))
        {
            Debug.Log("目录未创建，无法读取!");
            return false;
        }
        string playerSaveSlot = saveSlotDir + "/" + saveSlotPos[num];
        if (!File.Exists(playerSaveSlot))
        {
            Debug.Log("存档 " + playerSaveSlot + " 未建立，无法读取!");
            return false;
        }
        FileStream file = File.Open(playerSaveSlot, FileMode.Open); //如果文件存在，以只读方式打开文件；如果文件不存在，引发异常。
        Global.instance.playerData = (PlayerData)binaryFormatter.Deserialize(file);  //二进制反序列为对象
        //JsonUtility.FromJsonOverwrite(playerData, Global.instance.playerData);   //从JSON字符串覆盖
        Debug.Log("存档 " + playerSaveSlot + " 读取成功!");
        file.Close();
        return true;
    }

    public void onClicked()
    {
        if (Load(0))    //0用作测试 实际要通过playerdata中的数据 来确定玩家在哪个场景中
        {
            SceneLoader.instance.loadGameScene((int)SceneEnumVal.Main1L);
        }
        Debug.Log("load fail!");
    }
}