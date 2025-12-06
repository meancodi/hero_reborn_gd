using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class L2_EndTrigger : MonoBehaviour
{
    private bool detect = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!detect)
        {
            print("Detected");
        }
    }
}
