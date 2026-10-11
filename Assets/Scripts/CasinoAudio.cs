using UnityEngine;

public class CasinoAudio : MonoBehaviour
{
    public static CasinoAudio Instance;

    private AudioSource src;
    private AudioClip tickClip;
    private AudioClip coinClip;
    private AudioClip explosionClip;
    private AudioClip winClip;
    private AudioClip cardClip;
    private AudioClip lossClip;

    // create procedural sound effects so we don't need external audio asset files
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            src = gameObject.AddComponent<AudioSource>();

            // generate sound clips for game events
            tickClip = CreateTone(0.05f, 1200f, 1200f, 0.1f, 1);
            coinClip = CreateTone(0.3f, 1200f, 2200f, 0.15f, 2);
            explosionClip = CreateNoise(0.8f, 0.4f);
            winClip = CreateArpeggio(0.8f, new float[]{523.25f, 659.25f, 783.99f, 1046.50f}, 0.2f);
            cardClip = CreateTone(0.04f, 1800f, 900f, 0.1f, 1);
            lossClip = CreateTone(0.3f, 420f, 160f, 0.18f, 1);
        }
    }

    // audio playback methods
    public void PlayTick() { if(src) src.PlayOneShot(tickClip); }
    public void PlayCoin() { if(src) src.PlayOneShot(coinClip); }
    public void PlayExplosion() { if(src) src.PlayOneShot(explosionClip); }
    public void PlayWin() { if(src) src.PlayOneShot(winClip); }
    public void PlayCard() { if(src) src.PlayOneShot(cardClip); }
    public void PlayLoss() { if(src) src.PlayOneShot(lossClip); }

    // generate synthesized waveform
    private AudioClip CreateTone(float duration, float startFreq, float endFreq, float volume, int type)
    {
        int sampleRate = 44100;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];
        float phase = 0f;
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float freq = Mathf.Lerp(startFreq, endFreq, t);
            float env = 1f - t;
            if (type == 1) data[i] = Mathf.Sin(phase * 2 * Mathf.PI) * env * volume;
            else if (type == 2) data[i] = (Mathf.Repeat(phase, 1f) < 0.5f ? 1f : -1f) * env * volume;
            phase += freq / sampleRate;
        }
        AudioClip clip = AudioClip.Create("Tone", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateNoise(float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];
        System.Random rnd = new System.Random();
        for (int i = 0; i < samples; i++)
        {
            float env = 1f - ((float)i / samples);
            data[i] = ((float)(rnd.NextDouble() * 2.0 - 1.0)) * env * volume;
        }
        AudioClip clip = AudioClip.Create("Noise", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    private AudioClip CreateArpeggio(float duration, float[] freqs, float volume)
    {
        int sampleRate = 44100;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];
        float phase = 0f;
        int sectionLength = samples / freqs.Length;
        for (int i = 0; i < samples; i++)
        {
            int fIdx = Mathf.Clamp(i / sectionLength, 0, freqs.Length - 1);
            float freq = freqs[fIdx];
            float env = 1f - ((float)(i % sectionLength) / sectionLength);
            data[i] = Mathf.Sin(phase * 2 * Mathf.PI) * env * volume;
            phase += freq / sampleRate;
        }
        AudioClip clip = AudioClip.Create("Arp", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
