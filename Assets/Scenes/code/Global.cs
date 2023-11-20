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

//全局变量管理者脚本
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

    [Header("敌人的种类")]
    public GameObject[] enemies;

    public RectTransform[] enemiesTransform;

    public enum enemyName
    {
        Guard = 0,  //卫兵
        Goblin,     //哥布林
        Homeless,   //流浪汉
        Lizardman,  //蜥蜴人
        Nerd,       //书呆子
        Orc,        //兽人
        Prisoner,   //囚犯
        Rogue,      //盗贼
        Slime,      //史莱姆
        Thug,       //流氓
        Wolf,       //狼人
        Yeti,       //雪人
        Boss1L      //一层boss
    }

    [Header("技能消耗")]
    public float xuli;

    [Header("战斗中的敌人")]
    public string battlePrevNpcName;

    public float enemy_x;
    public float enemy_y;
    public float enemy_z;

    [Header("已阵亡的敌人列表")]
    public Dictionary<string,GameObject> diedDic;   //先死的先刷新，后死的后刷新

    //[Header("敌人的初始位置字典")]
    public Dictionary<string, Vector3> posDic;  //这个位置是要从数据库里面读出来的

    [Header("管理场景中的敌人")]
    public GameObject totalEnemy;

    [Header("玩家信息")]
    public PlayerData playerData;

    public bool isWin;

    [Header("灯光信息")]
    public float duration;

    public bool inMainScene;

    [Header("时间")]
    public float time;

    public TextMeshProUGUI timeText;
    public LightShift lightShift;
    private string morning;
    private string night;
    private int timer;

    public delegate void switchLight(LightShift type);

    public event switchLight onLightChange;

    [Header("SQLite数据库文件路径")]
    public string databasePath = "Data Source=Assets/StreamingAssets/Karryn";

    //声音类型
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

    //状态类型
    public enum StateType
    {
        IdleState,
        PatrolState,
        ChaseState
    }

    public enum EnemiesIndex    //非战斗场景，层级窗口中敌人的索引
    {
        goblin,
        goblin1
    }

    private void Start()
    {
        //从数据库把敌人的生死状态读取到该字典中,注意以下这些值仅作测试，实际要通过 数据库/本地 读取
        diedDic = new Dictionary<string, GameObject>();
        posDic = new Dictionary<string, Vector3>();
        enemy_x = -1f;
        enemy_y = -18.453f;
        enemy_z = 0f;
        xuli = 50;
        playerData = new PlayerData();
        duration = 15f;
        inMainScene = false;
        morning = "白天";
        night = "夜晚";
        timer = 0;
        timeText.text = "白天";
        timeText.text = "黑夜";
        ////测试用例
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
            time += Time.deltaTime / 5; // 将时间按5倍速度流逝
            if (time >= 30) // 这里的30对应的是5分钟游戏时间（实际为30分钟现实时间）
            {
                if (timer >= 1) // 随意定的，可以适当调整
                {
                    timer = 0;
                    timeText.text = morning;
                    return;
                }
                timer++;
                if (timer % 2 == 0) timeText.text = morning;
                else if (timer % 2 == 1) timeText.text = night;

                time -= 30; // 这里的30对应的是5分钟游戏时间（实际为30分钟现实时间）
                lightShift = lightShift == LightShift.morning ? LightShift.night : LightShift.morning;
                onLightChange?.Invoke(lightShift);
            }
        }
    }

}

[System.Serializable]
public class PlayerData
{
    public int num; //存档位编号
    public string name;
    public float x;
    public float y;
    public float z;
    public int level;
    public int sexVal;
    public int money;
    public string dateTime;
    public string nowTime;  //白天/夜晚
}