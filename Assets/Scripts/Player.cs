using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent (typeof(GunController))]
[RequireComponent (typeof(PlayerInputHandler))]
public class Player : LivingEntity
{
    [SerializeField] private float moveSpeed = 5f;

    private PlayerController _playerController;
    private PlayerInputHandler _inputHandler;
    private GunController _gunController;
    private Camera _viewCamera;

    private void Awake()
    {
        _viewCamera = Camera.main;
        _playerController = GetComponent<PlayerController>();
        _inputHandler = GetComponent<PlayerInputHandler>();
        _gunController = GetComponent<GunController>();
    }

    void Update()
    {
        MoveHandler();
        RotateHandler();
        GunHandler();
    }

    private void GunHandler()
    {
        if (_inputHandler.ShootInput)
            _gunController.OnTriggerHold();
        if (!_inputHandler.ShootInput)
            _gunController.OnTriggerRelease();
    }

    private void MoveHandler()
    {
        Vector3 moveInput = new Vector3(_inputHandler.MoveInput.x, 0f, _inputHandler.MoveInput.y);
        Vector3 moveVelocity = moveInput * moveSpeed;

        _playerController.Move(moveVelocity);
    }

    private void RotateHandler()
    {
        Ray ray = _viewCamera.ScreenPointToRay(_inputHandler.MousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 hitPoint = ray.GetPoint(rayDistance);
            _playerController.LookAt(hitPoint);
        }
    }
}