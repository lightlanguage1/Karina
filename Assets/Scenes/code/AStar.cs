using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovePoint
{
    public string areaName; //�����ڵ�������
    public Vector2Int gridMapPos;

    public MovePoint(string areaName_, Vector2Int gridMapPos_)
    {
        areaName = areaName_;
        gridMapPos = gridMapPos_;
    }
}

public class AStar : MonoBehaviour
{
    private GridNodes grid; //�����ͼ
    private Node startNode; //���
    private Node endNode;   //�յ�
    private int gridWidth;  //��
    private int gridHeight; //��
    private int originX;    //ԭ��x
    private int originY;    //ԭ��y

    private List<Node> openList;    //�����б�
    private HashSet<Node> closeList;   //�ر��б� HashSet�洢����ΨһԪ�صļ���
    private Node headNode;
    private Stack<MovePoint> points;

    [Header("��ͼ��")]
    public MapData_SO mapData_so;

    private bool isFoundPath;

    [Header("AStar����")]
    public Tilemap displayMap;

    public TileBase displayTile;
    public bool isStartTest;
    public bool displayPath;
    public bool clearPath;

    public Vector3Int startPos;

    public Vector3Int endPos;

    private void Update()
    {
        if (isStartTest)
            ShowPathOnGridMap((Vector2Int)startPos, (Vector2Int)endPos);
    }

    /// <summary>
    /// ���������յ�
    /// </summary>
    /// <param name="startPos_"></param>
    /// <param name="endPos_"></param>
    public void setStartAndEnd(Vector3Int startPos_, Vector3Int endPos_)
    {
        startPos = startPos_;
        endPos = endPos_;
    }

    public void ShowPathOnGridMap(Vector2Int startPos_, Vector2Int endPos_)
    {
        if (displayMap != null && displayTile != null)
        {
            //��������յ���Ƭ��ͼ
            //startPos = new Vector3Int(-20, -9, 0);
            //endPos = new Vector3Int(-10, -9, 0);
            if (displayPath)
            {
                displayPath = false;
                points = buildPath("Area1", (Vector2Int)startPos_, (Vector2Int)endPos_);
                if (points != null)
                {
                    foreach (var point in points)
                    {
                        displayMap.SetTile((Vector3Int)point.gridMapPos, displayTile);
                    }
                }
            }
            if (clearPath)
            {
                if (points != null && points.Count > 0)
                {
                    foreach (var point in points)
                    {
                        displayMap.SetTile((Vector3Int)point.gridMapPos, null);
                    }
                    points.Clear();
                    clearPath = false;
                }
            }
        }
    }

    private bool generateGridNodes(string sceneName, Vector2Int startPos, Vector2Int endPos)
    {
        MapData mapData = mapData_so.getMapData(sceneName);
        if (mapData != null)
        {
            //��ʼ��
            gridWidth = mapData.gridWidth;
            gridHeight = mapData.gridHeight;
            originX = mapData.originX;
            originY = mapData.originY;
            openList = new List<Node>();
            closeList = new HashSet<Node>();
            points = new Stack<MovePoint>();
            //��ʼ��ӳ������Ķ�ά����
            grid = new GridNodes(gridWidth, gridHeight, originX, originY);
            //�õ���ǰ��ͼ�����Ӧ�������ͼ�еĽڵ�
            startNode = grid.getGridNode(Mathf.Abs(startPos.x - originX), Mathf.Abs(startPos.y - originY));
            endNode = grid.getGridNode(Mathf.Abs(endPos.x - originX), Mathf.Abs(endPos.y - originY));

            return true;
        }
        Debug.Log("���ɵ�ͼʧ��!");
        return false;
    }

    /// <summary>
    /// ͨ��A*�㷨����·��
    /// </summary>
    /// <param name="sceneName">��ͼ�������Ӧ�ĳ�����</param>
    /// <param name="start">���</param>
    /// <param name="end">�յ�</param>
    /// <returns></returns>
    public Stack<MovePoint> buildPath(string sceneName, Vector2Int start, Vector2Int end)
    {
        isFoundPath = false;
        if (generateGridNodes(sceneName, start, end))
        {
            //�������·��
            if (findShortPath())
            {
                MapData mapData = mapData_so.getMapData(sceneName);
                Stack<MovePoint> stack = new Stack<MovePoint>();
                //�����㷨 ��������
                while (headNode.parentNode != null)
                {
                    MovePoint point = new MovePoint(mapData.childSceneName, new Vector2Int(originX + headNode.gridPos.x, originY - headNode.gridPos.y));
                    stack.Push(point);
                    headNode = headNode.parentNode;
                }
                return stack;
            }
            else
            {
                Debug.LogWarning("AStar�Ҳ����������·��!");
                return null;
            }
        }
        return null;
    }

    private bool findShortPath()
    {
        openList.Add(startNode);
        while (openList.Count > 0)
        {
            openList.Sort();
            Node node = openList[0];    //���Fֵ �� Hֵ ��С�Ľڵ�
            openList.RemoveAt(0);
            closeList.Add(node);
            if (node.gridPos.x == endNode.gridPos.x && node.gridPos.y == endNode.gridPos.y)   //�ҵ����յ�
            {
                isFoundPath = true;
                headNode = node;    //�õ�ͷ�ڵ�
                break;
            }
            evalueateNeighbourNodes(node);
        }
        return isFoundPath;
    }

    //������Χ8����
    private void evalueateNeighbourNodes(Node curNode)
    {
        Vector2Int pos = curNode.gridPos;
        Node validNeighbourNode;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; //exclude currentNode
                else if ((i == 1 || i == -1) && (j == -1 || j == 1)) continue;   //����б����
                validNeighbourNode = getValidNode(pos.x + i, pos.y + j);
                if (validNeighbourNode != null)
                {
                    validNeighbourNode.gCost = curNode.gCost + getNodesDistance(curNode, validNeighbourNode);
                    validNeighbourNode.hCost = getNodesDistance(curNode, endNode);
                    //�������ڵ���
                    validNeighbourNode.parentNode = curNode;
                    openList.Add(validNeighbourNode);
                }
            }
        }
    }

    //�õ���Ч�ĵ�
    private Node getValidNode(int x, int y)
    {
        if (x < 0 || y < 0 || x >= gridWidth || y >= gridHeight)
        {
            return null;
        }

        Node node = grid.getGridNode(x, y);
        if (closeList.Contains(node) || node.isObstacle)
        {
            return null;
        }
        else
        {
            return node;
        }
    }

    //ע������û��б�ŵ�4����
    private int getNodesDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
        int distanceY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);

        if (distanceX == 0) return distanceX * 10;    //10Ϊcost ����ֵ
        else if (distanceY == 0) return distanceY * 10;
        //б�ŵ��ù��ɶ������ֱ�߾���
        int res = Mathf.RoundToInt(Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY));
        return res * 10;
    }
}