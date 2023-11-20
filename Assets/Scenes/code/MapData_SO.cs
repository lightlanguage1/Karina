using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData_SO", menuName = "Map/MapData")]
public class MapData_SO : ScriptableObject
{
    public List<MapData> mapDataList;

    public MapData getMapData(string name)
    {
        return mapDataList.Find(m => m.childSceneName == name);
    }
}

[System.Serializable]
public class MapData
{
    [Header("��ͼ��Ϣ")]
    public string childSceneName;

    public int gridWidth;
    public int gridHeight;

    [Header("���Ͻ�ԭ��")]
    public int originX;

    public int originY;
}