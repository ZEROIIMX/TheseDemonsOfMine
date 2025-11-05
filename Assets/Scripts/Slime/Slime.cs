using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class Slime : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public float MaxHealth;
    [SerializeField] public float Health;

    [Header("Grab Mechanic")]
    [SerializeField] private float grabCooldown = 5f;
    [SerializeField] private int clicksToEscape = 5;
    [SerializeField] private float struggleDuration = 4f;
    [SerializeField] private float damagePerTick = 5f;
    [SerializeField] private float damageInterval = 0.5f;

    [Header("Scaling Mechanic")]
    [SerializeField] private float maxScaleMultiplier = 1.5f;
    [SerializeField] private float scaleDuration = 3.0f;

    private bool isStruggling = false;
    private SlimeTarget slimeTarget;
    private Animator m_animator;
    private Vector3 initialScale;
    private Coroutine scalingCoroutine;
    private void Start()
    {
        slimeTarget = GetComponent<SlimeTarget>();
        m_animator = GetComponent<Animator>();
        initialScale = transform.localScale;
    }

    private void Update()
    {
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public bool IsBusy()
    {
        return isStruggling;
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
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
        if (other.CompareTag("Player") && !isStruggling)
        {
            if (other.TryGetComponent<PlayerController>(out var playerMovement))
            {
                StartCoroutine(StruggleSequence(playerMovement));
            }
        }
    }

    private IEnumerator StruggleSequence(PlayerController playerMovement)
    {
        isStruggling = true;
        HandleStruggleStart(playerMovement);
        yield return StartCoroutine(StruggleLoop(playerMovement));
        HandleStruggleEnd(playerMovement);
        yield return new WaitForSeconds(grabCooldown);
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
        if (m_animator != null) m_animator.SetBool("IsGrabbing", true);
    }

    private IEnumerator StruggleLoop(PlayerController playerMovement)
    {
        var playerHealth = playerMovement.GetComponent<PlayerHealth>();
        int clickCount = 0;
        float struggleTimer = 0f;
        float damageTickTimer = 0f;

        while (struggleTimer < struggleDuration)
        {
            playerMovement.transform.position = transform.position;
            damageTickTimer += Time.deltaTime;
            if (damageTickTimer >= damageInterval)
            {
                if (playerHealth != null) playerHealth.TakeDamage(damagePerTick);
                damageTickTimer = 0f;
            }
            if (Mouse.current.leftButton.wasPressedThisFrame) clickCount++;
            if (clickCount >= clicksToEscape) break;
            struggleTimer += Time.deltaTime;
            yield return null;
        }
    }

    private void HandleStruggleEnd(PlayerController playerMovement)
    {
        if (m_animator != null) m_animator.SetBool("IsGrabbing", false);
        playerMovement.enabled = true;
        if (slimeTarget != null) slimeTarget.enabled = true;

        if (scalingCoroutine != null) StopCoroutine(scalingCoroutine);
        scalingCoroutine = StartCoroutine(ScaleOverTime(initialScale, scaleDuration));
    }

    private IEnumerator ScaleOverTime(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float timer = 0f;

        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        scalingCoroutine = null;
    }
}
