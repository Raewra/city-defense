using UnityEngine;

public class AreaController : MonoBehaviour
{
    public BoxCollider box;
    public LayerMask enemyLayer;
    public TopDownControllerExtended playerController;
    
    
    void Start()
    {
        box = GetComponent<BoxCollider>();
        playerController = GetComponentInParent<TopDownControllerExtended>();
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // playerController.canAttack = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // playerController.canAttack = false;
        }
    }
}
