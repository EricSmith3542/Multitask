using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class Jumper : MonoBehaviour
{
    private const string OBSTACLE_TAG = "JumpObstacle";
    
    private InputAction Jump;
    private float maxJumpHeight;
    private Rigidbody rb;


    [Header("Jump Settings")]
    [SerializeField]
    private JumpGameController controller;
    public float maxJumpForce = 1f;

    [Header("Danger Indicator Controls")]
    [SerializeField]
    private Light spotLight;
    private HDAdditionalLightData hdSpotLight;
    public float lightTempDangerMin = 6900;
    public float lightTempDangerMax = 1500;
    public float lightIntensityMin = 400;
    public float lightIntensityMax = 1100;
    public float noDangerDistance = 2f;

    void Start()
    {
        Jump = GetComponent<PlayerInput>().actions["Jump"];
        maxJumpHeight = transform.position.y + 5;
        rb = GetComponent<Rigidbody>();
        hdSpotLight = spotLight.GetComponent<HDAdditionalLightData>();
    }

    private void Update()
    {
        if (transform.position.y > maxJumpHeight)
        {
            // Set the player's position to the maximum jump height
            transform.position = new Vector3(transform.position.x, maxJumpHeight, transform.position.z);

            // Stop the player from jumping any higher
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }

        float closestDistance = 9999f;
        foreach (GameObject spawnedObstacle in GameObject.FindGameObjectsWithTag(OBSTACLE_TAG))
        {
            float dist = Vector3.Distance(transform.position, spawnedObstacle.GetComponent<Collider>().ClosestPoint(transform.position));
            if (dist < closestDistance)
            {
                closestDistance = dist;
            }
        }
        spotLight.colorTemperature = Utils.MapFloat(Mathf.Min(closestDistance, noDangerDistance), noDangerDistance, 0, lightTempDangerMin, lightTempDangerMax);
        hdSpotLight.intensity = Utils.MapFloat(Mathf.Min(closestDistance, noDangerDistance), noDangerDistance, 0, lightIntensityMin, lightIntensityMax);
    }

    void FixedUpdate()
    {
        if (Jump.IsPressed())
        {
            rb.AddForce(Vector3.up * maxJumpForce, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == OBSTACLE_TAG)
        {
            controller.fail();
        }
    }
}
