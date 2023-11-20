using System.Collections.Generic;
using TMPro;
using UnityEngine;

//跳转到子菜单
public class GotoChildMenu : MonoBehaviour
{
    public GameObject btnLoad;

    public void enableLoadButton()
    {
        btnLoad = transform.GetChild(5).gameObject;
        btnLoad.SetActive(true);
        List<PlayerData> dataList = Global.instance.gameObject.GetComponent<LocalData>().getPlayerLocalData();
        for (int i = 0; i < dataList.Count; i++) 
        {
            var obj = transform.GetChild(5).GetChild(1).GetChild(i + 1).gameObject;
            obj.SetActive(true);
            obj.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = dataList[i].dateTime;
            obj.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = dataList[i].sexVal.ToString();
            obj.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = dataList[i].level.ToString();
        }
    }

    private void Update()
    {
        if (btnLoad != null && Input.GetKeyDown(KeyCode.Escape))
        {
            btnLoad.SetActive(false);
            btnLoad = null;
        }
    }
}