using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleControllerExtended : MonoBehaviour
{
    public Button start;
    public Toggle options;
    public Button exit;

    public GameObject optionsWindow;
    
    void Start()
    {
        start.onClick.AddListener(OnStart);
        options.onValueChanged.AddListener(OnOptions);
    }
    
    public void OnStart()
    {
        SceneManager.LoadScene(1);
    }
    public void OnOptions(bool value)
    {
        if (value)
        {
            //Toggle is ON
            optionsWindow.SetActive(true);
        }
        else
        {
            //Toggle is OFF
            optionsWindow.SetActive(false);
        }
    }
    public void OnExit()
    {
        Application.Quit();
    }
}
