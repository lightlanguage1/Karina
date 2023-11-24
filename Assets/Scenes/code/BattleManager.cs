using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.VisualScripting.Metadata;

public class BattleManager : Singleton<BattleManager>
{
    [Header("玩家")]
    public KarrynState playerCurState;

    [Header("敌人列表")]
    [SerializeField] private Dictionary<int, GameObject> dictionary;  //保存实例化的敌人

    public List<GameObject> prefabs; //敌人的总数量 如果是12就代表获取12种敌人，每种敌人对应一个索引

    //[SerializeField] private int prefabSize;
    public bool isAtkFinished;  //判断玩家的攻击流程是否完成

    [Header("BOSS列表")]
    public GameObject boss1L;

    [Header("敌人对应的索引")]
    [HideInInspector] public int guardIndex;

    [HideInInspector] public int goblinIndex;
    [HideInInspector] public int homelessIndex;
    [HideInInspector] public int LizardmanIndex;
    [HideInInspector] public int nerdIndex;
    [HideInInspector] public int orcIndex;
    [HideInInspector] public int prisonerIndex;
    [HideInInspector] public int RogueIndex;
    [HideInInspector] public int SlimeIndex;
    [HideInInspector] public int ThugIndex;
    [HideInInspector] public int WolfIndex;
    [HideInInspector] public int YetiIndex;
    [HideInInspector] public int Boss1LIndex;

    public int index;   //玩家选择的敌人索引
    public int total;   //列表中敌人总数

    [Header("按钮列表")]
    public ButtonCommandManager commandManager;

    public List<GameObject> mainPanel1;
    public List<GameObject> mainPanel2;

    [HideInInspector] public Button[] buttons;

    public bool isFinished;
    private UnityEngine.UI.Button curButton;

    [HideInInspector] public GameObject prevPanel;
    [HideInInspector] public GameObject curPanel; //点击按钮后切换到的当前面板
    [HideInInspector] public Dictionary<string, List<Transform>> childrenDic;   //包含当前对象的所有子对象字典 （当前对象指的是canvas中的子对象）

    [Header("动画列表")]
    public Dictionary<string, Animator> animDic;

    public GameObject closeUpImg;

    [Header("光标和焦点的固定偏移量")]
    public Vector2 cursorOffset; //光标的固定偏移量

    public Vector2 focusOffset; //这个位置根据实际场景通过偏移手动计算得出
    public Vector2 curSkillPos;   //技能面板内首个技能的位置 第一行第一列
    public Vector2 skillIconOffset;

    [SerializeField] private Vector2 lightFrameOffset = new Vector2(0, 133);
    [SerializeField] private float yBottom = -324;
    [SerializeField] private float yTop = 349;
    [SerializeField] public float yTopChanged = 349;

    [Header("Battle Canvas")]
    public GameObject canvas;

    private List<Transform> disableChildrenList;    //禁用的子对象列表
    public bool isPanel;

    [Header("技能列表")]
    [SerializeField] private int SkillIndex = 0; //记录当前是什么技能，假设玩家想重新选择技能，这样可以定位到当前选择的技能

    [HideInInspector] public List<Transform> scrollViewDescList = new List<Transform>();

    private bool isPressKey = false;  // 是否允许按键触发
    private float keyPressInterval = 0.2f;  // 按键触发的间隔时间
    private float keyPressTimer = 0f;  // 按键触发的计时器

    private Scrollbar curScrollbar;
    private bool isSlide;
    private int prevIndex;    //滑动滚动条后的技能索引

    public bool isInputEnable;  //防止玩家短时间内多次输入造成bug

    [Header("是否撤销")]
    public bool isRepeal = false;

    [Header("协程引用")]
    public Coroutine coroutineSeletedEnemy;

    public Coroutine corStartInfoPanel;

    public Coroutine corIconScale;

    public Coroutine corSwitchOtherButton;

    //委托
    public delegate void onEnemyAtk();

    public event onEnemyAtk EnemyAtkEvent;

    private int maxScrollOffset = 1; // 添加这一行来初始化最大滚动偏移量

    private int scrollOffset = 0; // 添加这一行来跟踪当前滚动偏移量

    private int page = 0; //上下键分页

    private string keyDown;

    private int leftCount = 0;

    private int rightCount = 1;

    protected override void Awake() //****用于初始化各种变量和数据结构***//
    {
        //手动添加，比较清晰了解都添加了哪些敌人
        base.Awake();
        guardIndex = (int)Global.enemyName.Guard;
        goblinIndex = (int)Global.enemyName.Goblin;
        homelessIndex = (int)Global.enemyName.Homeless;
        LizardmanIndex = (int)Global.enemyName.Lizardman;
        nerdIndex = (int)Global.enemyName.Nerd;
        orcIndex = (int)Global.enemyName.Orc;
        prisonerIndex = (int)Global.enemyName.Prisoner;
        RogueIndex = (int)Global.enemyName.Rogue;
        SlimeIndex = (int)Global.enemyName.Slime;
        WolfIndex = (int)Global.enemyName.Wolf;
        YetiIndex = (int)Global.enemyName.Yeti;
        Boss1LIndex = (int)Global.enemyName.Boss1L;

        dictionary = new Dictionary<int, GameObject>();
        //敌人的索引与敌人组成键值对加入到字典中,Enum.GetValues获取枚举类中的所有成员，返回数组
        int i = 0;
        foreach (Global.enemyName name in Enum.GetValues(typeof(Global.enemyName)))  //typeof运算符可以获取类的信息，列如名称
        {
            dictionary.Add((int)name, prefabs[i]);
            i++;
        }
        playerCurState = KarrynState.Weapon1;
        isFinished = false;
        isPanel = false;
        isInputEnable = true;
        animDic = new Dictionary<string, Animator>();
        childrenDic = new Dictionary<string, List<Transform>>();
        cursorOffset = new Vector2(450, 105);
        focusOffset = new Vector2(450, 105);
        skillIconOffset = new Vector2(450, 103);
        mainPanel1 = new List<GameObject>();
        mainPanel2 = new List<GameObject>();

        Character.onTaskBegin += SwitchKarrynIcon1;
        Character.onTaskCompleted += SwitchKarrynIcon2;
        Character.onEnemyDie += RemoveEnemyFromList;
    }

