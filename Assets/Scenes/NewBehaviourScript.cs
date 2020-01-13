using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float speed = 1;
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    
    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        //Vector3 force = new Vector3(x, 0f, z) * speed;
        Vector3 force = Vector3.right * Mathf.Sin(Time.time) * speed;

        rb.AddForce(force);
    }
}
