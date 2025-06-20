using UnityEngine;

public class AmmoInventory : MonoBehaviour
{
    [Header("Balas")]
    public int currentAmmo = 12;
    public int maxAmmo = 60;

    public bool HasAmmo => currentAmmo > 0;

    public bool TryUseAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            return true;
        }
        return false;
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        Debug.Log($"[Ammo] Recogiste {amount} balas. Total: {currentAmmo}/{maxAmmo}");
    }
}