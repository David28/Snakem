using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StomachController : MonoBehaviour
{
    public Sprite[] stomachSprites;
    public int stomachState = 0;
    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void setStomach(int state) {
        if (state < 0 || state >= stomachSprites.Length) {
            return;
        }
        if (state > stomachState) {
            audioSource.Play();
        }
        stomachState = state;
        GetComponent<UnityEngine.UI.Image>().sprite = stomachSprites[stomachState];
    }
}
