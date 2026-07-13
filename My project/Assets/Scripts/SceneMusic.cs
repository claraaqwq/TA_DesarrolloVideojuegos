using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Reproduce una pieza instrumental distinta en las escenas principales.
/// Las pistas se sintetizan al iniciar cada escena, así que el proyecto no
/// depende de archivos de audio externos ni de música con derechos de autor.
/// </summary>
public sealed class SceneMusic : MonoBehaviour
{
    private const int SampleRate = 22050;
    private const int BeatsPerLoop = 16;

    private AudioSource source;
    private string currentScene;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateManager()
    {
        if (FindFirstObjectByType<SceneMusic>() != null)
        {
            return;
        }

        GameObject musicObject = new GameObject("Musica de escena");
        DontDestroyOnLoad(musicObject);
        musicObject.AddComponent<SceneMusic>();
    }

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
        source.volume = 0.32f;

        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayForScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayForScene(scene.name);
    }

    private void PlayForScene(string sceneName)
    {
        if (currentScene == sceneName)
        {
            return;
        }

        currentScene = sceneName;
        source.Stop();

        if (source.clip != null)
        {
            Destroy(source.clip);
            source.clip = null;
        }

        MusicStyle style;
        switch (sceneName)
        {
            case "MenuPrincipal":
                style = MusicStyle.Menu;
                source.volume = 0.26f;
                break;
            case "Nivel 1":
                style = MusicStyle.Level;
                source.volume = 0.30f;
                break;
            case "BossFinal":
                style = MusicStyle.Boss;
                source.volume = 0.34f;
                break;
            default:
                return;
        }

        source.clip = BuildLoop(style);
        source.Play();
    }

    private static AudioClip BuildLoop(MusicStyle style)
    {
        float bpm = style == MusicStyle.Menu ? 72f : style == MusicStyle.Level ? 104f : 132f;
        float secondsPerBeat = 60f / bpm;
        int sampleCount = Mathf.CeilToInt(BeatsPerLoop * secondsPerBeat * SampleRate);
        float[] samples = new float[sampleCount];

        int[] menuNotes = { 57, 60, 64, 60, 55, 59, 62, 59, 53, 57, 60, 57, 55, 59, 62, 64 };
        int[] levelNotes = { 57, 64, 60, 67, 57, 65, 62, 69, 55, 62, 59, 67, 53, 60, 57, 64 };
        int[] bossNotes = { 45, 46, 45, 52, 45, 48, 46, 43, 45, 46, 48, 52, 53, 52, 48, 46 };
        int[] notes = style == MusicStyle.Menu ? menuNotes : style == MusicStyle.Level ? levelNotes : bossNotes;

        System.Random noise = new System.Random(2026 + (int)style);
        for (int i = 0; i < sampleCount; i++)
        {
            float time = (float)i / SampleRate;
            float beatPosition = time / secondsPerBeat;
            int beat = Mathf.FloorToInt(beatPosition) % BeatsPerLoop;
            float beatPhase = beatPosition - Mathf.Floor(beatPosition);
            float frequency = MidiToFrequency(notes[beat]);

            float melodyEnvelope = Mathf.Exp(-3.2f * beatPhase);
            float melody = SoftWave(frequency, time) * melodyEnvelope;
            float bass = SoftWave(MidiToFrequency(notes[beat] - 24), time) * 0.42f;

            float rhythm = 0f;
            if (style != MusicStyle.Menu)
            {
                float kickEnvelope = Mathf.Exp(-18f * beatPhase);
                rhythm += Mathf.Sin(2f * Mathf.PI * (52f + 35f * kickEnvelope) * time) * kickEnvelope * 0.55f;

                if (style == MusicStyle.Boss && (beat % 2 == 1))
                {
                    float hitEnvelope = Mathf.Exp(-24f * beatPhase);
                    rhythm += ((float)noise.NextDouble() * 2f - 1f) * hitEnvelope * 0.22f;
                }
            }

            float pad = 0f;
            if (style == MusicStyle.Menu)
            {
                pad = Mathf.Sin(2f * Mathf.PI * frequency * 0.5f * time) * 0.28f;
                pad += Mathf.Sin(2f * Mathf.PI * frequency * 0.75f * time) * 0.12f;
            }

            float mix = melody * 0.34f + bass * 0.23f + rhythm * 0.36f + pad * 0.34f;
            samples[i] = Mathf.Clamp(mix, -0.85f, 0.85f);
        }

        AudioClip clip = AudioClip.Create("Musica " + style, sampleCount, 1, SampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static float SoftWave(float frequency, float time)
    {
        float phase = 2f * Mathf.PI * frequency * time;
        return Mathf.Sin(phase) * 0.78f + Mathf.Sin(phase * 2f) * 0.15f + Mathf.Sin(phase * 3f) * 0.07f;
    }

    private static float MidiToFrequency(int midiNote)
    {
        return 440f * Mathf.Pow(2f, (midiNote - 69) / 12f);
    }

    private enum MusicStyle
    {
        Menu,
        Level,
        Boss
    }
}
