using System.Threading;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ZombieAudio3D : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] private AudioClip[] idleClips;

    [Header("Delay Between Sounds")]
    [SerializeField] private Vector2 idleInterval = new Vector2(4f, 10f);

    [Header("Distance & Volume")]
    [SerializeField] private float hearDistance = 35f;
    [SerializeField] private float startDistance = 30f;
    [SerializeField] private float baseVolume = 0.7f;
    [Range(0f, 1f)]

    private AudioSource src;
    private Transform player;
    private float timer;

    void Start()
    {
        src = GetComponent<AudioSource>();
        src.spatialBlend = 1f;
        src.playOnAwake = false;
        src.loop = false;
        src.volume = baseVolume;

        player = gameManager.instance.player.transform;

        timer = Random.Range(0f, idleInterval.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > hearDistance)
        {
            if (src.isPlaying) src.Stop();
            return;
        }

        if (dist > startDistance)
            return;
        if (src.isPlaying) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        PlayRandomClip(idleClips);
        ResetTimer();
    }

    private void PlayRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        src.pitch = Random.Range(0.95f, 1.05f);
        src.volume = baseVolume;

        src.PlayOneShot(clip);
    }

    private void ResetTimer()
    {
        timer = Random.Range(idleInterval.x, idleInterval.y);
    }
}
