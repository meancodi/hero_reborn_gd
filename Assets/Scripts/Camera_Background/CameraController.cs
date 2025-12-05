using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    //room
    [SerializeField] private float speed;
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float playerUp;
    [SerializeField] private float leftBound;
    [SerializeField] private float rightBound;
    private float lookAhead;
    private float upDist;

    private void Awake()
    {
        Camera.main.backgroundColor = new Color(93f / 255f, 127f / 255f, 157f / 255f);
        upDist = player.position.y + playerUp;
    }

    private void Update()
    {
        //for room based movement
        //transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, transform.position.y, transform.position.z), ref velocity, speed );
        if(player.position.x > leftBound && player.position.x < rightBound)
            transform.position = new Vector3(player.position.x + lookAhead, upDist, transform.position.z);
        else
            transform.position = new Vector3(transform.position.x, upDist, transform.position.z);




        //lookAhead = Mathf.Lerp(playerUp, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);
        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);


    }

}
