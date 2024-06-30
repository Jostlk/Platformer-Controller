using System.Collections;
using UnityEngine;

public class Buster : MonoBehaviour
{
    private PlayerController _player;
    public bool isActive;

    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        isActive = true;
    }

    private void OnEnable()
    {
        isActive = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        _player.isDoubleJumped = false;
        isActive = false;     
    }

    private void OnTriggerEnter(Collider other)
    {
        _player.isDoubleJumped = false;
        isActive = false;
    }

}
