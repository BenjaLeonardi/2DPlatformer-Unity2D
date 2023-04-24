using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovemente : MonoBehaviour
{
    #region Data and Variables
    [Header("Accelerations Data")]
    [SerializeField] private float acceleration = 400f;
    [SerializeField] private float deceleration = 160f;
    [SerializeField] private float airAcceleration = 0.7f;
    [SerializeField] private float airDeceleration = 0.7f;
    [Space(5)]

    [Header("Running Data")]
    [SerializeField] private float maxSpeed = 18f;
    [Space(5)]

    [Header("Jumping Data")]
    [SerializeField] private float jumpHeight = 15f;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private AudioSource audioJump;
    private float coyoteTimeCounter;
    private float jumpBufferTimeCounter;
    private bool isJumping = false;
    [Space(5)]

    [Header("Dash Data")]
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashTime = 0.2f;
    private Vector2 dashDirection;
    private bool isDashing = false;
    private bool canDash = true;
    [Space]

    [Header("Walljumping Data")]
    [SerializeField] private float wallSlidingSpeed = 5f;
    [SerializeField] private Transform frontCheck;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private float xWallForce = 25f;
    [SerializeField] private float yWallForce = 15f;
    [SerializeField] private float wallJumpTime = 0.1f;
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
    private bool levelUp = false;
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
        var changeLevel = Input.GetKeyDown("j");

        if (!isDashing)
        {
            Run();
        }

        //Optimizar y ver como no trabarse en collisions o bien no quedar OOB
        if (levelUp && changeLevel)
        {
            audioSpaceShift.Play();
            levelUp = false;
            transform.position = new Vector2(transform.position.x, transform.position.y - 90);
        }
        else if (!levelUp && changeLevel)
        {
            audioSpaceShift.Play();
            levelUp = true;
            transform.position = new Vector2(transform.position.x, transform.position.y + 90);
        }

        if (jumpInput)
        {
            PlayerJump();
        }

        if (dashInput && canDash)
        {
            Dash();
        }
        Walljumping();
        UpdateAnimation();
        JumpBuffers();
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

        if (isTouchingFront == true && IsGrounded() == false && dirX != 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        if (isWallSliding == true)
        {
            rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, Mathf.Clamp(rbPlayer.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }

        if (Input.GetKeyDown("space") && isWallSliding == true)
        {
            wallJumping = true;
            Invoke("SetWallJumpingToFalse", wallJumpTime);
        }

        if (wallJumping == true)
        {
            dustParticles.Play();
            rbPlayer.velocity = new Vector2(xWallForce * -dirX, yWallForce);
        }
    }

    private void SetWallJumpingToFalse()
    {
        wallJumping = false;
    }

    //Arreglar
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
