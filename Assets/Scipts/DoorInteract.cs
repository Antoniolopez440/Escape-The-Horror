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



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         
    }

}
