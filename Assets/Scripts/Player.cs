using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{


    public int player = 0;

    public void ChangePlayer() {
        player = (player==0)?1:0;
    }


    public Vector2 GetAppleInput() {
        if (player == 0)
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        else
            return new Vector2(Input.GetAxisRaw("Horizontal2"), Input.GetAxisRaw("Vertical2"));
    }

    
    public Vector2 GetSnakeInput() {
        if (player == 0){
            if (Input.GetKeyDown(KeyCode.W)) {
                return new Vector2(0, 1);
            }else if (Input.GetKeyDown(KeyCode.A)) {
                return new Vector2(-1, 0);
            }else if (Input.GetKeyDown(KeyCode.S)) {
                return new Vector2(0, -1);
            }else if (Input.GetKeyDown(KeyCode.D)) {
                return new Vector2(1, 0);
            }
        }else {
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                return new Vector2(0, 1);
            }else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                return new Vector2(-1, 0);
            }else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                return new Vector2(0, -1);
            }else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                return new Vector2(1, 0);
            }
        }
        return new Vector2(0, 0);
    }


    public bool GetAppleAction() {
        if (player == 1)
            return Input.GetKeyDown(KeyCode.L);
        else
            return Input.GetKeyDown(KeyCode.B);
    }

    public bool GetSnakeAction() {
        if (player == 0)
            return Input.GetKeyDown(KeyCode.B);
        else
            return Input.GetKeyDown(KeyCode.L);
    }
    public bool GetApplePower() {
        if (player == 0)
            return Input.GetKeyDown(KeyCode.V);
        else
            return Input.GetKeyDown(KeyCode.K);
    }
    public void SetHeadshotSprite(string spriteName) {
        GameObject.Find("Player "+player+" Headshot").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/"+spriteName);
    }

    public void SetMunchSound(string soundName) {
        GameObject.Find("Player "+player+" Stomach").GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Sounds/"+soundName);
    }
}
