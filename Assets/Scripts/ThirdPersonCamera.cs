using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float pivotHeight = 1.6f;
    public float mouseSensitivity = 0.15f;
    public float minPitch = -30f;
    public float maxPitch = 60f;
    public float distance = 4f;
    public float minDistance = 1.5f;
    public float maxDistance = 10f;
    public float zoomStep = 0.8f;

    float pitch = 15f;

    void LateUpdate()
    {
        var mouse = Mouse.current;

        if (Cursor.lockState == CursorLockMode.Locked)
            pitch = Mathf.Clamp(pitch - mouse.delta.ReadValue().y * mouseSensitivity, minPitch, maxPitch);

        float scroll = mouse.scroll.ReadValue().y;
        if (scroll != 0f)
            distance = Mathf.Clamp(distance - Mathf.Sign(scroll) * zoomStep, minDistance, maxDistance);

        transform.rotation = Quaternion.Euler(pitch, target.eulerAngles.y, 0f);
        transform.position = target.position + Vector3.up * pivotHeight - transform.forward * distance;
    }
}
