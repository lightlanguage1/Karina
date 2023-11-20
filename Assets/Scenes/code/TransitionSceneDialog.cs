using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(1)]  //����ִ�иýű�
public class TransitionSceneDialog : MonoBehaviour
{
    [Header("���ɳ����ı��ļ�")]
    public TextAsset textFile;

    private List<string> list = new List<string>();

    public float textSpeed;

    public TextMeshProUGUI dialogText;

    public int index;

    public delegate void animationPlayCompleted(Animator[] animators);

    public static event animationPlayCompleted playCompleted;

    [SerializeField] private List<GameObject> objs;    //��̬����������������������������

    public Animator[] animators;

    private void Awake()
    {
        readTextFromFile();
        index = 0;
    }

    private void Start()
    {
        StartCoroutine(setText());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && index == list.Count)     //�ı���ʾ���
        {
            gameObject.SetActive(false);
            index = 0;
            foreach (GameObject obj in objs)
            {
                obj.SetActive(true);
            }
            playCompleted?.Invoke(animators);
        }
    }

    private IEnumerator setText()
    {
        dialogText.text = "";
        //�����ı���һ�е�ÿ���ַ�
        for (int i = 0; i < list[index].Length; i++)
        {
            dialogText.text += list[index][i];
            yield return new WaitForSeconds(textSpeed);
        }
        index++;    //��һ��
    }

    private void readTextFromFile()
    {
        list.Clear();
        string[] lineData = textFile.text.Split('\n');
        foreach (string line in lineData)
        {
            list.Add(line);
        }
    }
}