using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Global;

//ί��Ӧ�ö�д���������ҵ����⣬֮��Ϳ����Ż���
public class EventHandler 
{
    public static event Action<SoundType> onSoundChange;
    public static void callSoundChangeEvent(SoundType type)
    {
        onSoundChange?.Invoke(type);
    }

    public static event Action<bool> onTrigger;
    public static void callTriggerStopEvent(bool isStop)
    {
        onTrigger?.Invoke(isStop);
    }
}
