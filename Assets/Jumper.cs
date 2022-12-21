using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    void Start()
    {
        Jump = GetComponent<PlayerInput>().actions["Jump"];
        maxJumpHeight = transform.position.y + 5;
        rb = GetComponent<Rigidbody>();
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
