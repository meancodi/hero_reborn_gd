using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool gameOver = false;
    public bool isPaused = false;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject defeatScreen;
    [SerializeField] private GameObject successScreen;
    [SerializeField] private AudioSource currentBGM;
    [SerializeField] private UnityEngine.UI.Slider volumeSlider;

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
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (timerRunning)
            runTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

    }

    public void RegisterBGM(AudioSource bgm)
    {
        currentBGM = bgm;

        // Apply saved volume immediately
        float v = PlayerPrefs.GetFloat("musicVolume", 1f);
        currentBGM.volume = v;

        if (volumeSlider != null)
            volumeSlider.value = v;
    }

    public void SetVolume(float value)
    {
        if (currentBGM != null)
            currentBGM.volume = value;

        PlayerPrefs.SetFloat("musicVolume", value);
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

    public void TogglePause()
    {
        if (gameOver) return; // prevent pause after win/lose

        isPaused = !isPaused;

        if (isPaused)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }

}
