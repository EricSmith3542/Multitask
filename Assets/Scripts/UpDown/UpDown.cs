using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(PlayerInput))]
public class UpDown : MiniGame
{
    private string OBSTACLE_TAG = "UpDownObstacle";

    private InputAction Move;
    private Transform gameCenter;
    private float initialZ;
    private bool failed = false;

    public float moveSpeedMultiplier = .5f;
    public float maxHeight = 3.65f;

    public float rotateX = .1f;
    public float rotateY = .1f;
    public float rotateZ = .1f;

    [Header("Camera")]
    [SerializeField]
    private Camera gameCamera;

    [Header("Spawner Controls")]
    [SerializeField]
    private GameObject obstacle;
    public float spawnPointPadding = 5f;
    public float spawnFuzziness = 1f;
    public float secondsBetweenSpawns = 3f;
    public float obstacleSpeed = .1f;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        initialZ = transform.position.z;
        gameCenter = transform.parent;
        Move = GetComponent<PlayerInput>().actions["UpDown"];
        StartCoroutine(Spawning());
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        MoveCube();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(OBSTACLE_TAG) && !failed)
        {
            Debug.Log("UpDown FAILED");
            failed = true;
            EndGame();
        }
    }

    IEnumerator Spawning()
    {
        yield return new WaitForSeconds(secondsBetweenSpawns);

        float cameraRatio = gameCamera.rect.width / gameCamera.rect.height;
        int randomDirection = (Random.Range(0, 2) * 2 - 1);
        Vector3 spawnPoint = new Vector3(spawnPointPadding * randomDirection * cameraRatio, transform.localPosition.y + Random.Range(-spawnFuzziness, spawnFuzziness), initialZ) + gameCenter.position;
        GameObject spawned = Instantiate(obstacle, spawnPoint, gameCenter.localRotation, gameCenter);
        UpDownObstacle ob = spawned.GetComponent<UpDownObstacle>();
        ob.setDeleteDistance(((spawnPointPadding * cameraRatio) + 1) * randomDirection);
        ob.setSpeed(obstacleSpeed);

        StartCoroutine(Spawning());
    }

    void MoveCube()
    {
        Vector3 newPosition = transform.localPosition + new Vector3(transform.localPosition.x, Move.ReadValue<float>() * moveSpeedMultiplier * Time.timeScale, transform.localPosition.z);
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

    public override Vector2 DetectDanger()
    {
        float closestDistance = 9999f;
        foreach (GameObject spawnedObstacle in GameObject.FindGameObjectsWithTag(OBSTACLE_TAG))
        {
            float dist = Vector3.Distance(transform.position, spawnedObstacle.GetComponent<Collider>().ClosestPoint(transform.position));
            if (dist < closestDistance)
            {
                closestDistance = dist;
            }
        }
        float newTemp = Utils.MapFloat(Mathf.Min(closestDistance, noDangerDistance), noDangerDistance, 0, lightTempDangerMin, lightTempDangerMax);
        float newIntensity = Utils.MapFloat(Mathf.Min(closestDistance, noDangerDistance), noDangerDistance, 0, lightIntensityMin, lightIntensityMax);

        return new Vector2(newTemp, newIntensity);
    }
}
