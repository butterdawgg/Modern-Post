using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private Sound[] sounds;
    [SerializeField] private Sound[] music;

    private bool isCoroutineActive = false;
    private bool musicIsPlaying = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].source = gameObject.AddComponent<AudioSource>();
            sounds[i].source.clip = sounds[i].clip;

            sounds[i].source.volume = sounds[i].volume;
            sounds[i].source.pitch = sounds[i].pitch;
        }

        for (int i = 0; i < music.Length; i++)
        {
            music[i].source = gameObject.AddComponent<AudioSource>();
            music[i].source.clip = music[i].clip;

            music[i].source.volume = music[i].volume;
            music[i].source.pitch = music[i].pitch;
        }

        isCoroutineActive = false;
    }

    void Update()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].source.volume = sounds[i].volume * SerializeManager.Instance.GetFloat(FloatType.SfxVolume) * SerializeManager.Instance.GetFloat(FloatType.MasterVolume);
        }

        for (int i = 0; i < music.Length; i++)
        {
            music[i].source.volume = music[i].volume * SerializeManager.Instance.GetFloat(FloatType.MusicVolume) * SerializeManager.Instance.GetFloat(FloatType.MasterVolume);
        }

        if (!isCoroutineActive & !musicIsPlaying)
            StartCoroutine(PlayMusicCoroutine());
    }

    public void PlaySound(string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
                sounds[i].source.Play();
        }
    }

    public void StopSound(string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
                sounds[i].source.Stop();
        }
    }

    public IEnumerator PlayMusicCoroutine()
    {
        isCoroutineActive = true;

        int k = 0;
        for (int i = 0; i < music.Length; i++)
        {
            if (music[i].played)
                k++;
        }
        if (k == music.Length)
        {
            for (int i = 0; i < music.Length; i++)
            {
                music[i].played = false;
            }
        }

        int random = Random.Range(0, music.Length);
        while (music[random].played)
        {
            random = Random.Range(0, music.Length);
        }

        musicIsPlaying = true;
        music[random].source.Play();

        yield return new WaitForSecondsRealtime(music[random].clip.length);

        musicIsPlaying = false;

        music[random].played = true;

        isCoroutineActive = false;
    }
}
