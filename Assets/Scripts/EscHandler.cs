using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Use the New Input System

public class EscHandler : MonoBehaviour
{
    private void Update()
    {
        // New Input System check for Escape
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("[EscHandler] Escape pressed (New Input System). Returning to Main Menu.");
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
        // Fallback for legacy just in case
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }
}
