using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleController : MonoBehaviour
{
  Rigidbody2D body;

float horizontal;
float vertical;
float moveLimiter = 0.7f;
int ate = 0;
public float runSpeed = 20.0f;
private SpriteRenderer spriteRenderer;
void Start ()
{
   spriteRenderer = GetComponent<SpriteRenderer>();
   body = GetComponent<Rigidbody2D>();
}

public Vector2 shootDirection = new Vector2(1, 0);
private bool amDead = false;
void Update()
{

   // Gives a value between -1 and 1
   horizontal = Input.GetAxisRaw("Horizontal Apple"); // -1 is left
   vertical = Input.GetAxisRaw("Vertical Apple"); // -1 is down
   if (amDead) {
      return;
   }
   if (Input.GetKeyDown(KeyCode.L)) {
      Debug.Log("Fire1");
      spit();
   }
   if (vertical != 0) {
      if (vertical > 0) {
         spriteRenderer.flipY = false;
      } else {
         spriteRenderer.flipY = true;
      }
      
   }
    if (horizontal != 0) {
      if (horizontal > 0) {
         spriteRenderer.flipX = false;
      } else {
         spriteRenderer.flipX = true;
      }
      if (vertical == 0) {
         spriteRenderer.flipY = false;
      }

   }
   
   if (horizontal == 0 && vertical == 0) {
      if (currentState != 0){
         this.GetComponent<Animator>().SetTrigger("Idle");
      currentState = 0;
      }
      
   }else
   {
      shootDirection = new Vector2(horizontal, vertical);
      if (horizontal != 0 && vertical == 0){
         if (currentState != 2){
            this.GetComponent<Animator>().SetTrigger("Horizontal");
         currentState = 2;
         }
         
      }
      else if (currentState != 1){
         this.GetComponent<Animator>().SetTrigger("Vertical");
         currentState = 1;
      }
   }


   
}

   public GameObject spitPrefab;
    private int currentState = 0;

    void spit()  {
      if (ate == 0  || amDead) {
         return;
      }
      GameObject spit = Instantiate(spitPrefab, transform.position+((Vector3)shootDirection)*0.3f, Quaternion.identity);
      spit.GetComponent<SpitController>().direction = shootDirection;
      spit.SetActive(true);
      ate--;
      GameObject.Find("Apple Stomach").GetComponent<StomachController>().setStomach(ate);
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
   if (ate == 3) {
      return;
   }
   Debug.Log("Collision");
   if (other.gameObject.CompareTag("Strawberry"))
   {
      ate++;
      Destroy(other.gameObject);
      GameObject.Find("GameManager").GetComponent<GameManager>().SpawnStrawberry();
      GameObject.Find("Apple Stomach").GetComponent<StomachController>().setStomach(ate);
   }}

   internal void Kill()
   {
      this.GetComponent<Animator>().SetTrigger("Idle");
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
