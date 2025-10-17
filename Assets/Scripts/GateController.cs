using UnityEngine;

public class GateController : MonoBehaviour
{
    public float maxHealth = 1000f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Debug.Log("Gate destroyed!");
        
        }
    }
}
