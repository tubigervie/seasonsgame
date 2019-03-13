using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject instructionUI;

    void update()
    {
        
    }

    public void RestartGame()
    {
    	PauseMenu.gameIsPaused = false;
    	Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
    	Debug.Log("Quit");
    	Application.Quit();
    }

    public void loadLevel1()
    {
        SceneManager.LoadScene("Santiago 1");
    }

    public void loadTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void showInstruction()
    {
        instructionUI.SetActive(true);
    }

    public void hideInstruction()
    {
        instructionUI.SetActive(false);
    }
}
