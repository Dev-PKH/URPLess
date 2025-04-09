 using UnityEngine;

public class ANode
{
    public bool isWalkable;
    public Vector3 worldPos;
    public int gridX;
    public int gridZ;

    public int gCost;
    public int hCost;
    public ANode parentNode;


    public ANode(bool nWalkable, Vector3 nWorld, int X, int Z)
    {
        isWalkable = nWalkable;
        worldPos = nWorld;
        gridX = X;
        gridZ = Z;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }
}