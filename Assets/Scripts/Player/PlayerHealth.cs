using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("UI Health Bar")]
    [SerializeField] private Transform healthBarTransform; // This should be the fill sprite's transform

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
        UpdateHealthBar();
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene("GameOverScreen");
        }
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
