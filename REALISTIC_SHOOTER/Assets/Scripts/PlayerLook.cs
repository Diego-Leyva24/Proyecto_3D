using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Sensibilidad")]
    [SerializeField] private float sensX = 300f;
    [SerializeField] private float sensY = 300f;

    [Header("Head Bob")]
    [SerializeField] private float bobFrequency = 1.5f;
    [SerializeField] private float bobAmplitude = 0.05f;
    [SerializeField] private CharacterController controller;

    [Header("Camera Sway")]
    [SerializeField] private float swayAmount = 0.05f;
    [SerializeField] private float swaySmoothness = 4f;

    [Header("Lean / Peek")]
    [SerializeField] private float leanAngle = 15f;
    [SerializeField] private float leanSpeed = 8f;
    [SerializeField] private float leanOffset = 0.15f;

    [Header("Zoom")]
    [SerializeField] private float defaultFOV = 60f;
    [SerializeField] private float zoomFOV = 35f;
    [SerializeField] private float zoomSpeed = 10f;

    [Header("Referencias")]
    [SerializeField] private Transform cameraTransform; // Main Camera
    [SerializeField] private Transform bodyCapsule; // El cuerpo visual que se moverá con la cámara
    [SerializeField] private Camera mainCamera;

    private float currentLean = 0f;
    private float targetLean = 0f;
    private float xRot = 0f;
    private float mouseX, mouseY;

    private Transform charCtrl;
    private Vector3 initialLocalPos;
    private float bobTimer = 0f;
    private Transform rigTransform;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rigTransform = transform;
        charCtrl = transform.parent;
        initialLocalPos = rigTransform.localPosition;

        if (controller == null)
            controller = charCtrl.GetComponent<CharacterController>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        mainCamera.fieldOfView = defaultFOV;

        int cameraLayer = LayerMask.NameToLayer("CameraIgnore");
        for (int i = 0; i < 32; i++)
        {
            if (i == cameraLayer) continue;
            Physics.IgnoreLayerCollision(cameraLayer, i);
        }
    }

    void Update()
    {
        HandleMouseLook();
        HandleHeadBob();
        HandleCameraSway();
        HandleLeaning();
        HandleZoom();
    }

    private void HandleMouseLook()
    {
        mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90, 90);

        cameraTransform.localRotation = Quaternion.Euler(xRot, 0f, 0f);

        mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
        charCtrl.Rotate(Vector3.up * mouseX);
    }

    private void HandleHeadBob()
    {
        if (!controller.isGrounded || controller.velocity.magnitude < 0.1f)
        {
            bobTimer = 0f;
            rigTransform.localPosition = Vector3.Lerp(rigTransform.localPosition, initialLocalPos, Time.deltaTime * 5f);
            return;
        }

        bobTimer += Time.deltaTime * bobFrequency;
        float bobOffset = Mathf.Sin(bobTimer) * bobAmplitude;
        Vector3 bobPos = new Vector3(initialLocalPos.x, initialLocalPos.y + bobOffset, initialLocalPos.z);

        rigTransform.localPosition = Vector3.Lerp(rigTransform.localPosition, bobPos, Time.deltaTime * 8f);
    }

    private void HandleCameraSway()
    {
        float swayX = -Input.GetAxis("Horizontal") * swayAmount;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, swayX * 10f);
        rigTransform.localRotation = Quaternion.Slerp(rigTransform.localRotation, targetRotation, Time.deltaTime * swaySmoothness);
    }

    private void HandleLeaning()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Mathf.Approximately(targetLean, leanAngle)) targetLean = 0f;
            else targetLean = leanAngle;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (Mathf.Approximately(targetLean, -leanAngle)) targetLean = 0f;
            else targetLean = -leanAngle;
        }

        currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * leanSpeed);

        Quaternion leanRotation = Quaternion.Euler(0f, 0f, currentLean);
        rigTransform.localRotation = Quaternion.Slerp(rigTransform.localRotation, leanRotation, Time.deltaTime * leanSpeed);

        Vector3 offset = initialLocalPos + (rigTransform.right * (currentLean / leanAngle) * leanOffset);
        rigTransform.localPosition = Vector3.Lerp(rigTransform.localPosition, offset, Time.deltaTime * leanSpeed);

        if (bodyCapsule != null)
        {
            bodyCapsule.localPosition = new Vector3((currentLean / leanAngle) * leanOffset, 0f, 0f);
            bodyCapsule.localRotation = Quaternion.Euler(0f, 0f, currentLean);
        }
    }

    private void HandleZoom()
    {
        float targetFOV = Input.GetMouseButton(1) ? zoomFOV : defaultFOV;
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }
}