using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class Jumper : MiniGame
{
    private const string OBSTACLE_TAG = "JumpObstacle";

    private InputAction Jump;
    private Rigidbody rb;
    private bool failed = false;


    [Header("Jump Settings")]
    [SerializeField]
    private JumpGameController controller;
    [SerializeField]
    private float maxJumpForce = 1f;
    [SerializeField]
    private float maxJumpHeight = 9.5f;


    new void Start()
    {
        base.Start();
        maxJumpHeight = transform.position.y + maxJumpHeight;
        Jump = GetComponent<PlayerInput>().actions["Jump"];
        rb = GetComponent<Rigidbody>();
    }

    new void Update()
    {
        base.Update();
        //Method for moving up while pressing Jump button
        JumpIfJumping();
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
        if(other.tag == OBSTACLE_TAG && !failed)
        {
            Debug.Log("Jump Fail");
            failed = true;
            EndGame();
        }
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

    private void JumpIfJumping()
    {
        if (transform.position.y > maxJumpHeight)
        {
            // Set the player's position to the maximum jump height
            transform.position = new Vector3(transform.position.x, maxJumpHeight, transform.position.z);

            // Stop the player from jumping any higher
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }
}
