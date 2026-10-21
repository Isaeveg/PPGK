using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    public Camera thirdPersonCamera;
    public Camera topDownCamera;
    public PlayerController player;

    bool isTopDown;

    void Start() => Apply();

    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            isTopDown = !isTopDown;
            Apply();
        }

        if (!isTopDown)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame) SetCursor(false);
            else if (Mouse.current.leftButton.wasPressedThisFrame) SetCursor(true);
        }
    }

    void Apply()
    {
        thirdPersonCamera.gameObject.SetActive(!isTopDown);
        topDownCamera.gameObject.SetActive(isTopDown);
        player.topDownMode = isTopDown;
        player.StopMoving();

        if (isTopDown) topDownCamera.GetComponent<TopDownCamera>().CenterOn(player.transform.position);

        SetCursor(!isTopDown);
    }

    void SetCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
