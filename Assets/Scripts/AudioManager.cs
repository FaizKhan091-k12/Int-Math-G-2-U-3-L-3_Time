using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    private AudioClip clickClip;
    private AudioClip correctClip;
    private AudioClip wrongClip;
    private AudioClip fanfareClip;
    private AudioClip tickClip;

    private bool isMuted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();

        GenerateProceduralClips();

        isMuted = PlayerPrefs.GetInt("TimeDetective_Mute", 0) == 1;
        ApplyMute();
    }

    private void GenerateProceduralClips()
    {
        clickClip = CreateToneClip("Click", 0.08f, (t, dur) => {
            float freq = Mathf.Lerp(600f, 1200f, t / dur);
            float env = 1f - (t / dur);
            return Mathf.Sin(2 * Mathf.PI * freq * t) * env * 0.35f;
        });

        correctClip = CreateChimeClip("CorrectChime");
        wrongClip = CreateWrongClip("WrongBoing");
        fanfareClip = CreateFanfareClip("Fanfare");
        tickClip = CreateToneClip("Tick", 0.04f, (t, dur) => {
            float env = Mathf.Exp(-t * 80f);
            return (Mathf.Sin(2 * Mathf.PI * 1800f * t) + Random.Range(-0.2f, 0.2f)) * env * 0.25f;
        });
    }

    private AudioClip CreateToneClip(string name, float duration, System.Func<float, float, float> sampleFunc)
    {
        int sampleRate = 44100;
        int samplesCount = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samplesCount];

        for (int i = 0; i < samplesCount; i++)
        {
            float t = (float)i / sampleRate;
            data[i] = sampleFunc(t, duration);
        }

        AudioClip clip = AudioClip.Create(name, samplesCount, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateChimeClip(string name)
    {
        float duration = 0.65f;
        int sampleRate = 44100;
        int samplesCount = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samplesCount];

        // Harmonic arpeggio: C5 (523Hz), E5 (659Hz), G5 (784Hz), C6 (1046Hz)
        float[] notes = { 523.25f, 659.25f, 783.99f, 1046.50f };

        for (int i = 0; i < samplesCount; i++)
        {
            float t = (float)i / sampleRate;
            float sample = 0f;

            for (int n = 0; n < notes.Length; n++)
            {
                float noteStart = n * 0.09f;
                if (t >= noteStart)
                {
                    float noteT = t - noteStart;
                    float env = Mathf.Exp(-noteT * 6.5f);
                    float freq = notes[n];
                    sample += (Mathf.Sin(2 * Mathf.PI * freq * noteT) + 
                               0.3f * Mathf.Sin(2 * Mathf.PI * freq * 2f * noteT)) * env * 0.22f;
                }
            }
            data[i] = Mathf.Clamp(sample, -1f, 1f);
        }

        AudioClip clip = AudioClip.Create(name, samplesCount, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateWrongClip(string name)
    {
        float duration = 0.45f;
        int sampleRate = 44100;
        int samplesCount = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samplesCount];

        for (int i = 0; i < samplesCount; i++)
        {
            float t = (float)i / sampleRate;
            // Friendly warm wobble down: 320Hz to 180Hz
            float freq = Mathf.Lerp(340f, 190f, t / duration);
            float env = (1f - (t / duration));
            float wobble = 1f + 0.15f * Mathf.Sin(2 * Mathf.PI * 18f * t);
            data[i] = Mathf.Sin(2 * Mathf.PI * freq * t * wobble) * env * 0.3f;
        }

        AudioClip clip = AudioClip.Create(name, samplesCount, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateFanfareClip(string name)
    {
        float duration = 1.3f;
        int sampleRate = 44100;
        int samplesCount = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[samplesCount];

        // Trumpet/Fanfare pattern: G4, C5, E5, G5 sustained
        float[] freqs = { 392f, 523.25f, 659.25f, 783.99f };
        float[] timings = { 0.0f, 0.2f, 0.4f, 0.6f };
        float[] lengths = { 0.18f, 0.18f, 0.18f, 0.65f };

        for (int i = 0; i < samplesCount; i++)
        {
            float t = (float)i / sampleRate;
            float val = 0f;

            for (int k = 0; k < freqs.Length; k++)
            {
                if (t >= timings[k] && t < timings[k] + lengths[k])
                {
                    float noteT = t - timings[k];
                    float env = Mathf.Sin((noteT / lengths[k]) * Mathf.PI);
                    float f = freqs[k];
                    val += (Mathf.Sin(2 * Mathf.PI * f * noteT) + 
                            0.4f * Mathf.Sin(2 * Mathf.PI * f * 2f * noteT) + 
                            0.2f * Mathf.Sin(2 * Mathf.PI * f * 3f * noteT)) * env * 0.25f;
                }
            }
            data[i] = Mathf.Clamp(val, -1f, 1f);
        }

        AudioClip clip = AudioClip.Create(name, samplesCount, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    public void PlayClick()
    {
        if (isMuted || sfxSource == null || clickClip == null) return;
        sfxSource.PlayOneShot(clickClip, 0.8f);
    }

    public void PlayCorrect()
    {
        if (isMuted || sfxSource == null || correctClip == null) return;
        sfxSource.PlayOneShot(correctClip, 1.0f);
    }

    public void PlayWrong()
    {
        if (isMuted || sfxSource == null || wrongClip == null) return;
        sfxSource.PlayOneShot(wrongClip, 0.8f);
    }

    public void PlayFanfare()
    {
        if (isMuted || sfxSource == null || fanfareClip == null) return;
        sfxSource.PlayOneShot(fanfareClip, 1.0f);
    }

    public void PlayTick()
    {
        if (isMuted || sfxSource == null || tickClip == null) return;
        sfxSource.PlayOneShot(tickClip, 0.6f);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("TimeDetective_Mute", isMuted ? 1 : 0);
        PlayerPrefs.Save();
        ApplyMute();
    }

    public bool IsMuted() => isMuted;

    private void ApplyMute()
    {
        if (sfxSource != null) sfxSource.mute = isMuted;
        if (musicSource != null) musicSource.mute = isMuted;
    }
}
