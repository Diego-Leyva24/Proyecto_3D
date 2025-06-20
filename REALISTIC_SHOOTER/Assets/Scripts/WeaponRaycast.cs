using UnityEngine;

public class WeaponRaycast : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private float range = 100f;
    [SerializeField] private LayerMask hitLayer;

    [Header("Daño")]
    [SerializeField] private int bodyDamage = 25;
    [SerializeField] private int headDamage = 100;

    [Header("Efectos")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject hitEffect;

    [Header("Inventario")]
    [SerializeField] private AmmoInventory ammoInventory;

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (ammoInventory == null || !ammoInventory.HasAmmo)
        {
            Debug.Log("[Weapon] Click vacío o sin AmmoInventory.");
            return;
        }

        nextFireTime = Time.time + fireRate;
        ammoInventory.TryUseAmmo();

        if (muzzleFlash != null) muzzleFlash.Play();

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range, hitLayer))
        {
            EnemyHitbox hitbox = hit.collider.GetComponent<EnemyHitbox>();
            if (hitbox != null)
            {
                int damage = hit.collider.CompareTag("Head") ? headDamage : bodyDamage;
                hitbox.TakeDamage(damage);
            }

            if (hitEffect != null)
                Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
}