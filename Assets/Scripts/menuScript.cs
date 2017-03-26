using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class menuScript : MonoBehaviour {
    public Canvas quitMenu;
    public GameObject playCanvas;
    public Button LoadGameButton;
    void Start () {
        quitMenu = quitMenu.GetComponent<Canvas>();
        quitMenu.enabled = false;
        if (!PlayerPrefs.HasKey("Money")) {
            LoadGameButton.enabled = false;
        }
    }
	
	public void ExitPress()
    {
        quitMenu.enabled = true;
        playCanvas.SetActive(false);
    }
    public void NoPress()
    {
        quitMenu.enabled = false;
        playCanvas.SetActive(true);
    }
    public void StartLevel ()
    {
        PlayerPrefs.SetInt("LoadingGame", 1);
        SceneManager.LoadScene("Game") ;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void LoadGame()
    {
        PlayerPrefs.SetInt("LoadingGame", 2);
        SceneManager.LoadScene(1);
    }
}
