using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public bool ShootInput { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePosition { get; private set; }

    public void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
    }

    public void OnMouseMove(InputValue value)
    {
        MousePosition = value.Get<Vector2>();
    }

    public void OnShoot(InputValue value)
    {
        ShootInput = value.isPressed;
    }
}