using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManagerExtended : MonoBehaviour
{
    public TopDownControllerExtended player;

    public Button char1;
    public Button char2;
    public Button char3;
    public Button char4;
    public Button char5;

    public Button upgradeTower1;
    public Button upgradeTower2;


    public Button ready;
    public Button respawn;
    
    public Button endExit;
    public Button endRestart;
    

    public TextMeshProUGUI gameLog;


    public GameObject tower1Sphere1;
    public GameObject tower1Sphere2;

    public GameObject tower2Sphere1;
    public GameObject tower2Sphere2;

    public int goldNumber = 100;
    public int supplies = 100;
    public TextMeshProUGUI goldText;

    public Transform[] machineSpawnPoints;
    public GameObject machinePrefab;

    public bool isPlayerDead = false;

    public float gateHealth = 2000;
    public float maxGateHealth = 2000;

    public Image gateHealthFillImage;
    public bool isGateDestroyed = false;

    // End Screen
    public GameObject endScreen;
    public TextMeshProUGUI endScreenHeader;
    public TextMeshProUGUI endScreenGold;
    public TextMeshProUGUI endScreenSupplies;
    public TextMeshProUGUI endScreenStars;

    public string battleResult;
  
    
    //Update Tower Info
    public GameObject[] towers;
    public GameObject markerPrefab;
    public List<GameObject> markers = new List<GameObject>();

    private List<GameObject> selections = new List<GameObject>();
    public Button archerIncrease;
    public GameObject gatePrefab;
    
    public Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, 0.5f); // size of the box
    public float maxDistance = 1000f;
    public LayerMask hitLayers; // filter layers to hit
    public LayerMask uiLayer;
    public LayerMask combinedMask;
    
    //Wave Data
    private int waveGold = 0;
    private int waveSupply = 0;
    public int waveStars = 0;
    
    private bool isUIActive = true;
    private bool endHandled = false;
    //Player Settings
    public enum AttackTypes
    {
        Arrow,
        Ball,
        Char1
    }

    public enum GameStates
    {
        Prepeare,
        Battle,
        End
    }

    public AttackTypes playerState = AttackTypes.Arrow;

    public GameStates gameState = GameStates.Prepeare;
  
    void Start()
    {
        battleResult = "Battle Loss";
        combinedMask = hitLayers | uiLayer;
        player = GameObject.FindWithTag("Player").GetComponent<TopDownControllerExtended>();
        
        gameLog = GameObject.FindWithTag("GameLog").GetComponentInChildren<TextMeshProUGUI>();
        
        char1.onClick.AddListener(OnChar1);
        char2.onClick.AddListener(OnChar2);
        char3.onClick.AddListener(OnChar3);
        char4.onClick.AddListener(OnChar4);
        char5.onClick.AddListener(OnChar5);
        
        ready.onClick.AddListener(OnReady);
        endRestart.onClick.AddListener(OnEndRestart);
        endExit.onClick.AddListener(OnEndExit);
        
    
        respawn.onClick.AddListener(OnRespawn);
        
        archerIncrease.onClick.AddListener(OnHandleArchers);
        ArcherButtonCheck();
        ResourceUpdate();
        
    }

   
    void Update()
    {

        switch (gameState)
        {
            case GameStates.Prepeare:
                gameLog.text = "Prepeare for battle";
                break;
            case GameStates.Battle:
                gameLog.text = "Battle has begun";
                break;
            case GameStates.End:
                gameLog.text = "Battle has ended";
                if (!endHandled)
                {
                    endHandled = true;
                    EndScreenHandler(battleResult, waveGold, waveSupply, waveStars);
                }
                break;
        }
        
        if(gameState == GameStates.End)
            return;
        
        ResourceUpdate();
        
        if (gameState == GameStates.Battle && isUIActive)
        {
            UIReset();
            isUIActive = false;
            return;
        }
        else if(gameState == GameStates.Battle && !isUIActive)
        {
            return;
        }
        
        ArcherButtonCheck();
        RespawnButtonCheck();
        
        if (Input.GetMouseButtonDown(0)) 
        {
            if(IsPointerOverUIElement(GetEventSystemRaycastResults()) == true)
                return;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

          
            if (Physics.BoxCast(ray.origin, boxHalfExtents, ray.direction, out RaycastHit hit, Quaternion.identity, maxDistance, combinedMask))
            {
                Debug.Log("Hit: " + hit.collider.name);

                if (hit.collider.CompareTag("Tower")) 
                {
                    Debug.Log("You clicked the Tower!");
                    
                    if (selections.Count == 0)
                    {
                        selections.Add(hit.collider.gameObject);
                        TowerController towerScript = hit.collider.GetComponentInChildren<TowerController>();
                        if (towerScript != null)
                        {
                            ClearMarkers();
                            GameObject markerInstance = Instantiate(markerPrefab, towerScript.markerSpawnPoint.position, Quaternion.identity);
                            Debug.Log("Spawned marker");
                            markers.Add(markerInstance);
                        }
                    }
                    else
                    {
                        selections.Clear();
                       
                        TowerController towerScript = hit.collider.GetComponentInChildren<TowerController>();
                        if (towerScript != null)
                        {
                            ClearMarkers();
                            GameObject markerInstance = Instantiate(markerPrefab, towerScript.markerSpawnPoint.position, Quaternion.identity);
                            Debug.Log("Spawned marker");
                            markers.Add(markerInstance);
                        }
                        selections.Add(hit.collider.gameObject);
                    }
                    
                  
                    
                }
                else if(hit.collider.CompareTag("UIElement"))
                {
                    
                }
                else
                {
                    selections.Clear();
                    ClearMarkers();
                }
                
            }
            else
            {
                selections.Clear();
                ClearMarkers();
            }

         
            Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.green, 1f);
        }
    }
    
    public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults )
    {
        for(int index = 0;  index < eventSystemRaysastResults.Count; index ++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults [index];

            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }

        return false;
    }
    
    
    static List<RaycastResult> GetEventSystemRaycastResults()
    {   
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position =  Input.mousePosition;

        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll( eventData, raysastResults );

        return raysastResults;
    }

    void IncreaseArcher(GameObject tower)
    {
        TowerController towerScript = tower.GetComponentInChildren<TowerController>();
        if (towerScript != null && goldNumber >= 10)
        {
            towerScript.AddArcher(); 
            goldNumber -= 10;
        }
    }
    
    
    public void AddWaveData(int gold, int supply)
    {
        waveGold += gold;
        waveSupply += supply;
    }
    
    public void ResetWaveData()
    {
        waveGold = 0;
        waveSupply = 0;
    }

    public void EndScreenHandler(string header, int gold, int supplies, int stars)
    {
        endScreenHeader.text = header;
        endScreenGold.text = "Gold: " + gold.ToString();
        endScreenSupplies.text = "Supplies: " + supplies.ToString();
        endScreenStars.text = "Stars: " + stars.ToString();
        endScreen.SetActive(true);

     

        // Set Data
        DataManager.Instance.SetDungeonStars("Dungeon1", stars);
        DataManager.Instance.AddGold(gold);
        DataManager.Instance.AddSupplies(supplies);
    }


    public void OnChar1()
    {
        playerState = GameManagerExtended.AttackTypes.Arrow;
    }

    public void OnChar2()
    {
        playerState = GameManagerExtended.AttackTypes.Ball;
    }

    public void OnChar3()
    {
        playerState = GameManagerExtended.AttackTypes.Char1;
    }

    public void OnChar4()
    {
        playerState = GameManagerExtended.AttackTypes.Ball;
    }

    public void OnChar5()
    {
        playerState = GameManagerExtended.AttackTypes.Ball;
    }

    public void OnReady()
    {
        gameState = GameManagerExtended.GameStates.Battle;
    }
    

    // End Screen
    public void OnEndExit()
    {
        SceneManager.LoadScene(0);
    }

    public void OnEndRestart()
    {
        ResetWaveData();
        endHandled = false;
        SceneManager.LoadScene(1);
    }

    public void OnTower1Upgrade()
    {
        tower1Sphere1.SetActive(false);
        tower1Sphere2.SetActive(true);
    }

    public void OnTower2Upgrade()
    {
        tower2Sphere1.SetActive(false);
        tower2Sphere2.SetActive(true);
    }

    public void ResourceUpdate()
    {
        goldText.text = "Gold: " + goldNumber.ToString() + " " + "Supplies: " + supplies.ToString();
    }

    public void OnRespawn()
    {
        
        if (supplies >= 10 && isPlayerDead)
        {
            supplies -= 10;
            ResourceUpdate();
            int number = Random.Range(0, machineSpawnPoints.Length);
            Instantiate(machinePrefab, machineSpawnPoints[number].position, machineSpawnPoints[number].rotation);
            isPlayerDead = false;
        }
    }

    void GateHealthUpdate()
    {
        gateHealthFillImage.fillAmount = gateHealth / maxGateHealth;
    }
    public void GateDamage(float damage)
    {
        gateHealth -= damage;
        GateHealthUpdate();
        if (gateHealth <= 0)
        {
            gameState = GameStates.End;
            Destroy(gatePrefab);
        }
    }
    
    // Selection Check
    void OnHandleArchers()
    {
        if (selections.Count > 0)
        {
            IncreaseArcher(selections[0]);
        }
        else
        {
            Debug.Log("No towers selected");
        }
    }

    void RespawnButtonCheck()
    {
        if (!isPlayerDead)
        {
            respawn.interactable = false;
        }
        else
        {
            respawn.interactable = true;
        }
    }

    void ArcherButtonCheck()
    {
        if (selections.Count == 0 && archerIncrease.interactable == true)
        {
            archerIncrease.interactable = false;
        }
        else if(selections.Count > 0 && archerIncrease.interactable == false)
        {
            archerIncrease.interactable = true;
        }
    }

    void ClearMarkers()
    {
        foreach (var marker in markers)
        {
            Debug.Log("Clearing markers");
            if (marker != null)
                Destroy(marker);
        }
        markers.Clear();
    }

    void UIReset()
    {
        ClearMarkers(); 
        selections.Clear();
        archerIncrease.interactable = false;
    }
}
