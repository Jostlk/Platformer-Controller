using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float RunSpeed;
    public float JumpForce;
    public float dashKD;
    public float dashForseXThousand;
    public bool isDashed;
    [HideInInspector] public bool isDoubleJumped;
    [SerializeField] private float _gravity;
    [SerializeField] private float _jumpBufferingDistance;
    [SerializeField] private float _fallGravity;

    public Rigidbody rb;
    public Animator animator;

    private bool _turnLeft = false;
    private bool _turnRight = false;
    private bool _maxJump;
    private bool _isGrounded;
    private float _coyoteTimeCounter;
    private float _horizontale;
    private float _currentSpeed;
    private float _jump;
    private float _dashKDTime;
    private LayerMask _layerGround;

    private void Start()
    {
        SetOnStart();
    }

    private void Update()
    {
        Jump();
        DoubleJump();
        Sprint();
        Fall();
        Dash();
        DashKD();
        Debug.DrawRay(transform.up * 2, -transform.forward);
    }

    private void FixedUpdate()
    {
        JumpBuffering();
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
        _horizontale = Input.GetAxis("Horizontal");
        var move = _horizontale * _currentSpeed;
        if (move == 0)
        {
            animator.SetBool("Walk", false);
        }
        else
        {
            if (move < 0 && !_turnLeft)
            {
                transform.Rotate(0, 180, 0);
                _turnLeft = true;
                _turnRight = false;
            }
            else if(move > 0 && !_turnRight)
            {
                transform.Rotate(0, 180, 0);
                _turnRight = true;
                _turnLeft = false;
            }
            animator.SetBool("Walk", true);
        }
        rb.velocity = new Vector3(0, _jump, move);
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
            animator.SetTrigger("Jump");
            _isGrounded = false;
            _jump = JumpForce;
            isDoubleJumped = true;

        }
        else if (Input.GetKeyUp(KeyCode.Space) && !_isGrounded)
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
        if (!_isGrounded && !isDoubleJumped)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                animator.SetTrigger("Jump");
                _maxJump = false;
                _jump = JumpForce;
                isDoubleJumped = true;
            }
        }
    }

    private void Sprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _isGrounded)
        {
            animator.SetBool("Shift", true);
            _currentSpeed = RunSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("Shift", false);
            _currentSpeed = Speed;
        }
    }

    private void Fall()
    {
        if (!_isGrounded)
        {
            _jump -= _gravity * Time.deltaTime;
            if (_coyoteTimeCounter > 0)
            {
                isDoubleJumped = false;
            }
            _coyoteTimeCounter -= Time.deltaTime;
        }
        else
        {
            _jump = 0;
            _coyoteTimeCounter = 0.4f;
        }

        if (_maxJump)
        {
            _gravity = _fallGravity;
        }
        else
        {
            _gravity = 9.81f;
        }
    }
    private void JumpBuffering()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            if (hit.distance < _jumpBufferingDistance && !_isGrounded)
            {
                _coyoteTimeCounter = 0.2f;
                _maxJump = false;
                isDoubleJumped = false;
                _gravity = 9.81f;
            }
        }
    }

    private void OnGround(Collision collision)
    {
        if (collision.gameObject.layer == _layerGround)
        {
            animator.SetBool("Grounded", true);
            _isGrounded = true;
        }
        _jump = 0;
        _maxJump = true;
        Sliding();
    }

    private void OnOutGround(Collision collision)
    {
        if (collision.gameObject.layer == _layerGround)
        {
            animator.SetBool("Grounded", false);
            _maxJump = false;
            _isGrounded = false;
            _dashKDTime = 0;
            isDashed = false;
        }
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isDashed)
        {
            _horizontale = Input.GetAxis("Horizontal");

            if (_horizontale > 0)
            {
                rb.AddForce(Vector3.forward * dashForseXThousand * 1000);
            }
            else
            {
                rb.AddForce(Vector3.back * dashForseXThousand * 1000);
            }
            isDashed = true;
            if (_isGrounded)
            {
                _dashKDTime = dashKD;
            }
        }
    }
    private void DashKD()
    {
        if (_isGrounded && isDashed)
        {
            _dashKDTime -= Time.deltaTime;
            if (_dashKDTime <= 0)
            {
                isDashed = false;
            }
        }
    }

    private void Sliding()
    {
        if (Physics.Raycast(transform.up * 2, -transform.forward, out RaycastHit hit) || Physics.Raycast(transform.up * 2, transform.forward, out RaycastHit hit1))
        {
            isDoubleJumped = false;
            isDashed = false;
        }
    }


    private void SetOnStart()
    {
        _currentSpeed = Speed;
        _isGrounded = false;
        isDoubleJumped = true;
        _layerGround = LayerMask.NameToLayer("Ground");
    }
}