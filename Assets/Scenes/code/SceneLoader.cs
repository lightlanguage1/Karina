#define DEBUG

using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//ͬ���첽���س������Լ����ض��ĳ����£�Ϊ�־õ����Ǽ���һЩ��������
public class SceneLoader : Singleton<SceneLoader>
{
    public static string mainMenuSceneKeyName = "MainMenuScene";
    public static string mainSceneKeyName = "MainScene";
    public Image transitionImg;
    public float fadeTime = 2.5f;
    private Color color;

    [Header("�������Ȳ���")]
    public Material transitionMaterial;

    public float duration = 2;
    private float weight = 0;
    private Texture2D screenShot;
    private Camera mainCamera;

    //[Header("ս��������")]
    //public GameObject battelManager;

    //�����ʲ��������ҵ��ļ������س�����������ָ�������������Ƿ񸽼ӳ���,�Ƿ������ɺ󼤻�ó���
    public static void loadSceneByAddressable(string sceneKeyName, bool isAddtiveScene = false, bool isActivate = true)
    {
        LoadSceneMode mode = isAddtiveScene ? LoadSceneMode.Additive : LoadSceneMode.Single;    //׷�Ӻ��滻
        Addressables.LoadSceneAsync(sceneKeyName, mode, isActivate);
    }

    //�������� AssetReference sceneReference �ʲ�������
    public static void loadSceneByAddressable(AssetReference sceneReference, bool isAddtiveScene = false, bool isActivate = true)
    {
        LoadSceneMode mode = isAddtiveScene ? LoadSceneMode.Additive : LoadSceneMode.Single;
        Addressables.LoadSceneAsync(sceneReference, mode, isActivate);
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += onSceneChanged;   //Ϊ�¼�ע��ص������������л�ʱ����
        SceneManager.sceneLoaded += checkLoadedScene;   //����������ɺ󴥷����¼�
        SceneManager.sceneUnloaded += onSceneUnloaded;  //��������ж��ʱ�������¼�
    }

    private void onSceneUnloaded(Scene scene)
    {
        Global.instance.battlePrevSceneName = scene.name;   //��ȡս��������ǰһ����������
    }

    private void onSceneChanged(Scene previousScene, Scene loadScene)
    {
        if (loadScene.name == "BattleScene")
        {
            //������ع�������Ϊ�������Ӷ����ڻ�������ʾ
            PoolManager.instance.transform.SetParent(GameObject.Find("Battle Canvas").transform, false);
            PlayerManager.instance.transform.SetParent(GameObject.Find("Battle Canvas").transform, false);
        }
    }

    private void checkLoadedScene(Scene scene, LoadSceneMode mode)  //Scene scene�������й��Ѽ��س�������Ϣ�����糡�������ơ�·���͸���Ϸ����
    {
        //��ȡ���ص�����������
        if (scene.name == "main1L")
        {
            Global.instance.curMainSceneName = scene.name;
            Global.instance.inMainScene = true;
            //Global.instance.loadResource(scene.name);
        }

        //���е�û����˵��ǰһ��������ս����������ʤ����,���Ҫ���õ��˽���
        if (scene.name == "main1L" && Global.instance.isWin)
        {
            Global.instance.isWin = false;
            //���������������,����ˢ��
            NPC[] npcs = FindObjectsByType<NPC>(FindObjectsSortMode.None);
            for (int i = 0; i < npcs.Length; i++)
            {
                if (npcs[i].name == Global.instance.battlePrevNpcName) 
                {
                    Global.instance.diedDic.Add(npcs[i].name, npcs[i].gameObject);
                }
                if(Global.instance.diedDic.ContainsKey(npcs[i].name))
                {
                    npcs[i].gameObject.SetActive(false);
                    Global.instance.diedDic[npcs[i].name].gameObject.SetActive(false);  //ͬ���޸��ֵ��еĵ��˻�Ծ״̬
                }
            }
        }

        if (scene.name == "BattleScene")
        {
            GameObject.FindFirstObjectByType<BattleManager>().loadResource();   //���������Դ
            GameObject.FindFirstObjectByType<PlayerManager>().loadResource();
            Global.instance.inMainScene = false;
        }
        else
        {
            GameObject.FindFirstObjectByType<PlayerManager>().releaseResource();
        }
    }

