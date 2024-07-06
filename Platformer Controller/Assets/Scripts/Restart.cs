using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public GameObject spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.transform.position = spawnPoint.transform.position;
    }
}
