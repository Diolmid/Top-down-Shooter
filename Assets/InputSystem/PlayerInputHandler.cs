using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 MouseInput { get; private set; }

    public void OnMove(InputValue value)
    {
        SetMoveInput(value.Get<Vector2>());
    }

    public void OnMouse(InputValue value)
    {
        SetMouseInput(value.Get<Vector2>());
    }

    public void SetMoveInput(Vector2 moveInput) 
    {
        MoveInput = moveInput;
    }

    public void SetMouseInput(Vector2 mouseInput)
    {
        MouseInput = mouseInput;
    }
}