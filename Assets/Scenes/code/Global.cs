#define DEBUG

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum SceneEnumVal
{
    MainMenuScene = 0,
    Main1L,
    BattleScene,
    SexTransitionScene
}

//ȫ�ֱ��������߽ű�
[DefaultExecutionOrder(2)]
public class Global : Singleton<Global>
{
    public bool isSuppress = false;
    public bool isClear1L = false;

    public string battlePrevSceneName;
    public string curMainSceneName;

    public string mainScene1L = "main1L";
    public string mainScene2L = "main2L";
    public string mainScene3L = "main3L";

    [Header("���˵�����")]
    public GameObject[] enemies;

    public RectTransform[] enemiesTransform;

    public enum enemyName
    {
        Guard = 0,  //����
        Goblin,     //�粼��
        Homeless,   //���˺�
        Lizardman,  //������
        Nerd,       //�����
        Orc,        //����
        Prisoner,   //����
        Rogue,      //����
        Slime,      //ʷ��ķ
        Thug,       //��å
        Wolf,       //����
        Yeti,       //ѩ��
        Boss1L      //һ��boss
    }

    [Header("��������")]
    public float xuli;

    [Header("ս���еĵ���")]
    public string battlePrevNpcName;

    public float enemy_x;
    public float enemy_y;
    public float enemy_z;

    [Header("�������ĵ����б�")]
    public Dictionary<string,GameObject> diedDic;   //��������ˢ�£������ĺ�ˢ��

    //[Header("���˵ĳ�ʼλ���ֵ�")]
    public Dictionary<string, Vector3> posDic;  //���λ����Ҫ�����ݿ������������

    [Header("�������еĵ���")]
    public GameObject totalEnemy;

    [Header("�����Ϣ")]
    public PlayerData playerData;

    public bool isWin;

    [Header("�ƹ���Ϣ")]
    public float duration;

    public bool inMainScene;

    [Header("ʱ��")]
    public float time;

    public TextMeshProUGUI timeText;
    public LightShift lightShift;
    private string morning;
    private string night;
    private int timer;

    public delegate void switchLight(LightShift type);

    public event switchLight onLightChange;

    [Header("SQLite���ݿ��ļ�·��")]
    public string databasePath = "Data Source=Assets/StreamingAssets/Karryn";

    //��������
    public enum SoundType
    {
        None,
        StartMenu,
        Main1L,
        Battle,
        MoveCursor,
        Click,
        Walk,
        Daji,
        Xuli
    }

    //״̬����
    public enum StateType
    {
        IdleState,
        PatrolState,
        ChaseState
    }

    public enum EnemiesIndex    //��ս���������㼶�����е��˵�����
    {
        goblin,
        goblin1
    }

    private void Start()
    {
        //�����ݿ�ѵ��˵�����״̬��ȡ�����ֵ���,ע��������Щֵ�������ԣ�ʵ��Ҫͨ�� ���ݿ�/���� ��ȡ
        diedDic = new Dictionary<string, GameObject>();
        posDic = new Dictionary<string, Vector3>();
        enemy_x = -1f;
        enemy_y = -18.453f;
        enemy_z = 0f;
        xuli = 50;
        playerData = new PlayerData();
        duration = 15f;
        inMainScene = false;
        morning = "����";
        night = "ҹ��";
        timer = 0;
        timeText.text = "����";
        timeText.text = "��ҹ";
        ////��������
        playerData = new PlayerData();
        playerData.name = "Karryn";
        playerData.x = 29f;
        playerData.y = -33f;
        playerData.z = 0f;
        playerData.sexVal = 0;
        playerData.money = 1000;
        playerData.dateTime = "";
        playerData.num = 0;
        playerData.level = 1;
    }

    private void Update()
    {
        if (inMainScene)
        {
            time += Time.deltaTime / 5; // ��ʱ�䰴5���ٶ�����
            if (time >= 30) // �����30��Ӧ����5������Ϸʱ�䣨ʵ��Ϊ30������ʵʱ�䣩
            {
                if (timer >= 1) // ���ⶨ�ģ������ʵ�����
                {
                    timer = 0;
                    timeText.text = morning;
                    return;
                }
                timer++;
                if (timer % 2 == 0) timeText.text = morning;
                else if (timer % 2 == 1) timeText.text = night;

                time -= 30; // �����30��Ӧ����5������Ϸʱ�䣨ʵ��Ϊ30������ʵʱ�䣩
                lightShift = lightShift == LightShift.morning ? LightShift.night : LightShift.morning;
                onLightChange?.Invoke(lightShift);
            }
        }
    }

}

[System.Serializable]
public class PlayerData
{
    public int num; //�浵λ���
    public string name;
    public float x;
    public float y;
    public float z;
    public int level;
    public int sexVal;
    public int money;
    public string dateTime;
    public string nowTime;  //����/ҹ��
}