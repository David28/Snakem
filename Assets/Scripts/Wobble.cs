using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
    public float wobbleSpeed = 1.0f;

    public float maxScale = 2.0f;

    public float destroyDelay = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        if (destroyDelay > 0.0f)
            Destroy(gameObject, destroyDelay);
    }

    // Update is called once per frame
    void Update()
    {
       if (transform.localScale.x > maxScale)
       {
           wobbleSpeed = -wobbleSpeed;
       }
         if (transform.localScale.x < 1.0f)
         {
              wobbleSpeed = -wobbleSpeed;
         }

        //lerp between 1 and maxScale
        transform.localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(maxScale, maxScale, maxScale), Mathf.PingPong(Time.time * wobbleSpeed, 1.0f));  
    }
}
