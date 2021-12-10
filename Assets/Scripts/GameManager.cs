using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/*
 * The game manager script manages most of the UI, and the current difficulty.
 * 
 */

public class GameManager : MonoBehaviour
{
    #region SINGLETON

    public static GameManager _instance;
    public static GameManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null) {
                    GameObject container = new GameObject("GameManager");
                    _instance = container.AddComponent<GameManager>();
                }
            }

            return _instance;

        }
    }

    #endregion

    public float difficulty;
    public Sprite[] difficultyIcons = new Sprite[3];
    public Image difficultyImage;
    [Space]
    public GameObject loadingScreen;
    public Slider loadingSlider;
    public GameObject gameOverScreen;

    TextMeshProUGUI timerText;

    void Start()
    {
        difficulty = 0;
        Invoke("AddDifficulty", 60);
        timerText = gameOverScreen.transform.Find("Timer").GetComponent<TextMeshProUGUI>();
    }

    // The loop to add difficulty
    private void AddDifficulty() {
        difficulty++;
        if ((int)FindObjectOfType<Fire>().CurrentFireState >= 3) difficulty += 0.5f;

        if (difficulty >= 0 && difficulty < 5) {
            difficultyImage.sprite = difficultyIcons[0];
        }

        if (difficulty >= 5 && difficulty < 10) {
            difficultyImage.sprite = difficultyIcons[1];
        }

        if (difficulty >= 10) {
            difficultyImage.sprite = difficultyIcons[2];
        }
        Invoke("AddDifficulty", 60);
    }

    public void GameIsLost() {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
        timerText.text = "The fire survived " + Mathf.Round(Time.time) + "s.";
    }


    // The methods called by the buttons
    public void LoadMenu() {
        StartCoroutine(LoadAsynchronously(0));
        Time.timeScale = 1f;
    }

    public void Restart() {
        StartCoroutine(LoadAsynchronously(1));
        Time.timeScale = 1f;
    }

    IEnumerator LoadAsynchronously(int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        loadingScreen.SetActive(true);
        while (!operation.isDone) {

            float progress = Mathf.Clamp01(operation.progress / .9f);
            loadingSlider.value = progress;
            yield return null;
        }
        loadingScreen.SetActive(false);
    }

}
