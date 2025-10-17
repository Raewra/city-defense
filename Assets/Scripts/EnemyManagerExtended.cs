using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[System.Serializable]
public class Wave
{
    public int enemyCount;
    public float spawnInterval;
    public GameObject[] enemyTypes;
}
public class EnemyManagerExtended : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float generalHealth = 500f;
    public GameObject[] enemyPrefabs;

    [Header("Spawn Settings")]
    public Transform[] startPoints;   // Spawn positions
    public Transform gate;  
    
    [Header("Attack Points")]
    public Transform pointA;  // Left side attack point
    public Transform pointB;  // Right-side attack point
    
    public float spawnInterval = 1f;
    

    [Header("Gate Attack Settings")]
    public float attackRadius = 3f; 

    public GameManagerExtended gameManager;
    public TopDownControllerExtended player;
    
    [Header("Rewards")]
    public int baseRewardGoldPerEnemy = 10; 
    public int baseRewardSupplyPerEnemy = 8; 
    public float rewardMultiplierPerWave = 1.2f; 
    
    private float timer;

    public Wave[] waves;
    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;
    private bool spawningWave = false;

    List<Vector3> attackSlots = new List<Vector3>();
    int slotIndex = 0;
    
    

    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManagerExtended>();
        player = GameObject.FindWithTag("Player").GetComponent<TopDownControllerExtended>();
        GenerateSlots();
    }
    void Update()
    {
        HandleWaves();
    }

    void HandleWaves()
    {
        if (gameManager.gameState == GameManagerExtended.GameStates.Battle)
        {
            if (!spawningWave && enemiesAlive <= 0)
            {
                if (currentWaveIndex < waves.Length)
                {
                    StartCoroutine(SpawnWave(waves[currentWaveIndex]));
                }
                else
                {
                    gameManager.battleResult = "Battle Won";
                    gameManager.waveStars = 3;
                    gameManager.gameState = GameManagerExtended.GameStates.End;
                }
            }
        }
    }

    void GenerateSlots()
    {
        attackSlots.Clear();

        int enemiesPerLine = 5;
        float spacing = 0.5f;
        float distanceFromGate = 0.15f;

        Transform[] linePoints = { pointA, pointB };

        foreach (Transform point in linePoints)
        {
            if (point == null) continue;

            Vector3 gateForward = gate.forward;
            Vector3 gateRight = gate.right;

         
            Vector3 lineCenter = point.position - gateForward * distanceFromGate;

            for (int i = 0; i < enemiesPerLine; i++)
            {
                float t = (float)i / (enemiesPerLine - 1);
                Vector3 offset = gateRight * ((t - 0.5f) * (enemiesPerLine - 1) * spacing);
                Vector3 slotPos = lineCenter + offset;

                attackSlots.Add(slotPos);
            }
        }
    }


    IEnumerator SpawnWave(Wave wave)
    {
        spawningWave = true;

        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave.enemyTypes[Random.Range(0, wave.enemyTypes.Length)]);
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        spawningWave = false;
        currentWaveIndex++;
    }
    
    public int CalculateWaveReward(string reward)
    {
        float multiplier = Mathf.Pow(rewardMultiplierPerWave, currentWaveIndex);
        int rewardGold = Mathf.RoundToInt(baseRewardGoldPerEnemy * multiplier);
        int rewardSupply = Mathf.RoundToInt(baseRewardSupplyPerEnemy * multiplier);

        switch (reward)
        {
            case "Gold":
                return rewardGold;
            case "Supply":
                return rewardSupply;
            default:
                return 0;
        }
    }

    void SpawnEnemy(GameObject prefab)
    {
        if (startPoints.Length == 0 || enemyPrefabs.Length == 0) return;

        // Random spawn point
        Transform spawnPoint = startPoints[Random.Range(0, startPoints.Length)];

       
        
        GameObject e = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        enemiesAlive++;
        

        // Enemy Setup
        EnemyControllerExtended enemy = e.GetComponent<EnemyControllerExtended>();
        if (enemy != null)
        {
            enemy.health = Random.Range(50f, 150f);
            enemy.speed = Random.Range(1.5f, 3f);
            enemy.damage = Random.Range(1f, 2f);
            enemy.gameManager = gameManager;
            enemy.player = player.gameObject;
          

            Vector3 attackPoint = attackSlots[slotIndex % attackSlots.Count];
            slotIndex++;
            
            if (!NavMesh.SamplePosition(enemy.targetPosition, out var hit, 0.5f, NavMesh.AllAreas))
            {
                Debug.LogWarning($"Slot {slotIndex-1} is not on NavMesh! Enemy may not reach it.");
            }

            enemy.targetPosition = attackPoint; 
            enemy.gate = gate; 
            enemy.onDeath += () =>
            {
                enemiesAlive--;
                int rewardGold = CalculateWaveReward("Gold");
                int rewardSupply = CalculateWaveReward("Supply");
                gameManager.AddWaveData(rewardGold, rewardSupply);
            }; 
        }
    }
}

