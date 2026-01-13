using UnityEngine;
using UnityEngine.InputSystem.DualShock;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY; // Rotates player on the Y axis
    [SerializeField] Transform player;

    float camRotX; //rotates camera on X axis for camera



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime; //What mouse is doing on X axis
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime; // What mouse is doing on Y axis

        if (invertY)
            camRotX += mouseY;
        else
            camRotX -= mouseY;

        camRotX = Mathf.Clamp(camRotX, lockVertMin, lockVertMax);

        transform.localRotation = Quaternion.Euler(camRotX, 0, 0); //player rotation not world

        player.Rotate(Vector3.up * mouseX); 
    }
}
