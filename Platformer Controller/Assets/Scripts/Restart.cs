using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
   
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
