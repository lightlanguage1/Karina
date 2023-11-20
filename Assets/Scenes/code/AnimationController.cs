using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private void OnEnable()
    {
        TransitionSceneDialog.playCompleted += playAnim;
    }

    //�л�������������ʱ����
    private void OnDisable()
    {
        TransitionSceneDialog.playCompleted -= playAnim;
    }

    private void playAnim(Animator[] animators)
    {
        if (Global.instance.curMainSceneName == "main1L")   //�����ɫ��1¥������
        {
            string[] animState = { "Guard up", "Guard right", "Guard down", "Guard left" }; //ע���б���Զ���������aimator��˳��
            //�������н�ɫ�Ķ���
            for (int i = 0; i < animators.Length; i++)
            {
                animators[i].Play(animState[i]);
            }
        }
    }
}