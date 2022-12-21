using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGameController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject floorAndWall;
    [SerializeField]
    private GameObject tallObject;
    [SerializeField]
    private GameObject shortObject;

    [Header("Game Controls")]
    public float gameSpeed = 1f;


    private Vector3 gameSpeedVector;
    private float secondsBetweenSpawns;
    private float spawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        //TODO: This is mega jank, fix later lol
        secondsBetweenSpawns = (floorAndWall.GetComponent<Transform>().GetChild(0).GetComponent<Transform>().localScale.x * 10 / gameSpeed) - .05f;
        spawnTimer = secondsBetweenSpawns;
        //Create the second floor/wall set and make the beginning and end walls move
        Transform startFloor = transform.GetChild(0);
        GameObject spawnedFloor = Instantiate(floorAndWall, transform.position, Quaternion.identity, transform);

        gameSpeedVector = new Vector3(-gameSpeed, 0, 0);

        startFloor.GetComponent<Rigidbody>().velocity = gameSpeedVector;
        spawnedFloor.GetComponent<Rigidbody>().velocity = gameSpeedVector;


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(spawnTimer <= 0)
        {
            SpawnFloor();
            spawnTimer = secondsBetweenSpawns;
        }
        else
        {
            spawnTimer -= Time.deltaTime;
        }
    }

    void SpawnFloor()
    {
        GameObject spawnedFloor = Instantiate(floorAndWall, transform.position, Quaternion.identity, transform);
        spawnedFloor.GetComponent<Rigidbody>().velocity = gameSpeedVector;
    }
}
