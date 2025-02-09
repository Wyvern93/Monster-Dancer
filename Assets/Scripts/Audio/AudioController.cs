using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class BeatSound
{
    public AudioClip clip;
    public float volume;
    public float pitch;
    public bool side;

    public BeatSound(AudioClip clip, float volume, float pitch, bool side)
    {
        this.clip = clip;
        this.volume = volume;
        this.pitch = pitch;
        this.side = side;
    }
}

public class AudioController : MonoBehaviour
{
    public static AudioController instance;
    public AudioSounds sounds;
    private List<AudioClip> clipsPlaying;

    private List<BeatSound> clipsOnBeatQueue;

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
        clipsOnBeatQueue = new List<BeatSound>();
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
        if (BeatManager.isQuarterBeat) PlayBeatSounds();

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

    private void PlayBeatSounds()
    {
        foreach (BeatSound sound in clipsOnBeatQueue)
        {
            if (sound.pitch == 1 && !sound.side)
            {
                instance.sfx.pitch = sound.pitch;
                instance.sfx.PlayOneShot(sound.clip);
            }
            else
            {
                GameObject sfxObj = new GameObject(sound.clip.name);
                AudioSource currentSfx = sfxObj.AddComponent<AudioSource>();
                currentSfx.pitch = sound.pitch;
                if (sound.side)
                {
                    currentSfx.outputAudioMixerGroup = instance.sfxSideMixerGroup;
                }
                else
                {
                    currentSfx.outputAudioMixerGroup = instance.sfxMixerGroup;
                }
                currentSfx.PlayOneShot(sound.clip);
                Destroy(sfxObj, 3f);
            }
        }
        clipsOnBeatQueue.Clear();
    }

    public static void PlaySoundOnBeat(AudioClip sound, float pitch = 1f, bool side = false)
    {
        if (instance.clipsOnBeatQueue.FirstOrDefault(x=> x.clip == sound) != null) return;
        instance.clipsOnBeatQueue.Add(new BeatSound(sound, 1f, pitch, side));
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
