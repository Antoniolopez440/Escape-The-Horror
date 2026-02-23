using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadCredits()
    {
        Time.timeScale = 1f; // Ensure time scale is reset when loading credits
        SceneManager.LoadScene("Credits");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Ensure time scale is reset when restarting the game
        SceneManager.LoadScene("ZombieMansionAlpha");
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Ensure time scale is reset when loading main menu
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
     //   Debug.Log("Quit");
        Application.Quit();
    }


}
