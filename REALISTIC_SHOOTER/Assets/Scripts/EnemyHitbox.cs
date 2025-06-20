using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    [SerializeField] private EnemyHealthManager healthManager;

    public void TakeDamage(int amount)
    {
        if (healthManager != null)
        {
            healthManager.ApplyDamage(amount);
        }
    }
}
