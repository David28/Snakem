using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StomachController : MonoBehaviour
{
    public Sprite[] stomachSprites;
    public int stomachState = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setStomach(int state) {
        if (state < 0 || state >= stomachSprites.Length) {
            return;
        }

        stomachState = state;
        GetComponent<UnityEngine.UI.Image>().sprite = stomachSprites[stomachState];
    }
}
