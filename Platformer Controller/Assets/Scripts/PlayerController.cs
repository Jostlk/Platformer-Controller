using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float Speed = 5;
    public float JumpForce = 3;
    private float _jump = 0;
    private float _gravity = -9.81f;
    private bool _isGrounded = true;
    private void Update()
    {
        var horizontale = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _jump = JumpForce;
            _isGrounded = false;
        }
        if (!_isGrounded)
        {
            _jump += _gravity * Time.deltaTime;
        }
        rb.velocity = new Vector3(0, _jump, horizontale) * Speed;
    }
    public void OnCollisionEnter(Collision collision)
    {
        _isGrounded = true;
        _jump = 0;
    }
}
