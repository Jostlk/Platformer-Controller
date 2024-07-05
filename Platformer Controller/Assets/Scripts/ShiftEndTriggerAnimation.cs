using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftEndTriggerAnimation : MonoBehaviour
{
    public CapsuleCollider capsuleCollider;
    public PlayerController playerController;
    private void OnTriggerEnter(Collider other)
    {
        if (!TriggerAnimation._isToWallClimb)
        {
            capsuleCollider.enabled = true;
            playerController.enabled = true;
            TriggerAnimation._isFirstAction = true;
        }
    }
}
