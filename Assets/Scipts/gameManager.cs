using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using System.Collections;
using UnityEngine.SceneManagement;
public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text gameGoalCountText;

    [SerializeField] private AudioSource pauseMusic;

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

    [Header("Quest System")]
    [SerializeField] private int currentQuest = 1;
    [SerializeField] private QuestSpawner regularQuestSpawner;
    [SerializeField] private ObjectiveUI objectiveUI;
    [SerializeField] private DoorInteract frontDoor;
    private bool q1FlashlightDone;
    private bool q1GunDone;
    private bool q1KeyDone;
    private bool q1DoorDone;

    private bool q2ShedKeyDone;
    private bool q2CrowbarDone;
    private bool q2PlanksDone;
    private bool q2GateDone;

    private Coroutine autoAdvanceRoutine;
    public int CurrentQuest => currentQuest;

    private Coroutine bannerRoutine;

    private int currentLevel = 0;

    public GameObject player;
    public playerControllerNew playerScript;
    public Image playerHPBar;
    public GameObject playerDamageScreen;

    public CarPartsUI carPartsUI;

    public bool isPaused;

    float timeScaleOrig;
    int gameGoalCount;

    private int wheelsCollected = 0;
    private bool steeringFound = false;
    private bool stickFound = false;


    private void Start()
    {
        StartLevel(0);
        SetQuest(1);
    }

    //private void ShowLevelBanner(int levelNumber)
    //{
    //    if (levelBanner == null || levelBannerText == null) return;

    //    if(bannerRoutine != null) 
    //        StopCoroutine(bannerRoutine);

    //    bannerRoutine = StartCoroutine(levelBannerRoutine(levelNumber));
    //}

    //private IEnumerator levelBannerRoutine(int levelNumber)
    //{
    //    levelBannerText.text = "Level " + levelNumber;
    //    levelBanner.SetActive(true);
    //    yield return new WaitForSeconds(bannerTime);
    //    levelBanner.SetActive(false);

    //    bannerRoutine = null;
    //}

    private void StartLevel(int levelIndex)
    {
        if (enemiesPerLevelNew == null || enemiesPerLevelNew.Length == 0)
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

        //ShowLevelBanner(displayLevel);

        gameGoalCount = 0;

        LevelEnemies level = enemiesPerLevelNew[currentLevel];
        if (level == null || level.enemies == null)
        {

            UpdateGoalText();
            return;
        }
        for (int i = 0; i < level.enemies.Length; i++)
        {
            SpawnerAmount entry = level.enemies[i];
            if (entry == null || entry.spawner == null || entry.amount <= 0)
                continue;

            entry.spawner.StartLevel(entry.amount);
            gameGoalCount += entry.amount;
        }
       // Debug.Log($"[GM] StartLevel done. gameGoalCount={gameGoalCount}");
        UpdateGoalText();
    }
    private void UpdateGoalText()
    {

        if (gameGoalCountText != null)
            gameGoalCountText.text = gameGoalCount.ToString();
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
            if (menuActive == null)
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

        if (pauseMusic != null)
        {
            pauseMusic.Play();
        }
    }

    public void StateUnpaused()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;

            if (pauseMusic != null)
            {
                pauseMusic.Stop();
        }
    }

    public void updateGameGoal(int amount)
    {
      //  Debug.Log($"[GM] updateGameGoal({amount}) BEFORE count={gameGoalCount}");
        gameGoalCount += amount;
     //   Debug.Log($"[GM] updateGameGoal({amount}) AFTER  count={gameGoalCount}");
        gameGoalCountText.text = gameGoalCount.ToString("F0");

        //if(gameGoalCount<= 0)
        //{
        //    NextLevelOrWin();
        //}
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);

        if (pauseMusic != null)
        {
            pauseMusic.Play();
        }
    }

    public void WinGame()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);

        if (pauseMusic != null)
        {
            pauseMusic.Play();
        }


    }
    public void SetQuest(int quest)
    {
        Debug.Log($"[GM]SetQuest({quest}) -> currentQuest ={currentQuest} spawner={(regularQuestSpawner ? regularQuestSpawner.name : "NULL")}");
        currentQuest = Mathf.Max(1, quest);

        if (regularQuestSpawner != null)
        {
            regularQuestSpawner.SetQuest(currentQuest);
        }

        if (objectiveUI != null)
        {
            switch (currentQuest)
            {
                case 1:
                    objectiveUI.SetMain("Escape the Mansion");
                    objectiveUI.SetSubObjectivesInOrder(new string[]
                    {
                        "Find a flashlight",
                        "Find a Gun",
                        "Find the Key",
                        "Unlock the Front Door"
                    });
                    SyncQuest1Flags();
                    StartAutoAdvance();
                    break;
                case 2:
                    objectiveUI.SetMain("Open The Main Gate");
                    objectiveUI.SetSubObjectivesInOrder(new string[]
                    {
                        "Find the Key to the Shed",
                        "Find a Crowbar",
                        "Remove the Wooden Planks",
                        "Open Main Gate"
                    });
                    SyncQuest2Flags();
                    StartAutoAdvance();
                    break;
                case 3:
                     
                    objectiveUI.SetChecklist("Escape the Horror", new (string id, string text)[]
                    {
                        ("wheels", "Wheels (0/4)"),
                        ("steering", "Steering Wheel"),
                        ("stick", "Stick Shift")
                    });

                    RefreshQuest3ChecklistUI();
                    break;
                default:
                    objectiveUI.SetMain("Survive");
                    objectiveUI.SetSubObjectivesInOrder(new string[]
                    {
                        "Keep moving",
                        "Stay alive"
                    });
                    break;
            }
        }
    }

    public void OnCarPartPicked(CarPartsType type)
    {

        switch (type)
        {
            case CarPartsType.Wheel:
                wheelsCollected = Mathf.Min(4, wheelsCollected + 1);
                break;

            case CarPartsType.SteeringWheel:
                steeringFound = true;
                break;

            case CarPartsType.StickShift:
                stickFound = true;
                break;
        }

        if (objectiveUI != null && CurrentQuest == 3)
        {
            RefreshQuest3ChecklistUI();
        }
    }
    public void CompleteSubObjective()
    {
        if (objectiveUI == null) return;
        objectiveUI.CompleteCurrentSub();
        StartCoroutine(CheckQuestDoneAfterUI());
    }

    public IEnumerator CheckQuestDoneAfterUI()
    {
        yield return new WaitForSeconds(1.1f);
        if (objectiveUI != null && !objectiveUI.HasMoreSubs())
        {
            SetQuest(currentQuest + 1);
        }
    }

    private bool InventoryHas(string id)
    {
        var inv = PlayerInventory.Instance;
        if (inv == null) return false;
        var items = inv.GetItems();
        if(items == null) return false;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == id) return true;
        }
        return false;
    }

    private void SyncQuest1Flags()
    {
        q1FlashlightDone = InventoryHas("Flashlight");
        q1KeyDone = InventoryHas("DoubleDoorKey");
        q1DoorDone = (frontDoor != null && frontDoor.IsUnlocked);
    }
    private void SyncQuest2Flags()
    {
        q2ShedKeyDone = InventoryHas("ShedKey");
        q2CrowbarDone = InventoryHas("CrowbarKey");
    }

    private void StartAutoAdvance()
    {
        if (objectiveUI == null) return;
        if (autoAdvanceRoutine != null) StopCoroutine(autoAdvanceRoutine);
        autoAdvanceRoutine = StartCoroutine(AutoAdvanceRoutine());
    }

    private void RefreshQuest3ChecklistUI ()
    {
        if (objectiveUI == null) return;

        objectiveUI.SetChecklistItemText("wheels", $"Wheels ({wheelsCollected}/4");

        if (wheelsCollected >= 4)
            objectiveUI.CheckOff("wheels");

        if (steeringFound)
            objectiveUI.CheckOff("steering");
        if (stickFound)
            objectiveUI.CheckOff("stick");
    }

    IEnumerator AutoAdvanceRoutine()
    {
        yield return null;

        while (objectiveUI != null && objectiveUI.CurrentSubRaw != null)
        {
            string cur = objectiveUI.CurrentSubRaw;

            if (!IsCurrentStepAlreadyDone(cur))
                break;

            objectiveUI.CompleteCurrentSub();

            yield return new WaitForSeconds(objectiveUI.DoneDisplaySeconds + 0.05f);
        }
        if (objectiveUI != null && !objectiveUI.HasMoreSubs())
        {
            SetQuest(currentQuest + 1);

        }
        autoAdvanceRoutine = null;
    }

    private bool IsCurrentStepAlreadyDone(string stepText)
    {
        if (currentQuest == 1)
        {
            if (stepText == "Find a flashlight") return q1FlashlightDone;
            if (stepText == "Find a Gun") return q1GunDone;
            if (stepText == "Find the Key") return q1KeyDone;
            if (stepText == "Unlock the Front Door") return q1DoorDone;
        }
        else if (currentQuest == 2)
        {
            if (stepText == "Find the Key to the Shed") return q2ShedKeyDone;
            if (stepText == "Find a Crowbar") return q2CrowbarDone;
            if (stepText == "Remove the Wooden Planks") return q2PlanksDone;
            if (stepText == "Open Main Gate") return q2GateDone;
        }

        return false;
    }

    public void OnQuest1FlashlightFound()
    {
        q1FlashlightDone = true;
        StartAutoAdvance();
    }

    public void OnQuest1GunFound()
    {
        q1GunDone = true;
        StartAutoAdvance();
    }

    public void OnQuest1KeyFound()
    {
        q1KeyDone = true;
        StartAutoAdvance();
    }
    public void OnQuest1FrontDoorUnlocked()
    {
        q1DoorDone = true;
        StartAutoAdvance();
    }

    public void OnQuest2ShedKeyFound()
    {
        q2ShedKeyDone = true;
        StartAutoAdvance();
    }

    public void OnQuest2CrowbarFound()
    {
        q2CrowbarDone = true;
        StartAutoAdvance();
    }


    public void OnQuest2PlanksRemoved()
    {
        q2PlanksDone = true;
        StartAutoAdvance();
    }

    public void OnQuest2MainGateOpened()
    {
        q2GateDone = true;
        StartAutoAdvance();
    }
}
