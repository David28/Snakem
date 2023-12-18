using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BodyPartMovement : MonoBehaviour
{
    public Vector2 direction;

    public Sprite turnSprite;
    public Sprite bodySprite;

    private BoxCollider2D straightCollider;
    private PolygonCollider2D turnCollider;
    public bool isTail = false;
    public void Start()
    {
        Debug.Log("start");
        straightCollider = GetComponent<BoxCollider2D>();
        if (isTail)
            return;
        turnCollider = GetComponent<PolygonCollider2D>();
        turnCollider.enabled = false;
    }

    public Vector2 moveTo(Vector2 pos, Vector2 nextDirection) {   
        Vector2 oldPos = transform.position;
        this.direction = pos - oldPos;
        if (isTail) {
            
            transform.position = pos;
            //get the angle from direction
            if (nextDirection == Vector2.up) {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            } else if (nextDirection == Vector2.right) {
                transform.rotation = Quaternion.Euler(0, 0, -90);
            } else if (nextDirection == Vector2.down) {
                transform.rotation = Quaternion.Euler(0, 0, 180);
            } else if (nextDirection == Vector2.left) {
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            return this.direction;
        }
        
        if (this.direction != nextDirection) {
            //turn
            transform.position = pos;
            GetComponent<SpriteRenderer>().sprite = turnSprite;
            straightCollider.enabled = false;
            turnCollider.enabled = true;
            this.direction = pos - oldPos;
            //get the angle from direction to nextDirection default is from up to right
            if (this.direction == Vector2.up) {
                if (nextDirection == Vector2.right) {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                } else if (nextDirection == Vector2.left) {
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                }
            } else if (this.direction == Vector2.right) {
                if (nextDirection == Vector2.up) {
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                } else if (nextDirection == Vector2.down) 
                    transform.rotation = Quaternion.Euler(0, 0, -90);
            } else if (this.direction == Vector2.down) {
                if (nextDirection == Vector2.right) {
                    transform.rotation = Quaternion.Euler(0, 0, -270);
                } else if (nextDirection == Vector2.left) {
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                }
            } else if (this.direction == Vector2.left) {
                if (nextDirection == Vector2.up) 
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                else if (nextDirection == Vector2.down)
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                
            }

        } else {
            //straight
            transform.position = pos;
            GetComponent<SpriteRenderer>().sprite = bodySprite;
            straightCollider.enabled = true;
            turnCollider.enabled = false;
            //get the angle from direction
            if (this.direction == Vector2.up) {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            } else if (this.direction == Vector2.right) {
                transform.rotation = Quaternion.Euler(0, 0, -90);
            } else if (this.direction == Vector2.down) {
                transform.rotation = Quaternion.Euler(0, 0, 180);
            } else if (this.direction == Vector2.left) {
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }
        }

        return this.direction;
    }
}
