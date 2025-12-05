using UnityEngine;

public class DronePatrol : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;

    private Vector3 target;

    private void Start()
    {
        // Start moving toward point A
        target = pointA.position;
    }

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        // Move drone towards target point
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // If drone reached target, switch target
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            target = (target == pointA.position) ? pointB.position : pointA.position;
        }
    }
}
