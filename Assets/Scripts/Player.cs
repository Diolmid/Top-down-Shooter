using UnityEngine;

[RequireComponent (typeof(PlayerController))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    private PlayerController _playerController;
    private PlayerInputHandler _inputHandler;
    private Camera _viewCamera;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _inputHandler = GetComponent<PlayerInputHandler>();
        _viewCamera = Camera.main;
    }

    void Update()
    {
        Vector3 moveInput = new Vector3(_inputHandler.MoveInput.x, 0f, _inputHandler.MoveInput.y);
        Vector3 moveVelocity = moveInput * moveSpeed;

        _playerController.Move(moveVelocity);

        RotateInput();
    }

    private void RotateInput()
    {
        Ray ray = _viewCamera.ScreenPointToRay(_inputHandler.MouseInput);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 hitPoint = ray.GetPoint(rayDistance);
            _playerController.LookAt(hitPoint);
        }
    }
}