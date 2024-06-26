using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5;
    public float RunSpeed = 8;
    public float JumpForce = 3;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _jumpBufferingDistance = 1.5f;

    public Rigidbody rb;

    private float _currentSpeed;
    private float _jump = 0;
    private bool _maxJump = false;
    private bool _isGrounded = true;
    private bool _isDoubleJumped = false;
    private bool _secondJump = false;
    private float _coyoteTimeCounter;

    private void Start()
    {
        SetOnStart();
    }

    private void Update()
    {
        JumpBuffering();
        Jump();
        DoubleJump();
        Sprint();
        Fall();
    }

    private void FixedUpdate()
    {
        Movement();
    }
    private void OnCollisionEnter(Collision collision)
    {
        OnGround();
    }
    private void OnCollisionExit(Collision collision)
    {
        OnOutGround();
    }

    private void Movement()
    {
        var horizontale = Input.GetAxis("Horizontal");
        rb.velocity = new Vector3(0, _jump, horizontale * _currentSpeed);
    }

    private void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && !_maxJump)
        {
            if (_jump <= 0)
            {
                _maxJump = true;
            }
            _jump -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Space) && _coyoteTimeCounter > 0)
        {
            _isGrounded = false;
            _jump = JumpForce;
            _isDoubleJumped = true;
            _secondJump = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space) && !_isGrounded)
        {
            _isDoubleJumped = false;
            _maxJump = true;
            while (_jump > 0)
            {
                _jump -= Time.deltaTime;
            }
        }
    }

    private void DoubleJump()
    {
        if (!_isGrounded && !_isDoubleJumped && _secondJump)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _secondJump = false;
                _maxJump = false;
                _jump = JumpForce;
                _isDoubleJumped = true;
            }
        }
    }

    private void Sprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _isGrounded)
        {
            _currentSpeed = RunSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _currentSpeed = Speed;
        }
    }

    private void Fall()
    {
        if (!_isGrounded)
        {
            _jump -= _gravity * Time.deltaTime;
            _coyoteTimeCounter -= Time.deltaTime;
        }
        else
        {
            _jump = 0;
            _coyoteTimeCounter = 0.2f;
        }

        if (_maxJump)
        {
            _gravity = 14;
        }
        else
        {
            _gravity = 9.81f;
        }
    }
    private void JumpBuffering()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, _jumpBufferingDistance))
        {
            _maxJump = false;
            _isDoubleJumped = false;
            _gravity = 9.81f;
        }
    }

    private void OnGround()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 1))
        {
            _isGrounded = true;
        }
        _jump = 0;
        _maxJump = true;
    }

    private void OnOutGround()
    {
        _maxJump = false;
        _isGrounded = false;
    }

    private void SetOnStart()
    {
        _currentSpeed = Speed;
        _isGrounded = false;
        _isDoubleJumped = true;
        if (_jumpBufferingDistance < 1)
        {
            _jumpBufferingDistance = 1;
        }
    }
}