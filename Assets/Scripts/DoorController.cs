using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public bool canExit = false;
 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canExit)
        {
            SceneManager.LoadScene(2);
        }
    }
}
