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
    [SerializeField] private float grabCooldown = 5f;
    [SerializeField] private float damagePerTick = 5f;
    [SerializeField] private float damageInterval = 0.5f;

    [Header("Scaling Mechanic")]
    [SerializeField] private float maxScaleMultiplier = 1.5f;
    [SerializeField] private float scaleDuration = 3.0f;

    private bool isStruggling = false;
    private bool isDead = false;
    private SlimeTarget slimeTarget;
    private Animator m_animator;
    private Vector3 initialScale;
    private float initialMaxHealth;
    private float initialHealth;
    private Coroutine scalingCoroutine;

    private void Start()
    {
        slimeTarget = GetComponent<SlimeTarget>();
        m_animator = GetComponentInChildren<Animator>();
        initialScale = transform.localScale;
        initialMaxHealth = MaxHealth;
        initialHealth = Health;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isStruggling && !isDead)
        {
            if (other.TryGetComponent<PlayerController>(out var playerMovement))
            {
                StartCoroutine(StruggleSequence(playerMovement));
            }
        }
    }

    private IEnumerator StruggleSequence(PlayerController playerMovement)
    {
        if (playerMovement == null) yield break;

        isStruggling = true;
        HandleStruggleStart(playerMovement);
        yield return StartCoroutine(StruggleLoop(playerMovement));
        HandleStruggleEnd(playerMovement);

        if (!isDead)
        {
            yield return new WaitForSeconds(grabCooldown);
        }
        isStruggling = false;
    }

    private void HandleStruggleStart(PlayerController playerMovement)
    {
        if (scalingCoroutine != null)
        {
            StopCoroutine(scalingCoroutine);
            scalingCoroutine = null;
        }

        playerMovement.enabled = false;
        if (slimeTarget != null) slimeTarget.enabled = false;
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
                if (transform.localScale.x >= 1.75f && MaxHealth < HealthUpgrade1)
                {
                    MaxHealth = HealthUpgrade1;
                    Health = MaxHealth;
                }
                if (transform.localScale.x >= 2.49f && MaxHealth < HealthUpgrade2)
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

        if (Health <= 0)
        {
            m_animator.SetTrigger("Death");
        }
    }

    public void Death()
    {
        
        Destroy(gameObject);
    }
}
