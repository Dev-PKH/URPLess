using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    PathRequestManager requestManager;
    AGrid grid;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<AGrid>();
    }

    // PathRequestManager���� ���� ��ã�� ��û�� �����ϴ� �Լ�
    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    private IEnumerator FindPath(Vector3 stPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        ANode startNode = grid.GetNodeFromWorldPoint(stPos);
        ANode targetNode = grid.GetNodeFromWorldPoint(targetPos);

        if (startNode.isWalkable && targetNode.isWalkable)
        {

            List<ANode> openList = new List<ANode>();
            HashSet<ANode> closedList = new HashSet<ANode>();
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                ANode currentNode = openList[0];

                // ������Ͽ��� f cost�� ���� ���� ��带 Ž��. f cost�� �����ϸ� h�ڽ�Ʈ���� ��
                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost &&
                        openList[i].hCost < currentNode.hCost) currentNode = openList[i];
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode); // �ش� ��� Ž�� �Ϸ�

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (ANode n in grid.GetNeighbours(currentNode))
                {
                    // �̵� �Ұ� Ȥ�� �̹� Ž�� ����� ���
                    if (!n.isWalkable || closedList.Contains(n)) continue;

                    // Cost�� ����Ͽ� �ֺ� ����� ���� ���� �� �߰�
                    int newCurrentToNeighbourCost = currentNode.gCost + GetDistanceCost(currentNode, n);
                    if (newCurrentToNeighbourCost < n.gCost || !openList.Contains(n))
                    {
                        n.gCost = newCurrentToNeighbourCost;
                        n.hCost = GetDistanceCost(n, targetNode);
                        n.parentNode = currentNode;

                        if (!openList.Contains(n)) openList.Add(n);
                    }
                }
            }
        }

        yield return null;
        if(pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }

        // ������ WorldPosition�� ���� waypoints�� �������θ� �Ŵ����Լ����� �˷��ش�.
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    // Ž������ �� ���� ����� ParentNode�� �����ϸ� ����Ʈ�� ��´�.
    // ���� ��ο� �ִ� ������ WorldPosition�� ���������� ��� ����
    private Vector3[] RetracePath(ANode startNode, ANode endNode)
    {
        List<ANode> path = new List<ANode>();
        ANode currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }

        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints); // �������� ���������� ��ġ
        return waypoints;
    }

    // Path ����Ʈ�� �ִ� ������ WorldPosition�� Vector3[] �迭�� ��� ����
    Vector3[] SimplifyPath(List<ANode> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for(int i=1; i< path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridZ - path[i].gridZ);
            if(directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPos);
            }
            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

    // cost ���
    int GetDistanceCost(ANode nodeA, ANode nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

        // �Ѻ��� ���̸� 10�̶� �����Ͽ� ������ �����ϰԲ� ������ ���� ����
        if (distX > distZ) return 14 * distZ + 10 * (distX - distZ);
        else return 14 * distX + 10 * (distZ - distZ);
    }

}
