using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5;
    public float RunSpeed = 8;
    public float JumpForce = 3;
    [SerializeField] private float _gravity = -9.81f;

    public Rigidbody rb;

    private float _currentSpeed;
    private float _jump = 0;
    private bool _maxJump = false;
    private bool _isGrounded = true;
    private LayerMask _layerGround;

    private void Start()
    {
        SetOnStart();
    }

    private void Update()
    {
        Jump();
        Sprint();
        Fall();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    public void OnCollisionEnter(Collision collision)
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
            _jump += _gravity * Time.deltaTime;
        }
    }

    private void OnGround(Collision collision)
    {
        if (collision.gameObject.layer == _layerGround)
        {
            _isGrounded = true;
            _maxJump = false;
            _jump = 0;
        }
        else
        {
            _maxJump = true;
            while (_jump > 0)
            {
                _jump -= Time.deltaTime;
            };
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
    private void SetOnStart()
    {
        _currentSpeed = Speed;
        _layerGround = LayerMask.NameToLayer("Ground");
    }
}

