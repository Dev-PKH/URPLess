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

    // PathRequestManager에서 현재 길찾기 요청을 시작하는 함수
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

                // 열린목록에서 f cost가 가장 작은 노드를 탐색. f cost가 동일하면 h코스트까지 비교
                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost &&
                        openList[i].hCost < currentNode.hCost) currentNode = openList[i];
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode); // 해당 노드 탐색 완료

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (ANode n in grid.GetNeighbours(currentNode))
                {
                    // 이동 불가 혹은 이미 탐색 노드일 경우
                    if (!n.isWalkable || closedList.Contains(n)) continue;

                    // Cost를 계산하여 주변 노드의 값을 갱신 및 추가
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

        // 노드들의 WorldPosition을 담은 waypoints와 성공여부를 매니저함수에게 알려준다.
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    // 탐색종료 후 최종 노드의 ParentNode를 추적하며 리스트에 담는다.
    // 최종 경로에 있는 노드들의 WorldPosition을 순차적으로 담아 리턴
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
        Array.Reverse(waypoints); // 역순으로 시작점부터 배치
        return waypoints;
    }

    // Path 리스트에 있는 노드들의 WorldPosition을 Vector3[] 배열에 담아 리턴
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

    // cost 계산
    int GetDistanceCost(ANode nodeA, ANode nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

        // 한변의 길이를 10이라 가정하여 정수를 도출하게끔 다음과 같이 설정
        if (distX > distZ) return 14 * distZ + 10 * (distX - distZ);
        else return 14 * distX + 10 * (distZ - distZ);
    }

}
