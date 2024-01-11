using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public float lifeTime = 5.0f;
    private float timer = 0.0f;

    private GameObject storedFruit;
    // Start is called before the first frame update
    void Start()
    {
        //Call animator Destroy trigger after lifetime
        timer = lifeTime;
        GameObject[] fruits = GameObject.FindGameObjectsWithTag("Strawberry");
      foreach (GameObject fruit in fruits)
      {
         if (fruit.transform.position == transform.position)
         {
            fruit.GetComponent<SpriteRenderer>().color = new Color(1f,1f, 1f,0.6f);
            fruit.GetComponent<CapsuleCollider2D>().enabled = false;
            storedFruit = fruit;
         }
      }
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0.0f)
        {
            return;
        }
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            GetComponent<Animator>().SetTrigger("Destroy");
            if (storedFruit != null)
            {
                //change color to white
                storedFruit.GetComponent<SpriteRenderer>().color = Color.white;

                storedFruit.GetComponent<CapsuleCollider2D>().enabled = true;
            }
        }
    }
}
