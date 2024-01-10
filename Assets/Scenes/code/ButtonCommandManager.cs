using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//ͨ��ö��ֵ���ӿɶ���
public enum ChildrenType   //�Ӷ������� ����ʾ���ĸ��Ӷ���
{
    BG = 0,
    EnemyBar,
    Karryn,
    SkillEffect,
    BattleSkillPanel1,
    KarrynPropertyBar,
    Numbers,
    BattleSkillPanel2,
    VolitionPanel,
    SpiritualPanel1,
    SpiritualPanel2,
    AtkSkillPanel,
    SexSkillPanel,
    InfoPanel,
    SelectFrame,
    FrameCursor,
    PostProcess,
}

public enum SkillType //����ʵ�ִ��룬�����������Ż�
{
    daji = 1,
    xuli,
    mishujiashi,
    kanpojiashi,
    roubangguancha
}

public enum PanelType   //ע��Ͳ㼶�����е��Ӷ�����������һ��
{
    Karryn = 2,
    SkillEffect,
    BattleSkillPanel1 = 4,
    KarrynPropertyBar,
    Numbers,
    BattleSkillPanel2,
    VolitionPanel,
    SpiritualPanel1,
    SpiritualPanel2,
    SexPanel,
    AtkSkillPanel,
    InfoPanel
}

public enum panelInChildType
{
    Cursor = 0,
    Focus
}

public enum KarrynState
{
    Weapon1 = 0,
    Weapon2,
    Atk1
}

public interface Icommand
{
    void execute(); //����ӿ�

    //�ӿ����ڶ���ĺ�����������������ȫ��ʵ��
}

//��������Լ�������
public class SwitchPannel2Command : Icommand
{
    public void execute()
    {
        //ʹ��GetChild ���Ի�ȡ����or���õ��Ӷ���
        BattleManager battleManager = BattleManager.instance;

        battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel1).gameObject.SetActive(false);
        //ֹͣ���ź���ת��Э��,ע���ʱ��ť�������ţ�����Ϊ�˱�֤��ť��Сһ�£�Ҫ�ȵ����������Ŵ�С
        battleManager.StopCoroutine(battleManager.corSwitchOtherButton);
        battleManager.StopCoroutine(battleManager.corIconScale);
        battleManager.mainPanel1[0].transform.localScale = Vector3.one; //��ԭ

        battleManager.curPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject;
        battleManager.curPanel.SetActive(true);
        battleManager.useKeyboardOperationMainButton(battleManager.curPanel.transform.GetChild(0).gameObject);  //0���ȡ��
    }
}

public class CharacterInfo : Icommand
{
    public void execute()
    {
    }
}

public class switchAttackSkillPanelCommand : Icommand  //�������
{
    public void execute()
    {
        //ֹͣ��Э�̷�ֹ�����ƶ�ѡ�п�
        BattleManager.instance.StopCoroutine(BattleManager.instance.coroutineSeletedEnemy);
        openSkillPanel(ChildrenType.AtkSkillPanel);
    }

    public void openSkillPanel(ChildrenType type)
    {
        BattleManager battleManager = BattleManager.instance;
        GameObject frame = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.SelectFrame).gameObject;
        GameObject cursor = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.FrameCursor).gameObject;
        frame.SetActive(false);
        cursor.SetActive(false);
        battleManager.canvas.transform.GetChild((int)type).gameObject.SetActive(true);

        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;
        battleManager.animDic[foucsName].Play("focus");
        battleManager.isPanel = true;
    }
}

public class attackButtonCommand : Icommand //������ť
{
    public void execute()
    {
        BattleManager battleManager = BattleManager.instance;

        //ֹͣ���ź���ת��Э��,ע���ʱ��ť�������ţ�����Ϊ�˱�֤��ť��Сһ�£�Ҫ�ȵ����������Ŵ�С
        battleManager.StopCoroutine(battleManager.corSwitchOtherButton);
        battleManager.StopCoroutine(battleManager.corIconScale);
        battleManager.mainPanel2[0].transform.localScale = Vector3.one; //��ԭ

        battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject.SetActive(false);
        battleManager.curPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.AtkSkillPanel).gameObject;
        battleManager.curPanel.SetActive(true);

        //����  focus ����
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;

        battleManager.animDic[foucsName].Play("focus");

        battleManager.isPanel = true;
    }
}

public class DefenceButtonCommand : Icommand
{
    public void execute()
    {
    }
}

public class dajiButtonCommand : Icommand
{
    public void execute()
    {
        this.closeSkillPannel(ref BattleManager.instance.curPanel);
        showSelectFrame();
    }

