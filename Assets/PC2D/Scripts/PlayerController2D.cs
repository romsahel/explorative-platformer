
using UnityEngine;

/// <summary>
/// This class is a simple example of how to build a controller that interacts with PlatformerMotor2D.
/// </summary>
[RequireComponent(typeof(PlatformerMotor2D))]
[RequireComponent(typeof(AudioSource))]

public class PlayerController2D : MonoBehaviour
{
    private PlatformerMotor2D _motor;
    private bool enableInput;

    // Use this for initialization
    void Start()
    {
		_motor = GetComponent<PlatformerMotor2D>();
        enableInput = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (enableInput && Mathf.Abs(Input.GetAxis(PC2D.Input.HORIZONTAL)) > PC2D.Globals.INPUT_THRESHOLD)
        {
            _motor.normalizedXMovement = Input.GetAxis(PC2D.Input.HORIZONTAL);
        }
        else
        {
            _motor.normalizedXMovement = 0;
        }

        // Jump?
        if (enableInput && Input.GetKeyDown(KeyCode.UpArrow))
        {
            _motor.Jump();
        }

        _motor.jumpingHeld = Input.GetKey(KeyCode.UpArrow);

        if (enableInput && Input.GetAxis(PC2D.Input.VERTICAL) < -PC2D.Globals.FAST_FALL_THRESHOLD)
        {
            _motor.fallFast = true;
        }
        else
        {
            _motor.fallFast = false;
        }

        if (enableInput && Input.GetButtonDown(PC2D.Input.DASH))
        {
            _motor.Dash();
        }
    }

    public void enable(bool enabled)
    {
        this.enableInput = enabled;
        _motor.normalizedXMovement = 0;
    }
}
