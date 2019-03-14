using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject instructionUI;
    public static GameManager singleton;
    [SerializeField] GameObject img;
    static bool dontShowFade;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
            Destroy(this.gameObject);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && instructionUI.activeInHierarchy && !dontShowFade)
        {
            hideInstruction();
        }
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

    public void loadIntroCutscene()
    {
        StartCoroutine(FadeInToScene("IntroVideo"));
        dontShowFade = true;
    }

    public void loadLevel1()
    {
        SceneManager.LoadScene("Santiago 1");
    }

    public void loadTutorial()
    {
        StartCoroutine(FadeInToScene("Tutorial"));
        dontShowFade = true;
    }

    public void showInstruction()
    {
        if(img != null && !dontShowFade)
        {
            img.SetActive(true);
            StartCoroutine(Fade(false));
        }
        instructionUI.SetActive(true);
    }

    public void hideInstruction()
    {
        if(img != null && !dontShowFade)
        {
            StartCoroutine(Fade(true));
        }
        instructionUI.SetActive(false);
    }

    IEnumerator Fade(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = .5f; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                img.GetComponent<Image>().color = new Color(0, 0, 0, i);
                yield return null;
            }
            img.SetActive(false);
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= .5f; i += Time.deltaTime)
            {
                // set color with i as alpha
                img.GetComponent<Image>().color = new Color(0, 0, 0, i);
                yield return null;
            }
        }
    }

    public IEnumerator FadeInToScene(string SceneName)
    {
        // loop over 1 second
        img.SetActive(true);
        float startVolume = MusicManager.singleton.music.volume;
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            // set color with i as alpha
            MusicManager.singleton.music.volume -= startVolume * Time.deltaTime;
            img.GetComponent<Image>().color = new Color(1, 1, 1, i);
            yield return null;
        }
        SceneManager.LoadScene(SceneName);
    }
}
