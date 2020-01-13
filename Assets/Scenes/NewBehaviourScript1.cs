using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript1 : MonoBehaviour
{
    public float speed = 10;

    void Start()
    {
        
    }

 
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //transform.position += Vector3.right * x * speed * Time.deltaTime;
        transform.position += Vector3.right * speed * Time.deltaTime * Mathf.Sin(Time.time);
    }
}
