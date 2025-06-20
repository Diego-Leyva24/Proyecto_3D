using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 12;

    private void OnTriggerEnter(Collider other)
    {
        AmmoInventory inventory = other.GetComponent<AmmoInventory>();
        if (inventory != null)
        {
            inventory.AddAmmo(ammoAmount);
            Destroy(gameObject);
        }
    }
}