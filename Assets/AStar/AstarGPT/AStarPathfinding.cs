using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    public static List<Vector3> FindPath(Vector3 start, Vector3 target, GridManager grid)
    {
        Node startNode = grid.GetNodeFromWorldPoint(start);
        Node targetNode = grid.GetNodeFromWorldPoint(target);

        if (!startNode.walkable || !targetNode.walkable)
            return new List<Vector3>();

        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> gCost = new Dictionary<Node, float> { [startNode] = 0 };
        Dictionary<Node, float> fCost = new Dictionary<Node, float> { [startNode] = Heuristic(startNode, targetNode) };

        while (openSet.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(openSet, fCost);
            if (currentNode == targetNode)
                return RetracePath(cameFrom, targetNode);

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (Node neighbor in grid.GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                float tentativeGCost = gCost[currentNode] + Vector3.Distance(currentNode.worldPosition, neighbor.worldPosition);
                if (!gCost.ContainsKey(neighbor) || tentativeGCost < gCost[neighbor])
                {
                    cameFrom[neighbor] = currentNode;
                    gCost[neighbor] = tentativeGCost;
                    fCost[neighbor] = tentativeGCost + Heuristic(neighbor, targetNode);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
        return new List<Vector3>();
    }

    private static float Heuristic(Node a, Node b) => Vector3.Distance(a.worldPosition, b.worldPosition);

    private static Node GetLowestFCostNode(List<Node> openSet, Dictionary<Node, float> fCost)
    {
        Node bestNode = openSet[0];
        foreach (var node in openSet)
        {
            if (fCost[node] < fCost[bestNode])
                bestNode = node;
        }
        return bestNode;
    }

    private static List<Vector3> RetracePath(Dictionary<Node, Node> cameFrom, Node targetNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = targetNode;
        while (cameFrom.ContainsKey(currentNode))
        {
            path.Add(currentNode.worldPosition);
            currentNode = cameFrom[currentNode];
        }
        path.Reverse();
        return path;
    }
}

