using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;


    void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, player.transform.position.y + 2, player.transform.position.z);
    }
}
