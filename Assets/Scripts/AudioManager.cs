using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sound Effects - Asignar en Inspector")]
    public AudioClip jumpSound;
    public AudioClip coinSound;
    public AudioClip damageSound;
    public AudioClip deathSound;
    public AudioClip enemyDeathSound;
    public AudioClip levelCompleteSound;

    [Header("Music")]
    public AudioClip backgroundMusic;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Si no se asignaron clips, generar sonidos por código
        if (jumpSound == null) jumpSound = GenerateSound(0.1f, 600f, 900f);
        if (coinSound == null) coinSound = GenerateSound(0.15f, 800f, 1200f);
        if (damageSound == null) damageSound = GenerateSound(0.2f, 200f, 150f);
        if (deathSound == null) deathSound = GenerateSound(0.4f, 400f, 100f);
        if (enemyDeathSound == null) enemyDeathSound = GenerateSound(0.15f, 500f, 300f);
        if (levelCompleteSound == null) levelCompleteSound = GenerateSound(0.5f, 500f, 1000f);

        PlayMusic();
    }

    private void SetupAudioSources()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = 0.3f;
    }

    /// <summary>
    /// Genera un AudioClip sencillo con un barrido de frecuencia (efecto retro)
    /// </summary>
    private AudioClip GenerateSound(float duration, float startFreq, float endFreq)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleCount;
            float freq = Mathf.Lerp(startFreq, endFreq, t);
            float amplitude = 1f - t; // Fade out
            samples[i] = amplitude * 0.5f * Mathf.Sin(2f * Mathf.PI * freq * t * duration);
        }

        AudioClip clip = AudioClip.Create("GeneratedSFX", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private void PlayMusic()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void PlayJump()
    {
        sfxSource.PlayOneShot(jumpSound, 0.5f);
    }

    public void PlayCoin()
    {
        sfxSource.PlayOneShot(coinSound, 0.6f);
    }

    public void PlayDamage()
    {
        sfxSource.PlayOneShot(damageSound, 0.7f);
    }

    public void PlayDeath()
    {
        sfxSource.PlayOneShot(deathSound, 0.8f);
    }

    public void PlayEnemyDeath()
    {
        sfxSource.PlayOneShot(enemyDeathSound, 0.6f);
    }

    public void PlayLevelComplete()
    {
        sfxSource.PlayOneShot(levelCompleteSound, 0.8f);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
