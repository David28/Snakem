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
   
   public float runSpeed = 20.0f;
   private SpriteRenderer spriteRenderer;
   void Start()
   {
      spriteRenderer = GetComponent<SpriteRenderer>();
      body = GetComponent<Rigidbody2D>();
   }

   public Vector2 shootDirection = new Vector2(1, 0);

   public bool amDead = false;
   public bool canRespawn = false;
   private Vector2 input;


   void Update()
   {
      if (boosting)
      {
         boostTimer -= Time.deltaTime;
         if (boostTimer <= 0.0f || amDead)
         {
            boosting = false;
            this.GetComponent<ParticleSystem>().Stop();
         }
      }
      TickDizzy();
      Animator animator = this.GetComponent<Animator>();
      
      animator.SetBool("Dead", amDead);

      // Gives a value between -1 and 1
      input = GetAppleInput();
      horizontal = input.x;
      vertical = input.y;
      
      if (amDead)
      {
         if (canRespawn && GetAppleAction())
         {
            Respawn();
         }
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

      if (horizontal != 0 || vertical != 0)
      {
         shootDirection = new Vector2(horizontal, vertical);
         
      }
      animator.SetBool("Horizontal", horizontal != 0);
      animator.SetBool("Vertical", vertical != 0);
      animator.SetBool("Front", vertical < 0);

   }



   public GameObject spitPrefab;
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
      float speed = (boosting ? boostSpeed : runSpeed);
      body.velocity = new Vector2(horizontal * speed, vertical * speed);
      if (amDead) WrapAround();
   }

   


   void OnCollisionEnter2D(Collision2D other)
   {

      Debug.Log("Collision");
      if (other.gameObject.CompareTag("Strawberry"))
      {
         Destroy(other.gameObject);
         GameObject.Find("GameManager").GetComponent<GameManager>().SpawnStrawberry();
         SetDizzy(other.gameObject.GetComponent<RandomSprite>().isMutaded);
         int newAte = ate + (other.gameObject.GetComponent<RandomSprite>().isMutaded ? 2 : 1);

         if (newAte > 3)
         {
            Boost();
            newAte = 3;
         }
         SetAte(newAte);
      }

   }

    

    internal void Kill()
   {
      if (amDead) return;
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
      canRespawn = false;
      this.GetComponent<Animator>().SetBool("Dead", false);
      this.GetComponent<CapsuleCollider2D>().enabled = true;
      //Set my children to active
      foreach (Transform child in transform)
      {
         child.gameObject.SetActive(true);
      }
      SetDizzy(false);
   }

  
   private void WrapAround(){
      //wrap around
         Vector3 curPos = transform.position;
         float xBound = 5.5f;
         float offset = 0.5f;
         float yBound = 5.5f;
         if (curPos.x >= xBound - offset) {

            curPos.x = -xBound + offset;
         } else if (curPos.x <= -xBound + offset) {
            curPos.x = xBound - offset;
         }

         if (curPos.y >= yBound - offset) {
            curPos.y = -yBound + offset;
         } else if (curPos.y <= -yBound + offset) {
            curPos.y =     yBound - offset;
         }
         transform.position = curPos;
   }
   public bool boosting = false;
   public float boostTime = 1.0f;
   public float boostTimer = 0.0f;
   public float boostSpeed = 40.0f;
   private void Boost()
   {
      this.boosting = true;
      this.boostTimer = boostTime;
      this.GetComponent<AudioSource>().Play();
      this.GetComponent<ParticleSystem>().Play();
   }
}
