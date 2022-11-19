using UnityEngine.InputSystem;
using UnityEngine;
using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class Test_2DBlend : MonoBehaviour
{
    [Header("Input System Components")]
    [SerializeField] private PlayerInput avatar_MoveInput = null;
    [SerializeField] private PlayerInput avatar_RotateInput = null;

    [Header("Controller Components")]
    [SerializeField] private Animator avatar_Animator = null;
    [SerializeField] private Rigidbody avatar_Rigidbody = null;
    [SerializeField] private Transform avatar_CharBody = null;
    [SerializeField] private Transform avatar_CameraBody = null;

    [Header("Controller Velocity")]
    [SerializeField] private float RotateScale = 10f;
    [SerializeField] private float MovementScale = 10f;
    [SerializeField] private float Acceleration = .1f;
    [SerializeField] private float Deceleration = .5f;
    [SerializeField] private float WalkMaxVelocity = .5f;
    [SerializeField] private float RunMaxVelocity = 2f;

    private bool IsMoving = false;
    private bool forwardMoving = false;
    private bool backwardMoving = false;
    private bool leftsideMoving = false;
    private bool rightsideMoving = false;
    private bool boostSwitching = false;

    private int velocityHash_X = 0;
    private int velocityHash_Z = 0;

    private float currentMaxVelocity = 0f;
    private Vector2 moveVector = Vector2.zero;
    private Vector2 rotateVector = Vector2.zero;
    private Vector3 velocity_value = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;

    private void Start()
    {
        velocityHash_X = Animator.StringToHash("Velocity_X");
        velocityHash_Z = Animator.StringToHash("Velocity_Z");

        currentMaxVelocity = WalkMaxVelocity;

        moveVector = Vector2.zero;
        rotateVector = Vector2.zero;
        velocity_value = Vector3.zero;
        cameraRotation = Vector3.zero;

        avatar_MoveInput.enabled = true;
        avatar_RotateInput.enabled = true;
    }

    private void Update()
    {
        // MOVEMENT
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

        // ROTATE
        cameraRotation.y = Mathf.Repeat(cameraRotation.y + rotateVector.y * Time.deltaTime * RotateScale, 360);

        var velocity = avatar_CameraBody.forward * velocity_value.z + avatar_CameraBody.right * velocity_value.x;
        if (IsMoving) avatar_CharBody.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
        avatar_CameraBody.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
        avatar_Rigidbody.velocity = velocity * MovementScale;

        avatar_Animator.SetFloat(velocityHash_X, velocity_value.x);
        avatar_Animator.SetFloat(velocityHash_Z, velocity_value.z);
    }

    public void OnMove(InputValue value)
    {
        moveVector = value.Get<Vector2>();
        IsMoving = !moveVector.Equals(Vector2.zero);

        forwardMoving = moveVector.y > 0;
        backwardMoving = moveVector.y < 0;

        rightsideMoving = moveVector.x > 0;
        leftsideMoving = moveVector.x < 0;
    }

    public void OnRotate(InputContext value)
    {
        var vector = value.ReadValue<Vector2>();
        rotateVector.x = -vector.y;
        rotateVector.y = vector.x;
    }

    public void OnBoost(InputValue value)
    {
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
