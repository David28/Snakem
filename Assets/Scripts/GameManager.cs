using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public Vector2 mapSize = new Vector2(10, 10);

    public GameObject apple;
    public GameObject snake;
    private SnakeMovement snakeMovement;
    private AppleController appleController;
    private GameObject strawberry;
    public float timer = 0;
    
    private TMP_Text timerText;
    public Slider appleSlider;
    public bool hasStarted = false;
    public bool inMenu = true;

    private int[] score = new int[2];
    private int round = 0;
    private TMP_Text[] scoreDisplay;

    //Round Settings
    public float matchTime = 3; //3 minutes
    public int[] chosenApple = new int[2];
    public int fruitCount = 3;

    public int level = 0;
    void Start()
    {
        Time.timeScale = 1;
        Debug.Log("Awake");
        if (inMenu)
        {
            DontDestroyOnLoad(gameObject);
            if (round >= 2){
                if (score[0] == score[1]){
                    GameObject.Find("Win").GetComponent<TMP_Text>().text = "It's a tie!";
                    Destroy(GameObject.Find("Confetti"));
                }
                else{
                    GameObject.Find("Win").GetComponent<TMP_Text>().text = "Player " + ((score[0]>score[1])?"1":"2") + " Wins!";
                    GameObject.Find("Win").GetComponent<AudioSource>().Play();

                }

                Destroy(this.gameObject);
            }
            return;
        }else
            round++;
        //get everything
        apple = GameObject.Find("Apples");
        snake = GameObject.Find("Snake");
        
        snakeMovement = snake.GetComponent<SnakeMovement>();
        appleController = apple.GetComponent<AppleController>();
        
        timerText = GameObject.Find("Timer").GetComponent<TMP_Text>();
        strawberry = GameObject.Find("Strawberry");

        timer = matchTime*60f;
        int minutes = (int) (timer / 60);
        int seconds = (int) (timer % 60);
        timerText.text = minutes + ":" + seconds.ToString("00");

        scoreDisplay = GameObject.Find("Score Display").GetComponentsInChildren<TMP_Text>();
        //Change round
        scoreDisplay[0].text = "Round " + round;
        scoreDisplay[1].text = "Score: \nPlayer 1:  " + ((round==1)?"--":score[0]) + "\nPlayer 2:  "+ score[1];
        if (round > 1) {
            snakeMovement.ChangePlayer();
            appleController.ChangePlayer();
        }
        appleController.SetHeadshotSprite("Apple Headshot");
        appleController.SetMunchSound("Apple Munch");
        snakeMovement.SetHeadshotSprite("Snake Headshot");
        snakeMovement.SetMunchSound("Snake Munch");

        appleSlider = GameObject.Find("Player "+appleController.player+" Headshot").GetComponent<Slider>();
        apple.GetComponent<AppleAbbilityController>().type = chosenApple[appleController.player];

        appleSlider.maxValue = appleRespawnTime;
        appleRespawnTimer = 0;

        environmentObstacles = GameObject.FindGameObjectsWithTag("Environment Obstacle").ToList();

    }

  

    // Update is called once per frame
    void Update()
    {
        if (inMenu)
            return;

        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            //switch the pause menu child to active or inactive
            GameObject child = GameObject.Find("Pause Menu").transform.GetChild(0).gameObject;
            child.SetActive(!child.activeSelf);
            Time.timeScale = child.activeSelf ? 0 : 1;
        }
        if (!hasStarted)
        {
            return;
        }
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0;
            if (round <= 2)
                LoadGame();
                
                
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
                //
                appleController.canRespawn = true;
            }

        }


    }
    private List<GameObject> strawberries = new List<GameObject>();
    private List<GameObject> environmentObstacles = new List<GameObject>();
    internal bool IsOccupied(Vector3 position)
    {
        GameObject[] snakeBodyParts = GameObject.FindObjectsOfType<BodyPartMovement>().Select(bodyPart => bodyPart.gameObject).ToArray();
        GameObject[] head = {snake};
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        obstacles = obstacles.Concat(environmentObstacles).Concat(head).Concat(snakeBodyParts).ToArray(); //i dont like this
        return obstacles.Any(obstacle => obstacle.transform.position == position) || position.x >= 5.5 || position.x <= -5.5 || position.y >= 5.5 || position.y <= -5.5;
    }
    public void SpawnStrawberry()
    {
        //create all possible positions
        List<Vector3> possiblePositions = new List<Vector3>();
        
        
        for (float x = -mapSize.x / 2 + 0.5f; x < mapSize.x / 2 +1 - 0.5f; x += 1f)
        {
            for (float y = -mapSize.y / 2 + 0.5f; y < mapSize.y / 2 +1 - 0.5f; y += 1f)
            {
                if (IsOccupied(new Vector3(x, y,0)))
                {
                    continue;
                }
                possiblePositions.Add(new Vector3(x, y,0));
            }
        }


        for (int i = 0; i < strawberries.Count; i++)
        {
            if (strawberries[i] == null)
            {
                strawberries.RemoveAt(i); //concurrency issues
                i--;
            }
            else
            {
                possiblePositions.Remove(strawberries[i].transform.position);
            }
        }
   
        if (possiblePositions.Count == 0)
        {
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, possiblePositions.Count);
        Vector2 randomPosition = possiblePositions[randomIndex];
        strawberries.Add(Instantiate(strawberry, randomPosition, Quaternion.identity));

    }

    public float appleRespawnTimer;


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
        snakeMovement.enabled = true;   
        //spawn 3 strawberries in the grid except on the snake 
        for (int i = 0; i < fruitCount; i++)
        {
            SpawnStrawberry();
        }
        appleController.enabled = true;  
        hasStarted = true;  
    }

    public void LoadGame()
    {
        if (round == 2)
        {
            //load win screen
            inMenu = true;
            SceneManager.LoadScene("WinScene");
        }else
            UnityEngine.SceneManagement.SceneManager.LoadScene("Snake " + level);

            
        //get call back when scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadRoundMenu()
    {
        inMenu = true;
        SceneManager.LoadScene("RoundMenu");
       
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (inMenu && scene.name == "Snake " + level){
            inMenu = false;
            GetComponent<AudioSource>().Stop();
        }
        Start();
    }

    internal void RemovePoint(int player)
    {
        score[player]--;
        scoreDisplay[1].text = "Score: \nPlayer 1:  " + ((round==1)?"--":score[0]) + "\nPlayer 2:  "+ score[1];
    }

    internal void AddPoint(int player)
    {
        score[player]++;
        scoreDisplay[1].text = "Score: \nPlayer 1:  " + ((round==1)?"--":score[0]) + "\nPlayer 2:  "+ score[1];
        if (score[player] >= mapSize.x * mapSize.y - 3 - environmentObstacles.Count)
        {
            LoadGame();
        }
    }

    
    
}
