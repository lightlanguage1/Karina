using UnityEngine;

public class FPS : MonoBehaviour
{
    /// <summary>
    /// ��һ�θ���֡�ʵ�ʱ��
    /// </summary>
    private float m_lastUpdateShowTime = 0f;
    /// <summary>
    /// ������ʾ֡�ʵ�ʱ����
    /// </summary>
    private readonly float m_updateTime = 0.05f;
    /// <summary>
    /// ֡��
    /// </summary>
    private int m_frames = 0;
    /// <summary>
    /// ֡����
    /// </summary>
    private float m_frameDeltaTime = 0;
    private float m_FPS = 0;
    private Rect m_fps, m_dtime;
    private GUIStyle m_style = new GUIStyle();

    void Awake()
    {
        Application.targetFrameRate = 120;
    }

    void Start()
    {
        m_lastUpdateShowTime = Time.realtimeSinceStartup;
        m_fps = new Rect(0, 0, 100, 100);
        m_dtime = new Rect(0, 100, 100, 100);
        m_style.fontSize = 100;
        m_style.normal.textColor = Color.red;
    }

    void Update()
    {
        m_frames++;
        if (Time.realtimeSinceStartup - m_lastUpdateShowTime >= m_updateTime)
        {
            m_FPS = m_frames / (Time.realtimeSinceStartup - m_lastUpdateShowTime);
            m_frameDeltaTime = (Time.realtimeSinceStartup - m_lastUpdateShowTime) / m_frames;
            m_frames = 0;
            m_lastUpdateShowTime = Time.realtimeSinceStartup;
            //Debug.Log("FPS: " + m_FPS + "�����: " + m_FrameDeltaTime);
        }
    }

    void OnGUI()
    {
        GUI.Label(m_fps, "FPS: " + m_FPS, m_style);
        GUI.Label(m_dtime, "���: " + m_frameDeltaTime, m_style);
    }
}
