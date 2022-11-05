using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Test_2DBlend : MonoBehaviour
{
    [SerializeField] private float Acceleration = .1f;
    [SerializeField] private float Deceleration = .5f;
    [SerializeField] private float WalkMaxVelocity = .5f;
    [SerializeField] private float RunMaxVelocity = 2f;

    private Animator avatar_Animator = null;

    private bool forwardMoving = false;
    private bool backwardMoving = false;
    private bool leftsideMoving = false;
    private bool rightsideMoving = false;
    private bool boostSwitching = false;

    private int velocityHash_X = 0;
    private int velocityHash_Z = 0;

    private float currentMaxVelocity = 0f;
    private float velocity_x = 0f;
    private float velocity_z = 0f;
    private Vector2 inputVector = Vector2.zero;

    private void Start()
    {
        avatar_Animator = GetComponent<Animator>();

        velocityHash_X = Animator.StringToHash("Velocity_X");
        velocityHash_Z = Animator.StringToHash("Velocity_Z");

        currentMaxVelocity = WalkMaxVelocity;
    }

    private void Update()
    {
        if (forwardMoving && velocity_z < currentMaxVelocity) velocity_z += Time.deltaTime * Acceleration;
        if (backwardMoving && velocity_z > -.5f) velocity_z -= Time.deltaTime * Acceleration;

        if (backwardMoving)
        {
            if (rightsideMoving)
            {
                if (velocity_x < .5f) velocity_x += Time.deltaTime * Acceleration;
                else velocity_x -= Time.deltaTime * Acceleration;
            }
            if (leftsideMoving)
            {
                if (velocity_x > -.5f) velocity_x -= Time.deltaTime * Acceleration;
                else velocity_x += Time.deltaTime * Acceleration;
            }
        }
        else
        {
            if (rightsideMoving && velocity_x < currentMaxVelocity) velocity_x += Time.deltaTime * Acceleration;
            if (leftsideMoving && velocity_x > -currentMaxVelocity) velocity_x -= Time.deltaTime * Acceleration;
        }

        if (!forwardMoving && velocity_z > 0) velocity_z -= Time.deltaTime * Deceleration;
        if (!backwardMoving && velocity_z < 0) velocity_z += Time.deltaTime * Deceleration;
        if (!rightsideMoving && velocity_x > 0) velocity_x -= Time.deltaTime * Deceleration;
        if (!leftsideMoving && velocity_x < 0) velocity_x += Time.deltaTime * Deceleration;

        if (boostSwitching && !backwardMoving)
        {
            if (forwardMoving)
            {
                if (velocity_z > currentMaxVelocity) velocity_z -= Time.deltaTime * Deceleration;
                else boostSwitching = false;
            }
            if (leftsideMoving)
            {
                if (velocity_x < -currentMaxVelocity) velocity_x += Time.deltaTime * Deceleration;
                else boostSwitching = false;
            }
            if (rightsideMoving)
            {
                if (velocity_x > currentMaxVelocity) velocity_x -= Time.deltaTime * Deceleration;
                else boostSwitching = false;
            }
        }

        avatar_Animator.SetFloat(velocityHash_X, velocity_x);
        avatar_Animator.SetFloat(velocityHash_Z, velocity_z);
    }

    public void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();

        forwardMoving = inputVector.y > 0;
        backwardMoving = inputVector.y < 0;
        rightsideMoving = inputVector.x > 0;
        leftsideMoving = inputVector.x < 0;
    }

    public void OnBoost(InputValue value)
    {
        Debug.Log(value.isPressed);

        if (value.isPressed)
        {
            currentMaxVelocity = RunMaxVelocity;
            boostSwitching = false;
        }
        else
        {
            currentMaxVelocity = WalkMaxVelocity;
            boostSwitching = true;
        }
    }
}