    public IEnumerator PlayerRound()  //***等待玩家进行面板操作后判断是否符合条件进行协程的停止和继续运行***//
    {
        if (mainPanel1.Count == 0 || mainPanel2.Count == 0)
        {
            findChildrenInParent("Battle Skill Panel 1", mainPanel1);
            findChildrenInParent("Battle Skill Panel 2", mainPanel2);
        }

        if (mainPanel1.Count == 0)
        {
            Debug.LogError("无法找到 Battle Skill Panel 1 中的按钮。");
            yield break; // 退出协程
        }

        curButton = mainPanel1[0].GetComponent<UnityEngine.UI.Button>();
        useKeyboardOperationMainButton(curButton.gameObject);

        // 等待玩家完成操作
        yield return new WaitUntil(() => isFinished);  //协程一直等待直到返回为真
        isFinished = false; //再次设置false 将协程挂起
        SkillIndex = 0; //归0 让描述面板描述首个技能
        isPanel = false;
    }


    //回调
    private void SwitchKarrynIcon1()
    {
        AudioManager.instance.playCustomAudio(Global.SoundType.Daji);
        childrenDic["Karryn"][(int)playerCurState].gameObject.SetActive(false);
        playerCurState = KarrynState.Atk1;
        childrenDic["Karryn"][(int)playerCurState].gameObject.SetActive(true);
    }

    //回调
    private void SwitchKarrynIcon2()
    {
        childrenDic["Karryn"][(int)playerCurState].gameObject.SetActive(false);
        playerCurState = KarrynState.Weapon1;
        childrenDic["Karryn"][(int)playerCurState].gameObject.SetActive(true);
        //停止协程，进入判断是否胜利回合
        StopCoroutine(coroutineSeletedEnemy);
        isFinished = true;
        isInputEnable = true;
    }

    //回调
    private void RemoveEnemyFromList()
    {
        prefabs[BattleManager.instance.index].SetActive(false);

        prefabs.RemoveAt(BattleManager.instance.index);
        total = BattleManager.instance.prefabs.Count;
        //重定 光标 选择框的 位置
        GameObject frame = canvas.transform.GetChild((int)ChildrenType.SelectFrame).gameObject;
        GameObject cursor = canvas.transform.GetChild((int)ChildrenType.FrameCursor).gameObject;
        Vector3 pos;
        if (prefabs.Count > 0)
        {
            pos = prefabs[0].transform.localPosition;
        }
        else
        {
            //敌人都死了，防止访问空数组报错，因此要单独判断，接着把光标选择框移动到首个敌人的位置（复原）
            pos = new Vector3(-890, 349, 0);    //手动取得
        }
        frame.transform.localPosition = new Vector3(frame.transform.localPosition.x, pos.y, 0);
        cursor.transform.localPosition = new Vector3(cursor.transform.localPosition.x, pos.y, 0);
        yTopChanged = pos.y; //改变顶部的值，从而控制光标和选择框的移动范围
        index = 0;   //注意归0，因为首个元素移除后，第二个元素顶替他的位置
    }

    private void Start()    //脚本被启用时调用,注意所属的对象不销毁，他在对象的整个生命周期内只调用一次
    {

    }

  

    public void loadResource()
    {
        prefabs.Clear();    //每次调用start函数时，要清空之前存储的敌人
        //首先获取场景内画布
        canvas = GameObject.Find("Battle Canvas").gameObject;
        //启用目标子对象，因为当前场景内有些对象处于禁用状态无法获取他们的组件，导致下面的buttons Animation is null
        string[] strArr = { "Battle Skill Panel 2", "Volition Panel", "Spiritual  Panel 1" ,"Spiritual  Panel 2","Sex Panel","Special Skill Panel" ,"Atk Skill Panel", "info panel"};
        disableChildrenList = enableChildren(strArr, canvas.transform);

        buttons = FindObjectsByType<UnityEngine.UI.Button>(FindObjectsSortMode.None);
        
        Animator[] animArr = FindObjectsByType<Animator>(FindObjectsInactive.Include,FindObjectsSortMode.None);//BUG(没有insctive导致查找失败)
        if(animDic.Count == 0) LoadToDictionary(animDic, animArr);  //防止重复添加 要考虑再次进入战斗场景
        curScrollbar = canvas.transform.Find("Volition Panel").Find("Scroll View").Find("Scrollbar Vertical").GetComponent<Scrollbar>();
        curScrollbar.interactable = false;
        //找到后禁用这些启用的子对象
        foreach (var child in disableChildrenList)
        {
            child.gameObject.SetActive(false);
        }

        //实例化按钮命令管理器（命令模式）
        commandManager = new ButtonCommandManager(buttons);

        //材质权重初始化为0，因为实现光边溶解后不为0
        playerCurState = KarrynState.Weapon1;
        childrenDic["Karryn"][(int)playerCurState].GetComponent<Image>().material.SetFloat("dissolution", 0f);

        foreach (var button in buttons)
        {
            button.onClick.AddListener(() => { ButtonClicked(button.gameObject); });
        }
        StartCoroutine(BattleLoop());
    }

