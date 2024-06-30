using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Player;
    public float CameraSpeed;
    void Update()
    {
        Vector3 newCamPosition = new Vector3(transform.position.x, Player.position.y + 3, Player.position.z);
        transform.position = Vector3.Lerp(transform.position, newCamPosition, CameraSpeed * Time.deltaTime);
    }
}
