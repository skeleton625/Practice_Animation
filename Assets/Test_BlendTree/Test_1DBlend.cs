using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Test_1DBlend : MonoBehaviour
{
    [SerializeField] private float acceleration = .1f;
    [SerializeField] private float deceleration = .5f;

    private bool isAcceleration = false;

    private int velocityHash = 0;
    private float avatar_velocity = 0f;
    private Animator avatar_animator = null;

    private void Start()
    {
        avatar_animator = GetComponent<Animator>();

        velocityHash = Animator.StringToHash("Velocity");
    }

    private void Update()
    {
        if (isAcceleration)
        {
            avatar_velocity = Mathf.Clamp01(avatar_velocity + Time.deltaTime * acceleration);
        }
        else
        {
            avatar_velocity = Mathf.Clamp01(avatar_velocity - Time.deltaTime * deceleration);
        }

        avatar_animator.SetFloat(velocityHash, avatar_velocity);
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        isAcceleration = input.y > 0;
    }
}
