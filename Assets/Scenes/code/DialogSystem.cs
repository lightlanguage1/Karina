using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [Header("UI���")]
    public Image dialogImg;

    public Text dialogText;

    public float textSpeed;

    // ����UI����
    public GameObject interactUI;

    [Header("NPC�ı��ļ�")]
    public TextAsset textFile;

    [Header("�Ƿ��ڶԻ�")]
    public bool isTalk;

    public int index = 0;

    private karryn playerScript;    //���濨�սű��ı���

    private List<string> list = new List<string>(); //�������list��ʼ��

    //ί�У��¼�ʵ�����Ǹ����ٶȣ�ʵ�ֶ���ͨ�ţ�
    public delegate void heroSpeedDele(bool isTalk);
    public static event heroSpeedDele heroSpeedEvent;

    private void Awake()
    {
        readTextFromFile();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && index == list.Count) //ѭ������list�е�string ��ȻԽ��
        {
            //index == list.Count ˵����ʱ�Ѿ��Ի�����ˣ�indexΪ���Ԫ�ص��¸�Ԫ������
            playerScript.enabled = true;
            playerScript.GetComponent<Animator>().enabled = true;
            playerScript.GetComponent<Rigidbody2D>().WakeUp();
            index = 0;
            dialogImg.gameObject.SetActive(false);  //���öԻ����Լ����е��ı�
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isTalk)
        {
            if (index == 0) //indexΪ0˵����һ�ΰ��ո����������öԻ���
            {
                //���öԻ����Լ��ı�
                dialogImg.gameObject.SetActive(true);
                //������ҵĿ������ű�,��ֹ���δ��ɶԻ����߶�
                playerScript = GameObject.Find("Karryn").GetComponent<karryn>();
                //���Ұ����ƶ����󣬸�����ٶȸ�ֵΪ2��ֻ�е���ֹͣ����ʱ�����ٶȲŻ�Ϊ0������ͣ������
                //��˽��ýű�ʱ���Ҳ�û��ֹͣ���루���֣�Ҳ��ʱ�ٶȻ���2�����Խ�ɫ��һֱ�ƶ�
                playerScript.enabled = false;   //���øýű���ondisable����
                playerScript.GetComponent<Animator>().enabled = false;
                playerScript.GetComponent<Rigidbody2D>().Sleep(); 
                //���ؽ�����ʾ
                interactUI.SetActive(false);
            }
            //����Э�̣������ʾ�����е���
            StartCoroutine(setText());
        }
    }

    IEnumerator setText()
    {
        isTalk = false; //��ֹ����ٴΰ�������������bug
        dialogText.text = "";   //��գ���ֹ�����ϴεĶԻ�
        for(int i = 0; i < list[index].Length;i++)
        {
            dialogText.text += list[index][i];  //���ַ������еĵ�һ���ַ���ӵ��ı���
            yield return new WaitForSeconds(textSpeed); //text��ʾһ���ַ���ȴ�����������ʾ��һ��
        }
        index++;    //ѭ��������һ�仰ȫ����ʾ��
        isTalk = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTalk = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTalk = false;
            if (interactUI.activeSelf)   //��ֹ��Ұ�space�Ի����������ٴν�����
            {
                interactUI.SetActive(false);
            }
        }
    }

    private void readTextFromFile()
    {
        list.Clear();   //���������£�ȷ���ϴζԻ���������ݱ����
        string[] lineData = textFile.text.Split('n');
        foreach (var line in lineData)
        {
            list.Add(line);
        }
    }
}