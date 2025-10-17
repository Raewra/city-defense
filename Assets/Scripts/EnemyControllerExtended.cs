using System.Collections;
using UnityEngine;
using UnityEngine.AI; 


public class EnemyControllerExtended : MonoBehaviour
{
    public bool isAttacking = false;

    [Header("References")]
    public EnemyManagerExtended enemyManager;
    public EnemyStatsExtended stats; 
    public Transform enemyFill;
    public GameObject arrowPrefab;
    public Transform spawnPoint;
    public HitBoxControllerExtended hitBoxController;
    public GameObject player;

    [Header("Stats")]
    public float health;
    public float speed;
    public float damage;

    [Header("Combat")]
    public float attackCooldown = 1f; 
    private float attackTimer = 0f;

    [Header("Gate Attack")]
    public Vector3 targetPosition;  
    public Transform gate;         
    public float attackRange = 1f;

    // private float nextAttackTime = 0f;
    private float currentHealth;
    private float maxHealth;
    private bool isDead = false;
    public string currentTarget;
    
   

    public GameObject gateObject;


    // Enemy Object
    public GameObject enemyObject;
    public Animator enemyAnimator;
    
    // NavMesh
    private NavMeshAgent agent;
    public System.Action onDeath;
    public GameManagerExtended gameManager;
    
    private bool isInAttackRange = false;
    public bool isDying = false;
    
    private EnemyStates currentState;
    private enum  EnemyStates
    {
        Move,
        Attack,
        Die
    }
    
    void Start()
    {
       
        maxHealth = stats.health;
        currentHealth = maxHealth;
        attackTimer = attackCooldown;

        enemyManager = GameObject.FindWithTag("Manager").GetComponent<EnemyManagerExtended>();
        hitBoxController = transform.GetChild(0).gameObject.GetComponent<HitBoxControllerExtended>();
        
        
        enemyAnimator = GetComponentInChildren<Animator>();
        
       
        agent = GetComponent<NavMeshAgent>();
       
        

        HealthUpdate();
        currentState = EnemyStates.Move;
       
    }

    void Update()
    {
        if (gate == null || agent == null || gameManager.gameState == GameManagerExtended.GameStates.End)
            return;
        
        DistanceCheck();
        switch (currentState)
        {
            case EnemyStates.Move:
                HandleMovement();
                break;
            case EnemyStates.Attack:
                HandleAttack();
                break;
            case EnemyStates.Die:
                if(!isDying)
                    HandleDeath();
                break;
        }
        
        
    }

    void DistanceCheck()
    {
        float distToGate = Vector3.Distance(transform.position, gate.transform.position);
        float distToSlot = Vector3.Distance(transform.position, targetPosition);
        
        
        if (distToSlot <= agent.stoppingDistance + 0.2f && !agent.isStopped)
        {
            agent.isStopped = true;
        }
        
    
        if (!isInAttackRange && distToGate <= attackRange)
        {
            isInAttackRange = true;
            agent.isStopped = true;
            currentState = EnemyStates.Attack;
            
        }
      
        else if (isInAttackRange && distToGate > attackRange + 0.2f)
        {
            isInAttackRange = false;
            agent.isStopped = false;
            currentState = EnemyStates.Move;
        }
        
    }

    void HandleAttack()
    {
        FaceGate();
        enemyAnimator.SetBool("isMoving", false);
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            StartCoroutine(HandleAttackAnimation());
            AttackTarget();
            attackTimer = attackCooldown;
        }
        
    }
    
    void FaceGate()
    {
        Vector3 dir = gate.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
    }

    void HandleMovement()
    {
        if (agent != null)
        {
            agent.speed = speed;
            agent.SetDestination(targetPosition);
            enemyAnimator.SetBool("isMoving", true);
            
            Vector3 lookDir = gate.transform.position - transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);
        }
    }

    void HandleDeath()
    {
        isDying = true;
        onDeath?.Invoke();
        StartCoroutine(HandleDeathAnimation());
    }


    IEnumerator HandleAttackAnimation()
    {
        enemyAnimator.SetTrigger("Attack");
        yield return null;
    }
    
    IEnumerator HandleDeathAnimation()
    {
        enemyAnimator.SetTrigger("isDead");
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    void AttackTarget()
    {
        gameManager.GateDamage(damage);
    }

    public void TakeDamage(float dmg)
    {
        if (isDead) return;
        
        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        HealthUpdate();

        if (currentHealth <= 0)
        {
            isDead = true;
            currentState = EnemyStates.Die;
        }
            
    }

    void HealthUpdate()
    {
        float ratio = currentHealth / maxHealth;


        Vector3 scale = enemyFill.localScale;
        scale.x = ratio;
        enemyFill.localScale = scale;

      
        Vector3 pos = enemyFill.localPosition;
        float fullWidth = 1f; 
        pos.x = (ratio - 1f) * (fullWidth * 0.5f);
        enemyFill.localPosition = pos;
    }
}
