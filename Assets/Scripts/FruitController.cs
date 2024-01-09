using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    public Sprite[] sprites;
    // Start is called before the first frame update
    void Start()
    {
        int index = Random.Range(0, sprites.Length);
        GetComponent<SpriteRenderer>().sprite = sprites[index];

        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle.transform.position == transform.position)
            {
                //set alpha to 0.8
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);
                this.GetComponent<CapsuleCollider2D>().enabled = false;
            }
        }
    }

}
