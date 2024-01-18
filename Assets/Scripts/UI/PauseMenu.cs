using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu");
        Destroy(GameObject.FindObjectOfType<GameManager>().gameObject);
    }
}