    private void ButtonClicked(GameObject btn)
    {
        EventSystem.current.SetSelectedGameObject(btn);

        // 获取当前被点击的按钮
        curButton = EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.Button>();

        // 禁用其他的按钮防止玩家点击其他的按钮出现 bug
        foreach (var button in buttons)
        {
            if (button != curButton)
            {
                button.interactable = false;
            }
        }

        // 创建对应的按钮处理器实例来处理按钮的点击事件，用命令模式避免重复写 if-else if，增加扩展性
        if (commandManager == null)
        {
            /*#if UNITY_EDITOR
            Debug.LogError("按钮命令器初始化失败，请检查!");
            #endif*/
        }
        else
        {
            if (curButton.gameObject.name == "run")  // 检查按钮是否为“run”按钮
            {
                // 执行逃跑/退出逻辑
                StartCoroutine(RunAway());
            }
            else
            {
                // 执行其他按钮的逻辑
                commandManager.ExecuteCommand(curButton.gameObject.name);  // 执行命令（设计模式：命令模式）
            }

            // 等待处理按钮事件的逻辑执行完后，再将其他按钮启用
            foreach (var button in buttons)
            {
                if (button.gameObject != curButton)
                {
                    button.interactable = true;
                }
            }
        }
    }

    private IEnumerator RunAway()
    {
        Debug.Log("玩家选择逃跑！");

        // 播放逃跑动画，这里假设有一个名为 "RunAwayAnimation" 的动画组件
        // 如果有特定的逃跑动画，请将其替换为实际的动画名称
        //animDic["Karryn"].SetTrigger("RunAwayAnimation");

        // 显示逃跑提示信息，这里假设有一个名为 "RunAwayText" 的UI文本组件
        // 如果有特定的提示信息，请将其替换为实际的文本信息
        // 如果你的逃跑过程涉及多个步骤，你可以在此处添加适当的等待时间和过渡效果
        //UIManager.instance.ShowText("逃跑成功！");

        // 等待一段时间，模拟逃跑后的处理
        yield return new WaitForSeconds(3f);

        // 结束战斗，切换到逃跑后的场景
        StopAllCoroutines();

        // 这里可以添加切换场景的逻辑，假设有一个名为 "RunAwayScene" 的场景
        SceneLoader.instance.loadGameScene((int)SceneEnumVal.Main1L);
    }



