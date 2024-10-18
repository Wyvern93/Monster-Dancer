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
    [SerializeField] private AudioMixerGroup sfxMixerGroup, musicMixerGroup, sfxSideMixerGroup;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private float mainVolume = 1f;

    public AudioClip gameOverFanfare;
    private static Dictionary<string, float> sfxCooldowns;
    public float sfxCooldown = 0.01f;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        clipsPlaying = new List<AudioClip>();
        audioMixer.updateMode = AudioMixerUpdateMode.UnscaledTime;
        sfxCooldowns = new Dictionary<string, float>();
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
        audioMixer.SetFloat("SfxSideVol", Mathf.Log10(sfxVolume + 0.0001f) * 20f);
    }

    // Update is called once per frame
    void Update()
    {
        clipsPlaying.Clear();

        // Create a list to store keys that need updating
        List<string> keysToUpdate = new List<string>();

        // Collect keys that need to be updated
        foreach (string key in sfxCooldowns.Keys)
        {
            if (sfxCooldowns[key] > 0)
            {
                keysToUpdate.Add(key);
            }
        }

        // Apply changes to the keys
        foreach (string key in keysToUpdate)
        {
            sfxCooldowns[key] -= Time.deltaTime;
        }
    }

    public static void PlaySound(AudioClip sound, float pitch = 1f, bool side = false)
    {
        if (instance.clipsPlaying.Contains(sound)) return;

        // Cooldown
        bool play = true;
        if (sfxCooldowns.ContainsKey(sound.name))
        {
            if (sfxCooldowns[sound.name] > 0)
            {
                play = false;
            }
        }
        else
        {
            sfxCooldowns.Add(sound.name, 0);
        }
        if (!play) return;
        sfxCooldowns[sound.name] = instance.sfxCooldown;
        instance.clipsPlaying.Add(sound);

        //if (sound.name == instance.sounds.enemyHurtSound.name || sound.name == "rabi_attack") return;
        if (pitch == 1 && !side)
        {
            instance.sfx.pitch = pitch;
            instance.sfx.PlayOneShot(sound);
        }
        else
        {
            GameObject sfxObj = new GameObject(sound.name);
            AudioSource currentSfx = sfxObj.AddComponent<AudioSource>();
            currentSfx.pitch = pitch;
            if (side)
            {
                currentSfx.outputAudioMixerGroup = instance.sfxSideMixerGroup;
            }
            else
            {
                currentSfx.outputAudioMixerGroup = instance.sfxMixerGroup;
            }
            
            currentSfx.PlayOneShot(sound);
            Destroy(sfxObj, 3f);
        }
    }

    public static void PlaySoundWithoutCooldown(AudioClip sound, float pitch = 1f, bool side = false)
    {
        if (instance.clipsPlaying.Contains(sound)) return;
        instance.clipsPlaying.Add(sound);
        
        //if (sound.name == instance.sounds.enemyHurtSound.name || sound.name == "rabi_attack") return;
        if (pitch == 1 && !side)
        {
            instance.sfx.pitch = pitch;
            instance.sfx.PlayOneShot(sound);
        }
        else
        {
            GameObject sfxObj = new GameObject(sound.name);
            AudioSource currentSfx = sfxObj.AddComponent<AudioSource>();
            currentSfx.pitch = pitch;
            if (side)
            {
                currentSfx.outputAudioMixerGroup = instance.sfxSideMixerGroup;
            }
            else
            {
                currentSfx.outputAudioMixerGroup = instance.sfxMixerGroup;
            }
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
