using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseGame : MonoBehaviour
{
    private const string COLLECTABLE_TAG = "MouseCollect";
    private InputAction MouseMove;
    private float minX, maxX, minY, maxY;
    private float spawnTimer;

    public float sensitivity = 1f;
    public float xPadding = 6f;
    public float yPadding = 3f;
    public float secondsPerSpawn = 2f;
    [SerializeField]
    private Camera gameCamera;
    [SerializeField]
    private GameObject collectable;

    // Start is called before the first frame update
    void Start()
    {
        MouseMove = GetComponent<PlayerInput>().actions["MouseMove"];
        minX = transform.position.x - xPadding;
        maxX = transform.position.x + xPadding;
        minY = transform.position.y - yPadding;
        maxY = transform.position.y + yPadding;
        spawnTimer = secondsPerSpawn;

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = MouseMove.ReadValue<Vector2>() * Time.deltaTime * sensitivity;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x + move.x, minX, maxX), Mathf.Clamp(transform.position.y + move.y, minY, maxY), 0);

        if (spawnTimer <= 0)
        {
            SpawnCollectable();
            spawnTimer = secondsPerSpawn;
        }
        else
        {
            spawnTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == COLLECTABLE_TAG)
        {
            Destroy(other.gameObject);
        }
    }

    private void SpawnCollectable()
    {
        Instantiate(collectable, new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), transform.position.z), Quaternion.identity);
    }
}
