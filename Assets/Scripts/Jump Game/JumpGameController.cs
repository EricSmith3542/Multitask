using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGameController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject floorAndWall;
    [SerializeField]
    private GameObject obstacle;

    [Header("Game Controls")]
    public float gameSpeed = 1f;
    public float secondsPerObstacle = 1f;
    public float bottomSpawnYPadding = .5f;
    public float topSpawnYPadding = 3.5f;
    public float middleSpawnYPadding = 1.5f;


    private Vector3 gameSpeedVector, bottomVector, topVector, middleVector;
    private float secondsBetweenSpawns;
    private float obstacleTimer;

    // Start is called before the first frame update
    void Start()
    {
        //TODO: This is mega jank, fix later lol
        secondsBetweenSpawns = (floorAndWall.GetComponent<Transform>().GetChild(0).GetComponent<Transform>().localScale.x * 10 / gameSpeed) - .05f;
        obstacleTimer = secondsPerObstacle;
        gameSpeedVector = new Vector3(-gameSpeed, 0, 0);
        bottomVector = Vector3.up * bottomSpawnYPadding;
        topVector = Vector3.up * topSpawnYPadding;
        middleVector = Vector3.up * middleSpawnYPadding;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(obstacleTimer <= 0)
        {
            SpawnObstacle();
            obstacleTimer = secondsPerObstacle;
        }
        else
        {
            obstacleTimer -= Time.deltaTime;
        }
    }

    void SpawnFloor()
    {
        GameObject spawnedFloor = Instantiate(floorAndWall, transform.position, Quaternion.identity, transform);
        spawnedFloor.GetComponent<Rigidbody>().velocity = gameSpeedVector;
    }

    void SpawnObstacle()
    {
        Vector3 spawnPos = transform.position + topVector;
        int rand = Random.Range(0, 3);
        if (rand == 0)
        {
            spawnPos = transform.position + bottomVector;
        }
        else if(rand == 1)
        {
            spawnPos = transform.position + middleVector;
        }
        GameObject spawnedObstacle = Instantiate(obstacle, spawnPos, Quaternion.identity, transform);
        spawnedObstacle.GetComponent<Rigidbody>().velocity = gameSpeedVector;
    }

}
