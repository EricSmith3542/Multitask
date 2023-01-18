using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class BalanceBoard : MiniGame
{
    private const float failDistance = 3f;
    private PlayerInput PlayerInput;
    private Rigidbody Rigidbody;
    private InputAction Tilt;
    private bool failed = false;

    [SerializeField]
    private GameObject ball;

    [Header("Movement Controls")]
    [SerializeField] private float rotationSpeedMultiplier = .5f;
    [SerializeField] private float maxAngularVelocity = 1f;
    [SerializeField] private float nudgeVelocity = 0.01f;
    private float currentSpeedMultiplier = 0f;
    private Vector3 velocityDirection;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        Tilt = GetComponent<PlayerInput>().actions["Tilt"];
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.maxAngularVelocity = maxAngularVelocity;
    }

    new private void Update()
    {
        if(ball.transform.position.y < transform.position.y - failDistance && !failed)
        {
            Debug.Log("Balance Fail");
            failed = true;
            EndGame();
        }
    }

    void FixedUpdate()
    {
        velocityDirection = Rigidbody.velocity.normalized;
        if (Rigidbody.velocity.magnitude == 0)
        {
            Rigidbody.velocity = velocityDirection * nudgeVelocity;
        }

        //if(Tilt.ReadValue<float>() == 0)
        //{
        //    currentSpeedMultiplier = Mathf.Max(Mathf.Lerp(currentSpeedMultiplier, 0, Time.deltaTime), 0);
        //}
        //else
        //{
        //    currentSpeedMultiplier += Mathf.Min(Mathf.Lerp(currentSpeedMultiplier, rotationSpeedMultiplier, Time.deltaTime), rotationSpeedMultiplier);
        //}


        Rigidbody.AddTorque(new Vector3(0, 0, Tilt.ReadValue<float>() * rotationSpeedMultiplier), ForceMode.Force);
    }

    //This is the result of bad design on my end. I didnt fully think about how I was going to structure the DangerDetection until after the creation of this minigame
    public override Vector2 DetectDanger()
    {
        throw new System.NotImplementedException();
    }
}
