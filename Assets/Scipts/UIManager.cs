using UnityEngine;
using TMPro;
using System;
using System.Collections;
using UnityEngine.EventSystems;

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

    [SerializeField] private MonoBehaviour playerControllerToDisable; // Optional reference to your player controller script for input locking

    private bool codeOpen = false;
    public bool IsCodePanelOpen => codeOpen;
    private Action onCodeSuccess;


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
        if (messagePanel != null)
            messagePanel.SetActive(false);

        if (codePanel != null)
            codePanel.SetActive(false);

        HideHint();
    }


    public void ShowMessage(string msg, float autoHideSeconds = 2f)
    {
        if (messagePanel == null || messageText == null) return;

        messagePanel.SetActive(true);
        messageText.text = msg;

        CancelInvoke(nameof(HideMessage));

        if (autoHideSeconds > 0f)
            Invoke(nameof(HideMessage), autoHideSeconds);
    }

    public void HideMessage()
    {
        if (messagePanel != null)
            messagePanel.SetActive(false);
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

    public void OpenCodePanel(Action onSuccess, string prompt)
    {
        if (codePanel == null || codePromptText == null || codeInput == null) return;


        HideHint(); // Hide any hints when opening code panel, since they might conflict
        onCodeSuccess = onSuccess;
        codeOpen = true;

        codePanel.SetActive(true);
        codePromptText.text = prompt;

        // Stop player from eating keyboard input (BIG one)
        if (playerControllerToDisable != null)
            playerControllerToDisable.enabled = false;

        // Cursor + locking player look is the #1 reason people think UI "isn't opening"
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        StartCoroutine(FocusInputNextFrame());

  
    }

    private IEnumerator FocusInputNextFrame()
    {
        yield return null; // wait 1 frame after enabling panel

        codeInput.text = "";
        codeInput.interactable = true;

        // EventSystem selection is what actually routes typing
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(codeInput.gameObject);

        codeInput.ActivateInputField();
        codeInput.Select();
    }

    public void CloseCodePanel()
    {
        if (codePanel != null)
            codePanel.SetActive(false);

        codeOpen = false;
        onCodeSuccess = null;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerControllerToDisable != null)
            playerControllerToDisable.enabled = true;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

       
    }


    // Hook this to your Submit button OnClick (you already did this)
    public void SubmitCode()
    {
        if (!codeOpen) return;

       
        string input = codeInput != null ? codeInput.text : "";

        bool correct = CodeManager.Instance != null && CodeManager.Instance.CheckCode(input);


        if (correct)
        {
            ShowMessage("Unlocked!");
            onCodeSuccess.Invoke();
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
