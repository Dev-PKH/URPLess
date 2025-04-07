using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreate : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;

    public GameObject cube;

    public int x, y;
    public int maxCount = 10; // Enemy
    public int curCount = 0;

    private void Awake()
    {
        CreateMap();

        int a = x / 2;
        int b = y / 2;

        Instantiate(player, new Vector3(a, 1, b), Quaternion.identity);
    }

    private void CreateMap()
    {
        for(int i=0; i<x; i++)
        {
            for(int j=0; j<y; j++)
            {
                Instantiate(cube, new Vector3(i, 0, j), Quaternion.identity);
                int r = Random.Range(0, 10);
                if (r <= 2)
                {
                    GameObject wall = Instantiate(cube, new Vector3(i, 1, j), Quaternion.identity);
                    wall.layer = LayerMask.NameToLayer("Obstacle");
                }
                else if (r == 3 && curCount < maxCount)
                {
                    Instantiate(enemy, new Vector3(i, 1, j), Quaternion.identity);
                    curCount++;
                }
            }
        }
    }
}
