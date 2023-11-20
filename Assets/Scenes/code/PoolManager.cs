using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField] private Pool[] Enemiespools;  //����Ķ�������
    private  Dictionary<GameObject, Pool> dictionary;

    protected override void Awake()
    {
        base.Awake();   
        dictionary = new Dictionary<GameObject, Pool>();
        initialize(Enemiespools);
       
    }

    private void OnDisable()    //��������ʱ���ø÷���
    {
#if UNITY_EDITOR
        checkPoolSize();
#endif
       
    }

    public void checkPoolSize()
    {
        foreach (var pool in Enemiespools)
        {
            if (pool.Size < pool.RuntimeSize)
            {
                Debug.LogWarning(string.Format("Pool:{0} runtime size is {1} bigger src size,Please change!",
                    pool.Prefab.name,
                    pool.RuntimeSize, pool.Size));
            }
        }
    }

    private void initialize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
#if UNITY_EDITOR
            if (dictionary.ContainsKey(pool.Prefab))
            {
                Debug.LogError("Same prefab in dictionary!");   //��ֹ�������ͬ��key����Ϊ�ֵ��е�keyҪ����Ψһ
            }
#endif
            dictionary.Add(pool.Prefab, pool);  //��Ӽ�ֵ��
            //�������ж�����е����ж���,ע������Ļ�ռ�Ķ���
            Transform poolParent = new GameObject("Pool: " + pool.Prefab.name,typeof(RectTransform)).transform;   //ָ�������������ص����
            //poolParent.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;   //λ����Ϊ0
            poolParent.parent = transform;  //���ö���صĸ�����ָ��Ϊ��ǰ�����˽ű��Ķ���
            pool.initialize(poolParent);    //��ʼ��
        }
    }

    /// <summary>
    /// ͨ���ֵ��ҵ�Ԥ�����Ӧ�Ķ���أ�Ȼ���ٵõ����õĶ���
    /// </summary>
    /// <param name="prefab">ָ������Ϸ����Ԥ����</param>
    /// <param name="postion">����λ������</param>
    /// <param name="rotation">������ת����</param>
    /// <param name="localScale">���ı�����������</param>
    /// <returns>
    /// �����Ӧ�Ķ�����еĿ��ö���
    /// </returns>
    public  GameObject getObjInPool(GameObject prefab, Vector3 postion, Quaternion rotation, Vector3 localScale)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager: The dictionary does not contain the object");
            return null;
        }
#endif
        return dictionary[prefab].preparedObj(postion, rotation, localScale);   //ͨ��Ԥ��������ҵ���Ӧ�ĳأ�Ȼ��õ��Ѿ�׼���õĶ���
    }

    public  GameObject getObjInPool(GameObject prefab, RectTransform postion)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager: The dictionary does not contain the object");
            return null;
        }
#endif
        return dictionary[prefab].preparedObj(postion);   //ͨ��Ԥ��������ҵ���Ӧ�ĳأ�Ȼ��õ��Ѿ�׼���õĶ���
    }

}