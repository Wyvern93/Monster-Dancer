using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public static AudioController instance;
    public AudioSounds sounds;
    private List<AudioClip> clipsPlaying;

    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioSource music;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup sfxMixerGroup, musicMixerGroup;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private float mainVolume = 1f;

    public AudioClip gameOverFanfare;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        clipsPlaying = new List<AudioClip>();
        audioMixer.updateMode = AudioMixerUpdateMode.UnscaledTime;
    }
    void Start()
    {
    }

    public void SetMaxVolume()
    {
        mainVolume = SaveManager.PersistentSaveData.GetData<float>("settings.maxVolume");
        musicVolume = SaveManager.PersistentSaveData.GetData<float>("settings.maxMusicVolume");
        sfxVolume = SaveManager.PersistentSaveData.GetData<float>("settings.maxSfxVolume");

        audioMixer.SetFloat("MainVol", Mathf.Log10(mainVolume + 0.0001f) * 20f);
        audioMixer.SetFloat("MusicVol", Mathf.Log10(musicVolume + 0.0001f) * 20f);
        audioMixer.SetFloat("SfxVol", Mathf.Log10(sfxVolume + 0.0001f) * 20f);
    }

    // Update is called once per frame
    void Update()
    {
        clipsPlaying.Clear();
    }

    public static void PlaySound(AudioClip sound, float pitch = 1f)
    {
        if (instance.clipsPlaying.Contains(sound)) return;
        instance.clipsPlaying.Add(sound);

        //if (sound.name == instance.sounds.enemyHurtSound.name || sound.name == "rabi_attack") return;
        if (pitch == 1)
        {
            instance.sfx.pitch = pitch;
            instance.sfx.PlayOneShot(sound);
        }
        else
        {
            GameObject sfxObj = new GameObject(sound.name);
            AudioSource currentSfx = sfxObj.AddComponent<AudioSource>();
            currentSfx.pitch = pitch;
            currentSfx.outputAudioMixerGroup = instance.sfxMixerGroup;
            currentSfx.PlayOneShot(sound);
            Destroy(sfxObj, 3f);
        }
    }
    public static void PlayMusic(AudioClip music, bool loop = true)
    {
        instance.music.clip = music;
        instance.music.volume = 1f;
        instance.music.Play();
        instance.music.loop = loop;
    }

    public static void FadeOut()
    {
        instance.StartCoroutine(instance.FadeOutCoroutine());
    }

    public IEnumerator FadeOutCoroutine()
    {
        while (music.volume > 0f)
        {
            music.volume = Mathf.MoveTowards(music.volume, 0f, Time.deltaTime * 2f);
            yield return new WaitForEndOfFrame();
        }
        music.Stop();
    }
}
