using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Transform respawnPoint;
    public Text healthText;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        Debug.Log("Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
            Invoke(nameof(Respawn), 0.5f); // Delay para evitar spam
        }
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "VIDA: " + currentHealth;
        }
    }

    void Respawn()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        transform.position = respawnPoint.position;

        if (controller != null) controller.enabled = true;

        currentHealth = maxHealth;
        isDead = false;
        UpdateHealthUI();
        Debug.Log("Jugador respawneado con vida completa.");
    }
}
