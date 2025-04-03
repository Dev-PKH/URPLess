using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX, gridZ;

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridZ)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridZ = gridZ;
    }
}

public class GridManager : MonoBehaviour
{
    public int gridSizeX, gridSizeZ;
    public float nodeSize;
    public LayerMask unwalkableMask;
    private Node[,] grid;

    void Start() => CreateGrid();

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeZ];
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 worldPoint = new Vector3(x * nodeSize, 0, z * nodeSize);
                bool walkable = !Physics.CheckSphere(worldPoint, nodeSize / 2, unwalkableMask);
                grid[x, z] = new Node(walkable, worldPoint, x, z);
            }
        }
    }

    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / nodeSize);
        int z = Mathf.RoundToInt(worldPosition.z / nodeSize);
        return grid[x, z];
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                if (dx == 0 && dz == 0) continue;
                int checkX = node.gridX + dx;
                int checkZ = node.gridZ + dz;
                if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ)
                    neighbors.Add(grid[checkX, checkZ]);
            }
        }
        return neighbors;
    }

    public void SetWalkable(Vector3 position, bool walkable)
    {
        Node node = GetNodeFromWorldPoint(position);
        if (node != null) node.walkable = walkable;
    }
}
