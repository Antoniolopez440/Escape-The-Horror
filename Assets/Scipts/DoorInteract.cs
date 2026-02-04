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


    // State variables
    bool playerInRange;
    bool isOpen;
    bool isMoving;


    // Rotations
    // Store the closed and open rotations
    Quaternion closedRotation;
    Quaternion openRotation;


    // Awake is called when the script instance is being loaded
    // This happens before any Start functions and also just after a prefab is instantiated
    private void Awake()
    {
        closedRotation = hinge.rotation;
        openRotation = Quaternion.Euler(hinge.eulerAngles + new Vector3(0f, openAngle, 0f));
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
        Quaternion targetRotation = isOpen ? closedRotation : openRotation;
        while (Quaternion.Angle(hinge.rotation, targetRotation) > 0.1f)
        {
            hinge.rotation = Quaternion.Slerp(hinge.rotation, targetRotation, Time.deltaTime * speed);
            yield return null;
        }
        hinge.rotation = targetRotation;
        isOpen = !isOpen;
        isMoving = false;
    }

}
