using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
    private 
    void Start() 
    {
        //set all rotations to 0
        transform.rotation = Quaternion.Euler(0, 0, 0);
        for (int i = 0; i < bodyParts.Count; i++)
        {
            bodyParts[i].transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        direction = Vector3.up;
    }
    private float timer = 0;

    //keep track of last key pressed
    private KeyCode lastKey = KeyCode.W;
    public List<GameObject> bodyParts = new List<GameObject>();

    public Sprite[] turnSprite;
    public Sprite bodySprite;
    public int ate = 0;
    public Vector3 direction;

    public float stunTimer = 0.0f;
    public float stunTime = 3.0f;

    public GameObject miniGame;
    // Update is called once per frame
    void Update()
    {
        getValidKey();
        if (stunTimer > 0.0f)
        {
            stunTimer -= Time.deltaTime;
            return;
        }
        if (timer < 0.5)
        {
            timer+=Time.deltaTime;
            return;
        }else
        {
            timer = 0;
        }
        


        Vector3 newDirection = Vector3.zero;
        if (lastKey == KeyCode.W ){
            transform.rotation = Quaternion.Euler(0, 0, 0);
            newDirection = Vector3.up;
            //transform.position.x += 0.5f;
            //transform.position.y += 0.547f;
        }
        else if (lastKey == KeyCode.S)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
            newDirection = Vector3.down;
            //transform.position.x += 0.5f;
            //transform.position.y += .453f;
        }
        else if (lastKey == KeyCode.D)
        {
            transform.rotation = Quaternion.Euler(0, 0, 270);
            newDirection = Vector3.right;
            //transform.position.y += 0.5f;
            //transform.position.x += .453f;
        }
        else if (lastKey == KeyCode.A)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            newDirection = Vector3.left;
            //transform.position.y += 0.5f;
            //transform.position.x += 0.547f;
        }

        if (ate >= 3) {
            //spawn it closer to middle
            GameObject g = (GameObject)Instantiate(bodyParts[0], transform.position, transform.rotation);
            g.GetComponent<SpriteRenderer>().sprite = bodySprite;
            bodyParts.Insert(0, g);
            
            ate -= 3;
            GameObject.Find("Snake Stomach").GetComponent<StomachController>().setStomach(Mathf.Min(ate,3));
            //floor the position
            transform.position += transform.rotation*Vector3.up;
            return;
        }
        //check if head is gonna hit a body part or the wall
        Vector3 nextPos = transform.position + transform.rotation*Vector3.up;
        if (nextPos.x >= 5.5 || nextPos.x <= -5.5 || nextPos.y >= 5.5 || nextPos.y <= -5.5)
        {
            //hit the wall
            Debug.Log("Hit the wall");
            startMiniGame();
            return;
        }
        else
        {
            for (int i = 0; i < bodyParts.Count; i++)
            {
                if (nextPos == bodyParts[i].transform.position)
                {
                    //hit a body part
                    Debug.Log("Hit a body part");
                    startMiniGame();
                    return;
                }
            }
        }


        transform.position += transform.rotation*Vector3.up;
        tryEat();
        int tailIndex = bodyParts.Count-1;
        float[] prevRotations = new float[bodyParts.Count+1];
        prevRotations[0] = transform.rotation.eulerAngles.z;
        // move the body adjusting the sprites and bodyParts rotation
        for (int i = 0; i < bodyParts.Count; i++)
        {
            Transform rot = bodyParts[i].transform;
            float newRot = rot.rotation.eulerAngles.z;
            bodyParts[i].transform.position += rot.rotation*Vector3.up;

            //save the rotation for the next body part, and update this body part's rotation
            bodyParts[i].transform.rotation = Quaternion.Euler(0, 0, prevRotations[i]);
            prevRotations[i+1] = newRot;

        } 
        // one more loop to fix the sprites
        for (int i = 0; i < bodyParts.Count-1; i++)
        {
            Transform front = (i == 0) ? transform : bodyParts[i-1].transform;
            Transform back = bodyParts[i+1].transform;
            int index = getTurnSpriteIndex(Quaternion.Euler(0,0,prevRotations[i+1])*Vector3.up, Quaternion.Euler(0,0,prevRotations[i])*Vector3.up);

            if (index != -1)
            {
                bodyParts[i].GetComponent<SpriteRenderer>().sprite = turnSprite[index];
            }
            else
            {
                bodyParts[i].GetComponent<SpriteRenderer>().sprite = bodySprite;
            }
        }

        direction = newDirection;
    }

    private void tryEat()
    {
        GameObject[] strawberries = GameObject.FindGameObjectsWithTag("Strawberry");
        for (int i = 0; i < strawberries.Length; i++)
        {
            if (strawberries[i].transform.position == transform.position)
            {
                Destroy(strawberries[i]);
                GameObject.Find("GameManager").GetComponent<GameManager>().SpawnStrawberry();
                ate++;
                GameObject.Find("Snake Stomach").GetComponent<StomachController>().setStomach(ate);
            }
        }
    }

    private int getTurnSpriteIndex(Vector3 oldDirection, Vector3 newDirection){
            // sprite 0 is left up
            // sprite 1 is right up
            // sprite 2 is up left
            // sprite 3 is up right
        if (oldDirection == Vector3.up && newDirection == Vector3.left)
        {
            return 1;
        }
        else if (oldDirection == Vector3.up && newDirection == Vector3.right)
        {
            return 0;
        }
        else if (oldDirection == Vector3.down && newDirection == Vector3.left)
        {
            return 0;
        }
        else if (oldDirection == Vector3.down && newDirection == Vector3.right)
        {
            return 1;
        }
        else if (oldDirection == Vector3.left && newDirection == Vector3.up)
        {
            return 0;
        }
        else if (oldDirection == Vector3.left && newDirection == Vector3.down)
        {
            return 1;
        }
        else if (oldDirection == Vector3.right && newDirection == Vector3.up)
        {
            return 1;
        }
        else if (oldDirection == Vector3.right && newDirection == Vector3.down)
        {
            return 0;
        }
        else
        {
            return -1;
        }
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
        }
    }

    public void startMiniGame()
    {
    stunTimer = stunTime;
        //spawn the minigame closer to the middle so it doesn't get cut off
        Vector3 pos = transform.position;
        pos  = pos + ( new Vector3(0,0,0) - pos).normalized*2f;
        GameObject obj = Instantiate(miniGame,  pos, Quaternion.identity);
        obj.SetActive(true);
        Destroy(obj, stunTime);
    }

    internal bool isFreePosition(Vector3 spawnPos)
    {
        if (spawnPos == transform.position)
        {
            return false;
        }
        for (int i = 0; i < bodyParts.Count; i++)
        {
            if (spawnPos == bodyParts[i].transform.position)
            {
                return false;
            }
        }
        return true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Apple")
        {
            Destroy(collision.gameObject);
            ate+=3;
        }
    }
}
