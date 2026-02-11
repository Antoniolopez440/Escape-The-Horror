using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [Header("Message UI")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TMP_Text messageText;

    [Header("Hint UI")]
    [SerializeField] private TMP_Text hintText;
    [Header("Code UI")]
    [SerializeField] private GameObject codePanel;
    [SerializeField] private TMP_Text codePromptText;
    [SerializeField] private TMP_InputField codeInput;

    private DoorCodeLock activeDoor;
    private bool codeOpen = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (messagePanel != null) messagePanel.SetActive(false);
        if (codePanel != null) codePanel.SetActive(false);
        HideHint();
    }


    public void ShowMessage(string msg, float autoHideSeconds = 2f)
    {
        if (messagePanel == null || messageText == null) return;

        messagePanel.SetActive(true); messageText.text = msg;

        CancelInvoke(nameof(HideMessage));
        if (autoHideSeconds > 0f) Invoke(nameof(HideMessage), autoHideSeconds);
    }

    public void HideMessage()
    {
        if (messagePanel != null) messagePanel.SetActive(false);
    }

    public void ShowHint(string msg)
    {
        if (hintText == null) return;
        hintText.gameObject.SetActive(true);
        hintText.text = msg;
    }

    public void HideHint()
    {
        if (hintText == null) return;
        hintText.text = "";
        hintText.gameObject.SetActive(false);
    }

    public void OpenCodePanel(DoorCodeLock door, string prompt)
    {
        if (codePanel == null || codePromptText == null || codeInput == null) return;

        activeDoor = door;
        codeOpen = true;

        codePanel.SetActive(true);
        codePromptText.text = prompt;

        codeInput.text = "";
        codeInput.ActivateInputField();
        codeInput.Select();

        // Cursor + locking player look is the #1 reason people think UI "isn't opening"
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Optional: if you have a player script, call something like:
        // PlayerController.Instance.SetInputLocked(true);
     }

    public void CloseCodePanel()
    {
        if (codePanel != null) codePanel.SetActive(false);

        activeDoor = null;
        codeOpen = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Optional: PlayerController.Instance.SetInputLocked(false);
    }


    // Hook this to your Submit button OnClick (you already did this)
    public void SubmitCode()
    {
        if (!codeOpen) return;
        if (activeDoor == null) { CloseCodePanel(); return; }
        string input = codeInput != null ? codeInput.text : "";

        bool correct = CodeManager.Instance != null && CodeManager.Instance.CheckCode(input);
        if (correct)
        {
            ShowMessage("Unlocked!");
            activeDoor.UnlockAndOpen();
            CloseCodePanel();
        }
        else
        {
            ShowMessage("Wrong code.");
            // Keep panel open so player can try again
            if (codeInput != null)
            {
                codeInput.text = "";
                codeInput.ActivateInputField();
            }
        }
    }
}
