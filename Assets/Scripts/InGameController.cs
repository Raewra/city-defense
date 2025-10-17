using UnityEngine;
using UnityEngine.UI;

public class InGameController : MonoBehaviour
{
    public Button buttonArrow;
    public Button buttonBall;

    public GameManagerExtended gameManager;
   
    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManagerExtended>();

        buttonArrow.onClick.AddListener(OnArrow);
        buttonBall.onClick.AddListener(OnBall);
    }

    

    void OnArrow()
    {
        gameManager.playerState = GameManagerExtended.AttackTypes.Arrow;
    }

    void OnBall()
    {
        gameManager.playerState = GameManagerExtended.AttackTypes.Ball;
    }
}
