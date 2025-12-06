using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool gameOver = false;

    [SerializeField] private GameObject defeatScreen;
    [SerializeField] private GameObject successScreen;

    public float runTimer = 0f;
    private bool timerRunning = true;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        defeatScreen.SetActive(false);
        successScreen.SetActive(false);
    }

    private void Update()
    {
        if (timerRunning)
            runTimer += Time.unscaledDeltaTime;

    }


    public void PlayerDefeated()
    {
        defeatScreen.SetActive(true);
        gameOver = true;
        Time.timeScale = 0f;   // freeze the game
    }
    public void RestartLevel()
    {
        Debug.Log("Restart pressed! Unfreezing.");
        Time.timeScale = 1f; // unfreeze
        gameOver = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BacktoMainMenu()
    {
        Debug.Log("Restart pressed! Unfreezing.");
        Time.timeScale = 1f; // unfreeze
        gameOver = true;
        SceneManager.LoadScene(0);
    }

    public void LevelClear()
    {
        Debug.Log("Completed!");
        timerRunning = false;

        successScreen.SetActive(true);
        gameOver = true;
        Time.timeScale = 0f;

        // Send final time to UI
        SpeedrunTimerUI.instance.ShowFinalTime(runTimer);
    }


}
