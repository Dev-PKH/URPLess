 using UnityEngine;

public class ANode
{
    public bool isWalkable;
    public Vector3 worldPos;

    public ANode(bool nWalkable, Vector3 nWorld)
    {
        isWalkable = nWalkable;
        worldPos = nWorld;
    }
}