using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public Vector2 mapSize = new Vector2(10, 10);

    private GameObject apple;
    private GameObject snake;
    private SnakeMovement snakeMovement;
    private AppleController appleController;
    private GameObject strawberry;
    public float timer = 0;
    public float matchTime = 60*3; //3 minutes
    private TMP_Text timerText;
    private Slider appleSlider;
    public bool hasStarted = false;
    public bool inMenu = true;
    void Start()
    {
        Debug.Log("Awake");
        if (inMenu)
        {
            DontDestroyOnLoad(gameObject);
            return;
        }
        //get everything
        apple = GameObject.Find("Apples");
        snake = GameObject.Find("Snake");
        snakeMovement = snake.GetComponent<SnakeMovement>();
        appleController = apple.GetComponent<AppleController>();
        timerText = GameObject.Find("Timer").GetComponent<TMP_Text>();
        strawberry = GameObject.Find("Strawberry");
        appleSlider = GameObject.Find("Apple Headshot").GetComponent<Slider>();

        timerText.text = "3:00";
        timer = matchTime;
        
        appleSlider.maxValue = appleRespawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasStarted)
        {
            return;
        }
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
            appleSlider.value = appleRespawnTimer;
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
        appleController.Respawn();
    }

    public float appleRespawnTime = 5f;
    internal void SetAppleRespawnTimer()
    {
        appleRespawnTimer = appleRespawnTime;
    }

    //function for animation events
    public void BeginGame()
    {
        Destroy(GameObject.Find("Go"));
        Destroy(GameObject.Find("Ready"));
        timerText.gameObject.SetActive(true);     
        snakeMovement.enabled = true;   
        //spawn 3 strawberries in the grid except on the snake 
        for (int i = 0; i < 3; i++)
        {
            SpawnStrawberry();
        }
        appleController.enabled = true;  
        hasStarted = true;  
    }

    public void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Snake");
        //get call back when scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    private int round = 1;

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (inMenu)
            inMenu = false;
        else
            round++;
        
        Start();
    }

    public Vector2 GetPlayerInput(int playerIndex)
    {
        if (playerIndex == 1)
        {
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        else
        {
            return new Vector2(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"));
        }
    }

}
