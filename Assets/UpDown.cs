using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class UpDown : MonoBehaviour
{
    //Test comment
    private string OBSTACLE_TAG = "UpDownObstacle";

    private PlayerInput PlayerInput;
    private Rigidbody Rigidbody;
    private InputAction Move;
    private Transform gameCenter;
    private float initialZ;
    private bool gameFailed = false;

    public float moveSpeedMultiplier = .5f;
    public float maxHeight = 3.65f;

    public float rotateX = .1f;
    public float rotateY = .1f;
    public float rotateZ = .1f;

    [Header("Camera")]
    [SerializeField]
    private Camera gameCamera;
    public float sizeChangeSpeed = 2f;

    [Header("Spawner Controls")]
    [SerializeField]
    private GameObject obstacle;
    public float spawnPointPadding = 5f;
    public float spawnFuzziness = 1f;
    public float secondsBetweenSpawns = 3f;
    public float obstacleSpeed = .1f;

    [Header("Danger Indicator Controls")]
    [SerializeField]
    private Light spotLight;
    public float lightTempDangerMin = 6900;
    public float lightTempDangerMax = 1500;
    public float noDangerDistance = 2f;

    // Start is called before the first frame update
    void Start()
    {
        initialZ = transform.position.z;
        gameCenter = GameObject.Find("Up Down Game Objects").transform;
        Move = GetComponent<PlayerInput>().actions["UpDown"];
        StartCoroutine(Spawning());
    }

    // Update is called once per frame
    void Update()
    {
        MoveCube();

        float closestDistance = 9999f;
        foreach (GameObject spawnedObstacle in GameObject.FindGameObjectsWithTag(OBSTACLE_TAG))
        {
            float dist = Vector3.Distance(transform.position, spawnedObstacle.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
            }
        }
        spotLight.colorTemperature = Utils.MapFloat(closestDistance, noDangerDistance, 0, lightTempDangerMin, lightTempDangerMax);

        //Dont actually use this, just test code for controlling cameras
        //Use something similar to this in the overall game controller
        //if (gameFailed)
        //{
        //    gameCamera.rect = new Rect(Mathf.Lerp(gameCamera.rect.x, 0, Time.deltaTime * sizeChangeSpeed), 0, .5f, Mathf.Lerp(gameCamera.rect.height, .5f, Time.deltaTime * sizeChangeSpeed));
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(OBSTACLE_TAG))
        {
            Debug.Log("UpDown FAILED");
            gameFailed = true;
        }
    }

    IEnumerator Spawning()
    {
        yield return new WaitForSeconds(secondsBetweenSpawns);
        int randomDirection = (Random.Range(0, 2) * 2 - 1);
        Vector3 spawnPoint = new Vector3(spawnPointPadding * randomDirection, transform.localPosition.y + Random.Range(-spawnFuzziness, spawnFuzziness), initialZ) + gameCenter.localPosition;
        GameObject spawned = Instantiate(obstacle, spawnPoint, gameCenter.localRotation, gameCenter);
        UpDownObstacle ob = spawned.GetComponent<UpDownObstacle>();
        ob.setDeleteDistance((spawnPointPadding + 1) * randomDirection);
        ob.setSpeed(obstacleSpeed);
        StartCoroutine(Spawning());
    }

    void MoveCube()
    {
        Vector3 newPosition = transform.localPosition + new Vector3(transform.localPosition.x, Move.ReadValue<float>() * moveSpeedMultiplier, transform.localPosition.z);
        if (Mathf.Abs(newPosition.y) >= maxHeight)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, maxHeight * (newPosition.y / Mathf.Abs(newPosition.y)), transform.localPosition.z);
        }
        else
        {
            transform.localPosition = newPosition;
        }

        transform.Rotate(Vector3.left, rotateX);
        transform.Rotate(Vector3.up, rotateY);
        transform.Rotate(Vector3.forward, rotateZ);
    }
}
