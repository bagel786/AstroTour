using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance { get; private set; }
    private static SoundEffectLibrary soundEffectLibrary;
    private static AudioSource audioSource;
    private static AudioSource randomPitchAudioSource;
    private static AudioSource musicSource;

    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider masterSlider;

    [Header("Music Settings")]
    [SerializeField] private AudioClip backgroundMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Grab AudioSources (expecting 3 total: SFX, RandomPitch, Music)
            AudioSource[] audioSources = GetComponents<AudioSource>();

            if (audioSources.Length < 3)
            {
                Debug.LogError("SoundEffectManager requires 3 AudioSources: [0]=SFX, [1]=RandomPitch, [2]=Music");
                return;
            }

            audioSource = audioSources[0];
            randomPitchAudioSource = audioSources[1];
            musicSource = audioSources[2];

            soundEffectLibrary = GetComponent<SoundEffectLibrary>();

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Set up slider listeners
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            
        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        // Initialize slider values from settings
        InitializeSliderValues();

        // Play background music if set
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    
    private void InitializeSliderValues()
    {
        if (SettingsManager.Instance != null)
        {
            GameSettings settings = SettingsManager.Instance.Settings;
            
            if (sfxSlider != null)
                sfxSlider.value = settings.sfxVolume;
                
            if (musicSlider != null)
                musicSlider.value = settings.musicVolume;
                
            if (masterSlider != null)
                masterSlider.value = settings.masterVolume;
        }
    }

    // --------------------
    // SOUND EFFECTS
    // --------------------
    public static void Play(string soundName, bool randomPitch = false)
    {
        if (soundEffectLibrary == null) return;

        AudioClip audioClip = soundEffectLibrary.GetRandomClip(soundName);
        if (audioClip != null)
        {

            if (randomPitch)
            {
                randomPitchAudioSource.pitch = Random.Range(1f, 1.5f);
                randomPitchAudioSource.PlayOneShot(audioClip);
            }
            else
            {
                audioSource.PlayOneShot(audioClip);
            }
        }
      
    }

    public static void SetVolume(float volume)
    {
       
        audioSource.volume = volume;
        randomPitchAudioSource.volume = volume;
        musicSource.volume = volume;
    }

    public void OnSFXVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetSFXVolume(value);
        }
        else
        {
            SetVolume(value); // Fallback for immediate feedback
        }
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetMusicVolume(value);
        }
        else
        {
            SetMusicVolume(value); // Fallback for immediate feedback
        }
    }
    
    public void OnMasterVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetMasterVolume(value);
        }
    }

    // --------------------
    // MUSIC
    // --------------------
    public static void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = volume;
    }

    public static void ChangeMusic(AudioClip newTrack, bool loop = true)
    {
        if (musicSource == null || newTrack == null) return;

        musicSource.clip = newTrack;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public static void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }
    
    // --------------------
    // VOICE
    // --------------------
    public static void PlayVoice(AudioClip voiceClip, float pitch = 1.0f)
    {
        if (voiceClip == null || randomPitchAudioSource == null) return;
        
        // Use the random pitch audio source for voice with custom pitch
        randomPitchAudioSource.pitch = pitch;
        randomPitchAudioSource.PlayOneShot(voiceClip);
    }
}