    public void showSelectFrame()
    {
        GameObject frame = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.SelectFrame).gameObject;
        GameObject cursor = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.FrameCursor).gameObject;
        frame.SetActive(true);
        cursor.SetActive(true);
        //��ֹͣFixedUpdate�ڵ� checkUserMoveAction(); Ȼ��������Э��ִ��ѡ�е��˹�������
        BattleManager.instance.isPanel = false;
        BattleManager.instance.isInputEnable = true;
        BattleManager.instance.coroutineSeletedEnemy = BattleManager.instance.
        StartCoroutine(BattleManager.instance.beginSelectEnemyAtk(BattleManager.instance.prefabs, SkillType.daji));
    }

    public void closeSkillPannel(ref GameObject curPanel_)
    {
        BattleManager battleManager = BattleManager.instance;
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;
        battleManager.animDic[foucsName].StopPlayback();
        curPanel_.SetActive(false);
    }
}

public class zhanjiButtonCommand : Icommand
{
    public void execute()
    {
    }
}

public class cijiButtonCommand : Icommand
{
    public void execute()
    {
    }
}

public class rollBackBattleSkillPanel2 : Icommand
{
    public void execute()
    {
        BattleManager battleManager = BattleManager.instance;
        battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject.SetActive(true);

        battleManager.curPanel.SetActive(false);
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;
        if (battleManager.animDic.ContainsKey(foucsName))
        {
            battleManager.animDic[foucsName].StopPlayback();
        }
        //��Ϊ�ж����������Ի���ʱҪ����ж�
        if (battleManager.curPanel.name == "Info Panel")
        {
            battleManager.StopCoroutine(battleManager.corStartInfoPanel);
        }
        battleManager.isPanel = false;
        battleManager.curPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject;
        battleManager.useKeyboardOperationMainButton(battleManager.curPanel.transform.GetChild(0).gameObject);  //0���ȡ��
        BattleManager.instance.isInputEnable = true;
    }
}

public class rollBackBattleSkillPanel1 : Icommand
{
    public void execute()
    {
        BattleManager battleManager = BattleManager.instance;

        if (battleManager.corStartInfoPanel != null)
        {
            battleManager.StopCoroutine(battleManager.corStartInfoPanel);
            battleManager.corStartInfoPanel = null;
        }
        battleManager.curPanel.SetActive(false);
        battleManager.curPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel1).gameObject;
    }
}

public class switchInfoPanel : Icommand
{
    public void execute()
    {
        //���µ�ǰ���
        var battleManager = BattleManager.instance;
        BattleManager.instance.curPanel = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.InfoPanel).gameObject;
        BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject.SetActive(false);
        BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.InfoPanel).gameObject.SetActive(true);
        BattleManager.instance.corStartInfoPanel = BattleManager.instance.StartCoroutine(BattleManager.instance.startInfoPanel());
        battleManager.StopCoroutine(battleManager.corSwitchOtherButton);
        battleManager.StopCoroutine(battleManager.corIconScale);
        battleManager.mainPanel2[0].transform.localScale = Vector3.one; //��ԭ
    }
}

public class switchVolitionPanel : Icommand
{
    public void execute()
    {
        var battleManager = BattleManager.instance;
        BattleManager.instance.curPanel = BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.VolitionPanel).gameObject;
        BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel1).gameObject.SetActive(false);
        BattleManager.instance.canvas.transform.GetChild((int)ChildrenType.VolitionPanel).gameObject.SetActive(true);

        battleManager.StopCoroutine(battleManager.corSwitchOtherButton);
        battleManager.StopCoroutine(battleManager.corIconScale);
        battleManager.mainPanel1[0].transform.localScale = Vector3.one; //��ԭ

        //����  focus ����
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;

        battleManager.animDic[foucsName].Play("focus");

        battleManager.isPanel = true;
    }
}

public class xuliSkillCommand : Icommand
{
    public void execute()
    {
        //�������ǹ�����, ������������

        var battleManager = BattleManager.instance;
        var karryn = battleManager.canvas.transform.GetChild((int)ChildrenType.Karryn).gameObject;

        battleManager.canvas.transform.GetChild((int)ChildrenType.SkillEffect).gameObject.SetActive(true);
        Animator animator = battleManager.canvas.transform.GetChild((int)ChildrenType.SkillEffect).GetComponent<Animator>();
        animator.Play("xuli");

        var player = PlayerManager.instance;
        player.atk += 50 * player.level;
        //�ı乥����
        GameObject infoPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.InfoPanel).gameObject;
        GameObject targetObj = infoPanel.transform.GetChild(0).GetChild(0).GetChild(14).gameObject;
        TextMeshProUGUI textPro = targetObj.GetComponent<TextMeshProUGUI>();
        textPro.text = player.atk.ToString();
        //�л����յ�״̬ͼ
        battleManager.childrenDic["Karryn"][(int)battleManager.playerCurState].gameObject.SetActive(false);
        battleManager.playerCurState = KarrynState.Weapon2;
        battleManager.childrenDic["Karryn"][(int)battleManager.playerCurState].gameObject.SetActive(true);
        //�رվ������ ���ü�������
        battleManager.canvas.transform.GetChild((int)ChildrenType.VolitionPanel).gameObject.SetActive(false);
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;
        battleManager.animDic[foucsName].StopPlayback();
        battleManager.curPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject;
        battleManager.curPanel.SetActive(true);
        battleManager.useKeyboardOperationMainButton(battleManager.curPanel.transform.GetChild(0).gameObject);  //0���ȡ��
        battleManager.isPanel = false;
        PlayerManager.instance.subtractVolition(Global.instance.xuli);    //50
        battleManager.isInputEnable = true;
    }
}

