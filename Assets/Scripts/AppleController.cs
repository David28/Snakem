using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleController : Player
{
   Rigidbody2D body;

   float horizontal;
   float vertical;
   float moveLimiter = 0.7f;
   int ate = 0;
   public float runSpeed = 20.0f;
   private SpriteRenderer spriteRenderer;
   void Start()
   {
      spriteRenderer = GetComponent<SpriteRenderer>();
      body = GetComponent<Rigidbody2D>();
   }

   public Vector2 shootDirection = new Vector2(1, 0);
   public bool amDead = false;

   private Vector2 input;

   private int buildBlocks = 0;

   void Update()
   {

      // Gives a value between -1 and 1
      input = GetAppleInput();
      horizontal = input.x;
      vertical = input.y;
      if (GetAppleAction())
      {
         Debug.Log("Fire1");
         spit();
      }
      if (amDead)
      {
         return;
      }
      if (GetApplePower())
      {
         Debug.Log("Fire2");
         Build();
      }
      if (horizontal != 0)
      {
         if (horizontal > 0)
         {
            spriteRenderer.flipX = false;
         }
         else
         {
            spriteRenderer.flipX = true;
         }
         if (vertical == 0)
         {
            spriteRenderer.flipY = false;
         }

      }

      if (horizontal == 0 && vertical == 0)
      {
         if (currentState != 0)
         {
            this.GetComponent<Animator>().SetTrigger("Idle");
            currentState = 0;
         }

      }
      else
      {
         shootDirection = new Vector2(horizontal, vertical);
         if (horizontal != 0 && vertical == 0)
         {
            if (currentState != 2)
            {
               this.GetComponent<Animator>().SetTrigger("Horizontal");
               currentState = 2;
            }

         }
         else if (currentState != 1 && vertical > 0)
         {
               this.GetComponent<Animator>().SetTrigger("Vertical Back");
            currentState = 1;
         }else if (currentState != 3 && vertical < 0)
         {
            Debug.Log("Vertical Front");
               this.GetComponent<Animator>().SetTrigger("Vertical Front");
            currentState = 3;
         }
      }



   }



   public GameObject spitPrefab;
   private int currentState = 0;
   public float blastRadius = 0.4f;
   void spit()
   {
      if (ate == 0)
      {
         return;
      }
      if (amDead)
      {
         //new effect blast into the ground if it hits the snake stun it
         ate--;
         GameObject.Find("Player " + this.player + " Stomach").GetComponent<StomachController>().setStomach(ate);
         //check if was above snake
         GameObject snake = GameObject.Find("Snake");
         if (Math.Abs(snake.transform.position.x - this.transform.position.x) < blastRadius && Math.Abs(snake.transform.position.y - this.transform.position.y) < blastRadius)
         {
            snake.GetComponent<SnakeMovement>().startMiniGame();
         }
         this.GetComponent<Animator>().SetTrigger("Blast");

         return;
      }

      GameObject spit = Instantiate(spitPrefab, transform.position + ((Vector3)shootDirection) * 0.3f, Quaternion.identity);
      spit.GetComponent<SpitController>().direction = shootDirection;
      spit.SetActive(true);
      ate--;
      GameObject.Find("Player " + this.player + " Stomach").GetComponent<StomachController>().setStomach(ate);
   }

   void FixedUpdate()
   {
      if (horizontal != 0 && vertical != 0) // Check for diagonal movement
      {
         // limit movement speed diagonally, so you move at 70% speed
         horizontal *= moveLimiter;
         vertical *= moveLimiter;
      }

      body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);

   }

   public GameObject blockPrefab;
   private void Build()
   {
      if (buildBlocks == 0) return;
      buildBlocks--;
      Vector3 spawnPosition = transform.position;
      //make sure it is in the middle of the grid should be .5
      spawnPosition.x = Mathf.Round(spawnPosition.x) - 0.5f;
      spawnPosition.y = Mathf.Round(spawnPosition.y) - 0.5f;

      Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
   }


   void OnCollisionEnter2D(Collision2D other)
   {

      Debug.Log("Collision");
      if (other.gameObject.CompareTag("Strawberry"))
      {
         ate++;
         if (ate == 3)
         {
            ate = 0;
            buildBlocks++;
         }
         Destroy(other.gameObject);
         GameObject.Find("GameManager").GetComponent<GameManager>().SpawnStrawberry();
         GameObject.Find("Player " + this.player + " Stomach").GetComponent<StomachController>().setStomach(ate);
      }

   }

   internal void Kill()
   {
      if (amDead) return;
      this.GetComponent<Animator>().SetTrigger("Dead");
      this.transform.position = new Vector3();
      this.GetComponent<CapsuleCollider2D>().enabled = false;
      amDead = true;
   }

   internal void Respawn()
   {
      amDead = false;
      this.GetComponent<Animator>().SetTrigger("Idle");
      this.GetComponent<CapsuleCollider2D>().enabled = true;
   }


}
