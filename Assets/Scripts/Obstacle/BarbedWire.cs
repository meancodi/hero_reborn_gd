using UnityEngine;

public class BarbedWire : MonoBehaviour
{
    private bool collisionHappened = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collisionHappened == false && collision.CompareTag("Player"))
        {
            Object.FindAnyObjectByType<GameManager>().PlayerDefeated();
            collisionHappened = true;
        }
    }
}
