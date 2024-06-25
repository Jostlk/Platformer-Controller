using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5;
    public float RunSpeed = 8;
    public float JumpForce = 3;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _jumpBufferingDistance;

    public Rigidbody rb;

    private float _currentSpeed;
    private float _jump = 0;
    private bool _maxJump = false;
    private bool _isGrounded = true;
    private bool _isDoubleJumped = false;
    private LayerMask _layerGround;

    private void Start()
    {
        SetOnStart();
    }

    private void Update()
    {
        Jump();
        //DoubleJump();
        Sprint();
        Fall();
    }

    private void FixedUpdate()
    {
        Movement();
    }
    private void OnCollisionEnter(Collision collision)
    {
        OnGround(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        OnOutGround(collision);
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
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _jump = JumpForce;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            _maxJump = true;
            while (_jump > 0)
            {
                _jump -= Time.deltaTime;
            }
        }
    }

    private void DoubleJump()
    {
        if (!_isGrounded && !_isDoubleJumped && _maxJump)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
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
        }

        if (_maxJump)
        {
            _gravity = 14;
        }
        else
        {
            _gravity = 9.81f;
        }
        JumpBuffering();
    }
    private void OnGround(Collision collision)
    {
        // if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 1))
        if (collision.gameObject.layer == _layerGround)
        {
            _isGrounded = true;
            _maxJump = false;
            _jump = 0;
            _isDoubleJumped = false;
            _gravity = 9.81f;
        }
        else
        {
            _maxJump = true;
            while (_jump > 0)
            {
                _jump -= Time.deltaTime;
            }
        }
    }
    private void OnOutGround(Collision collision)
    {
        if (collision.gameObject.layer == _layerGround)
        {
            _maxJump = false;
            _isGrounded = false;
        }
    }

   private void JumpBuffering()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit))
        {
            if (hit.collider.gameObject.layer == _layerGround)
            {
                if (hit.distance <= _jumpBufferingDistance)
                {
                    Debug.Log(1);
                    _isGrounded = true;
                    _maxJump = false;
                    _isDoubleJumped = false;
                }
            }
            if (hit.distance > _jumpBufferingDistance)
            {
                Debug.Log(2);
                _isGrounded = false;
            }
            else
            {

            }

        }
    }
    
    
    private void SetOnStart()
    {
        _currentSpeed = Speed;
        _layerGround = LayerMask.NameToLayer("Ground");
    }
}