using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerByAStar : MonoBehaviour
{
    private static ManagerByAStar instance;
    public static ManagerByAStar Instance => instance;

    public List<AIFSM> ai = new List<AIFSM>();

    public APlayer player;
    public fc c;

    private void Awake()
    {
        if (Instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    public void GetAI(AIFSM a)
    {
        ai.Add(a);
    }

    public void OnAI()
    {
        c.player = player.transform;
        c.check = true;

        foreach (var a in ai)
        {
            a.player = player.transform;
            a.check = true;
        }
    }

}
