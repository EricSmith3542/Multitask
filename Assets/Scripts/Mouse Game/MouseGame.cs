using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

//TODO: Refactor this class to inherit the MiniGame class
//Need to think through how this script could keep track of the obstacles spawned and set the danger according to the one that has been alive the longest
public class MouseGame : MonoBehaviour
{
    private const string COLLECTABLE_TAG = "MouseCollect";
    private InputAction MouseMove;
    private float minX, maxX, minY, maxY;
    private float spawnTimer;

    public float sensitivity = 1f;
    public float xPadding = 6f;
    public float yPadding = 3f;

    private float cameraScaledMinX;
    private float cameraScaledMaxX;

    [SerializeField]
    private Camera gameCamera;

    [Header("Danger Indicator Controls")]
    [SerializeField] private Light spotLight;
    private HDAdditionalLightData hdSpotLight;
    public float lightTempDangerMin = 6900;
    public float lightTempDangerMax = 1500;
    public float lightIntensityMin = 400;
    public float lightIntensityMax = 1100;

    [Header("Bomb Spawn Configuration")]
    [SerializeField] private GameObject collectable;
    [SerializeField] private GameObject bombHolder;
    public float secondsPerSpawn = 2f;
    private bool firstBomb = true;

    // Start is called before the first frame update
    void Start()
    {
        MouseMove = GetComponent<PlayerInput>().actions["MouseMove"];
        hdSpotLight = spotLight.GetComponent<HDAdditionalLightData>();
        minX = transform.position.x - xPadding;
        maxX = transform.position.x + xPadding;
        minY = transform.position.y - yPadding;
        maxY = transform.position.y + yPadding;
        spawnTimer = secondsPerSpawn;

    }

    // Update is called once per frame
    void Update()
    {
        float cameraRatio = (gameCamera.rect.width / gameCamera.rect.height);
        cameraScaledMinX = minX * cameraRatio;
        cameraScaledMaxX = maxX * cameraRatio;

        Vector2 move = MouseMove.ReadValue<Vector2>() * Time.deltaTime * sensitivity;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x + move.x, cameraScaledMinX, cameraScaledMaxX), Mathf.Clamp(transform.position.y + move.y, minY, maxY), 0);

        if (spawnTimer <= 0)
        {
            SpawnCollectable();
            spawnTimer = secondsPerSpawn;
        }
        else
        {
            spawnTimer -= Time.deltaTime;
        }

        if(bombHolder.transform.childCount == 0)
        {
            firstBomb = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == COLLECTABLE_TAG)
        {
            Destroy(other.gameObject);
            spotLight.colorTemperature = lightTempDangerMin;
            hdSpotLight.intensity = lightIntensityMin;
        }
    }

    private void SpawnCollectable()
    {
        GameObject ob = Instantiate(collectable, new Vector3(Random.Range(cameraScaledMinX, cameraScaledMaxX), Random.Range(minY, maxY), transform.position.z), Quaternion.identity, bombHolder.transform);
        ob.GetComponent<MouseGameBomb>().attachLight(spotLight, lightTempDangerMin, lightTempDangerMax, lightIntensityMin, lightIntensityMax);
        if (firstBomb)
        {
            ob.GetComponent<MouseGameBomb>().SetAsLightController();
            firstBomb = false;
        }
    }
}
