using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class GameTimeManager : MonoBehaviour
{
    public bool inMainScene = true;
    public float time = 0;
    public float timer = 0;

    public TextMeshProUGUI timeText;
    public Image hourHand;
    public Image minuteHand;

    [Header("ʱ��ģ��")]
    // ����Ч������ʱ��
    public float duration;

    [SerializeField] private List<LightControl> sceneLights; // ���泡���е����еƹ�
    private LightShift lightShift = LightShift.Forenoon;
    public static GameTimeManager instance;
  



    // �������ɫ��
    /* public Color morningColor = Color.white;
     public Color nightColor = Color.blue;
     public Color forenoonColor = Color.blue;
     public Color noonColor = Color.blue;
     public Color afternoonColor = Color.blue;
     public Color midnightColor = Color.blue;*/

    // ��� setGameVariable ���
    public int year = 1;
    public int month = 1;
    public int day = 1;
    public int hour = 0;
    public int minute = 0;
    public int timeZone = 0;
    public int weatherType = 0;
    public int totalTime = 0;
    public int totalDay = 1;

    // ��� Game_Chronus ���
    private Game_Chronus chronus;

    private void Start()
    {
        // ��ʼ�� Game_Chronus
        inMainScene = true;
        duration = 20f;
        // ��ʼ�� sceneLights �б�
        sceneLights = new List<LightControl>();
        instance = this;  // ʹ�õ�ǰʵ����ֵ�� instance
        UpdateLights(LightShift.Forenoon);
    }


    private int currentPhase = 0;  // ����׷�ٵ�ǰʱ���

    private void Update()
    {
        if (inMainScene)
        {
            time += Time.deltaTime / 1;

            // ���� setGameVariable
            UpdateGameVariables();

            if (time >= 4320)
            {
                if (timer >= 1)
                {
                    timer = 1;
                    timeText.text = "�賿";
                  
                    // ���ü�ʱ��������
                    ResetTimerAndDay();

                    return;
                }

                timer++;

                time -= 4320;
            }
            UpdateOtherTexts(currentPhase);
            // ���ü�ʱ����ʱ���
            SetTimerAndPhase();
            // ����ʱ�� UI
            UpdateClockUI();

        }
    }

    private void UpdateLights(LightShift lightShift)
    {
        // ���֮ǰ����ĵƹ�ؼ��б�
        sceneLights.Clear();
        // ���ҳ��������еĵƹ�ؼ�����ӵ��б���
        sceneLights.AddRange(FindObjectsByType<LightControl>(FindObjectsSortMode.None));
        // �������еƹ�ؼ��������µĵƹ�ת�����ͽ��еƹ�仯
        foreach (var light in sceneLights)
        {
            light.ChangeLight(lightShift);
        }
    }

    // ���ü�ʱ����ʱ���

    private void SetTimerAndPhase()
    {
        if (time >= 720 && time < 1440)
        {
            timer = 1;
            currentPhase = 1;
            UpdateLights(LightShift.Forenoon);
        }
        else if (time >= 1440 && time < 2160)
        {
            timer = 2;
            currentPhase = 2;
            UpdateLights(LightShift.Morning);
        }
        else if (time >= 2160 && time < 2880)
        {
            timer = 3;
            currentPhase = 3;
            UpdateLights(LightShift.Noon);
        }
        else if (time >= 2880 && time < 3600)
        {
            timer = 4;
            currentPhase = 4;
            UpdateLights(LightShift.Afternoon);
        }
        else if (time >= 3600 && time < 4320)
        {
            timer = 5;
            currentPhase = 5;
            UpdateLights(LightShift.Night);
        }
        else if (time >= 4320)
        {
            timer = 6;
            currentPhase = 6;
            UpdateLights(LightShift.Midnight);
            // ������һ�����ü�ʱ��
            IncrementDay();
            ResetTimerAndDay();
        }
    }

    private void UpdateOtherTexts(int currentPhase)
    {
        switch (currentPhase)
        {
            case 1:
                timeText.text = "�賿";
                break;
            case 2:
                timeText.text = "����";
                break;
            case 3:
                timeText.text = "����";
                break;
            case 4:
                timeText.text = "����";
                break;
            case 5:
                timeText.text = "����";
                break; 
            case 6:
                timeText.text = "��ҹ";
                break;
            default:
                //Ĭ�ϵĸ����߼��������� night ������
                timeText.text = "�賿";
                break;
        }
        //Debug.Log("Text updated: " + timeText.text); // ������н��е���
    }

    // ���ü�ʱ��������
    private void ResetTimerAndDay()
    {
        timer = 0;
        currentPhase = 0;
    }

    // ������һ
    private void IncrementDay()
    {
        totalDay++;
    }


    // ���� setGameVariable
    void UpdateGameVariables()
    {
        // ȷ�� Game_Chronus ����Ϊ��
        if (chronus == null)
        {
            return;
        }
        year = chronus.getYear();
        month = chronus.getMonth();
        day = chronus.getDay();
        hour = chronus.getHour();
        minute = chronus.getMinute();
        timeZone = chronus.getTimeZone();
        weatherType = chronus.getWeatherTypeId();
        totalTime = chronus.getTotalTime();
        totalDay = chronus.getTotalDay();
    }
    public class Game_Chronus
    {
        private DateTime _nowDate;
        private int _dayMeter;
        private int _timeMeter;
        private int _frameCount;
        private bool _disablePadding;

        private List<int> _daysOfMonth;
        private string[] _weekNames;
        private string[] _monthNames;

        private static (int start, int end, int timeId)[] timeZone;

        private int _weatherType;

        public Game_Chronus()
        {
            // �ڹ��캯���г�ʼ����Ҫ�ı������б�
            _daysOfMonth = new List<int> { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            _weekNames = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            _monthNames = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            timeZone = new (int, int, int)[]
  {
    // ��ʽ��(��ʼСʱ, ����Сʱ, ʱ��ID)
    (0, 6, 1),    // �賿 0 �㵽���� 6 �㣬ʱ��ID 1
    (6, 12, 2),   // ���� 6 �㵽���� 12 �㣬ʱ��ID 2
    (12, 18, 3),  // ���� 12 �㵽���� 6 �㣬ʱ��ID 3
    (18, 24, 4)   // ���� 6 �㵽���� 12 �㣬ʱ��ID 4
  };


            // ��ʼ����������
            _nowDate = DateTime.Now;
            _dayMeter = 0;
            _timeMeter = 0;
            _frameCount = 0;
            _disablePadding = false;
            _weatherType = 0;
        }

        public int getYear()
        {
            return _nowDate.Year;
        }

        public int getMonth()
        {
            return _nowDate.Month;
        }

        public int getDay()
        {
            return _nowDate.Day;
        }

        public int getHour()
        {
            return _nowDate.Hour;
        }

        public int getMinute()
        {
            return _nowDate.Minute;
        }

        public int getTimeZone()
        {
            int timeId = 0;
            foreach (var zoneInfo in timeZone)
            {
                if (isHourInRange(zoneInfo.start, zoneInfo.end)) timeId = zoneInfo.timeId;
            }
            return timeId;
        }

        public int getWeatherTypeId()
        {
            return _weatherType;
        }

        public int getTotalTime()
        {
            return (int)(_nowDate - DateTime.Now).TotalMinutes;
        }

        public int getTotalDay()
        {
            return (int)Math.Floor((double)getTotalTime() / (24 * 60)) + 1;
        }

        private bool isHourInRange(int min, int max)
        {
            var hour = getHour();
            return hour >= min && hour <= max;
        }
    }

    // �л�����ɫ��
/*    void ChangeWeatherColor(LightShift shift)
    {
        switch (shift)
        {
            case LightShift.Morning:
                Camera.main.backgroundColor = morningColor;
                break;
            case LightShift.Forenoon:
                Camera.main.backgroundColor = forenoonColor;
                break;
            case LightShift.Noon:
                Camera.main.backgroundColor = noonColor;
                break;
            case LightShift.Afternoon:
                Camera.main.backgroundColor = afternoonColor;
                break;
            case LightShift.Night:
                Camera.main.backgroundColor = nightColor;
                break;
            case LightShift.Midnight:
                Camera.main.backgroundColor = midnightColor;
                break;
            default:
                // Ĭ��ʹ�� nightColor �������������߼�
                Camera.main.backgroundColor = nightColor;
                break;
        }
    }*/



    // ʱ�� UI ����
    private void UpdateClockUI()
    {
        // ����ʱ��ͷ������ת�Ƕ�
        float hourRotation = -(time / 4320f) * 360f;  // 360 ���ʾһСʱ
        float minuteRotation = -(time % 4320f / 60f) * 360f;  // 60 ���ʾһ����

        // ����ʱ�ӵ���ת
        hourHand.rectTransform.rotation = Quaternion.Euler(0f, 0f, hourRotation);
        minuteHand.rectTransform.rotation = Quaternion.Euler(0f, 0f, minuteRotation);
    }

}