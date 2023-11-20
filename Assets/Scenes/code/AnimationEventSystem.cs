using UnityEngine;

public class AnimationEventSystem : MonoBehaviour
{
    //������������ϣ�����������message ���øýű��ڵ�ע��ķ���
    public void onAnimationEnd()
    {
        gameObject.SetActive(false);       
    }

    public void onPlayerMove()
    {
        EventHandler.callSoundChangeEvent(Global.SoundType.Walk);
    }

    public void onDaji()
    {
        EventHandler.callSoundChangeEvent(Global.SoundType.Daji);
    }

    public void onXuli()
    {
        EventHandler.callSoundChangeEvent(Global.SoundType.Xuli);
    }
}
