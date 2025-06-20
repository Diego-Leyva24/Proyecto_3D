using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    [Header("Salud")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Muerte")]
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private float destroyDelay = 2f;
    [SerializeField] private GameObject meshVisual;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"[Enemy] Spawned with {maxHealth} HP.");
    }

    public void ApplyDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[Enemy] Took {amount} damage. HP left: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        Debug.Log($"[Enemy] Died. Playing death effect and destroying in {destroyDelay} seconds.");

        if (meshVisual != null)
            meshVisual.SetActive(false);

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject, destroyDelay);
    }
}
