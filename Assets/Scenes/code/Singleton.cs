using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>    //����Լ�������߱�����T��Singleton�е�T
{
    public static T instance { get; private set; }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;   //����ת�����̶�д��
        }
        else if (instance != this)  //��Ϊ�״�ʵ����ʱinstance�Ѿ�����˶�������ã���ʱthis�������ʵ��
        {
           Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}