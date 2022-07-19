
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void LoadWinScreen()
    {
        SceneManager.LoadScene("WinScreen");
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
