using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGameController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField]
    private Camera gameCamera;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject floorAndWall;
    [SerializeField]
    private GameObject obstacle;

    [Header("Game Controls")]
    [SerializeField] private float gameSpeed = 1f;
    [SerializeField] private float secondsPerObstacle = 1f;
    [SerializeField] private float bottomSpawnYPadding = .5f;
    [SerializeField] private float topSpawnYPadding = 3.5f;
    [SerializeField] private float middleSpawnYPadding = 1.5f;
    [SerializeField] private float horizontalSpawnPadding = 10f;
    [Range(0,1)]
    [SerializeField] private float doubleObstacleChance = .1f;


    private Vector3 gameSpeedVector;
    private float obstacleTimer;
    private float initialZ;
    private Transform gameCenter;

    // Start is called before the first frame update
    void Start()
    {
        gameSpeedVector = new Vector3(-gameSpeed, 0, 0);
        initialZ = transform.position.z;
        gameCenter = transform.parent;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (obstacleTimer <= 0)
        {
            SpawnObstacle();
            obstacleTimer = secondsPerObstacle;
        }
        else
        {
            obstacleTimer -= Time.deltaTime;
        }
    }

    void SpawnObstacle()
    {
        float spawnHeight = topSpawnYPadding;
        float otherHeight1 = bottomSpawnYPadding;
        float otherHeight2 = middleSpawnYPadding;
        int rand = Random.Range(0, 3);
        if (rand == 0)
        {
            spawnHeight = bottomSpawnYPadding;
            otherHeight1 = topSpawnYPadding;
            otherHeight2 = middleSpawnYPadding;
        }
        else if (rand == 1)
        {
            spawnHeight = middleSpawnYPadding;
            otherHeight1 = topSpawnYPadding;
            otherHeight2 = bottomSpawnYPadding;
        }

        float cameraRatio = gameCamera.rect.width / gameCamera.rect.height;
        Vector3 spawnPoint = new Vector3(horizontalSpawnPadding * cameraRatio, spawnHeight, initialZ) + gameCenter.position;
        GameObject spawnedObstacle = Instantiate(obstacle, spawnPoint, Quaternion.identity, transform);
        spawnedObstacle.GetComponent<Rigidbody>().velocity = gameSpeedVector;

        if(doubleObstacleChance >= Random.Range(0f, 1f))
        {
            spawnHeight = Random.Range(0f, 1f) > .5f ? otherHeight1 : otherHeight2;
            spawnPoint = new Vector3(horizontalSpawnPadding * cameraRatio, spawnHeight, initialZ) + gameCenter.position;
            spawnedObstacle = Instantiate(obstacle, spawnPoint, Quaternion.identity, transform);
            spawnedObstacle.GetComponent<Rigidbody>().velocity = gameSpeedVector;
        }
    }

}
