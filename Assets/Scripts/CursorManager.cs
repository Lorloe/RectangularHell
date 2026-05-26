using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    private SpriteRenderer cursorImage;

    void Awake()
    {
        cursorImage = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Cursor.visible = false;
        if (Application.isPlaying)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    void Update()
    {
        RotateCursor();
    }

    public void RotateCursor()
    {
        Vector2 currentMousePosition = Mouse.current.position.ReadValue();
        // cursorImage.transform.position = currentMousePosition;

        Vector2 direction = PlayerController.Instance.transform.InverseTransformPoint(currentMousePosition);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        cursorImage.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
