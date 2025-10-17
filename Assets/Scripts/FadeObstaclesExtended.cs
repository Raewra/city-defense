using UnityEngine;

public class ObstacleFadeExtended : MonoBehaviour
{
    public Transform player;
    public LayerMask obstacleMask;
    public float fadedAlpha = 0.3f;
    public float fadeSpeed = 5f;

    private Renderer currentRenderer;
    public GameManagerExtended gameManager;
    private float[] defaultAlphas;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManagerExtended>();
    }

    void Update()
    {
        
        if (gameManager == null || gameManager.gameState == GameManagerExtended.GameStates.End)
        {
            ResetObstacle();
            return;
        }

        if (player == null || gameManager.isPlayerDead)
        {
            ResetObstacle();
            return;
        }
        
        Vector3 camPos = Camera.main.transform.position;
        if (!gameManager.isPlayerDead)
        {
            player = GameObject.FindWithTag("Player").GetComponent<Transform>();
            Vector3 dir = player.position - camPos;
            float dist = dir.magnitude;

            // Raycast from camera to player
            RaycastHit hit;
            if (Physics.Raycast(camPos, dir.normalized, out hit, dist, obstacleMask))
            {
                Renderer rend = hit.collider.GetComponent<Renderer>();
                if (rend != null)
                {
                    if (currentRenderer != rend)
                    {
                        ResetObstacle();
                        currentRenderer = rend;

                        // Cache default alpha per material
                        defaultAlphas = new float[rend.materials.Length];
                        for (int i = 0; i < rend.materials.Length; i++)
                            defaultAlphas[i] = rend.materials[i].color.a;
                    }

                    // Fade all materials
                    for (int i = 0; i < rend.materials.Length; i++)
                    {
                        Material mat = rend.materials[i];
                        Color c = mat.color;
                        c.a = Mathf.Lerp(c.a, fadedAlpha, Time.deltaTime * fadeSpeed);
                        mat.color = c;
                    }
                }
            }
            else
            {
                ResetObstacle();
            }
        }
    }

    void ResetObstacle()
    {
        if (currentRenderer != null && defaultAlphas != null)
        {
            Material[] mats = currentRenderer.materials;
            bool fullyReset = true;

            for (int i = 0; i < mats.Length; i++)
            {
                Color c = mats[i].color;
                c.a = Mathf.Lerp(c.a, defaultAlphas[i], Time.deltaTime * fadeSpeed);
                mats[i].color = c;

                if (Mathf.Abs(c.a - defaultAlphas[i]) > 0.01f)
                    fullyReset = false;
            }

            if (fullyReset)
            {
                // Restore alpha and clear reference
                for (int i = 0; i < mats.Length; i++)
                {
                    Color c = mats[i].color;
                    c.a = defaultAlphas[i];
                    mats[i].color = c;
                }
                currentRenderer = null;
                defaultAlphas = null;
            }
        }
    }
}
