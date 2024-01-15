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
   public int ate = 0;
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


   void Update()
   {
      

      // Gives a value between -1 and 1
      input = GetAppleInput();
      horizontal = input.x;
      vertical = input.y;
      
      if (amDead)
      {
         WrapAround();
         return;
      }
      if (GetAppleAction())
      {
         Debug.Log("Fire1");
         spit();
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
         if (currentState != 0 && amDead == false)
         {
            this.GetComponent<Animator>().SetTrigger("Idle");
            Debug.Log("Idle");
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
   void spit()
   {
      if (ate == 0)
      {
         return;
      }
     

      GameObject spit = Instantiate(spitPrefab, transform.position + ((Vector3)shootDirection) * 0.3f, Quaternion.identity);
      spit.GetComponent<SpitController>().direction = shootDirection;
      spit.SetActive(true);
      
      SetAte(ate - 1);
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

   


   void OnCollisionEnter2D(Collision2D other)
   {

      Debug.Log("Collision");
      if (other.gameObject.CompareTag("Strawberry"))
      {
         Destroy(other.gameObject);
         GameObject.Find("GameManager").GetComponent<GameManager>().SpawnStrawberry();

         if (ate >= 3) return;

         SetAte(ate + 1);
      }

   }

   internal void Kill()
   {
      if (amDead) return;
      this.GetComponent<Animator>().SetTrigger("Dead");
      this.transform.position = new Vector3();
      this.GetComponent<CapsuleCollider2D>().enabled = false;
      amDead = true;
      //Set my children to inactive
      foreach (Transform child in transform)
      {
         child.gameObject.SetActive(false);
      }
   }

   internal void Respawn()
   {
      amDead = false;
      this.GetComponent<Animator>().SetTrigger("Idle");
      this.GetComponent<CapsuleCollider2D>().enabled = true;
      //Set my children to active
      foreach (Transform child in transform)
      {
         child.gameObject.SetActive(true);
      }
   }

   public void SetAte(int ate)
   {
      this.ate = ate;
      GameObject.Find("Player " + this.player + " Stomach").GetComponent<StomachController>().setStomach(ate);
   }

   private void WrapAround(){
      //wrap around
         Vector3 curPos = transform.position;
         float xBound = 5.5f;
         float offset = 0.5f;
         float yBound = 5.5f;
         if (curPos.x >= xBound - offset) {

            curPos.x = -xBound;
         } else if (curPos.x <= -xBound + offset) {
            curPos.x = xBound;
         }

         if (curPos.y >= yBound - offset) {
            curPos.y = -yBound;
         } else if (curPos.y <= -yBound + offset) {
            curPos.y = yBound;
         }
         transform.position = curPos;
   }
}
