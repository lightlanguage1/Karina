using System.Collections.Generic;
using UnityEngine;
using static Global;

public class NPC : MonoBehaviour
{
    [Header("敌人基础信息")]
    public bool playerInRange = false;

    public bool isUP = false;   //是否从触发器上方进入

    public bool isDiscoverPlayer;   //是否发现了玩家

    //通过精灵渲染器改变层级，让玩家在npc上方时被遮挡
    private SpriteRenderer playerRenderer;

    private SpriteRenderer npcRenderer;

    private bool triggerCompleted;

    [Header("敌人动画器")]
    public Animator animator;

    [Header("计时器等待时间")]
    public float waitTime;

    public float timer;
    public bool isWait;

    [Header("npc移动")]
    public float speed;

    public float length;  //射线长度
    public Vector3 moveDir;
    public bool isMove;

    [Header("npc2d刚体和检测")]
    public Rigidbody2D rb;

    public Vector2 offset;
    public Vector2 checkSize;
    public float checkDis;
    public LayerMask attackMask;

    [Header("算法")]
    public AStar astar;

    [Header("玩家的位置")]
    public Vector3 playerPos;

    public Grid grid;

    //拥有的状态
    public Dictionary<StateType, FSM> states;

    [HideInInspector] public StateType curState;

    private void Awake()
    {
        states = new Dictionary<StateType, FSM>
        {
            {StateType.IdleState,new GoblinIdleState()},
            {StateType.PatrolState,new GoblinPatrolState()},
            {StateType.ChaseState,new GoblinChaseState()}
        };

        triggerCompleted = false;
    }

    private void OnEnable()
    {
        curState = StateType.IdleState;
        isMove = false;
        moveDir = generateRandomXyDir();
        states[curState].onEnter(this);
        EventHandler.onTrigger += stopTrigger;
    }

    private void OnDisable()
    {
        EventHandler.onTrigger -= stopTrigger;
    }

    //回调
    private void stopTrigger(bool isStop)
    {
        triggerCompleted = isStop;
    }

    private void Start()
    {
        string selfName = gameObject.name;
        Vector3 v3;
        if (Global.instance.posDic.ContainsKey(selfName))
        {
            v3 = Global.instance.posDic[selfName];
        }
        else
        {
            //用作测试
            v3 = new Vector3(Global.instance.enemy_x, Global.instance.enemy_y, Global.instance.enemy_z);
        }
        transform.localPosition = new Vector3(v3.x, v3.y, 0);
    }

    private void FixedUpdate()
    {
        if (isMove)
        {
            Vector3 v3 = new Vector3(moveDir.x * Random.Range(speed, speed * 2) * Time.deltaTime,
                moveDir.y * Random.Range(speed, speed * 2) * Time.deltaTime, 0);
            rb.MovePosition((Vector2)v3 + (Vector2)transform.position);
        }
    }

    private void Update()
    {
        states[curState].logicUpdate();
    }

    // 玩家进入碰撞体时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 如果进入的是玩家
        if (other.CompareTag("Player") && !triggerCompleted)
        {
            EventHandler.callTriggerStopEvent(true);    //防止多个触发器同时触发
            isDiscoverPlayer = true;
            if (isUP)
            {
                playerRenderer = other.GetComponent<SpriteRenderer>();
                npcRenderer = GetComponent<SpriteRenderer>();
                npcRenderer.sortingOrder += playerRenderer.sortingOrder;
            }
            NPC[] enemies = FindObjectsByType<NPC>(FindObjectsSortMode.None);
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].GetComponent<Collider2D>().isTrigger = false;
                enemies[i].rb.Sleep();
                enemies[i].animator.StopPlayback();
            }
            // 设置玩家在NPC附近
            playerInRange = true;
            Global.instance.battlePrevNpcName = gameObject.name; //获得要战斗的敌人名字
            //遇敌
            //将所有敌人的当前位置添加到字典中 可以优化比如做多个分区 一个区对应一个字典什么的
            var parent = GameObject.Find("Enemies");
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                if (!Global.instance.posDic.ContainsKey(parent.transform.GetChild(i).name)) //防止添加重复的键，因为添加代码在每次遇敌时都会运行
                {
                    Global.instance.posDic.Add(parent.transform.GetChild(i).name, parent.transform.GetChild(i).localPosition);
                }
            }
            //进入战斗场景
            SceneLoader.instance.loadGameScene((int)SceneEnumVal.BattleScene);
        }
    }

    // 玩家离开碰撞体时调用
    private void OnTriggerExit2D(Collider2D other)
    {
        // 如果离开的是玩家
        if (other.CompareTag("Player"))
        {
            if (isUP)
            {
                isUP = false;
                npcRenderer.sortingOrder -= playerRenderer.sortingOrder;
            }
            // 设置玩家不在NPC附近
            playerInRange = false;
        }
    }

    public void switchState(StateType state)
    {
        switch (state)
        {
            case StateType.IdleState:
                {
                    states[curState].onExit();
                    curState = StateType.IdleState;
                    states[curState].onEnter(this);
                    break;
                }
            case StateType.PatrolState:
                {
                    states[curState].onExit();
                    curState = StateType.PatrolState;
                    states[curState].onEnter(this);
                    break;
                }
            case StateType.ChaseState:
                {
                    states[curState].onExit();
                    curState = StateType.ChaseState;
                    states[curState].onEnter(this);
                    break;
                }
        }
    }

    public void updateMoveDir()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, moveDir, length, LayerMask.GetMask("BG"));
#if UNITY_EDITOR
        //Debug.DrawRay(transform.position, moveDir * length);
#endif
        if (hit2D.collider)   //击中碰撞体 说明前方有碰撞体 掉头
        {
#if UNITY_EDITOR
            //Debug.Log("碰撞体被击中：" + hit2D.collider.gameObject.name);
#endif
            moveDir = -moveDir;
            playNextAnim(moveDir, "goblin", "Walk");
        }
        else
        {
#if UNITY_EDITOR
            //Debug.LogWarning("没有击中碰撞体");
#endif
            isMove = true;
        }
    }

    public void playNextAnim(Vector3 moveDir_, string npcName, string actionName)
    {
        if (moveDir_.x == 1)
        {
            animator.StopPlayback();
            animator.Play(npcName + actionName + "Right");
        }
        else if (moveDir_.x == -1)
        {
            animator.StopPlayback();
            animator.Play(npcName + actionName + "Left");
        }
        else if (moveDir_.y == 1)
        {
            animator.StopPlayback();
            animator.Play(npcName + actionName + "Up");
        }
        else if (moveDir_.y == -1)
        {
            animator.StopPlayback();
            animator.Play(npcName + actionName + "Down");
        }
    }

    public Vector3 generateRandomXyDir()
    {
        // 生成一个随机数来表示方向
        int randomDirection = Random.Range(0, 4);

        switch (randomDirection)
        {
            case 0: // 上
                return Vector3.up;

            case 1: // 下
                return Vector3.down;

            case 2: // 左
                return Vector3.left;

            case 3: // 右
                return Vector3.right;
        }
        return Vector3.zero;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos() //用于射线可视化，方便调试
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, length * moveDir);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + offset, checkSize);
        Gizmos.DrawRay((Vector2)transform.position + offset, checkDis * moveDir);
    }

#endif

    public bool foundPlayer()
    {
        //distance 参数表示盒形投射的最大距离
        RaycastHit2D hit2D = Physics2D.BoxCast((Vector2)transform.position + offset, checkSize, 0, moveDir, checkDis, attackMask);
        if (hit2D.collider == null) return false;
        playerPos = hit2D.collider.transform.localPosition;
        return true;
    }
}