using System.Collections;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("UI Health Bar")]
    [SerializeField] private Transform healthBarTransform; // This should be the fill sprite's transform

    private bool damaged = false;

    private DamageFlash damageFlash;

    private ParentConnection parentConnection;

    private PlayerController playerController;

    private Sword playerSword;

    private bool isDead = false;

    private CharacterController controller;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerSword = GetComponent<Sword>();
        playerController = GetComponent<PlayerController>();
        parentConnection = GetComponentInChildren<ParentConnection>();
        damageFlash = GetComponentInChildren<DamageFlash>();
        currentHealth = maxHealth;
        UpdateHealthBar();
        damageFlash?.HealthUpdate(currentHealth);

    }

    public void TakeDamage(float damage)
    {
        if (!damaged && !isDead)
        {
            damaged = true;
            currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
            damageFlash?.Flash(currentHealth);
            UpdateHealthBar();
            Debug.Log("Player Health: " + currentHealth);
            StartCoroutine(DamageCooldown());

            if (currentHealth <= 0)
            {
                isDead = true;
                PlayerEvents.OnPlayerDeath?.Invoke();
                parentConnection?.Death();
                playerController?.DisableControls();
                playerSword?.DisableSword();
            }
        }
    }

    IEnumerator DamageCooldown()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        damaged = false;
    }

    private void UpdateHealthBar()
    {
        if (healthBarTransform != null)
        {
            float healthPercent = currentHealth / maxHealth;
            healthBarTransform.localScale = new Vector3(healthPercent, 1f, 1f);
        }
    }
}
