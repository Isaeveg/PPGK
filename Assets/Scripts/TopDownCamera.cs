using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class TopDownCamera : MonoBehaviour
{
    public PlayerController player;
    public GameObject clickMarker;
    public LayerMask groundMask = ~0;
    public float moveSpeed = 15f;
    public float height = 15f;
    public float minHeight = 5f;
    public float maxHeight = 40f;
    public float zoomStep = 2f;
    public float pitch = 60f;

    Camera cam;
    Vector3 focus;

    void Awake() => cam = GetComponent<Camera>();

    public void CenterOn(Vector3 point) => focus = point;

    void Update()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;

        float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float z = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
        focus += new Vector3(x, 0f, z).normalized * moveSpeed * Time.deltaTime;

        float scroll = mouse.scroll.ReadValue().y;
        if (scroll != 0f)
            height = Mathf.Clamp(height - Mathf.Sign(scroll) * zoomStep, minHeight, maxHeight);

        if (mouse.rightButton.wasPressedThisFrame &&
            Physics.Raycast(cam.ScreenPointToRay(mouse.position.ReadValue()), out RaycastHit hit, 1000f, groundMask))
        {
            player.MoveTo(hit.point);
            clickMarker.SetActive(true);
            clickMarker.transform.position = hit.point + Vector3.up * 0.05f;
        }

        if (!player.HasTarget) clickMarker.SetActive(false);
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.position = focus - transform.forward * height / Mathf.Sin(pitch * Mathf.Deg2Rad);
    }
}
