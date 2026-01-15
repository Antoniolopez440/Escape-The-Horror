using UnityEditor;
using UnityEngine;

// Connecting the GameManager to ui as GameManager is going to become a prefab and is important.
public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    public bool isPaused;
    public GameObject player;
    public playerController playerScript;

    float timeScaleOriginal;

    int gameGoalCount; // Number of goals to win the game

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this; // Singleton pattern
        timeScaleOriginal = Time.timeScale; // Store the original time scale

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel")) 
        {
            if(menuActive == null) 
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0f; // Pause the game
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; 
    }

    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOriginal; // Resume the game
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void youLose() // Calls the lose menu when you die
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void updateGameGoal(int amount) // Updates the game goal count and how to win
    {
        gameGoalCount += amount;

        if(gameGoalCount <= 0)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }
}
