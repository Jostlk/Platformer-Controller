using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float RunSpeed;
    public float JumpForce;
    public float slidindSpeed;
    [HideInInspector] public bool isDoubleJumped;
    [SerializeField] private float _gravity;
    [SerializeField] private float _jumpBufferingDistance;
    [SerializeField] private float _fallGravity;

    public Rigidbody rb;
    public CapsuleCollider capsuleCollider;
    public Animator animator;

    private bool _turnLeft = false;
    private bool _turnRight = false;
    private bool _isRun = false;
    private bool _isMove = false;
    private bool _maxJump;
    private bool _isGrounded;
    private bool _isOnWall;
    private bool _standUp = false;
    private float _coyoteTimeCounter;
    private float _horizontale;
    private float _currentSpeed;
    private float _jump;
    private float _jumpTime = 0;
    private bool SpaceUp = false;
    private LayerMask _layerGround;
    private LayerMask _layerWall;

    private void Start()
    {
        SetOnStart();
    }

    private void Update()
    {
        Debug.DrawRay(transform.position + Vector3.up + Vector3.back * 0.5f, Vector3.up * 3);
        Sit();
        Jump();
        DoubleJump();
        Sprint();
        Fall();
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

    private void OnCollisionStay(Collision collision)
    {
        Sliding(collision);
    }

    private void Movement()
    {
        _horizontale = Input.GetAxis("Horizontal");
        var move = _horizontale * _currentSpeed;
        if (move == 0)
        {
            _isMove = false;
            AudioManager.instance.Stop("Walk");
            AudioManager.instance.Stop("Run");
            animator.SetBool("Walk", false);
            animator.SetBool("Shift", false);
        }
        else
        {
            _isMove = true;
            if (_isGrounded && !_isRun)
            {
                AudioManager.instance.Play("Walk");
            }
            if (move < 0 && !_turnLeft)
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
                _turnLeft = true;
                _turnRight = false;
            }
            else if (move > 0 && !_turnRight)
            {
                transform.localEulerAngles = new Vector3(0, 180, 0);
                _turnRight = true;
                _turnLeft = false;
            }
            animator.SetBool("Walk", true);
        }
        rb.velocity = new Vector3(0, _jump, move);
    }

    private void Sit()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && _isGrounded)
        {
            StopRun();
            _standUp = false;
            animator.SetBool("Sit", true);
            capsuleCollider.center = new Vector3(0, 1.4f, 0);
            capsuleCollider.radius = 1.4f;
            capsuleCollider.height = 2.8f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) && _isGrounded)
        {
            _standUp = true;
        }
        else if (_standUp)
        {
            TryStandUp();
        }
    }

    private void Jump()
    {
        if (!_isOnWall)
        {
            if (Input.GetKey(KeyCode.Space) && !_maxJump)
            {
                if (_jump <= 0)
                {
                    _maxJump = true;
                }
                _jumpTime += Time.deltaTime;
            }
            if (Input.GetKeyDown(KeyCode.Space) && _coyoteTimeCounter > 0)
            {
                ResetColider();
                AudioManager.instance.Play("Jump");
                animator.SetTrigger("Jump");
                _isGrounded = false;
                _jump = JumpForce;
                _jumpTime = 0.02f;
                SpaceUp = false;
                isDoubleJumped = true;
            }
            else if (Input.GetKeyUp(KeyCode.Space) && !_isGrounded)
            {
                SpaceUp = true;
                _maxJump = true;
            }
        }
    }

    private void DoubleJump()
    {
        if (!_isGrounded && !isDoubleJumped)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!_isOnWall)
                {
                    AudioManager.instance.Play("Jump");
                    animator.SetTrigger("Jump");
                }
                _maxJump = false;
                _jump = JumpForce;
                isDoubleJumped = true;
                _jumpTime = 0.02f;
                SpaceUp = false;
            }
        }
    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _isGrounded && _isMove && !animator.GetBool("Sit"))
        {
            _isRun = true;
            AudioManager.instance.Stop("Walk");
            AudioManager.instance.Play("Run");
            animator.SetBool("Shift", true);
            _currentSpeed = RunSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            StopRun();
        }
    }

    private void Fall()
    {
        if (!_isGrounded && !_isOnWall)
        {
            if (_jump >= 0 && SpaceUp)
            {
                _jump -= _jumpTime + _gravity * Time.deltaTime;
            }
            else
            {
                _jump -= _gravity * Time.deltaTime;
            }
            if (_coyoteTimeCounter > 0)
            {
                isDoubleJumped = false;
            }
            _coyoteTimeCounter -= Time.deltaTime;
        }
        else
        {
            _jump = 0;
            _coyoteTimeCounter = 0.3f;
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
            ResetColider();
            AudioManager.instance.Play("Landing");
            animator.SetBool("Grounded", true);
            _isGrounded = true;
        }
        animator.SetBool("ClimbUp", false);
        animator.SetBool("ClimbDown", false);
        _jump = 0;
        _maxJump = true;
    }

    private void OnOutGround(Collision collision)
    {
        if (collision.gameObject.layer == _layerGround)
        {
            ResetColider();
            animator.SetBool("Grounded", false);
            _maxJump = false;
            _isGrounded = false;
        }
        AudioManager.instance.Stop("Walk");
        AudioManager.instance.Stop("Run");
        animator.SetBool("Climb", false);
        animator.SetBool("ClimbUp", false);
        animator.SetBool("ClimbDown", false);
        _isOnWall = false;
    }

    private void Sliding(Collision collision)
    {
        if (collision.gameObject.layer == _layerWall)
        {
            capsuleCollider.radius = 0.6f;
            animator.SetBool("Climb", true);
            if (Input.GetKey(KeyCode.W))
            {
                animator.SetBool("ClimbUp", true);
                animator.SetBool("ClimbDown", false);
                rb.AddForce(0, slidindSpeed * Time.deltaTime, 0);
            }
            else
                animator.SetBool("ClimbUp", false);

            if (Input.GetKey(KeyCode.S))
            {
                animator.SetBool("ClimbUp", false);
                animator.SetBool("ClimbDown", true);
                rb.AddForce(0, -(slidindSpeed * Time.deltaTime), 0);
            }
            else
                animator.SetBool("ClimbDown", false);

            _isOnWall = true;
            isDoubleJumped = false;
        }
    }


    private void TryStandUp()
    {
        var BackRay = Physics.Raycast(transform.position + Vector3.up + Vector3.back, Vector3.up, 3);
        var FrontRay = Physics.Raycast(transform.position + Vector3.up + Vector3.forward, Vector3.up, 3);
        if (!BackRay && !FrontRay)
        {
            ResetColider();
            _standUp = false;
        }
    }

    private void ResetColider()
    {
        animator.SetBool("Sit", false);
        capsuleCollider.center = new Vector3(0, 1.8f, 0);
        capsuleCollider.radius = 1;
        capsuleCollider.height = 3.57f;
    }

    private void StopRun()
    {
        _isRun = false;
        AudioManager.instance.Stop("Run");
        animator.SetBool("Shift", false);
        _currentSpeed = Speed;
    }
    private void SetOnStart()
    {
        _currentSpeed = Speed;
        _isGrounded = false;
        isDoubleJumped = true;
        _layerGround = LayerMask.NameToLayer("Ground");
        _layerWall = LayerMask.NameToLayer("Wall");
    }
}