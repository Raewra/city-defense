using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ProjectileControllerExtended : MonoBehaviour
{
    [HideInInspector] public Vector3 moveDirection; // direction of projectile
    public float speed = 10f;
    public float lifetime = 5f;
    public float damage = 20f;

    public string controllerCharacter;

    void Start()
    {
        // Destroy 
        Destroy(gameObject, lifetime);

        // Rotate projectile
        if (moveDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    void Update()
    {
        // Move projectile 
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(controllerCharacter == "Player")
        {
          
            if (other.CompareTag("Enemy"))
            {
            
                EnemyControllerExtended script = other.GetComponent<EnemyControllerExtended>();
                if (script != null)
                {
                    script.TakeDamage(damage);
                }

                Destroy(gameObject);
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
              
                TopDownControllerExtended script = other.GetComponent<TopDownControllerExtended>();
                if (script != null)
                {
                   script.Damage(damage);
                }

                Destroy(gameObject); 
            }
        }


        
        if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Destroy(gameObject);
        }
    }
}
