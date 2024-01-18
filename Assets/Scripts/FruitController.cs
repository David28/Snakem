using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    public Sprite[] sprites;
    
    public Color[] colors;

    public bool isMutaded = false; //mutaded fruit moves around and tries to go away from the the apple and snake
    public float mutationProbability = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        int index = Random.Range(0, colors.Length);

        if (sprites.Length == 0)
            {
                GetComponent<SpriteRenderer>().color = colors[index];
            }else
            {
                index = Random.Range(0, sprites.Length);
                GetComponent<SpriteRenderer>().sprite = sprites[index];
            }
        if (Random.Range(0.0f, 1.0f) < mutationProbability)
        {
            isMutaded = true;
            this.GetComponent<ParticleSystem>().Play();

        }else
        {
            isMutaded = false;
        }
    }
    
}
