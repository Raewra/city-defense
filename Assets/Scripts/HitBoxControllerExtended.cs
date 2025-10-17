using UnityEngine;

public class HitBoxControllerExtended : MonoBehaviour
{
    public TopDownControllerExtended player;
    public EnemyControllerExtended enemy;

    public string character;
    

    private MonoBehaviour activeController;

    void Start()
    {
        if (character == "Player")
        {
            player = GameObject.FindWithTag("Player").GetComponent<TopDownControllerExtended>();
            activeController = player;
        }
        else if (character == "Enemy")
        {
            enemy = transform.parent.gameObject.GetComponent<EnemyControllerExtended>();
            activeController = enemy;
        }
        else
        {
            Debug.LogError("Character string not recognized. Use 'Player' or 'Enemy'.");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (activeController == enemy) 
        {
            if (other.CompareTag("Gate"))
            {
                enemy.currentTarget = "Gate";
                enemy.isAttacking = true;
            }
            else if(other.CompareTag("Player"))
            {
                enemy.currentTarget = "Player";
                enemy.isAttacking = true;
            }
        }
        else
        {
            if (other.CompareTag("Enemy"))
            {
                player.isAttacking = true;
            }
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (activeController == enemy)
        {
            if (other.CompareTag("Player"))
            {
                enemy.isAttacking = false;
                enemy.currentTarget = null;
            }
            else 
            {
                enemy.isAttacking = false;
                enemy.currentTarget = null;
            }
            
        }
        else
        {
            if (other.CompareTag("Enemy"))
            {
                player.isAttacking = false;
            }
        }

    }
}
