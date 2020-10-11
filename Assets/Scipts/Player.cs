using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player main;

    private Vector3 spawn;

    // Start is called before the first frame update
    void Awake()
    {
        main = this;
        spawn = transform.position;
    }

    public void Fail()
    {
        transform.position = spawn;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            transform.position += Vector3.forward;
        if(Input.GetKeyDown(KeyCode.S))
            transform.position -= Vector3.forward;
        if(Input.GetKeyDown(KeyCode.A))
            transform.position -= Vector3.right;
        if(Input.GetKeyDown(KeyCode.D))
            transform.position += Vector3.right;


    }
}
