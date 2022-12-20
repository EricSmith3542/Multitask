using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class BalanceBoard : MonoBehaviour
{
    private PlayerInput PlayerInput;
    private Rigidbody Rigidbody;
    private InputAction Tilt;

    [Range(0.01f, 1)]
    public float rotationSpeedMultiplier = .5f;

    // Start is called before the first frame update
    void Start()
    {
        Tilt = GetComponent<PlayerInput>().actions["Tilt"];
        Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody.AddTorque(new Vector3(0, 0, Tilt.ReadValue<float>() * rotationSpeedMultiplier), ForceMode.VelocityChange);
    }
}