public class specialsexskillButtonCommand : Icommand//�¼ӵ�����߼�δ��ɣ�����ڰ�ť�����߼�
{
    public void execute()
    {
        // ����������壬��ʾ SpiritualSkillPanel2
        BattleManager battleManager = BattleManager.instance; ;

        // ֹͣһЩЭ��
        battleManager.StopCoroutine(battleManager.corSwitchOtherButton);
        battleManager.StopCoroutine(battleManager.corIconScale);

        // ��ԭ��ť��С
        battleManager.mainPanel2[0].transform.localScale = Vector3.one;

        // �����������
        battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject.SetActive(false);

        // ��ʾSpiritualPanel2���
        battleManager.curPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.SpiritualPanel2).gameObject;
        battleManager.curPanel.SetActive(true);

        // ���� "focus" ����
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;
        Debug.Log(foucsName);
        // Debug.Log(battleManager.animDic);
        battleManager.animDic[foucsName].Play("focus");

        // ���� isPanel Ϊ true
        battleManager.isPanel = true;
    }
}


public class SexSkillPanelCommand : Icommand//�¼ӵ�����߼�δ��ɣ�����ڰ�ť�����߼�
{
    public void execute()
    {
        // ����������壬��ʾ SpiritualSkillPanel2
        BattleManager battleManager = BattleManager.instance; ;

        // ֹͣһЩЭ��
        battleManager.StopCoroutine(battleManager.corSwitchOtherButton);
        battleManager.StopCoroutine(battleManager.corIconScale);

        // ��ԭ��ť��С
        battleManager.mainPanel2[0].transform.localScale = Vector3.one;

        // �����������
        battleManager.canvas.transform.GetChild((int)ChildrenType.BattleSkillPanel2).gameObject.SetActive(false);

        // ��ʾSexSkillPanelCommand���
        battleManager.curPanel = battleManager.canvas.transform.GetChild((int)ChildrenType.SexSkillPanel).gameObject;
        battleManager.curPanel.SetActive(true);

        // ���� "focus" ����
        string foucsName = battleManager.curPanel.transform.GetChild((int)panelInChildType.Focus).name;


        battleManager.animDic[foucsName].Play("focus");

        // ���� isPanel Ϊ true
        battleManager.isPanel = true;
    }
}


public class ButtonCommandManager //��彻�����ģ�����������Ż�
{
    private Dictionary<string, Icommand> commandDictionary;

    private List<Button> curButtonList;     //��ť���б�

    private List<GameObject> panelList;

