using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class BalanceBoard : MonoBehaviour
{
    private const float failDistance = 3f;
    private PlayerInput PlayerInput;
    private Rigidbody Rigidbody;
    private InputAction Tilt;

    [SerializeField]
    private GameObject ball;

    [Range(0.01f, 1)]
    public float rotationSpeedMultiplier = .5f;

    // Start is called before the first frame update
    void Start()
    {
        Tilt = GetComponent<PlayerInput>().actions["Tilt"];
        Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(ball.transform.position.y < transform.position.y - failDistance)
        {
            Debug.Log("Balance Fail");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Rigidbody.AddTorque(new Vector3(0, 0, Tilt.ReadValue<float>() * rotationSpeedMultiplier), ForceMode.VelocityChange);
    }
}
