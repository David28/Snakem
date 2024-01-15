using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float radius = 1.0f;
    public float angle = 0.0f;

    // Update is called once per frame
    void Update()
    {
        angle += speed * Time.deltaTime;
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;
        //local
        transform.localPosition = new Vector3(x, y, 0);   
    }
}
