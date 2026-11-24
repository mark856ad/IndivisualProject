using UnityEngine;
using UnityEngine.Audio;

public class SfxPlayer : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private int voiceCount = 16;
    [SerializeField] private float pitchVariation = 0.05f;

    private static SfxPlayer instance;

    private AudioSource[] voices;
    private int nextVoice;

    private void Awake()
    {
        instance = this;

        voices = new AudioSource[voiceCount];
        for (int i = 0; i < voiceCount; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.outputAudioMixerGroup = sfxGroup;
            voices[i] = source;
        }
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public static void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null || instance == null) return;
        instance.PlayInternal(clip, volume);
    }

    private void PlayInternal(AudioClip clip, float volume)
    {
        AudioSource source = voices[nextVoice];
        nextVoice = (nextVoice + 1) % voices.Length;

        source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        source.PlayOneShot(clip, volume);
    }
}