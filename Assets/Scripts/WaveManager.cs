using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticWaveController : MonoBehaviour
{
    [SerializeField] private GameObject[] waves;

    [SerializeField] private int startWaveIndex = 0;

    private int currentWaveIndex = -1;
    private bool levelCleared   = false;

    private void Start()
    {
        // Disable all waves first
        foreach (GameObject wave in waves)
            wave.SetActive(false);

        // LEVEL 3 NUCLEAR CLEANUP
        if (SceneManager.GetActiveScene().name == "Level3")
        {
            // 1. Destroy everything currently tagged "Enemy"
            GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject e in existingEnemies)
            {
                Destroy(e);
            }

            // 2. Destroy everything with EnemyHealth just in case tags are missing
            EnemyHealth[] allHealth = FindObjectsOfType<EnemyHealth>();
            foreach (EnemyHealth h in allHealth)
            {
                Destroy(h.gameObject);
            }

            // 3. Ensure Escape handler exists
            if (FindObjectOfType<EscHandler>() == null)
            {
                new GameObject("EscHandler").AddComponent<EscHandler>();
            }

            Debug.Log("[WaveManager] Level 3 Initialized: Nuclear cleanup complete. Ready for Boss.");
        }

        StartNextWave();
    }

    private void Update()
    {
        if (levelCleared) return;

        // Level 3 Special: If boss exists, don't advance
        if (SceneManager.GetActiveScene().name == "Level3")
        {
            if (GameObject.FindObjectOfType<RectangleFollow>() != null)
                return;
        }

        // Advance when all enemies are gone
        if (GameObject.FindObjectsOfType<EnemyHealth>().Length == 0)
            StartNextWave();
    }

    private void StartNextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            levelCleared = true;
            GameManager.instance.LevelClear();
            return;
        }

        GameObject currentWave = waves[currentWaveIndex];

        // SPECIAL FOR LEVEL 3: Remove all original enemies and spawn just one rectangle
        if (SceneManager.GetActiveScene().name == "Level3")
        {
            currentWave.SetActive(false);

            // ALWAYS DESTROY OLD BOSSES FIRST
            RectangleFollow[] oldBosses = GameObject.FindObjectsOfType<RectangleFollow>();
            foreach (RectangleFollow old in oldBosses)
            {
                Destroy(old.gameObject);
            }

            // Spawn the FRESH Boss
            GameObject boss = new GameObject("RectangleBoss");
            boss.AddComponent<RectangleFollow>();
            Debug.Log("[WaveManager] Level 3: SPAWNED FRESH BOSS. Hard-coded logic active.");
        }
        else
        {
            currentWave.SetActive(true);
        }

        Debug.Log($"[WaveManager] Wave {currentWaveIndex + 1} started.");
    }
}
