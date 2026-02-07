using UnityEngine;
using System.Collections;

public class DoorInteract : MonoBehaviour
{

    [Header("Door parts")]
    [SerializeField] Transform hinge;

    [Header("Open Settings")]
    [SerializeField] float openAngle = 90f;
    [SerializeField] float speed = 6f;

    [Header("Input")]
    [SerializeField] KeyCode interactKey = KeyCode.E;

    [Header("Auto Open For Zombies")]
    [SerializeField] string zombieTag = "Enemy";


    // State variables
    bool playerInRange;
    bool isOpen;
    bool isMoving;
    int zombiesInRange;


    // Rotations
    // Store the closed and open rotations
    Quaternion closedRotation;
    Quaternion openRotation;


    // Awake is called when the script instance is being loaded
    // This happens before any Start functions and also just after a prefab is instantiated
    private void Awake()
    {
        if (hinge == null) hinge = transform;

        closedRotation = hinge.rotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }



    // Update is called once per frame
    void Update()
    {
        if (!playerInRange) return;
        if (isMoving) return;

        if (Input.GetKeyDown(interactKey))
        {
            StartCoroutine(ToggleDoor());
        }
    }


    IEnumerator ToggleDoor()
    {
        isMoving = true;

        Quaternion start = hinge.localRotation;
        Quaternion targetRotation = isOpen ? closedRotation : openRotation;

        float t = 0f;
        while (t < 1f)
        {

            t += Time.deltaTime * speed;
            hinge.rotation = Quaternion.Slerp(start, targetRotation, t);
            yield return null;
        }
        hinge.rotation = targetRotation;
        isOpen = !isOpen;
        isMoving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
        if (other.CompareTag(zombieTag))
        {
            if (!isOpen && !isMoving)
            {
                StartCoroutine(ToggleDoor());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }


}
