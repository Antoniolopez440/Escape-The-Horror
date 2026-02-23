using UnityEngine;
using UnityEngine.UI;

public class MenuAudio : MonoBehaviour
{
    public static MenuAudio instance;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clickSound;

    private void Awake()
    {
        if (source == null) source = GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
    }

    private void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        foreach (Button b in buttons)
        {
            b.onClick.AddListener(PlayClick);
        }
    }

    public void PlayClick()
    {
        if (clickSound == null) return;
        source.PlayOneShot(clickSound);
    }
}