    public IEnumerator BattleLoop()
    {
        yield return StartCoroutine(PrepareRound());

        while (true)
        {
            yield return StartCoroutine(PlayerRound());

            if (CheckBattleEndCondition())
                break;

            yield return StartCoroutine(EnemyRound());

            if (CheckBattleEndCondition())
                break;
            //将战斗技能1号面板开启,供用户操作
            canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel1).gameObject.SetActive(true);
            yield return null;
        }
    }

    private void Update()
    {
        //通过计时器限制玩家短时间内多次输入，导致结果不可预料
        if (!isPressKey)
        {
            keyPressTimer += Time.deltaTime;
            if (keyPressTimer >= keyPressInterval)
            {
                isPressKey = true;
                keyPressTimer = 0;  //重置计时器时间
            }
        }
        else
        {
            if (isPanel && isInputEnable)
            {
                checkUserMoveAction();
            }
        }
    }

    public IEnumerator PrepareRound()
    {
        /*#if UNITY_EDITOR
            Debug.Log("战斗开始准备回合");
        #endif*/
        // 在这里处理战斗开始时的准备逻辑
        //1.播放音乐 2.显示主角 3.敌人列表上显示敌人

        if (Global.instance.battlePrevSceneName == "main1L")
        {
            if (!Global.instance.isSuppress)  // 未镇压
            {
                if (!Global.instance.isClear1L) // 一层未通关
                {
                    int enemySize = UnityEngine.Random.Range(1, 6); // 随机生成1到5之间的整数
                    int[] values = { goblinIndex, goblinIndex }; // 用于测试，实际应该添加适当的敌人索引
                                                                 // 一层 thug 和 goblin 镇压后 thug goblin guard
                    if (Global.instance.battlePrevNpcName.Contains("goblin"))
                    {
                        prefabs.AddRange(SpawnRandomEnemies(enemySize, values)); // 生成指定数量的随机敌人
                        /*#if UNITY_EDITOR
                        Debug.Log("敌人数量是" + prefabs.Count.ToString());
                        #endif*/
                        SetEnemiesHpAndMp(ref prefabs, 100, 100, 100, 100, 500, 20, 400); // 设置随机敌人的属性
                    }
                    else if (Global.instance.battlePrevNpcName == "thug")
                    {
                        prefabs.AddRange(SpawnRandomEnemies(enemySize, values)); // 生成指定数量的随机敌人
                        SetEnemiesHpAndMp(ref prefabs, 100, 100, 100, 100, 500, 20, 400); // 设置随机敌人的属性
                    }
                    else if (Global.instance.battlePrevNpcName == "1LBOSS")
                    {
                        enemySize = 3; // 指定敌人数量为3
                        prefabs.AddRange(SpawnRandomEnemies(enemySize, values)); // 生成指定数量的随机敌人
                        SetEnemiesHpAndMp(ref prefabs, 100, 100, 100, 100, 500, 20, 400); // 设置随机敌人的属性
                        boss1L.GetComponent<EnemyManager>().initialize(300, 300, 300, 300, 500, 50, 1000);
                    }
                }
                else
                {
                    // 通关后的逻辑
                }
            }
            else
            {
                // 镇压后的逻辑
            }
        }
        else if (SceneManager.GetActiveScene().name == Global.instance.mainScene2L)
        {
            // 处理第二层的逻辑
        }
        else if (SceneManager.GetActiveScene().name == Global.instance.mainScene3L)
        {
            // 处理第三层的逻辑
        }
        yield break;
    }

    private GameObject[] SpawnRandomEnemies(int enemySize, int[] enemyIndexArray)
    {
        GameObject[] arr = new GameObject[enemySize];
        for (int i = 0; i < enemySize; i++)
        {
            int num = UnityEngine.Random.Range(0, enemyIndexArray.Length); // 使用敌人索引数组的长度来确定范围
            arr[i] = PoolManager.instance.getObjInPool(dictionary[enemyIndexArray[num]], Global.instance.enemiesTransform[i]);
        }
        return arr;
    }

    private void SetEnemiesHpAndMp(ref List<GameObject> enemis_, float hp_, float mp_, float maxhp_, float maxmp_, float atk_, float def_, float sex_)
    {
        for (int i = 0; i < enemis_.Count; i++)
        {
            enemis_[i].GetComponent<EnemyManager>().initialize(hp_, mp_, maxhp_, maxmp_, atk_, def_, sex_);
            /*#if UNITY_EDITOR
            Debug.Log("生成的敌人是" + enemis_[i].gameObject.name + "他的索引是" + enemis_[i].GetComponent<EnemyManager>().EnemyIndex.ToString() +
                "hp_:" + hp_.ToString() + "mp_:" + mp_.ToString() + "maxhp_:" + maxhp_.ToString() + "maxmp_:" + maxmp_.ToString() + "atk_:" +
                atk_.ToString() + "atk_:" + atk_.ToString() + "def_:" + def_.ToString());
            #endif*/
        }
    }


    public IEnumerator EnemyRound()
    {
        Debug.Log("敌人回合");
        // 1.敌人使用普通atk 或者 sex atk
        for (int i = 0; i < prefabs.Count; i++)
        {
            int res = UnityEngine.Random.Range(1, 3);   //左闭右开 所以随机数是1or2 浮点型是左闭右闭
            res = 1;
            if (res % 2 == 0)   //使用普通atk
            {
                float atk = prefabs[i].GetComponent<EnemyManager>().getAtk();   //获得敌人的攻击力
                PlayerManager.instance.takeDamage(atk);
                yield return new WaitForSeconds(1f);  //等待血条减少
            }
            else    //使用sex atk
            {
                //播放sex动画
                EnemyAtkEvent?.Invoke();

                float sexVal = prefabs[i].GetComponent<EnemyManager>().getSexVal();
                PlayerManager.instance.addHappyVal(40f);   //20用作测试
                PlayerManager.instance.subtractMagic(sexVal + PlayerManager.instance.curHappyVal);  //测试
                yield return new WaitForSeconds(2f);    //因为要等待img对象动作执行完毕，所以多等一会
            }
        }
    }

    private bool CheckBattleEndCondition()
    {
        // 在这里编写判断战斗胜利或失败的条件
        // 如果满足胜利条件，返回true，战斗将结束
        // 如果满足sex条件，返回true，战斗将结束
        // 如果生命值不为0，返回false，继续进行下一回合
        // 私有变量isBattle为真(进入胜利计分面板)为假就(进入sex计分面板)

        if (PlayerManager.instance.checkFail())
        {
            //进入sex场景
            ReleaseResource();
            dontDestroy();
            StartCoroutine(lightDissolution());
            return true;
        }
        else if (prefabs.Count == 0)       //胜利敌人全部被消灭
        {
            Global.instance.isWin = true;
            ReleaseResource();
            StopAllCoroutines();
            dontDestroy();
            SceneLoader.instance.loadGameScene((int)SceneEnumVal.Main1L);
            return false;
        }
        return false;   //即不成功也不失败
    }

    private void ReleaseResource()
    {
        // 清空动画字典
        animDic.Clear();
        // 清空子对象字典
        childrenDic.Clear();
        // 清空预制体列表
        prefabs.Clear();
        // 清空主面板1列表
        mainPanel1.Clear();
        // 清空主面板2列表
        mainPanel2.Clear();
        // 清空需要禁用的子对象列表
        disableChildrenList.Clear();
        // 将按钮变量设置为 null，释放引用
        buttons = null;
    }


    private void dontDestroy()
    {
        // 将PlayerManager移回DontDestroyOnLoad场景 注意只有根对象才可以使用DontDestroyOnLoad方法
        PlayerManager.instance.GetComponent<RectTransform>().SetParent(null, false);
        DontDestroyOnLoad(PlayerManager.instance.gameObject);

        // 将对象池管理器移回DontDestroyOnLoad场景
        PoolManager.instance.transform.GetComponent<RectTransform>().SetParent(null, false);
        DontDestroyOnLoad(PoolManager.instance.gameObject);
    }

    private IEnumerator WaitForPlayerInput()
    {
        // 假设在面板管理器中等待玩家操作的逻辑
        yield return null;
    }

    //开始选择敌人并攻击他
    public IEnumerator beginSelectEnemyAtk(List<GameObject> enemies, SkillType skillType)
    {
        yield return null;  //将协程挂起等待至下一帧，避免当前帧同时判断2次用户输入
        index = 0;  //用户每向下移动一次，索引加1，用来定位敌人
        total = enemies.Count; //几个敌人移动几次
        int n = 1;  //至少出现一个敌人
        float y = canvas.transform.GetChild((int)ChildrenType.SelectFrame).gameObject.transform.localPosition.y;

        while (true)
        {
            //不移动选中框直接攻击当前敌人
            atkEnemies(prefabs, skillType);
            //撤销
            atkCancel();

            if (Input.GetKeyDown(KeyCode.S) && y >= yBottom && n < total && total > 1)
            {
                int dir = -1;
                //移动选择框和光标
                canvas.transform.GetChild((int)ChildrenType.SelectFrame).localPosition += new Vector3(0, dir * lightFrameOffset.y, 0);
                canvas.transform.GetChild((int)ChildrenType.FrameCursor).localPosition += new Vector3(0, dir * lightFrameOffset.y, 0);
                index++;
                n++;
                y = canvas.transform.GetChild((int)ChildrenType.SelectFrame).localPosition.y;

                //选中敌人进行攻击，敌人减血
                atkEnemies(prefabs, skillType);
                //撤销攻击
                atkCancel();
            }
            else if (Input.GetKeyDown(KeyCode.W) && y <= yTop && n > 1 && total > 1 && y <= yTopChanged)
            {
                int dir = 1;
                canvas.transform.GetChild((int)ChildrenType.SelectFrame).localPosition += new Vector3(0, dir * lightFrameOffset.y, 0);
                canvas.transform.GetChild((int)ChildrenType.FrameCursor).localPosition += new Vector3(0, dir * lightFrameOffset.y, 0);
                index--;
                n--;
                y = canvas.transform.GetChild((int)ChildrenType.SelectFrame).localPosition.y;

                atkEnemies(prefabs, skillType);
                atkCancel();
            }

            yield return null;
        }
    }

    private void atkEnemies(List<GameObject> enemies, SkillType skillType)
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && isInputEnable)
        {
            isInputEnable = false;
            //1.根据枚举值从技能缓存读技能 2.攻击后，播放敌人受伤动画
            int damageVal = SkillManager.instance.getSkillCache()[(int)skillType].damage;
            int practicalDamage = damageVal + (int)PlayerManager.instance.atk;
            enemies[index].GetComponent<EnemyManager>().takeDamage(100);    //150用作测试
            //引用光标和选择框
            GameObject frame = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.SelectFrame).gameObject;
            GameObject cursor = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.FrameCursor).gameObject;
            frame.SetActive(false);
            cursor.SetActive(false);
            //去看回调函数
        }
    }

    public IEnumerator startInfoPanel() //玩家信息面板
    {
        yield return null;

        int curIndex = canvas.transform.GetChild((int)ChildrenType.InfoPanel).GetSiblingIndex();    //记录当前位置的索引
        canvas.transform.GetChild((int)ChildrenType.InfoPanel).SetAsLastSibling();
        canvas.transform.GetChild(canvas.transform.childCount - 1).Find("1").Find("suzhi1").gameObject.SetActive(true);
        Vector3 pos = canvas.transform.GetChild(canvas.transform.childCount - 1).Find("2").Find("cursor").localPosition;

        Vector3 offset = new Vector3(475, 0, 0);
        int index = 0;
        while (true)
        {
            if (index == 4 && Input.GetKeyDown(KeyCode.Space))
            {
                //移动回原本的索引位置 保持列表中索引位置不变
                canvas.transform.GetChild(canvas.transform.childCount - 1).Find("2").Find("cursor").localPosition = pos;
                canvas.transform.GetChild(canvas.transform.childCount - 1).gameObject.SetActive(false);
                canvas.transform.GetChild(canvas.transform.childCount - 1).SetSiblingIndex(curIndex);
                commandManager.ExecuteCommand("", "Battle Skill Panel 2");
            }

            //向右
            if (Input.GetKeyDown(KeyCode.D) && index < 4)
            {
                //关闭当前面板 开启下个面板 切换标题 至于移动子对象 是为了不被poolmanager中的敌人覆盖 提示给到这了 自己思考下
                var obj1 = canvas.transform.GetChild(canvas.transform.childCount - 1).Find("1");
                obj1.GetChild(index).gameObject.SetActive(false);

                obj1.GetChild(index + 1).gameObject.SetActive(true);
                var obj2 = canvas.transform.GetChild(canvas.transform.childCount - 1).Find("2");
                var cursor = obj2.Find("cursor");
                cursor.localPosition += offset;
                index++;
            }
            //向左
            else if (Input.GetKeyDown(KeyCode.A) && index > 0)
            {
                var obj1 = canvas.transform.GetChild(canvas.transform.childCount - 1).Find("1");
                obj1.GetChild(index).gameObject.SetActive(false);

                obj1.GetChild(index - 1).gameObject.SetActive(true);
                var obj2 = canvas.transform.GetChild(canvas.transform.childCount - 1).Find("2");
                var cursor = obj2.Find("cursor");
                cursor.localPosition -= offset;
                index--;
            }

            yield return null;
        }
    }

    private void atkCancel()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(1))
        {
            commandManager.ExecuteCommand("", curPanel.name);
        }
    }

    private IEnumerator lightDissolution()  //光边溶解
    {
        float weight = 0f;  //详情见shader
        float duration = 2f;
        while (weight < 1.1f)
        {
            weight += Time.deltaTime / duration;
            weight = Mathf.Clamp(weight, 0f, 1.1f);
            Material material = childrenDic["Karryn"][(int)playerCurState].GetComponent<Image>().material;
            material.SetFloat("dissolution", weight);
            yield return null;
        }
        StopAllCoroutines();
        dontDestroy();
        SceneLoader.instance.loadGameScene((int)SceneEnumVal.SexTransitionScene);
    }

    private void LoadToDictionary(Dictionary<string, Animator> dic, Animator[] animations) //移动至BCM脚本-(ButtonCommandManager)
    {
        // 遍历所有的动画，将它们添加到字典中
        foreach (Animator animation in animations)
        {
            string objectName = animation.gameObject.name;
            //Debug.Log(objectName);
            // 检测并添加 cursorName
            if (objectName.EndsWith(" cursor"))
            {
                string panelName = objectName.Replace(" cursor", "");
                dic.Add(panelName + " cursor", animation);
                // Debug.Log(objectName);
            }
            // 检测并添加 focusName
            else if (objectName.EndsWith(" focus"))
            {
                string panelName = objectName.Replace(" focus", "");
                dic.Add(panelName + " focus", animation);
                // Debug.Log(objectName);
            }
        }
    }

    private void findChildrenInParent(string name, List<GameObject> list) //面板类-移动至考虑功能/流程是否保留(ButtonCommandManager)
    {
        GameObject parent;
        if (name == "Battle Skill Panel 1")
        {
            parent = canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel1).gameObject;
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                var obj = parent.transform.GetChild(i).gameObject;
                list.Add(obj);
            }
        }
        else if (name == "Battle Skill Panel 2")
        {
            parent = canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject;
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                var obj = parent.transform.GetChild(i).gameObject;
                list.Add(obj);
            }
        }
    }

    private List<Transform> enableChildren(string[] targetChild, Transform transform) //面板类-移动至考虑功能/流程是否保留(ButtonCommandManager)
    {
        List<Transform> childrenList = new List<Transform>();   //记录所有要启用的子对象   
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);    //根据索引获取子对象，列表中第一个子对象索引为0 第二个是1 以此类推
            for (int j = 0; j < targetChild.Length; j++)
            {
                if (child.name == targetChild[j])
                {
                    child.gameObject.SetActive(true);
                    childrenList.Add(child);
                }
            }
        }
        return childrenList;
    }

    //下面开始都是面板类代码，包括光标，按钮等移动至考虑功能/流程是否保留(ButtonCommandManager)
    private void checkUserMoveAction()
    {
        List<Transform> children = childrenDic[curPanel.gameObject.name];

        // 获取当前面板的光标和焦点的名字
        string cursorName = curPanel.transform.GetChild((int)panelInChildType.Cursor).name;
        string focusName = curPanel.transform.GetChild((int)panelInChildType.Focus).name;

        //获取行数
        int num = (children.Count + 3) / 4;

        // 不移动按技能攻击
        pressSkillAtk(SkillIndex);
        pressedRepeal(SkillIndex);

        int curSkillX, curSkillY;

        // 获取当前面板的光标位置（根据字典中的信息）
        curSkillPos = children[SkillIndex].GetComponent<RectTransform>().anchoredPosition;
        curSkillX = Mathf.RoundToInt(curSkillPos.x);
        curSkillY = Mathf.RoundToInt(curSkillPos.y);

        // 检查用户的按键操作来移动光标
        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && SkillIndex + 1 >= 0 && SkillIndex + 1 < children.Count &&
            (int)(children[SkillIndex + 1].GetComponent<RectTransform>().anchoredPosition.x) == curSkillX + skillIconOffset.x)
        {
            rightCount++;
            int dir = 1;
            int indexNum = 1;
            //if (rightCount == 4)
            //{
            //    rightCount = 0;
            //    scrollOffset++;
            //    //indexNum = 4;
            //    ///dir = -1;
            //}

            isInputEnable = false;
            prevIndex = SkillIndex;
            SkillIndex += indexNum;
            moveCursorAndFocus(cursorName, focusName, true, dir, SkillIndex, children);
            int tempIndex = SkillIndex - indexNum;
            updateSkillDescription(children[tempIndex].gameObject, children[SkillIndex].gameObject);
            isInputEnable = true;
        }
        else if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && SkillIndex - 1 >= 0 && SkillIndex - 1 < children.Count &&
                 (int)(children[SkillIndex - 1].GetComponent<RectTransform>().anchoredPosition.x) == curSkillX - skillIconOffset.x)
        {

            leftCount++;
            //int dir = -1;
            //if (leftCount == 4)
            //{
            //    dir++;
            //}
            isInputEnable = false;
            prevIndex = SkillIndex;
            SkillIndex -= 1;
            moveCursorAndFocus(cursorName, focusName, true, -1, SkillIndex, children);
            int tempIndex = SkillIndex + 1;
            updateSkillDescription(children[tempIndex].gameObject, children[SkillIndex].gameObject);
            isInputEnable = true;
        }
        else if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && SkillIndex - 4 >= 0 && SkillIndex - 4 < children.Count &&
                 (int)(children[SkillIndex - 4].GetComponent<RectTransform>().anchoredPosition.y) == curSkillY + skillIconOffset.y)
        {
            int dir = 1;
            if (scrollOffset > 0) // 检查是否还有上方隐藏的行
            {
                keyDown = "Up";
                scrollOffset--; // 向上滚动一行
                if (page > 0)
                {
                    page--;
                    updateScrollViewPosition(children, scrollOffset, keyDown);
                    dir = -2;
                }

            }

            isInputEnable = false;
            prevIndex = SkillIndex;
            SkillIndex -= 4;
            moveCursorAndFocus(cursorName, focusName, false, dir, SkillIndex, children);
            int tempIndex = SkillIndex + 4;
            updateSkillDescription(children[tempIndex].gameObject, children[SkillIndex].gameObject);
            isInputEnable = true;
        }
        else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && SkillIndex + 4 >= 0 && SkillIndex + 4 < children.Count &&
                 (int)(children[SkillIndex + 4].GetComponent<RectTransform>().anchoredPosition.y) == curSkillY - skillIconOffset.y)
        {
            //int num = (children.Count + 3) / 4;
            int dir = -1;
            if (scrollOffset < num - 1) // 检查是否还有下方隐藏的行 1:默认情况下占一行
            {
                scrollOffset++; // 向下滚动一行
                if (scrollOffset % 3 == 0)
                {
                    page++;
                    keyDown = "Down";
                    updateScrollViewPosition(children, scrollOffset, keyDown);
                    dir = 2;
                }

            }

            isInputEnable = false;
            prevIndex = SkillIndex;
            SkillIndex += 4;
            moveCursorAndFocus(cursorName, focusName, false, dir, SkillIndex, children);
            int tempIndex = SkillIndex - 4;
            updateSkillDescription(children[tempIndex].gameObject, children[SkillIndex].gameObject);
            isInputEnable = true;
        }
    }

    //移动至BCM脚本

    // 新增函数，用于更新滚动视图的位置
    private void updateScrollViewPosition(List<Transform> children, int scrollOffset, string KeyDowns)
    {
        foreach (var child in children)
        {
            var pos = child.GetComponent<RectTransform>().anchoredPosition;
            //pos.y += scrollOffset * skillIconOffset.y;
            if (keyDown.Equals("Down"))
            {
                pos.y += 3 * skillIconOffset.y;
            }

            if (keyDown.Equals("Up"))
            {
                pos.y += -3 * skillIconOffset.y;
            }

            child.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }


    private void moveCursorAndFocus(string cursorName, string focusName, bool isX_dirMove, int dir, int skillIndex, List<Transform> list) //光标组件移动代码
    {
        // 输出 focusName 和 cursorName 的值
        //Debug.Log("focusName: " + focusName);
        //Debug.Log("cursorName: " + cursorName);

        if (isX_dirMove)
        {
            // 移动光标和焦点
            Transform focusTransform = animDic[focusName].gameObject.transform;
            Vector3 localOffset1 = new Vector3(focusOffset.x * dir, 0, 0);
            focusTransform.localPosition += localOffset1;

            Transform cursorTransform = animDic[cursorName].gameObject.transform;
            Vector3 localOffset2 = new Vector3(cursorOffset.x * dir, 0, 0);
            cursorTransform.localPosition += localOffset2;
        }
        else
        {
            // 移动光标和焦点
            Transform focusTransform = animDic[focusName].gameObject.transform;
            Vector3 localOffset1 = new Vector3(0, focusOffset.y * dir, 0);
            focusTransform.localPosition += localOffset1;

            Transform cursorTransform = animDic[cursorName].gameObject.transform;
            Vector3 localOffset2 = new Vector3(0, cursorOffset.y * dir, 0);
            cursorTransform.localPosition += localOffset2;
        }
    } //移动至BCM脚本

    private void resetPos(int dir, int index, Transform focusTransform, Transform cursorTransform)
    {
        // 基础偏移量，根据 dir 确定
        int baseOffset = dir == -1 ? 0 : -2;

        // xOffset 根据 dir 和索引选择要使用的索引值
        int xOffset = dir == -1 ? index : prevIndex;

        // 计算 focusTransform 和 cursorTransform 的水平位置
        float focusX = -685 + focusOffset.x * xOffset;
        float cursorX = -900 + cursorOffset.x * xOffset;

        // 根据 xOffset 和基础偏移量设置位置
        if (xOffset % 12 == baseOffset)
        {
            // 当索引值满足条件时，手动定位 focusTransform 和 cursorTransform
            focusTransform.localPosition = new Vector3(focusX, 55, 0);
            cursorTransform.localPosition = new Vector3(cursorX, 90, 0);
        }
        else if (xOffset % 13 == baseOffset)
        {
            // 当索引值满足条件时，手动定位 focusTransform 和 cursorTransform，水平位置增加一个偏移量
            focusTransform.localPosition = new Vector3(focusX + focusOffset.x, 55, 0);
            cursorTransform.localPosition = new Vector3(cursorX + cursorOffset.x, 90, 0);
        }
        else if (xOffset % 14 == baseOffset)
        {
            // 当索引值满足条件时，手动定位 focusTransform 和 cursorTransform，水平位置增加两倍的偏移量
            focusTransform.localPosition = new Vector3(focusX + focusOffset.x * 2, 55, 0);
            cursorTransform.localPosition = new Vector3(cursorX + cursorOffset.x * 2, 90, 0);
        }
        else if (xOffset % 15 == baseOffset)
        {
            // 当索引值满足条件时，手动定位 focusTransform 和 cursorTransform，水平位置增加三倍的偏移量
            focusTransform.localPosition = new Vector3(focusX + focusOffset.x * 3, 55, 0);
            cursorTransform.localPosition = new Vector3(cursorX + cursorOffset.x * 3, 90, 0);
        }
    }


    private IEnumerator iconScale(GameObject button) //面板图标按钮放大缩小
    {
        float t = 0;    //插值
        float a = 1f, b = 1.5f;
        float scaleSpeed = 1.15f;
        bool isScale = true;    //是否放大

        while (true)
        {
            //主面板的当前按钮放大缩小
            if (isScale)   //放大
            {
                t += Time.deltaTime * scaleSpeed;
                t = Mathf.Clamp01(t);
                float res = Mathf.Lerp(a, b, t);
                button.transform.localScale = new Vector3(res, res, res);
                if (t == 1)
                {
                    isScale = false;
                    t = 0;
                }
                yield return null;
            }
            else  //缩小
            {
                t += Time.deltaTime * scaleSpeed;
                t = Mathf.Clamp01(t);
                float res = Mathf.Lerp(b, a, t);
                button.transform.localScale = new Vector3(res, res, res);
                if (t == 1)
                {
                    isScale = true;
                    t = 0;
                }
                yield return null;
            }
        }
    }

    private IEnumerator switchOtherButton(List<GameObject> buttonList) //面板按钮旋转
    {
        yield return null;

        float t = 0;
        float rotateSpeed = 3f;
        float left = 90f;
        float right = -90f; //当前旋转角度值
        float curAngle = 0f;

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                commandManager.ExecuteCommand(buttonList[0].name);
            }
            //向右旋转 顺时针
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                foreach (GameObject button in buttonList)
                {
                    button.GetComponent<UnityEngine.UI.Button>().interactable = false;
                }
                // 停止缩放并将其还原
                StopCoroutine(corIconScale);
                buttonList[0].transform.localScale = Vector3.one;
                //将最后一个元素作为首元素，整理数组
                buttonList.Insert(0, buttonList[buttonList.Count - 1]); //插入是将该元素的副本插入到指定位置上，因此要删除该元素
                buttonList.RemoveAt(buttonList.Count - 1);
                t = 0;
                while (t < 1.0f)
                {
                    t += Time.deltaTime * rotateSpeed;
                    t = Mathf.Clamp01(t);
                    float res = Mathf.Lerp(curAngle, right, t);
                    buttonList[0].transform.parent.localRotation = Quaternion.Euler(0, 0, res);
                    for (int i = 0; i < buttonList[0].transform.parent.childCount; i++)
                    {
                        //所有子按钮逆向旋转
                        buttonList[0].transform.parent.GetChild(i).localRotation = Quaternion.Euler(0, 0, -res);
                    }
                    yield return null;
                }
                //旋转完成后再次启动缩放协程
                curAngle -= 90f;
                right -= 90f;
                left -= 90f;
                foreach (GameObject button in buttonList)
                {
                    button.GetComponent<UnityEngine.UI.Button>().interactable = true;  //可以交互
                }
                corIconScale = StartCoroutine(iconScale(buttonList[0]));
            }
            //向左旋转 逆时针
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                foreach (GameObject button in buttonList)
                {
                    button.GetComponent<UnityEngine.UI.Button>().interactable = false;  //无法交互
                }
                // 停止缩放并将其还原
                StopCoroutine(corIconScale);
                buttonList[0].transform.localScale = Vector3.one;
                //将首元素插入到数组最后一个元素位置上，整理数组
                buttonList.Add(buttonList[0]);  //注意从尾部插入，思考下为什么不能用insert，实在不懂可以问我
                buttonList.RemoveAt(0);
                t = 0;
                while (t < 1.0f)
                {
                    t += Time.deltaTime * rotateSpeed;
                    t = Mathf.Clamp01(t);
                    float res = Mathf.Lerp(curAngle, left, t);
                    buttonList[0].transform.parent.localRotation = Quaternion.Euler(0, 0, res);
                    for (int i = 0; i < buttonList[0].transform.parent.childCount; i++)
                    {
                        //所有子按钮逆向旋转
                        buttonList[0].transform.parent.GetChild(i).localRotation = Quaternion.Euler(0, 0, -res);
                    }
                    yield return null;
                }
                //旋转完成后再次启动缩放协程
                curAngle += 90f;
                left += 90f;
                right += 90f;
                foreach (GameObject button in buttonList)
                {
                    button.GetComponent<UnityEngine.UI.Button>().interactable = true;  //可以交互
                }
                corIconScale = StartCoroutine(iconScale(buttonList[0]));
            }
            yield return null;
        }
    }

    private void adjustScrollBar(bool isDown_) //面板内的技能描述
    {
        if (curPanel.name == "Volition Panel")
        {
            Scrollbar scrollbar = canvas.transform.Find("Volition Panel").Find("Scroll View").Find("Scrollbar Vertical").GetComponent<Scrollbar>();
            float t = 0;
            float speed = 1f;
            float targetVal = 0.92f;
            if (isDown_)
            {
                while (true)
                {
                    t += Time.deltaTime * speed;
                    t = Mathf.Clamp01(t);
                    float res = Mathf.Lerp(0f, targetVal, t);
                    scrollbar.value -= res;
                    if (scrollbar.value < 1 - targetVal)
                    {
                        scrollbar.value = 1 - targetVal;
                        break;
                    }
                }
            }
            else
            {
                while (true)
                {
                    t += Time.deltaTime * speed;
                    t = Mathf.Clamp01(t);
                    float res = Mathf.Lerp(0f, targetVal, t);
                    scrollbar.value += res;
                    if (scrollbar.value > 1)
                    {
                        scrollbar.value = 1;
                        break;
                    }
                }
            }
        }
    }



    private void pressedRepeal(int index)
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButton(1))
        {
            commandManager.ExecuteCommand("", "Battle Skill Panel 2");
        }
    }

    private void pressSkillAtk(int index)
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {

            commandManager.ExecuteCommand(childrenDic[curPanel.name][index].gameObject.name);
        }
    }

    // 更新技能描述的方法
    private void updateSkillDescription(GameObject prevSkill, GameObject curSkill) //优化修改中！！ 11-05-2023
    {
        Debug.Log("Current panel name: " + curPanel.name); // 输出当前面板的名称

        // 检查当前面板的名称
        if (curPanel.name == "Volition Panel" ||
            curPanel.name == "Spiritual Panel 1" ||
            curPanel.name == "Spiritual Panel 2" ||
            curPanel.name == "Special Skill Panel" ||
            curPanel.name == "Spiritual Skill Panel2")
        {
            Debug.Log("Special panel detected."); // 输出特殊面板被检测到

            // 如果当前面板是滚动视图，则特殊处理
            for (int i = 0; i < scrollViewDescList.Count; i++)
            {
                // 生成关联的描述文本对象的名称
                string prevSkillDescName = prevSkill.name + "Text1";
                string curSkillDescName = curSkill.name + "Text1";

                //Debug.Log("Previous skill desc name: " + prevSkillDescName); // 输出前一个技能的描述文本名称
                //Debug.Log("Current skill desc name: " + curSkillDescName); // 输出当前技能的描述文本名称

                // 根据名称设置描述文本对象的可见性
                if (prevSkillDescName == scrollViewDescList[i].name)
                {
                    scrollViewDescList[i].gameObject.SetActive(false); // 设置为不可见
                    //Debug.Log("Setting " + scrollViewDescList[i].name + " to inactive."); // 输出将对象设置为不可见
                }
                if (curSkillDescName == scrollViewDescList[i].name)
                {
                    scrollViewDescList[i].gameObject.SetActive(true); // 设置为可见
                    //Debug.Log("Setting " + scrollViewDescList[i].name + " to active."); // 输出将对象设置为可见
                }
            }
            return;
        }

        // 如果当前面板不是滚动视图，则执行以下操作
        string name = curSkill.transform.GetChild(0).name;
        //Debug.Log("First child name of current skill: " + name); // 输出当前技能的第一个子对象的名称

        // 检查子对象的名称是否为 "Text1"
        if (name == "Text1")
        {
            // 设置前一个技能的子对象 "Text1" 为不可见，当前技能的子对象 "Text1" 为可见
            prevSkill.transform.GetChild(0).gameObject.SetActive(false);
            curSkill.transform.GetChild(0).gameObject.SetActive(true);
            //Debug.Log("Setting " + prevSkill.name + " Text1 to inactive."); // 输出将前一个技能的 Text1 设置为不可见
            //Debug.Log("Setting " + curSkill.name + " Text1 to active."); // 输出将当前技能的 Text1 设置为可见
        }
    }
    //移动至BCM脚本---//到此结束

    public void useKeyboardOperationMainButton(GameObject curObj)    //表示按钮或者面板 //---后续将该段代码移动至BCM脚本---(ButtonCommandManager)斟酌考虑移动//
    {
        if (curObj.transform.parent.name == "Battle Skill Panel 1")
        {
            corSwitchOtherButton = StartCoroutine(switchOtherButton(mainPanel1));
            corIconScale = StartCoroutine(iconScale(mainPanel1[0]));
        }
        //从Atk Skill Panel 可以回退到 2号技能面板
        else if (curObj.transform.parent.name == "Battle Skill Panel 2" || curObj.transform.parent.name == "Atk Skill Panel")
        {
            corSwitchOtherButton = StartCoroutine(switchOtherButton(mainPanel2));
            corIconScale = StartCoroutine(iconScale(mainPanel2[0]));
        }
    }



}