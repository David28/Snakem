using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Vector2 mapSize = new Vector2(10, 10);

    public GameObject apple;
    public GameObject snake;
    public SnakeMovement snakeMovement;
    public GameObject strawberry;
    public float timer = 0;
    public float matchTime = 60*3; //3 minutes
    public TMP_Text timerText;
    public Slider appleText;
    // Start is called before the first frame update
    void Start()
    {
        timerText = GameObject.Find("Timer").GetComponent<TMP_Text>();
        timerText.text = "3:00";
        timer = matchTime;
        SnakeMovement snakeMovement = snake.GetComponent<SnakeMovement>();
        //spawn 3 strawberries in the grid except on the snake 
        for (int i = 0; i < 3; i++)
        {
            SpawnStrawberry();
        }
        appleText.maxValue = appleRespawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0;
            //end game
        }
        
        int minutes = (int) (timer / 60);
        int seconds = (int) (timer % 60);
        timerText.text = minutes + ":" + seconds.ToString("00");
        

        if (appleRespawnTimer > 0)
        {
            appleRespawnTimer -= Time.deltaTime;
            appleText.value = appleRespawnTimer;
            if (appleRespawnTimer <= 0)
            {
                SpawnApple();
            }

        }
    }
    private List<GameObject> strawberries = new List<GameObject>();
    public void SpawnStrawberry()
    {
        //create all possible positions
        List<Vector2> possiblePositions = new List<Vector2>();
        for (float x = -mapSize.x / 2 + 0.5f; x < mapSize.x / 2 - 0.5f; x += 1f)
        {
            for (float y = -mapSize.y / 2 + 0.5f; y < mapSize.y / 2 - 0.5f; y += 1f)
            {
                if (!snakeMovement.isFreePosition(new Vector3(x, y, 0f)))
                {
                    continue;
                }
                possiblePositions.Add(new Vector2(x, y));
            }
        }


        for (int i = 0; i < strawberries.Count; i++)
        {
            if (strawberries[i] == null)
            {
                strawberries.RemoveAt(i);
                i--;
            }
            else
            {
                possiblePositions.Remove(strawberries[i].transform.position);
            }
        }

        int randomIndex = UnityEngine.Random.Range(0, possiblePositions.Count);
        Vector2 randomPosition = possiblePositions[randomIndex];
        strawberries.Add(Instantiate(strawberry, randomPosition, Quaternion.identity));

    }

    private float appleRespawnTimer;
    public void SpawnApple()
    {
        apple.GetComponent<AppleController>().Respawn();
    }

    public float appleRespawnTime = 5f;
    internal void SetAppleRespawnTimer()
    {
        appleRespawnTimer = appleRespawnTime;
    }
}
