using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private float parallaxFactor = 0.1f;
    private Transform cam;
    private Vector3 lastCamPos;

    private void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
    }

    private void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;
        transform.position += delta * parallaxFactor;
        lastCamPos = cam.position;
    }
}
