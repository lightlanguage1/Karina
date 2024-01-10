using UnityEngine;

// ����״̬����
public abstract class FSM
{
    protected NPC m_npc; // ���� NPC ��ʵ��

    // ����״̬ʱ���õķ���
    public abstract void onEnter(NPC npc);

    // �߼����·���������״̬����Ҫ�߼�
    public abstract void logicUpdate();

    // ������·�����������������ص��߼������� FixedUpdate �У�
    public abstract void physicsUpdate();

    // �˳�״̬ʱ���õķ���
    public abstract void onExit();
}

// ״̬��GoblinIdleState������״̬��
public class GoblinIdleState : FSM
{
    private float timer;    // ��ʱ����ͣ NPC �ƶ�
    private float timeout;  // ����ȴ�ʱ��

    public override void onEnter(NPC npc)
    {
        m_npc = npc;
        timer = 0f;
        timeout = Random.Range(0, 3); // ������ɵȴ�ʱ��
        m_npc.moveDir = m_npc.generateRandomXyDir(); // ��������ƶ�����
        m_npc.playNextAnim(m_npc.moveDir, "goblin", "Idle"); // ���ſ��ж���
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
                m_npc.switchState(Global.StateType.PatrolState); // û�з�����ң��л���Ѳ��״̬
            }
            else
            {
                m_npc.switchState(Global.StateType.ChaseState); // ������ң��л���׷��״̬
            }
        }
    }

    public override void physicsUpdate()
    {
        // �ڴ�״̬��û���������
    }

    public override void onExit()
    {
        m_npc.animator.StopPlayback(); // ֹͣ���Ŷ���
    }
}

// ״̬��GoblinPatrolState��Ѳ��״̬��
public class GoblinPatrolState : FSM
{
    private float timer;    // ���� NPC �ƶ���ʱ��
    private float timeout;  // Ѳ��ʱ��
    private bool isTimeout; // �Ƿ�ʱ

    public override void onEnter(NPC npc)
    {
        m_npc = npc;
        timer = 0f;
        isTimeout = false;
        timeout = Random.Range(2, 5); // �������Ѳ��ʱ��
        m_npc.moveDir = m_npc.generateRandomXyDir(); // ��������ƶ�����
        m_npc.playNextAnim(m_npc.moveDir, "goblin", "Walk"); // ����Ѳ�߶���
        m_npc.isMove = true;
    }

    public override void logicUpdate()
    {
        if (m_npc.foundPlayer())  // ���������
        {
            // �л�״̬
            m_npc.isMove = false;
            m_npc.switchState(Global.StateType.ChaseState); // �л���׷��״̬
        }
        else
        {
            m_npc.updateMoveDir(); // �����ƶ�����
            timer += Time.deltaTime;
            if (timer >= timeout)
            {
                timer = 0;
                m_npc.switchState(Global.StateType.IdleState); // ��ʱ���л�������״̬
            }
        }
    }

    public override void physicsUpdate()
    {
        // �ڴ�״̬��û���������
    }

    public override void onExit()
    {
        m_npc.animator.StopPlayback(); // ֹͣ���Ŷ���
    }
}

// ״̬��GoblinChaseState��׷��״̬��
public class GoblinChaseState : FSM
{
    private float speed; // ׷���ٶ�
    private float timer; // ��ʱ��
    private bool isSwitch; // �Ƿ�����л�״̬
    private Vector3 startPos; // �л�״̬ʱ����ʼλ��

    public override void onEnter(NPC npc)
    {
        m_npc = npc;
        timer = 0;
        speed = 1.5f; // ����׷���ٶ�
        m_npc.isMove = false;
        isSwitch = false;
        m_npc.playNextAnim(m_npc.moveDir, "goblin", "Walk"); // ����׷�𶯻�
        startPos = m_npc.transform.localPosition; // ��¼�л�״̬ʱ����ʼλ��
    }

    public override void logicUpdate()
    {
        if (!m_npc.foundPlayer() && isSwitch)   // ��������ⷶΧ
        {
            // �л�״̬
            m_npc.switchState(Global.StateType.IdleState); // ��������ⷶΧ���л�������״̬
        }
        else
        {
            checkCollider(); // �����ײ
            timer += Time.deltaTime;
            m_npc.transform.localPosition = Vector3.MoveTowards(m_npc.transform.localPosition, m_npc.playerPos, speed * Time.deltaTime);
            if (timer >= 2f) isSwitch = true; // ��ʱ������2�������л�״̬
        }
    }

    // �����ײ
    private void checkCollider()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(m_npc.transform.position, m_npc.moveDir, m_npc.length, LayerMask.GetMask("BG"));
        if (hit2D.collider)
        {
            m_npc.switchState(Global.StateType.IdleState); // �����ϰ���л�������״̬
        }
    }

    // ��ȡ��������
    public Vector3 getWorldPostion(Vector2Int gridPos)
    {
        Vector3 v3 = m_npc.grid.CellToWorld((Vector3Int)gridPos);
        return v3;
    }

    // ��ȡ׷����
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
        // �ڴ�״̬��û���������
    }

    public override void onExit()
    {
        m_npc.animator.StopPlayback(); // ֹͣ���Ŷ���
    }
}
