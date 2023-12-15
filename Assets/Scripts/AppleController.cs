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

void Update()
{
   // Gives a value between -1 and 1
   horizontal = Input.GetAxisRaw("Horizontal Apple"); // -1 is left
   vertical = Input.GetAxisRaw("Vertical Apple"); // -1 is down
   if (Input.GetKeyDown(KeyCode.L)) {
      Debug.Log("Fire1");
      spit();
   }
    if (horizontal != 0) {
      if (horizontal > 0) {
         spriteRenderer.flipX = false;
      } else {
         spriteRenderer.flipX = true;
      }
      if (vertical == 0) {
         this.GetComponent<Animator>().SetBool("Horizontal", true);
         spriteRenderer.flipY = false;
      }
   }else
   {
      this.GetComponent<Animator>().SetBool("Horizontal", false);
   }
   if (vertical != 0) {
      if (vertical > 0) {
         spriteRenderer.flipY = false;
      } else {
         spriteRenderer.flipY = true;
      }
      this.GetComponent<Animator>().SetBool("Vertical", true);
   }else
   {
      this.GetComponent<Animator>().SetBool("Vertical", false);
   }

   if (horizontal == 0 && vertical == 0) {
      this.GetComponent<Animator>().SetBool("Idle", true);
   }else
   {
      this.GetComponent<Animator>().SetBool("Idle", false);
   }


   
}

   public GameObject spitPrefab;
   void spit()  {
      if (ate == 0 || (horizontal == 0 && vertical == 0) ) {
         return;
      }
      GameObject spit = Instantiate(spitPrefab, transform.position+new Vector3(horizontal, vertical)*0.3f, Quaternion.identity);
      spit.GetComponent<SpitController>().direction = new Vector2(horizontal, vertical);
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

}
