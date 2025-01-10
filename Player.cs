using UnityEngine;

public class Player : MonoBehaviour
{
    public float _speed = 5f;
    public float _rotationSpeed = 700f;
    private CharacterController _controller;
    private Vector3 _direction;
    private Camera _camera;
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _camera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        _direction = new Vector3(horizontal, 0, vertical).normalized;

        if (_direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg +
                               _camera.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationSpeed, 0.1f);

            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 move_Direction = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            _controller.Move(move_Direction * _speed * Time.deltaTime);
        }
    }
}