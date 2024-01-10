using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LocalData : MonoBehaviour
{
    private BinaryFormatter binaryFormatter;    //�����Ƹ�ʽ����
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
                Debug.Log("load saveslot  " + Global.instance.playerData.num.ToString() + "  succeed��");
                playerDataList.Add(Global.instance.playerData);
            }
            else
            {
                Debug.Log("load saveslot  " + Global.instance.playerData.num.ToString() + "  failed��");
            }
        }
        return playerDataList;
    }

    public void Save(int num)   //�ڼ����浵λ
    {
        if (Directory.Exists(saveSlotDir))
        {
            Debug.Log("��ѯ���浵Ŀ¼!");
        }
        else
        {
            Debug.Log("δ��ѯ���浵Ŀ¼!");
            Directory.CreateDirectory(saveSlotDir);
            Debug.Log("������ϣ�Ŀ¼Ϊ" + saveSlotDir);
        }
        //��¼�µ�ǰ������ʱ��
        //������ ���浽����
        DateTime curDateTime = DateTime.Now;
        string formatTime = curDateTime.ToString("yyyy-MM-dd hh:mm:ss");
        //Global.instance.playerData.dateTime = formatTime;
        Global.instance.playerData.num = num;
        Global.instance.playerData.level = PlayerManager.instance.level;
        Global.instance.playerData.sexVal = (int)PlayerManager.instance.curHappyVal;
        Global.instance.playerData.x = GameObject.Find("Karryn").transform.localPosition.x;
        Global.instance.playerData.y = GameObject.Find("Karryn").transform.localPosition.y;
        Global.instance.playerData.z = GameObject.Find("Karryn").transform.localPosition.z;

        //�����ļ�������תΪjson������Ϊ������д���ļ���
        string playerSaveSlot = saveSlotDir + "/" + saveSlotPos[num];
        FileStream fStream = File.Create(playerSaveSlot);
        //string js = JsonUtility.ToJson(Global.instance.playerData);
        binaryFormatter.Serialize(fStream, Global.instance.playerData); //���������л�Ϊ������ д���ļ���
        Debug.Log("�浵���,�浵λ��" + playerSaveSlot);
        fStream.Close();

        //������� ���浽���ݿ�
        DbAccess db = GetComponent<DbAccess>();
        db.OpenDB(Global.instance.databasePath);
        string[] clos = { "Name", "PositionX", "PositionY", "PositionZ", "IsLive" };
        //�����������е�����ӵ�list�� ��live/die��
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
            Debug.Log("Ŀ¼δ�������޷���ȡ!");
            return false;
        }
        string playerSaveSlot = saveSlotDir + "/" + saveSlotPos[num];
        if (!File.Exists(playerSaveSlot))
        {
            Debug.Log("�浵 " + playerSaveSlot + " δ�������޷���ȡ!");
            return false;
        }
        FileStream file = File.Open(playerSaveSlot, FileMode.Open); //����ļ����ڣ���ֻ����ʽ���ļ�������ļ������ڣ������쳣��
        Global.instance.playerData = (PlayerData)binaryFormatter.Deserialize(file);  //�����Ʒ�����Ϊ����
        //JsonUtility.FromJsonOverwrite(playerData, Global.instance.playerData);   //��JSON�ַ�������
        Debug.Log("�浵 " + playerSaveSlot + " ��ȡ�ɹ�!");
        file.Close();
        return true;
    }

    public void onClicked()
    {
        if (Load(0))    //0�������� ʵ��Ҫͨ��playerdata�е����� ��ȷ��������ĸ�������
        {
            SceneLoader.instance.loadGameScene((int)SceneEnumVal.Main1L);
        }
        Debug.Log("load fail!");
    }
}