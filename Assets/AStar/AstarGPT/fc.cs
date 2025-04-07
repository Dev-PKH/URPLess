using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fc : MonoBehaviour
{
    public Vector3 offset;
    public Transform player;
    public bool check = false;


    private void Update()
    {
        if (!check) return;

        transform.position = player.position + offset;
    }
}
