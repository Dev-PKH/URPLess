using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APlayer : MonoBehaviour
{
    private Rigidbody rb;

    public float speed = 5;
    private float v, h;
    bool jump;

    public float jumpPower = 5;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        ManagerByAStar.Instance.player = this;
        ManagerByAStar.Instance.OnAI();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateKey();

        transform.position += new Vector3(h, 0, v).normalized * Time.deltaTime * speed;

        if (jump) rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

    void UpdateKey()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        jump = Input.GetButtonDown("Jump");
    }
}
