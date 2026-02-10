using UnityEngine;

public class FlashLightController : MonoBehaviour
{

    [SerializeField] private Light spotLight;
    [SerializeField] private KeyCode toggleKey = KeyCode.F;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spotLight == null)
            spotLight.enabled = false; //starts off
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