    private IEnumerator loadScneneByTransition(int sceneVal)
    {
        switch (sceneVal)
        {
            case (int)SceneEnumVal.MainMenuScene:
                {
                    //���ɼ����߼�������
                    yield break;
                }
            case (int)SceneEnumVal.Main1L:
                {
                    yield return StartCoroutine(FadeInOutLoadScene("main1L")); //ִ��loadScneneByTransition��Э��ִ����佫������ֱ������ִ�����
                    yield break;
                }
            case (int)SceneEnumVal.BattleScene:
                {
                    //��ֹ����ƶ���ɫ
                    GameObject obj = GameObject.Find("Karryn");
                    if (obj != null)
                    {
                        obj.GetComponent<Animator>().enabled = false;
                        obj.GetComponent<karryn>().enabled = false;
                        obj.GetComponent<Rigidbody2D>().Sleep();
                    }

                    //����Э�̼��س���
                    yield return StartCoroutine(LoadBattleScene("BattleScene"));
                    yield break;
                }
            case (int)SceneEnumVal.SexTransitionScene:
                {
                    yield return StartCoroutine(FadeInOutLoadScene("SexTransitionScene"));
                    yield break;
                }
        }
    }

    private IEnumerator FadeInOutLoadScene(string sceneName)
    {
        transitionImg.gameObject.SetActive(true);
        transitionImg.raycastTarget = true;

        //����
        while (transitionImg.color.a < 1)
        {
            color.a = Mathf.Clamp01(transitionImg.color.a + Time.unscaledDeltaTime / fadeTime);
            transitionImg.color = color;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);

        //����
        while (transitionImg.color.a > 0)
        {
            color.a = Mathf.Clamp01(transitionImg.color.a - Time.unscaledDeltaTime / fadeTime);
            transitionImg.color = color;
            yield return null;
        }

        transitionImg.gameObject.SetActive(false);
        transitionImg.raycastTarget = false;
    }

    private IEnumerator LoadBattleScene(string sceneName)
    {
        //��ȡ��Ļ��ͼ
        yield return StartCoroutine(getGrab());
        transitionMaterial.SetTexture("_MainTex", screenShot);  //�������Ļ��ͼ��Ϊ���ʵ�������
        if (!transitionMaterial.HasTexture("_MainTex")) Debug.Log("not _MainTex!");

        //����һ��Sprite����������ʾ����Ч��
        GameObject spriteObj = new GameObject("TransitionSprite");

        SpriteRenderer spriteRenderer = spriteObj.AddComponent<SpriteRenderer>();
        Sprite sprite = Sprite.Create(screenShot, new Rect(0, 0, screenShot.width, screenShot.height), new Vector3(0.5f, 0.5f, 0f));
#if DEBUG

        Debug.Log("Sprite size: " + spriteRenderer.bounds.size);
        Debug.Log("sprite size:" + sprite.textureRect.size.ToString());
#endif
        spriteRenderer.sprite = sprite;
        spriteRenderer.material = transitionMaterial;
        spriteRenderer.sortingOrder = 9999; //ȷ����Ⱦ����ǰ��

        spriteObj.transform.position = mainCamera.transform.position;

        while (weight < 1)
        {
            weight += Time.deltaTime / duration;
            weight = Mathf.Clamp01(weight); //������0-1֮��

            transitionMaterial.SetFloat("_TransitionProgress", weight);
            float val = transitionMaterial.GetFloat("_TransitionProgress");
            //Debug.Log(val);
            yield return null;
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);  //����ս������
        weight = 0; //ȷ���´�����ʱ��_TransitionProgress��Ȼ�Ǵ�0��ʼ
    }

    private IEnumerator getGrab()
    {
        yield return new WaitForEndOfFrame(); //�ȴ��������Ⱦ�굱ǰ֡
        mainCamera = FindFirstObjectByType<Camera>();

        int width = UnityEngine.Device.Screen.width;
        int height = UnityEngine.Device.Screen.height;

        screenShot = new Texture2D(width, height, TextureFormat.RGBA32, false); //�����������
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0); //0,0 ������Ļ����ԭ�㣬����Ļ�����½�
        screenShot.Apply(); //Ӧ�ö�������������

#if DEBUG
        Debug.Log("width:" + width.ToString() + "  " + "height:" + height.ToString());
        Debug.Log("screenShot width:" + screenShot.width.ToString() + "screenShot height:" + screenShot.height.ToString());
#endif
    }

    public void loadGameScene(int sceneVal)
    {
        StartCoroutine(loadScneneByTransition(sceneVal));
    }
}