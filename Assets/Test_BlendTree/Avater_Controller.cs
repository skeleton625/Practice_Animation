using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Avater_Controller : MonoBehaviour
{
    [SerializeField] private float acceleration = .1f;
    [SerializeField] private float deceleration = .5f;

    private bool isAcceleration = false;

    private int velocityHash = 0;
    private float avater_velocity = 0f;
    private Animator avater_animator = null;

    private void Start()
    {
        avater_animator = GetComponent<Animator>();

        velocityHash = Animator.StringToHash("Velocity");
    }

    private void Update()
    {
        if (isAcceleration)
        {
            avater_velocity = Mathf.Clamp01(avater_velocity + Time.deltaTime * acceleration);
        }
        else
        {
            avater_velocity = Mathf.Clamp01(avater_velocity - Time.deltaTime * deceleration);
        }

        avater_animator.SetFloat(velocityHash, avater_velocity);
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        isAcceleration = input.y > 0;
    }
}
