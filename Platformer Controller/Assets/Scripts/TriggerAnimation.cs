using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
    //Направления движения в которых можно использовать анимацию
    public bool Right;
    public bool Left;

    public bool ToWallClimb;
    public bool SprintToWallClimb;
    public PlayerController playerController;
    public Animator animator;
    public CapsuleCollider capsuleCollider;
    public static bool _isFirstAction = true;
    public static bool _OnTrigger = false;
    public static bool _isToWallClimb;

    private void OnTriggerStay(Collider other)
    {
        _OnTrigger = true;
        if (other.tag == "Player" && playerController._isGrounded)
        {
            if (ToWallClimb)
            {
                _isToWallClimb = true;
            }
            else
                _isToWallClimb = false;
            if (Input.GetKey(KeyCode.Space) && _isFirstAction && IsFacingCorrectDirection())
            {
                playerController.enabled = false;
                capsuleCollider.enabled = false;
                _isFirstAction = false;
                if (_isToWallClimb)
                {
                    animator.SetTrigger("ToWallClimb");
                    playerController.transform.position = new Vector3(playerController.transform.position.x,
                    playerController.transform.position.y, transform.position.z);
                    if (!IsInvoking())
                    {
                        Invoke("OnActive", 3.6f);
                    }
                }
                else if (SprintToWallClimb && !_isToWallClimb)
                {
                    animator.SetTrigger("SprintToWallClimb");
                    if (!IsInvoking())
                    {
                        Invoke("OnActive", 1.5f);
                    }
                }

            }
            else
                _OnTrigger = false;
        }
    }
    private void OnActive()
    {
        playerController.enabled = true;
        capsuleCollider.enabled = true;
        _isFirstAction = true;
    }
    private bool IsFacingCorrectDirection()
    {
        Vector3 forward = playerController.transform.forward;
        Vector3 rightDirection = transform.forward; // Направление, в котором должна быть стена

        if (Right)
        {
            return Vector3.Dot(forward, rightDirection) > 0.9f; // Проверьте, смотрит ли игрок вперед
        }
        else if (Left)
        {
            return Vector3.Dot(forward, -rightDirection) > 0.9f; // Проверьте, смотрит ли игрок назад
        }
        return false;
    }
    private void OnTriggerExit(Collider other)
    {
        _OnTrigger = false;
    }
}
