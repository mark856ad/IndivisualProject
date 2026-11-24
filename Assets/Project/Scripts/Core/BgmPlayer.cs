using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class BgmPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private float fadeInSeconds = 1.5f;

    private AudioSource source;
    private float fadeStartTime;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.clip = music;
        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.outputAudioMixerGroup = bgmGroup;
    }

    private void Start()
    {
        if (music == null) return;

        source.volume = 0f;
        fadeStartTime = Time.unscaledTime;
        source.Play();
    }

    private void Update()
    {
        if (source.volume >= 1f) return;

        float t = fadeInSeconds > 0f
            ? (Time.unscaledTime - fadeStartTime) / fadeInSeconds
            : 1f;

        source.volume = Mathf.Clamp01(t);
    }
}
