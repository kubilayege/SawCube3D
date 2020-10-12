using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player main;
    public Rigidbody rb;
    private Vector3 spawn;

    Vector2 previousPos;

    Coroutine commandCor;
    Queue<Command> commands;

    // Start is called before the first frame update
    void Awake()
    {
        commands = new Queue<Command>();
        main = this;
        rb = GetComponent<Rigidbody>();
        //rb.constraints = RigidbodyConstraints.FreezeRotation;
        //rb.constraints = RigidbodyConstraints.FreezePositionY;
        rb.isKinematic = true;
        spawn = transform.position;
    }

    public void Fail()
    {
        transform.position = spawn;
    }


    Vector2 dir;

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
                if (dir.magnitude < 25)
                    return;
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                {
                    //Horizontal
                    if (dir.x > 0)
                        commands.Enqueue(new Command(1, 0));
                    else
                        commands.Enqueue(new Command(-1, 0));

                }
                else
                {
                    //Vertical
                    if (dir.y > 0)
                        commands.Enqueue(new Command(0, 1));
                    else
                        commands.Enqueue(new Command(0, -1));
                }
                if(commandCor == null)
                    commandCor = StartCoroutine(ProcessCommands());
                previousPos = Input.mousePosition;
            }
            
            //Debug.Log(commands.Count);
        }




        //if (Input.GetKeyDown(KeyCode.W))
        //    Move(0,1);
        //if (Input.GetKeyDown(KeyCode.S))
        //    Move(0, -1);
        //if (Input.GetKeyDown(KeyCode.A))
        //    Move(-1, 0);
        //if (Input.GetKeyDown(KeyCode.D))
        //    Move(1, 0);
    }

    private IEnumerator Move(Command nextMove,float duration)
    {
        var t = 0f;
        var newPos = new Vector3(Mathf.Clamp(transform.position.x + nextMove.x, LevelManager.instance.xMin, LevelManager.instance.xMax),
                                        transform.position.y,
                                        Mathf.Clamp(transform.position.z + nextMove.z, LevelManager.instance.zMin, LevelManager.instance.zMax));
        while (t < duration)
        {
            t += Time.deltaTime;
            Debug.Log(t);
            t = Mathf.Clamp(t, 0, duration);
            //Debug.Log(t);
            rb.MovePosition(Vector3.Lerp(transform.position, newPos, t/duration));
            yield return new WaitForSeconds(Time.deltaTime);
        }

        FinishMove();
    }

    private void FinishMove()
    {
        if(currentMove != null)
            StopCoroutine(currentMove);
        currentMove = null;
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

        Coroutine currentMove;

    public IEnumerator ProcessCommands()
    {
        var duration = 0.5f / commands.Count;
        var currentCommand = commands.Dequeue();
        currentMove = StartCoroutine(Move(currentCommand, duration));
        yield return new WaitForSeconds(duration);
        while (commands.Count>0)
        {
            Debug.Log(commands.Count);
            if (currentMove != null)
                yield return new WaitForEndOfFrame();
            duration = 1 / commands.Count;
            currentCommand = commands.Dequeue();
            currentMove = StartCoroutine(Move(currentCommand,duration));
            yield return new WaitForSeconds(duration);
        }
    }

}
