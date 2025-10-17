using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class TopDownControllerExtended : MonoBehaviour
{
    public Camera mainCamera;
    public float moveSpeed = 6f;

    private CharacterController characterController;

    public bool isAttacking = false;  
    
    public GameManagerExtended gameManager;

    //Range Control
    public GameObject hitBoxRange;
    public GameObject hitBoxMalee;


    public float gravity = -9.81f;
    private float verticalVelocity;

    public Transform spawnPoint;

    //Weapon Types
    public GameObject arrow;
    public GameObject ball;

    public float currentDamage;
    public float fireSpeed;
    public float fireInterval;

    private float lastFireTime; // Cooldown tracker

    public AreaController enemyChecker;


    //Health
    public float maxHealth = 300;
    public float health = 300;

    public float maxMana = 300;
    public float mana = 300;

    public Image healthFillImage;
    public Image manaFillImage;

    public LayerMask enemyLayer;
    public float radius = 5f;
    public GameObject aoePrefab;
    public float fillDuration = 2f;


    public bool isManaFillCalled = false;
    
    
    //Animations
    
    public Animator animatorTank;
    public GameObject tankBody;
    


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManagerExtended>();
      
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        enemyChecker = GetComponentInChildren<AreaController>();
        healthFillImage = GameObject.FindWithTag("MachineHealth").GetComponent<Image>();
        manaFillImage = GameObject.FindWithTag("Mana").GetComponent<Image>();
       
        HealthUpdate();
  
        
        tankBody = GameObject.FindWithTag("TankBody");
        animatorTank = tankBody.GetComponent<Animator>();
    }

    void Update()
    {
        if (gameManager.gameState == GameManagerExtended.GameStates.End || 
            gameManager.gameState == GameManagerExtended.GameStates.Prepeare)
        {
            verticalVelocity = 0f;
            characterController.enabled = false;
            return;
        }
        else
        {
            characterController.enabled = true;
        }

        HandleMovement();
        HandleAttack();
        
    }

    void SpendMana(float amount)
    {
        mana -= amount;
        ManaUpdate();

        if (mana <= 0)
        {
            StartCoroutine(FillManaBar(1f, manaFillImage));
        }
    }

    IEnumerator FillManaBar(float targetValue, Image bar)
    {
        
        float startValue = bar.fillAmount;
        float time = 0f;

        while (time < fillDuration)
        {
            time += Time.deltaTime;
            bar.fillAmount = Mathf.Lerp(startValue, targetValue, time / fillDuration);
            yield return null; 
        }

        bar.fillAmount = targetValue; 
        mana = maxMana;
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > lastFireTime + fireInterval) 
        {
            
            lastFireTime = Time.time; 

            animatorTank.SetBool("isAttacking", true); // <-- set attack animation
            
            if (gameManager.playerState == GameManagerExtended.AttackTypes.Arrow)
            {
                currentDamage = 20;
                fireSpeed = 5;
                fireInterval = 1;
                HandleShoot(arrow);
            }
            else if (gameManager.playerState == GameManagerExtended.AttackTypes.Ball)
            {
                currentDamage = 50;
                fireSpeed = 3;
                fireInterval = 2;
                HandleShoot(ball);
            }
            else if(gameManager.playerState == GameManagerExtended.AttackTypes.Char1)
            {
                HandleSpecialAttack(Input.mousePosition);
            }
            StartCoroutine(ResetAttackBool());
        }
      
       
    }

    IEnumerator ResetAttackBool()
    {
        yield return new WaitForSeconds(0.5f);
        if (animatorTank.GetBool("isAttacking") == true)
        {
            animatorTank.SetBool("isAttacking", false);
        }
    }

    void HandleShoot(GameObject prefab)
    {
        GameObject projectile = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        ProjectileControllerExtended projectileScript = projectile.GetComponent<ProjectileControllerExtended>();
        animatorTank.SetBool("isAttacking", true);
        if (projectileScript != null)
        {
            projectileScript.speed = fireSpeed;

           
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                projectileScript.moveDirection = spawnPoint.transform.forward;
            }
        }

    }

    void HandleSpecialAttack(Vector3 currentMousePos)
    {
        if(mana >= 20)
        {
            Ray ray = Camera.main.ScreenPointToRay(currentMousePos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 aoeCenter = hit.point;
                
                if (aoePrefab != null)
                {
                    GameObject effect = Instantiate(aoePrefab, aoeCenter, Quaternion.identity);
                    SpendMana(20);
                    Destroy(effect, 2f); 
                }
                
                Collider[] enemies = Physics.OverlapSphere(aoeCenter, radius, enemyLayer);
                foreach (Collider enemy in enemies)
                {
                    if (enemy.CompareTag("Enemy"))
                    {
                        Destroy(enemy.gameObject); 
                    }
                }

                // Debug 
                Debug.DrawLine(aoeCenter, aoeCenter + Vector3.up * 2f, Color.red, 1f);
                Debug.Log($"AOE hit {enemies.Length} enemies");
            }
        }
        else
        {
            Debug.Log("Not enough mana");
        }
        
    }

    void HealthUpdate()
    {
       healthFillImage.fillAmount = health / maxHealth;
    }

    void ManaUpdate()
    {
        manaFillImage.fillAmount = mana / maxMana;
    }


    public void Damage(float damage)
    {
        health -= damage;
        HealthUpdate();
        if (health <= 0)
        {
            gameManager.isPlayerDead = true;
            Destroy(gameObject);
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 flatInput = new Vector3(moveX, 0f, moveZ);
        Vector3 flatMove = Vector3.zero;
        if (flatInput.sqrMagnitude > 0.0001f)
            flatMove = flatInput.normalized * moveSpeed;

        if (characterController.isGrounded)
            verticalVelocity = -1f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = flatMove + Vector3.up * verticalVelocity;

        if (flatMove.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatMove, Vector3.up);
            targetRotation *= Quaternion.Euler(0f, 90f, 0f);

            tankBody.transform.rotation = Quaternion.Slerp(
                tankBody.transform.rotation,
                targetRotation,
                Time.deltaTime * 10f
            );
        }

        characterController.Move(move * Time.deltaTime);
    }

    
}
