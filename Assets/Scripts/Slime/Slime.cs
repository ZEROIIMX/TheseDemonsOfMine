using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class Slime : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public float MaxHealth;
    [SerializeField] public float Health;
    [SerializeField] public float HealthUpgrade1;
    [SerializeField] public float HealthUpgrade2;

    [Header("Grab Mechanic")]
    [SerializeField] private float damagePerTick = 5f;
    [SerializeField] private float damageInterval = 0.5f;

    [Header("Scaling Mechanic")]
    [SerializeField] private float maxScaleMultiplier = 1f;
    [SerializeField] private float scaleDuration = 2.0f;

    public bool isStruggling = false;
    public bool isDead = false;
    private SlimeTarget slimeTarget;
    private Animator m_animator;
    private Vector3 initialScale;
    private float initialMaxHealth;
    private float initialHealth;
    private Coroutine scalingCoroutine;
    public Rigidbody rb;

    private Sword sword;

    private Collider slimeCollider;
    private Collider playerCollider;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        slimeTarget = GetComponent<SlimeTarget>();
        m_animator = GetComponentInChildren<Animator>();
        initialScale = transform.localScale;
        initialMaxHealth = MaxHealth;
        initialHealth = Health;
        slimeCollider = GetComponent<Collider>();

    }

    private void Update()
    {

    }

    public bool IsBusy()
    {
        return isStruggling || isDead;
    }

    public void SetChaseState(bool isChasing)
    {
        if (scalingCoroutine != null)
        {
            StopCoroutine(scalingCoroutine);
        }

        if (isChasing)
        {
            Vector3 targetScale = initialScale * maxScaleMultiplier;
            scalingCoroutine = StartCoroutine(ScaleOverTime(targetScale, scaleDuration));
        }
    }

    public void Grab(PlayerController player)
    {
        if (!isStruggling && !isDead)
        {
            sword = player.GetComponentInChildren<Sword>();
            if (sword != null)
            {
                sword?.Held();
            }
            playerCollider = player.GetComponent<Collider>();

            slimeTarget.hasGrabbed = true;

            StartCoroutine(StruggleSequence(player));
        }
    }

    public IEnumerator StruggleSequence(PlayerController playerMovement)
    {
        if (playerMovement == null) yield break;

        isStruggling = true;
        HandleStruggleStart(playerMovement);
        yield return StartCoroutine(StruggleLoop(playerMovement));
        HandleStruggleEnd(playerMovement);
    }

    private void HandleStruggleStart(PlayerController playerMovement)
    {
        slimeTarget.hasGrabbed = true;

        playerMovement.enabled = false;
        if (slimeTarget != null) slimeTarget.enabled = false;

        if (slimeCollider != null && playerCollider != null)
        {
            Physics.IgnoreCollision(slimeCollider, playerCollider, true);
        }
        if (m_animator != null)
        {
            m_animator.SetBool("Walk", false);
            m_animator.SetTrigger("OnGrab");
        }
    }

    private IEnumerator StruggleLoop(PlayerController playerMovement)
    {
        var playerHealth = playerMovement.GetComponent<PlayerHealth>();
        float damageTickTimer = 0f;

        while (Health > 0)
        {
            if (playerMovement == null) yield break;

            playerMovement.transform.position = transform.position;
            damageTickTimer += Time.deltaTime;
            if (damageTickTimer >= damageInterval)
            {
                if (playerHealth != null) playerHealth.TakeDamage(damagePerTick);
                damageTickTimer = 0f;
            }
            yield return null;
        }
    }

    private void HandleStruggleEnd(PlayerController playerMovement)
    {
        if (playerMovement == null) return;

        playerMovement.enabled = true;
        if (slimeTarget != null) slimeTarget.enabled = true;

        if (scalingCoroutine != null) StopCoroutine(scalingCoroutine);
    }

    private IEnumerator ScaleOverTime(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float timer = 0f;
        bool isScalingUp = targetScale.x > startScale.x;

        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, timer / duration);
            timer += Time.deltaTime;

            if (isScalingUp)
            {
                if (transform.localScale.x >= 1.5f && MaxHealth < HealthUpgrade1)
                {
                    MaxHealth = HealthUpgrade1;
                    Health = MaxHealth;
                }
                if (transform.localScale.x >= 2f && MaxHealth < HealthUpgrade2)
                {
                    MaxHealth = HealthUpgrade2;
                    Health = MaxHealth;
                }
            }

            yield return null;
        }

        transform.localScale = targetScale;
        scalingCoroutine = null;
    }

    public void TakeDamage(int damageAmount)
    {
        Health -= damageAmount;
        m_animator.SetTrigger("OnHit");
        Debug.Log(Health);

        if (Health <= 0)
        {
            isDead = true;
            m_animator.SetTrigger("Death");
        }
    }

    public void TakeForce()
    {
        StartCoroutine(DelayedResetVelocity());
    }
    IEnumerator DelayedResetVelocity()
    {
        yield return new WaitForSeconds(0.3f);
        rb.linearVelocity = Vector3.zero;
    }
    public void Death()
    {
        if (sword != null)
        {
        sword?.NotHeld();
        }

        Destroy(gameObject);
    }
}
