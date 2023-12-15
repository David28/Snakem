using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnakeMove : MonoBehaviour
{

    void Start() 
    {
        //set all rotations to 0
        transform.rotation = Quaternion.Euler(0, 0, 0);
        direction = Vector3.up;
        targetPos = transform.position;
    }

    //keep track of last key pressed
    private KeyCode lastKey = KeyCode.W;

    public LineRenderer line;
    public bool ate = false;
    public Vector3 direction;
    public GameObject tail;

    public float stunTimer = 0.0f;
    public float stunTime = 3.0f;
    public float speed = 1.0f;
    public Vector3 targetPos;
    public GameObject miniGame;
    public float pointDistance = 0.5f;
    public Vector3 oldHeadPos;
    // Update is called once per frame
    void Update()
    {
        getValidKey();
        updateBody();
        if (stunTimer > 0.0f)
        {
            stunTimer -= Time.deltaTime;
            return;
        }
        
        //check if head reached target position
        if (Vector3.Distance(transform.position, targetPos) >= 0.001f)
        {
            //move towards target
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            //first point is always the head
            line.SetPosition(0, transform.position-transform.rotation*Vector3.up*pointDistance);
            
            return;
        }
        
        updateBody();

        if (lastKey == KeyCode.W ){
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (lastKey == KeyCode.S)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (lastKey == KeyCode.D)
        {
            transform.rotation = Quaternion.Euler(0, 0, 270);
        }
        else if (lastKey == KeyCode.A)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }

        if (ate) {
            Vector3 prev = transform.position;
            if (line.positionCount > 1)
            {
                prev = line.GetPosition(line.positionCount - 2);
            }
            //add a new body part by adding a point to the line renderer
            Vector3 newPos = line.GetPosition(line.positionCount-1) + (line.GetPosition(line.positionCount-1) - prev).normalized*pointDistance;
            line.positionCount++;
            line.SetPosition(line.positionCount-1, newPos);

            ate = false;
            //floor the position
            transform.position += transform.rotation*Vector3.up;
            return;
        }
        //check if head is gonna hit a body part or the wall
        Vector3 nextPos = transform.position + transform.rotation*Vector3.up;
        targetPos = nextPos;
        oldHeadPos = line.GetPosition(0);


    }

  
    private void getValidKey()
    {
        if (Input.GetKeyDown(KeyCode.W) && transform.rotation.eulerAngles.z != 180)
        {
            lastKey = KeyCode.W;
        }
        else if (Input.GetKeyDown(KeyCode.S) && transform.rotation.eulerAngles.z != 0)
        {
            lastKey = KeyCode.S;
        }
        else if (Input.GetKeyDown(KeyCode.D) && transform.rotation.eulerAngles.z != 90)
        {
            lastKey = KeyCode.D;
        }
        else if (Input.GetKeyDown(KeyCode.A) && transform.rotation.eulerAngles.z != 270)
        {
            lastKey = KeyCode.A;
        }else if (Input.GetKeyDown(KeyCode.Space))
        {
            ate = true;
        }
    }

    private void startMiniGame()
    {
    stunTimer = stunTime;
        //spawn the minigame closer to the middle so it doesn't get cut off
        Vector3 pos = transform.position;
        pos  = pos + ( new Vector3(0,0,0) - pos).normalized*2f;
        GameObject obj = Instantiate(miniGame,  pos, Quaternion.identity);
        obj.SetActive(true);
        Destroy(obj, stunTime);
    }

    private void updateBody()
    {
        if (line.positionCount <2)
        {
            return;
        }
        Vector3 oldPos = oldHeadPos;
        for (int i = 1; i < line.positionCount; i++)
        {
            Vector3 newPos = line.GetPosition(i);
            line.SetPosition(i, oldPos);
            oldPos = newPos;
        }
    }
}
