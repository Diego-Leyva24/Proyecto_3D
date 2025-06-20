using UnityEngine;

public class SmartDoorRotator : MonoBehaviour
{
    [Header("Puerta")]
    public Transform hingeTransform;         // Empty que rota (la bisagra)
    public float openAngle = 90f;            // Ángulo base de apertura
    public float rotationSpeed = 5f;

    [Header("Interacción")]
    public KeyCode interactionKey = KeyCode.F;
    public Transform player;

    private bool isOpen = false;
    private bool isPlayerNear = false;
    private Quaternion closedRot;
    private Quaternion targetOpenRot;

    void Start()
    {
        closedRot = hingeTransform.localRotation;
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(interactionKey))
        {
            isOpen = !isOpen;

            if (isOpen)
            {
                // Dirección desde la puerta hacia el jugador
                Vector3 toPlayer = (player.position - hingeTransform.position).normalized;
                float dot = Vector3.Dot(hingeTransform.forward, toPlayer);

                float angle = (dot >= 0) ? openAngle : -openAngle;
                targetOpenRot = Quaternion.Euler(0f, angle, 0f) * closedRot;
            }
        }

        Quaternion targetRot = isOpen ? targetOpenRot : closedRot;
        hingeTransform.localRotation = Quaternion.Lerp(hingeTransform.localRotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
