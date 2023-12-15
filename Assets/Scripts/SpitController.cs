using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitController : MonoBehaviour
{
    public float speed = 5.0f;
    public float destroyDelay = 3.0f;
    public Vector2 direction;
    public void Awake()
    {
        if (destroyDelay > 0.0f)
            Destroy(gameObject, destroyDelay);
    }

    // Update is called once per frame
    void Update()
    {
        //move spit
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Snake"))
        {
            other.gameObject.GetComponent<SnakeMovement>().startMiniGame();
            Destroy(gameObject);
        }
    }
}
