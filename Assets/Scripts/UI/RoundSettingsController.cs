using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundSettingsController : MonoBehaviour
{
    public Slider matchTimeSlider; // match time
    public Slider fruitSlider; // fruit count
    public GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        SetMatchTime();
        SetFruitCount();
    }
    public void SetMatchTime()
    {
        gameManager.matchTime = (int) matchTimeSlider.value;
    }

    public void SetFruitCount()
    {
        gameManager.fruitCount = (int)fruitSlider.value;
    }
    public void SetPlayer1Apple(int value)
    {
        gameManager.chosenApple[0] = value;
    }

    public void SetPlayer2Apple(int value)
    {
        gameManager.chosenApple[1] = value;
    }

    public void SetLevel(int value)
    {
        gameManager.level = value;
    }

    public void StartGame()
    {
        gameManager.LoadGame();
    }
}
