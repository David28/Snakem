using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PointerMinigame : MonoBehaviour
{
    public GameObject pointer; // pointer goes back and forth horizontally
    public GameObject area; // 

    public GameObject target; // target to hit

    public float pointerSpeed = 1.0f;


    public float offset = 0.5f;
    public float targetSize = 0.3f;
    private SnakeMovement snakeMovement;
    // Start is called before the first frame update
    void Start()
    {
        //Get snake movement script
        snakeMovement = GameObject.Find("Snake").GetComponent<SnakeMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        //move pointer back and forth
        
        
        if (snakeMovement.GetSnakeAction())
        {
            
            //check if pointer is within target
            if (Mathf.Abs(pointer.transform.localPosition.x - target.transform.localPosition.x) < targetSize)
            {
                //pointer is within target
                Debug.Log("You win!");
                try {
                    snakeMovement.endStun();
                }
                catch
                {
                    Debug.Log("Snake not found");
                }
            }
            else
            {
                //pointer is not within target
                Debug.Log("You lose!");
                //change win animation text to Nice Try and change color to red
                try {
                    snakeMovement.DestroyBodyPart();
                }
                catch
                {
                    Debug.Log("Snake not found");
                }
            }
           target.GetComponent<SpriteRenderer>().enabled = false;
            pointer.GetComponent<SpriteRenderer>().enabled = false;
            area.GetComponent<Animator>().enabled = true;
        }

    }

    void FixedUpdate() {
        pointer.transform.localPosition = new Vector3(pointer.transform.localPosition.x + pointerSpeed * Time.deltaTime, pointer.transform.localPosition.y, pointer.transform.localPosition.z);
        if (pointerSpeed > 0 && pointer.transform.localPosition.x > 0.5f - offset)
            pointerSpeed = -pointerSpeed;
        else if (pointerSpeed < 0 && pointer.transform.localPosition.x < -0.5f+offset)
            pointerSpeed = -pointerSpeed;
    }

}
