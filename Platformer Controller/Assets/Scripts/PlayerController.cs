using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float Speed = 5;
    public float RunSpeed = 8;
    private float _currentSpeed;
    public float JumpForce = 3;
    private float _jump = 0;
    private float _gravity = -9.81f;
    private bool _isGrounded = true;
    private void Start()
    {
        _currentSpeed = Speed;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _jump = JumpForce;
            _isGrounded = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && _isGrounded)
        {
            _currentSpeed = RunSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _currentSpeed = Speed;
        }
        if (!_isGrounded)
        {
            _jump += _gravity * Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        var horizontale = Input.GetAxis("Horizontal");
        rb.velocity = new Vector3(0, _jump, horizontale * _currentSpeed);
    }
    public void OnCollisionEnter(Collision collision)
    {
        _isGrounded = true;
        _jump = 0;
    }
}
