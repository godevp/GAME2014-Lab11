using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Player Movement")] 
    public float horizontalForce;
    public float horizontalSpeed;
    public float verticalForce;
    public float airFactor;
    public Transform groundPoint;
    public float groundRadius;
    public LayerMask groundLayerMask;
    public bool isGrounded;

    [Header("Animations")]
    public Animator animator;
    public PlayerAnimationState state;

    [Header("Dust Trail")] 
    public ParticleSystem dustTrail;
    public Color dustTrailColour;

    [Header("Screen Shake Properties")]
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineBasicMultiChannelPerlin perlin;
    public float shakeIntensity;
    public float shakeDuration;
    public float shakeTimer;
    public bool isCameraShaking;

    [Header("Health System")] 
    public HealthBarController health;
    public LifeCounterController life;
    public DeathPlaneController deathPlane;

    [Header("Controls")] 
    public Joystick leftJoystick;
    [Range(0.1f, 1.0f)]
    public float verticalThreshold;

    private Rigidbody2D rigidbody2D;
    private SoundManager soundManager;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = FindObjectOfType<PlayerHealth>().GetComponent<HealthBarController>();
        life = FindObjectOfType<LifeCounterController>();
        deathPlane = FindObjectOfType<DeathPlaneController>();
        soundManager = FindObjectOfType<SoundManager>();
        dustTrail = GetComponentInChildren<ParticleSystem>();

        // Camera Shake
        isCameraShaking = false;
        shakeTimer = shakeDuration;
        virtualCamera = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
        perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();


        if (GameObject.Find("OnScreenControls"))
        {
            leftJoystick = GameObject.Find("LeftStick").GetComponent<Joystick>();
        }
    }

    void Update()
    {
        if (health.value <= 0)
        {
            life.LoseLife();

            if (life.value > 0)
            {
                health.ResetHealth();
                deathPlane.ReSpawn(gameObject);
                soundManager.PlaySoundFX(SoundFX.DEATH, Channel.PLAYER_DEATH_FX);
            }
        }

        if (life.value <= 0)
        {
            SceneManager.LoadScene("End");
        }
    }


    void FixedUpdate()
    {
        var hit = Physics2D.OverlapCircle(groundPoint.position, groundRadius, groundLayerMask);
        isGrounded = hit;

        Move();
        Jump();
        AirCheck();

        // Camera Shake Control
        if (isCameraShaking)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0.0f) // timed out
            {
                perlin.m_AmplitudeGain = 0.0f;
                shakeTimer = shakeDuration;
                isCameraShaking = false;
            }
        }
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal") + 
                  (GameObject.Find("OnScreenControls") ? leftJoystick.Horizontal : 0.0f);

        if (x != 0.0f)
        {
            Flip(x);
        
            rigidbody2D.AddForce(Vector2.right * ((x > 0.0) ? 1.0f : -1.0f) * horizontalForce * ((isGrounded) ? 1 : airFactor));

            var clampedX  = Mathf.Clamp(rigidbody2D.velocity.x, -horizontalSpeed, horizontalSpeed);

            rigidbody2D.velocity = new Vector2(clampedX, rigidbody2D.velocity.y);

            ChangeAnimation(PlayerAnimationState.RUN);

            if (isGrounded)
            {
                CreateDustTrail();
            }
            
        }

        if ((isGrounded) && (x == 0))
        {
            ChangeAnimation(PlayerAnimationState.IDLE);
        }
    }

    private void CreateDustTrail()
    {
        dustTrail.GetComponent<Renderer>().material.SetColor("_Color", dustTrailColour);
        dustTrail.Play();
    }

    private void ShakeCamera()
    {
        perlin.m_AmplitudeGain = shakeIntensity;
        isCameraShaking = true;
    }

    private void ChangeAnimation(PlayerAnimationState animationState)
    {
        // Change the Animation to RUN
        state = animationState;
        animator.SetInteger("AnimationState", (int)state);
    }

    private void Jump()
    {
        var y = Input.GetAxis("Jump") + 
                (GameObject.Find("OnScreenControls") ? leftJoystick.Vertical : 0.0f);

        if ((isGrounded) && (y > verticalThreshold))
        {
            rigidbody2D.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);
            soundManager.PlaySoundFX(SoundFX.JUMP, Channel.PLAYER_SOUND_FX);
        }
    }

    private void AirCheck()
    {
        if (!isGrounded)
        {
            ChangeAnimation(PlayerAnimationState.JUMP);
        }
    }

    public void Flip(float x)
    {
        if (x != 0.0f)
        {
            transform.localScale = new Vector3((x > 0.0f) ? 1.0f : -1.0f, 1.0f, 1.0f);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundPoint.position, groundRadius);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            health.TakeDamage(20);
            if (life.value > 0)
            {
                soundManager.PlaySoundFX(SoundFX.HURT, Channel.PLAYER_HURT_FX);
            }
            ShakeCamera();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Hazard"))
        {
            health.TakeDamage(30);
            if (life.value > 0)
            {
                soundManager.PlaySoundFX(SoundFX.HURT, Channel.PLAYER_HURT_FX);
            }
            ShakeCamera();
        }

        if (other.gameObject.CompareTag("Bullet"))
        {
            health.TakeDamage(10);
            if (life.value > 0)
            {
                soundManager.PlaySoundFX(SoundFX.HURT, Channel.PLAYER_HURT_FX);
            }
            ShakeCamera();
        }
    }

}
