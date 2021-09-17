using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public float gameProcessAsteroidSpawnDelay = 2.0f;
    public int asteroidsAmount = 2;
    public string asteroidGameObjectName = "asteroid(Clone)";
    public string controlsKeyboardMouseText = "CONTROLS: KEYBOARD + MOUSE";
    public string controlsKeyboardText = "CONTROLS: KEYBOARD";

    public GameObject spaceShip;
    public GameObject ufo;
    public GameObject controlButtonText;
    public GameObject continueButton;

    public GameObject livesCountText;
    public GameObject scoreCountText;
    public int livesCount = 20;
    private int scoreCount = 0;
    public string asteroidGameObjectTag = "asteroid";

    public GameObject menu;

    private bool gameProcessFlag = false;
    private float asteroidsSpawnDelay = 0.1f;
    private bool gamePauseFlag = true;
    private bool showMenuFlag = true;
    private bool keyboardControl = true;

    public bool isKeyboardControl()
    {
        return keyboardControl;
    }

    void Start()
    {
        StartCoroutine(createAsteroids());
        livesCountText.GetComponent<Text>().text = livesCount.ToString();
        menu.SetActive(false);
    }

    IEnumerator createAsteroids()
    {
        gameProcessFlag = false;

        yield return new WaitForSeconds(asteroidsSpawnDelay);

        for (int i = 0; i < asteroidsAmount; i++)
        {
            createAsteroid();
        }

        gameProcessFlag = true;
    }

    void Update()
    {
        if (gameProcessFlag)
        {
            GameObject asteroid = GameObject.Find(asteroidGameObjectName);

            if (asteroid == null)
            {
                asteroidsAmount++;
                asteroidsSpawnDelay = gameProcessAsteroidSpawnDelay;
                StartCoroutine(createAsteroids());
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showMenu(showMenuFlag);
            gamePause(gamePauseFlag);
            gamePauseFlag = !gamePauseFlag;
            showMenuFlag = !showMenuFlag;
        }
    }

    void showMenu(bool value = true, bool continueButtonInteractable = true)
    {
        menu.SetActive(value);
        continueButton.GetComponent<Button>().interactable = continueButtonInteractable;
    }

    void gamePause(bool value)
    {
        spaceShip.GetComponent<SpaceShip>().setUpdateEnabled(value);
        spaceShip.GetComponent<FPSInput>().setUpdateEnabled(value);
        ufo.GetComponent<Ufo>().setUpdateEnabled(value);

        GameObject[] asteroids = GameObject.FindGameObjectsWithTag(asteroidGameObjectTag);

        foreach (var asteroid in asteroids)
        {
            asteroid.GetComponent<Asteroid>().setUpdateEnabled(value);
        }
    }

    void createAsteroid()
    {
        GameObject asteroid = ObjectPool.SharedInstance.GetPooledObject("asteroid");

        if (asteroid != null)
        {
            asteroid.transform.position = transform.position;
            asteroid.transform.rotation = transform.rotation;
            asteroid.SetActive(true);

            asteroid.SendMessage("createNewAsteroid", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void decreaseLives()
    {
        if (livesCount > 0)
        {
            livesCount--;
            livesCountText.GetComponent<Text>().text = livesCount.ToString();
        }

        if (livesCount == 0)
        {
            showMenu(true, false);
            gamePause(true);
        }
    }

    public void increaseScore(int scores)
    {
        scoreCount += scores;
        scoreCountText.GetComponent<Text>().text = scoreCount.ToString();
    }

    public void OnControlToggle()
    {
        if (keyboardControl)
        {
            controlButtonText.GetComponent<Text>().text = controlsKeyboardMouseText;
        }
        else
        {
            controlButtonText.GetComponent<Text>().text = controlsKeyboardText;
        }

        keyboardControl = !keyboardControl;
    }

    public void OnContinueToggle()
    {
        showMenu(false);
        gamePause(false);

        gamePauseFlag = !gamePauseFlag;
        showMenuFlag = !showMenuFlag;
    }

    public void OnNewGameToggle()
    {
        SceneManager.LoadScene("Asteroid");
    }

    public void OnExitToggle()
    {
        Application.Quit();
    }
}
