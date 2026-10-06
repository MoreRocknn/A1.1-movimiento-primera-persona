using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;

    public float maxfuel = 4f;
    public float thrustForce = 8f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    private float curfuel;

    void Start()
    {
        curfuel = maxfuel;
    }

    void Update()
    {
        // Comprobar si estamos en el suelo
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        // Si estamos en el suelo
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Movimiento
        float X = Input.GetAxis("Horizontal");
        float Z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * X + transform.forward * Z;

        controller.Move(move * speed * Time.deltaTime);

        // Salto normal
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(-2f * gravity);
        }

        // Jetpack
        if (Input.GetKey(KeyCode.Space) && curfuel > 0)
        {
            velocity.y = thrustForce;
            curfuel -= Time.deltaTime;
        }

        // Recargar combustible cuando estamos en el suelo
        if (isGrounded)
        {
            curfuel += Time.deltaTime;
            curfuel = Mathf.Clamp(curfuel, 0, maxfuel);
        }

        // Gravedad
        velocity.y += gravity * Time.deltaTime;

        // Movimiento vertical
        controller.Move(velocity * Time.deltaTime);
    }
}