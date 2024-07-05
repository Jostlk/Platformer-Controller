using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float RunSpeed;
    public float JumpForce;
    [HideInInspector] public bool isDoubleJumped;
    [HideInInspector] public bool _isGrounded;
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
    private bool _standUp = false;
    private float _coyoteTimeCounter;
    private float _horizontale;
    private float _currentSpeed;
    private float _jump;
    private float _jumpTime = 0;
    private float _fallTime = 0;
    private bool _stopActive = false;
    private bool SpaceUp = false;
    private LayerMask _layerGround;

    private void Start()
    {
        SetOnStart();
    }

    private void Update()
    {
        if (!_stopActive)
        {
            Jump();
            DoubleJump();
            Debug.DrawRay(transform.position + Vector3.up + Vector3.back * 0.5f, Vector3.up * 3);
            Sit();
            Sprint();
            Fall();
        }
    }

    private void FixedUpdate()
    {
        if (!_stopActive)
        {
            JumpBuffering();
            Movement();
        }
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
            _isRun = false;
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
                transform.localEulerAngles = new Vector3(0, 180, 0);
                _turnLeft = true;
                _turnRight = false;
            }
            else if (move > 0 && !_turnRight)
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
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
            if (_isRun)
            {
                AudioManager.instance.Play("RunningSlide");
                AudioManager.instance.Stop("Run");
                if (!IsInvoking())
                {
                    Invoke("StopRun", 1);
                }
            }
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
        if (!TriggerAnimation._OnTrigger)
        {
            if (Input.GetKey(KeyCode.Space) && !_maxJump)
            {
                if (_jump <= 0)
                {
                    _maxJump = true;
                }
                _jumpTime += Time.deltaTime;
                _fallTime = 0;
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
                _stopActive = false;
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
        if (!_isGrounded && !isDoubleJumped && !TriggerAnimation._OnTrigger)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AudioManager.instance.Play("Jump");
                animator.SetTrigger("Jump");
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
        Debug.Log(_jump);
        if (!_isGrounded)
        {
            VeryFall();
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
            _fallTime = 0;
            _jump = 0;
            _coyoteTimeCounter = 0.3f;
        }

        if (_maxJump)
        {
            _gravity = _fallGravity;
        }
        else
        {
            _gravity = 15f;
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
            if (animator.GetBool("VeryFall"))
            {
                _stopActive = true;
                if (!IsInvoking())
                {
                    Invoke("PlayActive", 1.5f);
                }
            }
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

    private void VeryFall()
    {
        _fallTime += Time.deltaTime;
        if (_fallTime >= 1)
        {
            animator.SetBool("VeryFall", true);
        }
        else
            animator.SetBool("VeryFall", false);
    }
    private void PlayActive()
    {
        _fallTime = 0;
        rb.AddForce(0, 40, 0, ForceMode.VelocityChange);
        _stopActive = false;
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
    }
}