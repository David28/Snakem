using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnakeMovement : Player
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
    public float boostValue = 0.3f;
    public float boostDecreaseRate = 20f;

    public GameObject plusAnim;
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

        if (GetSnakeAction())
        {
            //spend 1 strawberry to get a boost
            if (ate > 0){
                ate--;
                GameObject.Find("Player "+this.player +" Stomach").GetComponent<StomachController>().setStomach(ate);
                if (boost == 0)
                    this.GetComponent<Animator>().SetBool("Boosting", true);
                boost = boostValue;
                
            }
        }

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
            boost -= boostDecreaseRate;
        }
        if (boost < 0.0f){
            this.GetComponent<Animator>().SetBool("Boosting", false);
            boost = 0.0f;
        }

        
        //check if head is gonna hit a body part or the wall
        Vector3 nextPos = transform.position + newDirection;
        if (CheckColisions(nextPos)) return;

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
            GameObject g = Instantiate(lastBodyPart, tail.transform.position, tail.transform.rotation, this.transform.parent);
            Instantiate(plusAnim,transform.position+new Vector3(0.5f,0f,0f), transform.rotation).SetActive(true);
            bodyParts.Insert(bodyParts.Count-1, g.gameObject.GetComponent<BodyPartMovement>());
            g.gameObject.GetComponent<BodyPartMovement>().Start();
            g.gameObject.GetComponent<BodyPartMovement>().direction = tail.GetComponent<BodyPartMovement>().direction;
            ate -= 3;
            GameObject.Find("Player "+this.player +" Stomach").GetComponent<StomachController>().setStomach(Mathf.Min(ate,3));

            GameObject.FindObjectOfType<GameManager>().AddPoint(this.player);
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

    private bool CheckColisions(Vector3 nextPos)
    {
        
        if (nextPos.x >= 5.5 || nextPos.x <= -5.5 || nextPos.y >= 5.5 || nextPos.y <= -5.5)
        {
            //hit the wall
            Debug.Log("Hit the wall");
            startMiniGame();
            fixRotationFromSideImpact(transform.position, nextPos);
            return true;
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
                    return true;
                }
            }

            //check for apple created obstacles
            GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
            for (int i = 0; i < obstacles.Length; i++)
            {
                //need to put obtacle in refence to snake parent
                if (nextPos == obstacles[i].transform.position)
                {
                    //hit an obstacle
                    if (boost > 0.0f){
                        DestroyObstacle(obstacles[i]);
                        return false;
                    }
                    Debug.Log("Hit an obstacle");
                    fixRotationFromSideImpact(transform.position, obstacles[i].transform.position);
                    startMiniGame();
                    return true;
                }
            }
        }
        return false;
    }
    public GameObject rockSmashEffect;
    private void DestroyObstacle(GameObject gameObject)
    {
        // Calculate the angle of collision
        Vector2 collisionDirection = (Vector2)gameObject.transform.position - (Vector2)transform.position;
        float collisionAngle = Mathf.Atan2(collisionDirection.y, collisionDirection.x);
        // Instantiate and configure the rockSmashEffect particle system at the asteroid's position
        GameObject effect = Instantiate(rockSmashEffect, transform.position, Quaternion.identity);
        var particleSystem = effect.GetComponent<ParticleSystem>();
        var mainModule = particleSystem.main;
        // Set the cone emission angle based on the collision angle
        effect.transform.rotation = Quaternion.Euler(collisionAngle * Mathf.Rad2Deg,-90,0);

        // Enable the game object
        effect.SetActive(true);

        // Play the particle system
        particleSystem.Play();

        // Destroy the particle system after its duration
        Destroy(effect, mainModule.duration);

        // Destroy this obstacle
        Destroy(gameObject);
    }

    private Quaternion getRotationFromDirection(Vector3 newDirection)
    {
        return Quaternion.Euler(0, 0,  (int) (Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg) -90);
    }

    private void fixRotationFromSideImpact(Vector3 headPos, Vector3 hitPos)
    {
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();

        Debug.Log("headPos: " + headPos);
        Debug.Log("hitPos: " + hitPos);
        Vector3 direction = transform.rotation * Vector3.up;
        
        if (hitPos - headPos == direction){
            this.GetComponent<Animator>().SetBool("Front Stunned", true);
            return;

        }
        Debug.Log("fixing rotation");
        Vector2 hitDirection = hitPos - headPos;
        Quaternion rotation = transform.rotation;
        
        //must flip sprite depending on rotation
        if (rotation.eulerAngles.z == 0)
            sr.flipX = hitPos.x < headPos.x;
        else if (rotation.eulerAngles.z == 90)
            sr.flipX = hitPos.y < headPos.y;
        else if (rotation.eulerAngles.z == 180)
            sr.flipX = hitPos.x > headPos.x;
        else if (rotation.eulerAngles.z == 270)
            sr.flipX = hitPos.y > headPos.y;

        this.GetComponent<Animator>().SetBool("Side Stunned", true);
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
                GameObject.Find("Player "+this.player +" Stomach").GetComponent<StomachController>().setStomach(ate);
            }
        }
    }

    private void getValidKey()
    {
        Vector2 input = GetSnakeInput();
        if (input == Vector2.up && transform.rotation.eulerAngles.z != 180)
        {
            SetNextDirection(Vector3.up);
        }
        else if (input == Vector2.down && transform.rotation.eulerAngles.z != 0)
        {
            SetNextDirection(Vector3.down);
        }
        else if (input == Vector2.right && transform.rotation.eulerAngles.z != 90)
        {
            SetNextDirection(Vector3.right);
        }
        else if (input == Vector2.left && transform.rotation.eulerAngles.z != 270)
        {
            SetNextDirection(Vector3.left);
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
        if (collision.gameObject.name == "Apples" && stunTimer == -1.0f)
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
        this.GetComponent<Animator>().SetBool("Side Stunned", false);
        this.GetComponent<Animator>().SetBool("Front Stunned", false);
        this.GetComponent<Animator>().SetBool("Boosting", false);
        boost = 0.0f;
        this.GetComponent<SpriteRenderer>().flipY = false;
        this.GetComponent<SpriteRenderer>().flipX = false;
        this.transform.rotation = getRotationFromDirection(direction);

    }

    public GameObject minusAnim;
    public void DestroyBodyPart(bool stopStun = false)
    {
        Debug.Log("Destroying body part");
        if (bodyParts.Count <= 2)
            return;
        //remove the neck
        GameObject neck = bodyParts[0].gameObject;
        bodyParts.RemoveAt(0);
        Instantiate(minusAnim, neck.transform.position+new Vector3(0.5f,0f,0f), neck.transform.rotation).SetActive(true);
        this.transform.position = neck.transform.position;
        Vector2 direction = neck.transform.position - bodyParts[0].transform.position;
        Destroy(neck);
        if (stopStun)
            endStun();
        this.transform.rotation = getRotationFromDirection(direction);
        SetNextDirection(this.newDirection);
        GameObject.FindObjectOfType<GameManager>().RemovePoint(this.player);
    }

    public GameObject NextDirectionSprite;
    private void SetNextDirection(Vector3 newDirection)
    {
        this.newDirection = newDirection;
        NextDirectionSprite.transform.rotation = getRotationFromDirection(newDirection)*Quaternion.Euler(0,0,90);
        NextDirectionSprite.transform.position = transform.position + newDirection;

    }
}