    public ButtonCommandManager(Button[] buttons)
    {
        // ��ʼ���ֵ䡢�б�����ݽṹ
        commandDictionary = new Dictionary<string, Icommand>();
        curButtonList = new List<Button>();
        panelList = new List<GameObject>();
        curButtonList.AddRange(buttons);

        if (curButtonList.Count > 0)
        {
            BattleManager battleManager = BattleManager.instance;
            Transform canvasTransform = battleManager.canvas.transform;
            battleManager.curPanel = canvasTransform.Find("Battle Skill Panel 1").gameObject;

            // ��ȡ������������ö��ֵ
            var panelTypes = Enum.GetValues(typeof(PanelType)).Cast<PanelType>();
            foreach (var panelType in panelTypes)
            {
                Transform panelTransform = canvasTransform.GetChild((int)panelType);
                GameObject panelObject = panelTransform.gameObject;

                if (panelObject.name == "Pool Manager")
                {
                    continue; // �����ض����
                }

                List<Transform> children = new List<Transform>();
                List<Transform> scrollViewDescChildren = new List<Transform>();

                if (panelObject.name == "Volition Panel" || panelObject.name == "Spiritual  Panel 1" || panelObject.name == "Spiritual Panel 2")
                {
                    // ����ض�������ʹ����Ӷ���
                    Transform scrollView = panelTransform.Find("Scroll View");
                    Transform content = scrollView.Find("Viewport").Find("Content");

                    foreach (Transform child in content)
                    {
                        children.Add(child);
                    }

                    scrollViewDescChildren.AddRange(content.Cast<Transform>());
                }
                else if (panelObject.name == "Karryn")
                {
                    // ����ض�������ʹ����Ӷ���
                    foreach (Transform child in panelTransform)
                    {
                        children.Add(child);
                    }
                }
                else
                {
                    // �������������ʹ����Ӷ���
                    foreach (Transform child in panelTransform)
                    {
                        if (!ShouldSkipChild(panelTransform, child))
                        {
                            children.Add(child);
                        }
                    }
                }
                // �������Ӷ�����ӵ��ֵ�
                var dict = battleManager.childrenDic;
                //var list = new List<Dictionary<string, List<Transform>>>();
                var list = new List<string>
                {
                    panelObject.name
                };
                //battleManager.childrenDic.Add(panelObject.name, children);

                foreach (var name in list)
                {
                    // ����ֵ䲻�����ü�
                    if (!dict.ContainsKey(name))
                    {
                        // �ֵ�Ϊ����ֱ�����
                        if (!dict.Any())
                        {
                            dict.Add(name, children);
                        }
                        // �ֵ䲻Ϊ��Ҳ���
                        else
                        {
                            dict.Add(name, children);
                        }
                    }
                    else if (dict[name].Count == 0)
                    {
                        dict[name] = children;
                    }

                }

                // ������ڹ�����ͼ�����Ӷ�����ӵ��б���
                if (scrollViewDescChildren.Any())
                {
                    battleManager.scrollViewDescList.AddRange(scrollViewDescChildren);
                }

                panelList.Add(panelObject);
            }

            // ��������ֵ�
            PopulateCommandDictionary();
        }
        else
        {
            //Debug.LogError("curButtonList ����˿յİ�ť���飬����");
        }
    }

    // �ж��Ƿ�����ĳ���Ӷ��������
    private bool ShouldSkipChild(Transform panelTransform, Transform child)
    {
        return child.name == panelTransform.GetChild((int)panelInChildType.Cursor).name
            || child.name == panelTransform.GetChild((int)panelInChildType.Focus).name
            || child.name == "description";
    }

    // ��������ֵ�
    private void PopulateCommandDictionary()
    {
        foreach (var button in curButtonList)
        {
            Icommand icommand = getCommandObj(button.gameObject.name);
            if (icommand == null)
            {
                //Debug.Log("û�������İ�ť��,���鰴ť������");
            }
            commandDictionary.Add(button.gameObject.name, icommand);
        }

        foreach (var panel in panelList)
        {
            Icommand icommand = getCommandObj(panel.gameObject.name);
            if (icommand == null)
            {
                //Debug.LogWarning("û����������壡,���鰴ť������");
            }
            commandDictionary.Add(panel.gameObject.name, icommand);
        }
    }


    private Icommand getCommandObj(string buttonName)   //buttonName�ж�����������͵���Ͳ�����~
    {
        switch (buttonName)
        {
            //����ȥ�㼶�����ң�������ƴ������
            case "info":
                return new switchInfoPanel();

            case "volition skill":
                return new switchVolitionPanel();

            case "xuli":
                return new xuliSkillCommand();

            case "switch panel2":
                return new SwitchPannel2Command();

            case "Battle Skill Panel 1":
                return new rollBackBattleSkillPanel1();

            case "Battle Skill Panel 2":
                return new rollBackBattleSkillPanel2();

            case "Atk Skill Panel":
                return new switchAttackSkillPanelCommand();

            case "atk":
                return new attackButtonCommand();

            case "daji":
                return new dajiButtonCommand();

            case "zhanji":
                return new zhanjiButtonCommand();

            case "ciji":
                return new cijiButtonCommand();

            case "special sex skill":
                return new specialsexskillButtonCommand();

            case "Sex SkillPanel":
                return new SexSkillPanelCommand();


            default:
                return null;
        }
    }

    public void ExecuteCommand(string buttonName, string panelName = "")
    {
        // ����ִ�а�ť����
        if (TryExecuteCommand(commandDictionary, buttonName))
        {
            return; // �ɹ�ִ�а�ť����
        }

        // ����ִ���������
        if (!string.IsNullOrEmpty(panelName) && TryExecuteCommand(commandDictionary, panelName))
        {
            return; // �ɹ�ִ���������
        }

        // δ�ҵ�ƥ��İ�ť���������
        // Debug.LogError($"δ�ҵ���ť��������ButtonName: {buttonName}, PanelName: {panelName}");
    }

    // ����ִ������������Ƿ�ɹ�
    private bool TryExecuteCommand(Dictionary<string, Icommand> commandDict, string commandName)
    {
        if (commandDict.ContainsKey(commandName))
        {
            Icommand command = commandDict[commandName];
            command.execute();
            return true;
        }
        return false;
    }

}