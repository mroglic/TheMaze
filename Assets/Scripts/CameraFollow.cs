using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Camera cam;

    public GameObject target;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

     
    void Update()
    {
        cam.transform.LookAt(target.transform.position);
        
    }
}
