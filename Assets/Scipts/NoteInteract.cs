using System;
using UnityEngine;

public class NoteInteract : MonoBehaviour
{
    [Header("Note Settings")]
    [SerializeField] private GameObject notePanel;
    [SerializeField] private Transform player;
    [SerializeField] private float interactionDistance = 2.0f;

    private bool open;


    // Update is called once per frame
    void Update()
    {
        if (player == null || notePanel == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (!open)
        {
            if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.E))
            {

                Open();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
        }
    }

    private void Open()
    {
        open = true;
        notePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    private void Close()
    {
        open = false;
        notePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }
}
