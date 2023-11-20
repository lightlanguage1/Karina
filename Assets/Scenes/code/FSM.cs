using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.UIElements;

public abstract class FSM
{
    protected NPC m_npc;

    public abstract void onEnter(NPC npc);

    public abstract void logicUpdate();

    public abstract void physicsUpdate();

    public abstract void onExit();
}

public class GoblinIdleState : FSM
{
    private float timer;    //��ʱ����ͣnpc�ƶ�
    private float timeout;

    public override void onEnter(NPC npc)
    {
        m_npc = npc;
        timer = 0f;
        timeout = Random.Range(0, 3);
        m_npc.moveDir = m_npc.generateRandomXyDir();
        m_npc.playNextAnim(m_npc.moveDir, "goblin", "Idle");
        m_npc.isMove = false;
    }

    public override void logicUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= timeout)
        {
            timer = 0;
            if (!m_npc.foundPlayer())
            {
                m_npc.switchState(Global.StateType.PatrolState);
            }
            else
            {
                m_npc.switchState(Global.StateType.ChaseState);
            }
        }
    }

    public override void physicsUpdate()
    {
    }

    public override void onExit()
    {
        m_npc.animator.StopPlayback();
    }
}

public class GoblinPatrolState : FSM
{
    private float timer;    //����npc�ƶ���ʱ��
    private float timeout;
    private bool isTimeout;

    public override void onEnter(NPC npc)
    {
        m_npc = npc;
        timer = 0f;
        isTimeout = false;
        timeout = Random.Range(2, 5);
        m_npc.moveDir = m_npc.generateRandomXyDir();
        m_npc.playNextAnim(m_npc.moveDir, "goblin", "Walk");
        m_npc.isMove = true;
    }

    public override void logicUpdate()
    {
        if (m_npc.foundPlayer())  //���������
        {
            //�л�״̬
            m_npc.isMove = false;
            m_npc.switchState(Global.StateType.ChaseState);
        }
        else
        {
            m_npc.updateMoveDir();
            timer += Time.deltaTime;
            if (timer >= timeout)
            {
                timer = 0;
                m_npc.switchState(Global.StateType.IdleState);
            }
        }
    }

    public override void physicsUpdate()    //����fixedupdate�� ��������
    {
    }

    public override void onExit()
    {
        m_npc.animator.StopPlayback();
    }
}

public class GoblinChaseState : FSM
{
    private float speed;
    private float timer;
    private bool isSwitch;
    private Vector3 startPos;

    public override void onEnter(NPC npc)
    {
        m_npc = npc;
        timer = 0;
        speed = 1.5f;
        m_npc.isMove = false;
        isSwitch = false;
        m_npc.playNextAnim(m_npc.moveDir, "goblin", "Walk");
        startPos = m_npc.transform.localPosition;
    }

    public override void logicUpdate()
    {
        if (!m_npc.foundPlayer() && isSwitch)   //��������ⷶΧ
        {
            //�л�״̬
            m_npc.switchState(Global.StateType.IdleState);
        }
        else
        {
            checkCollider();
            timer += Time.deltaTime;
            m_npc.transform.localPosition = Vector3.MoveTowards(m_npc.transform.localPosition, m_npc.playerPos, speed * Time.deltaTime);
            if (timer >= 2f) isSwitch = true;
        }
    }

    private void checkCollider()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(m_npc.transform.position, m_npc.moveDir, m_npc.length, LayerMask.GetMask("BG"));
        if (hit2D.collider)
        {
            m_npc.switchState(Global.StateType.IdleState);
        }
    }

    public Vector3 getWorldPostion(Vector2Int gridPos)
    {
        Vector3 v3 = m_npc.grid.CellToWorld((Vector3Int)gridPos);
        return v3;
    }

    public Vector3 getChaseDir(Vector3 curPos, Vector3 nextPos)
    {
        // �����ƶ���������
        Vector3 chaseDir = nextPos - curPos;

        // ��һ���ƶ���������
        chaseDir.Normalize();

        // ���ع�һ������ƶ���������
        return chaseDir;
    }

    public override void physicsUpdate()
    {
    }

    public override void onExit()
    {
        m_npc.animator.StopPlayback();
    }
}