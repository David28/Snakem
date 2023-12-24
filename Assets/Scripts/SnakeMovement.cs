using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
     
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
    private Vector3 newDirection = Vector3.up;
    public List<BodyPartMovement> bodyParts = new List<BodyPartMovement>();
    public int ate = 0;
    public Vector3 direction;

    public float stunTimer = 0.0f;
    public float stunTime = 3.0f;

    public GameObject miniGame;

    public float boost = 0.0f;
    public float boostDecreaseRate = 20f;
    // Update is called once per frame
    void Update()
    {
        getValidKey();
        if (stunTimer > 0.0f)
        {
            stunTimer -= Time.deltaTime;
            return;
        }else 
            endStun();

        //movement timer
        if (timer < 0.5-boost)
        {
            timer+=Time.deltaTime;
            return;
        }else
        {
            timer = 0;
        }
        if (boost > 0.0f){
            boost -= Time.deltaTime*boostDecreaseRate;
            
        }
        else
        {
            this.GetComponent<Animator>().SetTrigger("End Stun");
            boost = 0.0f;
        }

        

        
        //check if head is gonna hit a body part or the wall
        Vector3 nextPos = transform.position + newDirection;
        if (nextPos.x >= 5.5 || nextPos.x <= -5.5 || nextPos.y >= 5.5 || nextPos.y <= -5.5)
        {
            //hit the wall
            Debug.Log("Hit the wall");
            startMiniGame();
            fixRotationFromSideImpact(transform.position, nextPos);
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
                    fixRotationFromSideImpact(transform.position, bodyParts[i].transform.position);
                    startMiniGame();
                    return;
                }
            }
        }

        Vector2 oldHeadPos = transform.position;
        Vector2 oldDirection = newDirection;

        //move head and fix rotation
        transform.position += newDirection;
        transform.rotation = getRotationFromDirection(newDirection);

        //make position round to nearest 0.5
        transform.position = new Vector3(Mathf.Round(transform.position.x*2)/2, Mathf.Round(transform.position.y*2)/2, 0);
        tryEat();
        if (ate >= 3) {
            GameObject lastBodyPart = bodyParts[bodyParts.Count-2].gameObject;
            GameObject tail = bodyParts[bodyParts.Count-1].gameObject;
            //spawn it closer to middle
            GameObject g = Instantiate(lastBodyPart, tail.transform.position, tail.transform.rotation);
            bodyParts.Insert(bodyParts.Count-1, g.gameObject.GetComponent<BodyPartMovement>());
            g.gameObject.GetComponent<BodyPartMovement>().Start();
            g.gameObject.GetComponent<BodyPartMovement>().direction = tail.GetComponent<BodyPartMovement>().direction;
            ate -= 3;
            GameObject.Find("Snake Stomach").GetComponent<StomachController>().setStomach(Mathf.Min(ate,3));
            //floor the position
        }
        for (int i = 0; i < bodyParts.Count; i++)
        {
            Vector2 newPos = bodyParts[i].gameObject.transform.position;
            oldDirection = bodyParts[i].moveTo(oldHeadPos, oldDirection);
            oldHeadPos = newPos;
        }
        
        direction = newDirection;
        SetNextDirection(direction);
    }

    private Quaternion getRotationFromDirection(Vector3 newDirection)
    {
        return Quaternion.Euler(0, 0,  (int) (Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg) -90);
    }

    private void fixRotationFromSideImpact(Vector3 headPos, Vector3 hitPos)
    {
        Debug.Log("headPos: " + headPos);
        Debug.Log("hitPos: " + hitPos);
        Vector3 direction = transform.rotation * Vector3.up;
        if (hitPos - headPos == direction)
            return;
        Debug.Log("fixing rotation");
        Vector2 hitDirection = hitPos - headPos;
        Quaternion rotation = Quaternion.Euler(0, 0, 90) * getRotationFromDirection(hitDirection);
        transform.rotation = rotation;
        this.GetComponent<Animator>().SetTrigger("Side Impact");
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

    private void getValidKey()
    {
        if (Input.GetKeyDown(KeyCode.W) && transform.rotation.eulerAngles.z != 180)
        {
            SetNextDirection(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) && transform.rotation.eulerAngles.z != 0)
        {
            SetNextDirection(Vector3.down);
        }
        else if (Input.GetKeyDown(KeyCode.D) && transform.rotation.eulerAngles.z != 90)
        {
            SetNextDirection(Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.A) && transform.rotation.eulerAngles.z != 270)
        {
            SetNextDirection(Vector3.left);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //spend 1 strawberry to get a boost
            if (ate > 0){
                ate--;
                GameObject.Find("Snake Stomach").GetComponent<StomachController>().setStomach(ate);
                boost = 0.3f;
                this.GetComponent<Animator>().SetTrigger("Boost");
            }
        }
    }

    public GameObject stunEffect;
    public void startMiniGame()
    {
        stunEffect.SetActive(true);

        if (stunTimer > 0.0f)
            return;
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
            if (spawnPos == bodyParts[i].gameObject.transform.position)
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
            ate+=3;
            GameObject.FindObjectOfType<GameManager>().SetAppleRespawnTimer();
            collision.gameObject.GetComponent<AppleController>().Kill();
        }
    }

    internal void endStun()
    {
        if (stunTimer == -1.0f)
            return;
        stunTimer = -1.0f;
        this.GetComponent<Animator>().SetTrigger("End Stun");
        this.transform.rotation = getRotationFromDirection(direction);
    }

    internal void DestroyBodyPart()
    {
        Debug.Log("Destroying body part");
        if (bodyParts.Count <= 2)
            return;
        //remove the neck
        GameObject neck = bodyParts[0].gameObject;
        bodyParts.RemoveAt(0);
        this.transform.position = neck.transform.position;
        Vector2 direction = neck.transform.position - bodyParts[0].transform.position;
        SetNextDirection(direction);
        this.transform.rotation = getRotationFromDirection(direction);
        Destroy(neck);

    }

    public GameObject NextDirectionSprite;
    private void SetNextDirection(Vector3 newDirection)
    {
        this.newDirection = newDirection;
        NextDirectionSprite.transform.rotation = getRotationFromDirection(newDirection)*Quaternion.Euler(0,0,90);
        NextDirectionSprite.transform.position = transform.position + newDirection;

    }
}
