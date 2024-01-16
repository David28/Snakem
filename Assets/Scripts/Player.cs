using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{


    public int player = 0;
    public GameObject dizzyDisplay;
    public float dizzyTime = 5.0f;
    public float dizzyTimer = 0.0f;

    public int ate = 0;

    public void ChangePlayer()
    {
        player = (player == 0) ? 1 : 0;
    }


    public Vector2 GetAppleInput()
    {
        Vector2 input = new Vector2(0, 0);
        if (player == 0)
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        else
            input = new Vector2(Input.GetAxisRaw("Horizontal2"), Input.GetAxisRaw("Vertical2"));

        if (dizzyTimer > 0.0f)
            input *= -1;
        return input;
    }


        public Vector2 GetSnakeInput()
        {
            Vector2 input = new Vector2(0, 0);

            if (player == 0)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    input = new Vector2(0, 1);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    input = new Vector2(-1, 0);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    input = new Vector2(0, -1);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    input = new Vector2(1, 0);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    input = new Vector2(0, 1);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    input = new Vector2(-1, 0);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    input = new Vector2(0, -1);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    input = new Vector2(1, 0);
                }
            }
            if (dizzyTimer > 0.0f)
            {
                input *= -1;
            }
            return input;
        }


        public bool GetAppleAction()
        {
            if (player == 1)
                return Input.GetKeyDown(KeyCode.L);
            else
                return Input.GetKeyDown(KeyCode.B);
        }

        public bool GetSnakeAction()
        {
            if (player == 0)
                return Input.GetKeyDown(KeyCode.B);
            else
                return Input.GetKeyDown(KeyCode.L);
        }
        public bool GetApplePowerDown()
        {
            if (player == 0)
                return Input.GetKey(KeyCode.V);
            else
                return Input.GetKey(KeyCode.K);
        }

        public bool GetApplePowerRelease()
        {
            if (player == 0)
                return Input.GetKeyUp(KeyCode.V);
            else
                return Input.GetKeyUp(KeyCode.K);
        }
        public void SetHeadshotSprite(string spriteName)
        {
            GameObject.Find("Player " + player + " Headshot").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + spriteName);
        }

        public void SetMunchSound(string soundName)
        {
            GameObject.Find("Player " + player + " Stomach").GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Sounds/" + soundName);
        }

        public void SetDizzy(bool isDizzy)
        {
            dizzyDisplay.SetActive(isDizzy);
            dizzyTimer = (isDizzy) ? dizzyTime : 0.0f;
        }

        internal void TickDizzy()
        {
            if (dizzyTimer > 0.0f)
            {
                dizzyTimer -= Time.deltaTime;
                if (dizzyTimer <= 0.0f)
                {
                    dizzyTimer = 0.0f;
                    dizzyDisplay.SetActive(false);
                }
            }
        }

        public void SetAte(int ate)
        {
            
            this.ate = ate;
            GameObject.Find("Player " + this.player + " Stomach").GetComponent<StomachController>().setStomach(ate);
        }

    }
