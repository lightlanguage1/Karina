using System;
using UnityEngine;

//�����е�ÿһ������
public class Node : IComparable<Node>
{
    public Vector2Int gridPos;  //��������
    public int gCost = 0;
    public int hCost = 0;
    public int fCost => gCost + hCost;
    public bool isObstacle = false;
    public Node parentNode;

    public Node(Vector2Int pos)
    {
        gridPos = pos;
        parentNode = null;
    }

    public int CompareTo(Node other)
    {
        int res = fCost.CompareTo(other.fCost);
        if (res == 0)
        {
            res = hCost.CompareTo(other.hCost);
        }
        return res;
    }
}