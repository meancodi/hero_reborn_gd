using UnityEngine;

public class BGMRegister : MonoBehaviour
{
    void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RegisterBGM(GetComponent<AudioSource>());
        }
    }
}