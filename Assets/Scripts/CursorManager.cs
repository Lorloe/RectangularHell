using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    private SpriteRenderer _cursorSprite;

    void Awake()
    {
        _cursorSprite = GetComponent<SpriteRenderer>();
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
        _cursorSprite.transform.position = currentMousePosition;
    }
}
