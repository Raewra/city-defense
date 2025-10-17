using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class TowerController : MonoBehaviour
{
    [Header("Combat Settings")]
    public float fireRate = 1f; // Arrow fire rate
    public int attackDamage = 5; // Arrow damage
    public GameObject arrowPrefab; 
    public Transform firePoint;

    [Header("Archer Crew")]
    public int archerCount = 3; 
    public int maxArchers = 10;
    public List<Transform> allFirePoints; // Arrow spawn spots

    private List<Transform> activeFirePoints = new List<Transform>();

    private List<GameObject> enemiesInRange = new List<GameObject>();
    private float fireCooldown;
    
    public Transform markerSpawnPoint;
    
    public UIDocument uiDocument;
    private VisualElement root;
    private Label archerLabel;
    
    [SerializeField] private VisualTreeAsset uiAsset;
    

    void Start()
    {
      
        for (int i = 0; i < archerCount && i < allFirePoints.Count; i++)
        {
            activeFirePoints.Add(allFirePoints[i]);
        }
        
        // UI
        root = uiAsset.CloneTree();
        uiDocument.rootVisualElement.Clear();
        uiDocument.rootVisualElement.Add(root);
        archerLabel = root.Q<Label>("Storage");
        archerLabel.text = "Archers: " + archerCount;
    }

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (enemiesInRange.Count > 0 && fireCooldown <= 0f)
        {
            StartCoroutine(FireVolley());
            fireCooldown = 1f / fireRate;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }
    
    public void AddArcher(int amount = 1)
    {
        int oldCount = archerCount;
        archerCount = Mathf.Min(archerCount + amount, maxArchers);
        Debug.Log(archerCount);
        archerLabel.text = "Archers: " + archerCount;
        for (int i = oldCount; i < archerCount && i < allFirePoints.Count; i++)
        {
            activeFirePoints.Add(allFirePoints[i]);
        }
    }

    GameObject FindRandomEnemy()
    {
        enemiesInRange.RemoveAll(e => e == null); // clean up dead enemies
        if (enemiesInRange.Count == 0) return null;
        return enemiesInRange[Random.Range(0, enemiesInRange.Count)];
    }

    IEnumerator FireVolley()
    {
        for (int i = 0; i < archerCount; i++)
        {
            GameObject target = FindRandomEnemy();
            if (target != null && activeFirePoints.Count > 0)
            {
                float delay = Random.Range(0f, 0.2f);
                yield return new WaitForSeconds(delay);
                
                // Check if enemy died early
                if (target == null) continue;
                
                // Pick a fire point by index
                Transform firePoint = activeFirePoints[i % activeFirePoints.Count];
                Shoot(target, firePoint);
            }
        }
    }

    void Shoot(GameObject target, Transform firePoint)
    {
        if (target == null) return; 
        
        GameObject arrow = Instantiate(
            arrowPrefab,
            firePoint.position,
            Quaternion.LookRotation(target.transform.position - firePoint.position)
        );

        Destroy(arrow, 1f);

        EnemyControllerExtended enemy = target.GetComponent<EnemyControllerExtended>();
        if (enemy != null && !enemy.Equals(null))
        {
            enemy.TakeDamage(attackDamage);
        }
    }

    
}
