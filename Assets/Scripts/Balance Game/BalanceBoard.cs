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

    [Range(0.01f, 1)]
    public float rotationSpeedMultiplier = .5f;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        Tilt = GetComponent<PlayerInput>().actions["Tilt"];
        Rigidbody = GetComponent<Rigidbody>();
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

    // Update is called once per frame
    void FixedUpdate()
    {
        Rigidbody.AddTorque(new Vector3(0, 0, Tilt.ReadValue<float>() * rotationSpeedMultiplier), ForceMode.VelocityChange);
    }

    //This is the result of bad design on my end. I didnt fully think about how I was going to structure the DangerDetection until after the creation of this minigame
    public override Vector2 DetectDanger()
    {
        throw new System.NotImplementedException();
    }
}
