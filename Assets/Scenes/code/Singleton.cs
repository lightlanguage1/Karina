using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>    //泛型约束，告诉编译器T是Singleton中的T
{
    public static T instance { get; private set; }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;   //必须转换，固定写法
        }
        else if (instance != this)  //因为首次实例化时instance已经获得了对象的引用，此时this是子类的实例
        {
           Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}