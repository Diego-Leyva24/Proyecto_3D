using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private AmmoInventory ammoInventory;
    [SerializeField] private Text ammoText;

    private void Update()
    {
        if (ammoInventory == null || ammoText == null) return;

        ammoText.text = $"{ammoInventory.currentAmmo} / {ammoInventory.maxAmmo}";
    }
}