using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using System.Collections;
public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text gameGoalCountText;

    [System.Serializable]
    public class SpawnerAmount
    {
        public spawner spawner;
        public int amount;
    }

    [System.Serializable]
    public class LevelEnemies
    {
        public SpawnerAmount[] enemies;
    }

    [SerializeField] LevelEnemies[] enemiesPerLevelNew;

    [Header("Level UI")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject levelBanner;
    [SerializeField] private TMP_Text levelBannerText;
    [SerializeField] private float bannerTime = 1.2f;

    private Coroutine bannerRoutine;

    private int currentLevel = 0;

    public GameObject player;
    public playerControllerNew playerScript;
    public Image playerHPBar;
    public GameObject playerDamageScreen;

    public bool isPaused;

    float timeScaleOrig;
    int gameGoalCount;


    private void Start()
    {
        StartLevel(0);
    }

    private void ShowLevelBanner(int levelNumber)
    {
        if (levelBanner == null || levelBannerText == null) return;

        if(bannerRoutine != null) 
            StopCoroutine(bannerRoutine);

        bannerRoutine = StartCoroutine(levelBannerRoutine(levelNumber));
    }

    private IEnumerator levelBannerRoutine(int levelNumber)
    {
        levelBannerText.text = "Level " + levelNumber;
        levelBanner.SetActive(true);
        yield return new WaitForSeconds(bannerTime);
        levelBanner.SetActive(false);

        bannerRoutine = null;
    }

    private void StartLevel(int levelIndex)
    {
        if(enemiesPerLevelNew == null || enemiesPerLevelNew.Length == 0)
        {
            return;
        }

        if (levelIndex < 0)
            levelIndex = 0;
        if (levelIndex >= enemiesPerLevelNew.Length)
            levelIndex = enemiesPerLevelNew.Length - 1;

        currentLevel = levelIndex;

        int displayLevel = currentLevel + 1;

        if (levelText != null)
        {
            levelText.text = "Level: " + (displayLevel);
        }

        ShowLevelBanner(displayLevel);

        gameGoalCount = 0;

        LevelEnemies level = enemiesPerLevelNew[currentLevel];
        for(int i = 0; i < level.enemies.Length; i++)
        {
            SpawnerAmount entry = level.enemies[i];
            if (entry == null || entry.spawner == null || entry.amount <= 0)
                continue;

            entry.spawner.StartLevel(entry.amount);
            gameGoalCountText.text = gameGoalCount.ToString("F0");
        }
    }

    private void NextLevelOrWin()
    {
        int next = currentLevel + 1;

        if (next >= enemiesPerLevelNew.Length)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
            return;
        }

        StartLevel(next);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;

        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerControllerNew>();

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
                StateUnpaused();
            }
        } 
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StateUnpaused()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");
        
        if(gameGoalCount<= 0)
        {
            NextLevelOrWin();
        }
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}
