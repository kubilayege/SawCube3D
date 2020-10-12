using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player main;
    public Rigidbody rb;

    Vector2 previousPos;
    Vector2 dir;

    // Start is called before the first frame update
    void Awake()
    {
        main = this;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            previousPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            if (previousPos != (Vector2)Input.mousePosition)
            {
                dir = (Vector2)Input.mousePosition - previousPos;
                if (dir.magnitude < 100)
                    return;
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                {
                    //Horizontal
                    if (dir.x > 0)
                        Move(new Command(1, 0));
                    else
                        Move(new Command(-1, 0));

                }
                else
                {
                    //Vertical
                    if (dir.y > 0)
                        Move(new Command(0, 1));
                    else
                        Move(new Command(0, -1));
                }

                previousPos = Input.mousePosition;
            }
        }
    }

    private void Move(Command move)
    {
        var newPos = new Vector3(Mathf.Clamp(transform.position.x + move.x, LevelManager.instance.xMin, LevelManager.instance.xMax),
                                        transform.position.y,
                                        Mathf.Clamp(transform.position.z + move.z, LevelManager.instance.zMin, LevelManager.instance.zMax));
        rb.MovePosition(newPos);
    }

    public class Command
    {
        public int x;
        public int z;
        public Command(int _x, int _z)
        {
            x = _x;
            z = _z;
        }
    }
}
