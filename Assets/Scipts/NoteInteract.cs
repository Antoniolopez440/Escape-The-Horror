using System;
using UnityEngine;

public class NoteInteract : MonoBehaviour
{
    [Header("Note Settings")]
    [SerializeField] private GameObject notePanel;
    [SerializeField] private Transform player;
    [SerializeField] private float interactionDistance = 2.0f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip readSound;
    [SerializeField] [Range(0f, 1f)] private float readVolume;

    private bool open;
    public static bool NoteRead = false;
    private bool hintShowing = false;


    // Update is called once per frame
    void Update()
    {
        if (player == null || notePanel == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (!open)
            if (distance < interactionDistance)
            {
                if (!hintShowing && UIManager.Instance != null)
                {
                    UIManager.Instance.ShowMessage("Press E to Read");
                    hintShowing = true;
                }
            }
            else
            {
                if (hintShowing && UIManager.Instance != null)
                {
                    UIManager.Instance.HideMessage();
                    hintShowing = false;
                }
            }

        if (!open)
            {
                if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.E))
                {

                if (UIManager.Instance != null)
                    UIManager.Instance.HideMessage();

                hintShowing = false;

                    Open();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
                {
                    Close();

                hintShowing = false;
                if (UIManager.Instance != null)
                    UIManager.Instance.HideMessage();
                }
            }
    }

    private void Open()
    {
        open = true;
        notePanel.SetActive(true);

        if (audioSource != null && readSound != null)
            AudioSource.PlayClipAtPoint(readSound, transform.position, readVolume);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        NoteRead = true;

    }

    private void Close()
    {
        open = false;
        notePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        NoteRead = true;

    }
}
