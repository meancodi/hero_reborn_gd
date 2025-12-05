using UnityEngine;

public class EndGame : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            GameManager.instance.LevelClear();
    }
}
