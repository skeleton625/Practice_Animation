using UnityEngine.InputSystem;
using UnityEngine;

public class Test_2DBlend : MonoBehaviour
{
    [Header("Controller Components")]
    [SerializeField] private Animator avatar_Animator = null;
    [SerializeField] private Rigidbody avatar_Rigidbody = null;

    [Header("Controller Velocity")]
    [SerializeField] private float MovementScale = 10f;
    [SerializeField] private float Acceleration = .1f;
    [SerializeField] private float Deceleration = .5f;
    [SerializeField] private float WalkMaxVelocity = .5f;
    [SerializeField] private float RunMaxVelocity = 2f;

    private bool forwardMoving = false;
    private bool backwardMoving = false;
    private bool leftsideMoving = false;
    private bool rightsideMoving = false;
    private bool boostSwitching = false;

    private int velocityHash_X = 0;
    private int velocityHash_Z = 0;

    private float currentMaxVelocity = 0f;
    private Vector2 inputVector = Vector2.zero;
    private Vector3 velocity_value = Vector3.zero;

    private void Start()
    {
        velocityHash_X = Animator.StringToHash("Velocity_X");
        velocityHash_Z = Animator.StringToHash("Velocity_Z");

        currentMaxVelocity = WalkMaxVelocity;
    }

    private void Update()
    {
        if (forwardMoving && velocity_value.z < currentMaxVelocity) velocity_value.z += Time.deltaTime * Acceleration;
        if (backwardMoving && velocity_value .z > -.5f) velocity_value.z -= Time.deltaTime * Acceleration;

        if (backwardMoving)
        {
            if (rightsideMoving)
            {
                if (velocity_value.x < .5f) velocity_value.x += Time.deltaTime * Acceleration;
                else velocity_value.x -= Time.deltaTime * Acceleration;
            }
            if (leftsideMoving)
            {
                if (velocity_value.x > -.5f) velocity_value.x -= Time.deltaTime * Acceleration;
                else velocity_value.x += Time.deltaTime * Acceleration;
            }
        }
        else
        {
            if (rightsideMoving && velocity_value.x < currentMaxVelocity) velocity_value.x += Time.deltaTime * Acceleration;
            if (leftsideMoving && velocity_value.x > -currentMaxVelocity) velocity_value.x -= Time.deltaTime * Acceleration;
        }

        if (!forwardMoving && velocity_value.z > 0) velocity_value.z -= Time.deltaTime * Deceleration;
        if (!backwardMoving && velocity_value.z < 0) velocity_value.z += Time.deltaTime * Deceleration;
        if (!rightsideMoving && velocity_value.x > 0) velocity_value.x -= Time.deltaTime * Deceleration;
        if (!leftsideMoving && velocity_value.x < 0) velocity_value.x += Time.deltaTime * Deceleration;

        if (boostSwitching && !backwardMoving)
        {
            if (forwardMoving)
            {
                if (velocity_value.z > currentMaxVelocity) velocity_value.z -= Time.deltaTime * Deceleration;
                else boostSwitching = false;
            }
            if (leftsideMoving)
            {
                if (velocity_value.x < -currentMaxVelocity) velocity_value.x += Time.deltaTime * Deceleration;
                else boostSwitching = false;
            }
            if (rightsideMoving)
            {
                if (velocity_value.x > currentMaxVelocity) velocity_value.x -= Time.deltaTime * Deceleration;
                else boostSwitching = false;
            }
        }

        avatar_Rigidbody.velocity = velocity_value * MovementScale;
        avatar_Animator.SetFloat(velocityHash_X, velocity_value.x);
        avatar_Animator.SetFloat(velocityHash_Z, velocity_value.z);
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
