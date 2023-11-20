using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    [SerializeField] private GameObject prefab; //����Ԥ����
    [SerializeField] private Queue<GameObject> queue;
    [SerializeField] private int size = 1;  //���д�С
    public GameObject Prefab => prefab; //���Ա��ʽ������˽�е�Ԥ�������
    public int Size => size;    
    public int RuntimeSize => queue.Count;  //�������ʱ�����ʵ�ʴ�С���Ż�������ֹ������
    private Transform parent;

    private GameObject spawnObj()   //���ɶ���
    {
        var obj = GameObject.Instantiate(prefab, parent);   //����Ԥ���岢Ϊ��ָ��������
        obj.SetActive(false);
        obj.name = obj.name.Replace("(Clone)", "");
        return obj;
    }

    public void initialize(Transform parent)
    {
        queue = new Queue<GameObject>();    //��������
        this.parent = parent;

        for (int i = 0; i < size; i++)
        {
            queue.Enqueue(spawnObj());
        }
    }

    private GameObject getAvailableObj()
    {
        GameObject availableObj = null;
        if (size > 1 && !queue.Peek().activeSelf)   //���г������ٴ���1�����Ҳ��ܽ����׻�Ծ������ִ������ģ����������
        {
            availableObj = queue.Dequeue();
        }
        else
        {
            availableObj = spawnObj();
        }
        //���ӵĶ���ִ��������������Ϊ�ǽ����β������д���㣬������дһ����Ӻ�����
        queue.Enqueue(availableObj);

        return availableObj;
    }

    public GameObject preparedObj(RectTransform position)   //׼���õĶ���
    {
        var obj = getAvailableObj();
        obj.GetComponent<RectTransform>().anchoredPosition = position.anchoredPosition;
        obj.SetActive(true);
        return obj;
    }
    
    public GameObject preparedObj(Vector3 position, Quaternion rotation)    //Quaternion ������ת
    {
        var obj = getAvailableObj();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        return obj;
    }

    public GameObject preparedObj(Vector3 position, Quaternion rotation,Vector3 localScale) //���������ǻ��ܵ������������Ӱ�죨һ������ָ��������
    {
        var obj = getAvailableObj();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.localScale = localScale;
        obj.SetActive(true);
        return obj;
    }
}