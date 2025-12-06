//using UnityEngine;

//public class StaticWaveController : MonoBehaviour
//{
//    [Header("Wave Parents (order matters)")]
//    [SerializeField] private GameObject[] waves;

//    private int currentWaveIndex = -1;

//    private void Start()
//    {
//        // Safety: disable all waves
//        foreach (GameObject wave in waves)
//            wave.SetActive(false);

//        StartNextWave();
//    }

//    private void Update()
//    {
//        // If no enemies left in scene, move to next wave
//        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
//        {
//            StartNextWave();
//        }
//    }

//    private void StartNextWave()
//    {
//        currentWaveIndex++;

//        if (currentWaveIndex >= waves.Length)
//        {
//            GameManager.instance.LevelClear();
//            return;
//        }

//        Debug.Log("Starting Wave " + (currentWaveIndex + 1));
//        waves[currentWaveIndex].SetActive(true);
//    }
//}


using UnityEngine;

public class StaticWaveController : MonoBehaviour
{
    [SerializeField] private GameObject[] waves;
    private int currentWaveIndex = -1;

    private void Start()
    {
        foreach (GameObject wave in waves)
            wave.SetActive(false);

        StartNextWave();
    }

    private void Update()
    {
        if (FindObjectsOfType<EnemyHealth>().Length == 0)
        {
            StartNextWave();
        }
    }

    private void StartNextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            GameManager.instance.LevelClear();
            return;
        }

        waves[currentWaveIndex].SetActive(true);
    }
}
