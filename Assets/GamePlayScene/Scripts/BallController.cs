using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Physics")]
    public float launchAngle = 45f;
    public float gravityScale = 1f;
    public float rollDrag = 1f;

    [Header("Tail")]
    public GameObject tailObject;
    public float tailMinSpeed = 1.5f;
    public float tailOffset = 0.3f;

    [Header("Sound")]
    public AudioSource hitAudioSource;     // AudioSource на м€че
    public AudioClip hitClip;              // «вук удара

    private bool isFlying = false;
    private bool hasLanded = false;

    private GameManager gameManager;

    void Start()
    {
        rb.gravityScale = gravityScale;
        gameManager = FindObjectOfType<GameManager>();

        if (tailObject != null)
            tailObject.SetActive(false);
    }

    public void Launch(float distance)
    {
        // ===== PLAY HIT SOUND =====
        if (PlayerData.Instance != null &&
            PlayerData.Instance.SoundOn &&
            hitAudioSource != null &&
            hitClip != null)
        {
            hitAudioSource.PlayOneShot(hitClip);
        }

        float angleRad = launchAngle * Mathf.Deg2Rad;
        float g = Mathf.Abs(Physics2D.gravity.y * gravityScale);

        float v = Mathf.Sqrt(g * distance / Mathf.Sin(2 * angleRad));

        rb.linearVelocity = new Vector2(
            v * Mathf.Cos(angleRad),
            v * Mathf.Sin(angleRad)
        );

        isFlying = true;
        hasLanded = false;
        rb.linearDamping = 0f;

        if (tailObject != null)
            tailObject.SetActive(true);
    }

    void FixedUpdate()
    {
        // === TAIL LOGIC ===
        if (isFlying && tailObject != null)
        {
            Vector2 velocity = rb.linearVelocity;

            if (velocity.magnitude > tailMinSpeed)
            {
                Vector2 dir = -velocity.normalized;

                tailObject.transform.position =
                    (Vector2)transform.position + dir * tailOffset;

                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                tailObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        // === STOP CHECK ===
        if (hasLanded && rb.linearVelocity.magnitude < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
            hasLanded = false;

            if (tailObject != null)
                tailObject.SetActive(false);

            gameManager.OnBallStopped();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFlying)
        {
            isFlying = false;
            hasLanded = true;
            rb.linearDamping = rollDrag;

            if (tailObject != null)
                tailObject.SetActive(false);
        }
    }
}
