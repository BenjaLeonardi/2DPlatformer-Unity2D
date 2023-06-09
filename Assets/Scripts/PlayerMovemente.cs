using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovemente : MonoBehaviour
{
    #region Data and Variables
    [Header("Accelerations Data")]
    [SerializeField] private float acceleration = 225f;
    [SerializeField] private float deceleration = 115f;
    [SerializeField] private float airAcceleration = 0.5f;
    [SerializeField] private float airDeceleration = 3f;
    [Space(5)]

    [Header("Running Data")]
    [SerializeField] public float maxSpeed = 20f;
    [Space(5)]

    [Header("Jumping Data")]
    [SerializeField] private float jumpHeight = 20f;
    [SerializeField] private float jumpTrampMult = 1.25f;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private AudioSource audioJump;
    private float coyoteTimeCounter;
    private bool isJumping = false;
    [Space(5)]

    [Header("Dash Data")]
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float dashTime = 0.2f;
    private Vector2 dashDirection;
    private bool isDashing = false;
    private bool canDash = true;
    [Space]

    [Header("Walljumping Data")]
    [SerializeField] private float wallSlidingSpeed = 5f;
    [SerializeField] private Transform frontCheck;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private float xWallForce = 15f;
    [SerializeField] private float yWallForce = 15f;
    [SerializeField] private float wallJumpTime = 0.1f;
    [SerializeField] private float stickyTimer = 0.4f;
    private float stickyTimerCounter;
    private bool isTouchingFront;
    private bool isWallSliding;
    private bool wallJumping;
    [Space]

    [Header("Space Shift Data")]
    [SerializeField] private AudioSource audioSpaceShift;
    [Space]

    [Header("Player FX")]
    [SerializeField] private ParticleSystem dustParticles;

    private enum MovementState { idle, running, jumping, falling };
    //Lee la data desde el input horizontal y la carga en el float
    private float dirX = 0f;
    private float dirY = 0f;
    private float defaultGravity;
    //Todos los componentes que cargamos desde el mismo player
    private BoxCollider2D collision;
    private Rigidbody2D rbPlayer;
    private Animator animPlayer;
    private SpriteRenderer spritePlayer;
    private TrailRenderer trailPlayer;
    private PlayerMovemente movementScript;
    private bool stickToWalls;
    #endregion

    // Esto es similar a Godot
    private void Start()
    {
        //Aca cargo los componentes a usar del mismo player
        rbPlayer = GetComponent<Rigidbody2D>();
        animPlayer = GetComponent<Animator>();
        spritePlayer = GetComponent<SpriteRenderer>();
        collision = GetComponent<BoxCollider2D>();
        trailPlayer = GetComponent<TrailRenderer>();

        defaultGravity = rbPlayer.gravityScale;
    }

    private void Update()
    {
        // Permite tomar la direccion en caso de que use un joystick
        // GetAxisRaw permite conseguir el 0 inmediatamente, para no tener friccion
        //dirX = Input.GetAxisRaw("Horizontal");
        dirX = SimpleInput.GetAxisRaw("Horizontal");
        dirY = SimpleInput.GetAxisRaw("Vertical");
        var jumpInput = Input.GetButtonDown("Jump");
        var dashInput = Input.GetButtonDown("Dash");
        stickToWalls = Input.GetKey("j");

        if (!isDashing)
        {
            Run();
        }

        if (jumpInput)
        {
            PlayerJump();
        }

        //Solucion temporal, pero me gusto como queda el efecto
        if (Input.GetButtonUp("Jump"))
        {
            rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, rbPlayer.velocity.y / 4);
        }

        if (dashInput && canDash)
        {
            Dash();
        }

        WallGrab();
        Walljumping();
        UpdateAnimation();
        JumpBuffers();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trampoline"))
        {
            //rbPlayer.AddForce(new Vector2(0, jumpHeight * 3), ForceMode2D.Impulse);
            rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, jumpHeight * jumpTrampMult);
        }
    }

    private void Run()
    {
        #region Speed Code
        float targetSpeed = dirX * maxSpeed;
        targetSpeed = Mathf.Lerp(rbPlayer.velocity.x, targetSpeed, 1);
        #endregion

        #region Acceleration Code
        float accelRate;
        if (IsGrounded())
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration);
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f ? acceleration * airAcceleration : airDeceleration * deceleration);
        }
        #endregion

        #region Apply Forces
        //Diferencia entre velocidad que tenemos y la que queremos
        float speedDifference = targetSpeed - rbPlayer.velocity.x;
        //Calculamos el movimiento que vamos a aplicar
        float movement = speedDifference * accelRate * Time.deltaTime;

        //Paso la fuerza que calculamos anteriormente
        rbPlayer.AddForce(movement * Vector2.right, ForceMode2D.Force);

        //rbPlayer.velocity = new Vector2(targetSpeed, rbPlayer.velocity.y);
        #endregion


    }

    private void Walljumping()
    {
        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, jumpableGround);

        //Seteamos el estado que corresponde
        if (isTouchingFront == true && IsGrounded() == false)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        //Caida libre
        if (isWallSliding == true)
        {
            rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, Mathf.Clamp(rbPlayer.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }

        if (Input.GetKeyDown("space") && (isWallSliding == true || stickyTimerCounter > 0f))
        {
            //Para no saltar en el aire dos veces en caso de muchos fps, optimizar
            wallJumping = true;
            Invoke("SetWallJumpingToFalse", wallJumpTime);
            if (isWallSliding)
            {
                rbPlayer.velocity = new Vector2(xWallForce * -transform.localScale.x, yWallForce);
            }
            else
            {
                rbPlayer.velocity = new Vector2(xWallForce * transform.localScale.x, yWallForce);
            }
        }

        //Podria optimizar este codigo
        if (wallJumping == true)
        {
            dustParticles.Play();
        }
    }

    private void SetWallJumpingToFalse()
    {
        wallJumping = false;
    }

    //Arreglado, falta ver colision con bordes
    private void Dash()
    {
        isDashing = true;
        canDash = false;
        trailPlayer.emitting = true;
        dashDirection = new Vector2(dirX, dirY);
        rbPlayer.gravityScale = 0;

        //En caso de que la direccion por input sea nula tenemos que dashear hacia el lado en el que estamos viendo, como en la mayoria de juegos
        if (dashDirection == Vector2.zero)
        {
            dashDirection = new Vector2(transform.localScale.x, 0);
        }

        StartCoroutine(StopDashing());

        if (isDashing)
        {
            rbPlayer.velocity = dashDirection.normalized * dashSpeed;
            return;
        }

    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashTime);
        trailPlayer.emitting = false;
        isDashing = false;
        rbPlayer.gravityScale = defaultGravity;
        rbPlayer.velocity = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    public void PlayerJump()
    {
        if (coyoteTimeCounter > 0f && !isJumping)
        {
            audioJump.Play();
            dustParticles.Play();
            rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, jumpHeight);

            coyoteTimeCounter = 0f;

            StartCoroutine(JumpCooldown());

        }
    }

    private void WallGrab()
    {
        if (stickToWalls && isWallSliding)
        {
            rbPlayer.isKinematic = true;
            rbPlayer.velocity = Vector2.zero;
        }
        else if (!stickToWalls)
        {
            rbPlayer.isKinematic = false;
        }
    }

    private IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(coyoteTime);
        isJumping = false;
    }

    private void JumpBuffers()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (isWallSliding)
        {
            stickyTimerCounter = stickyTimer;
        }
        else
        {
            stickyTimerCounter -= Time.deltaTime;
        }
    }

    private void UpdateAnimation()
    {

        MovementState state;

        //Running
        if (dirX > 0f)
        {
            state = MovementState.running;
            //spritePlayer.flipX = false;
            transform.localScale = new Vector2(1, transform.localScale.y);
            dustParticles.Play();
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            //spritePlayer.flipX = true;
            transform.localScale = new Vector2(-1, transform.localScale.y);
            dustParticles.Play();
        }
        else
        {
            state = MovementState.idle;
        }

        //Jumping
        if (rbPlayer.velocity.y > 0.1f)
        {
            state = MovementState.jumping;
        }
        else if (rbPlayer.velocity.y < -0.1f)
        {
            state = MovementState.falling;
        }

        // Convierte el Enum en integer
        animPlayer.SetInteger("movementState", (int)state);
    }

    private bool IsGrounded()
    {
        canDash = true;
        return Physics2D.BoxCast(collision.bounds.center, collision.bounds.size, 0f, Vector2.down, 0.1f, jumpableGround);
    }
}
