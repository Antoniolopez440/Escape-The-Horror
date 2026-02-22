using UnityEngine;

public class CreditsScroll : MonoBehaviour
{
    public float speed = 40f; // Speed of the credits scroll

    private void Start()
    {
        Time.timeScale = 1f; // Ensure time scale is normal when credits start
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime); // Move the credits up
    }
}
