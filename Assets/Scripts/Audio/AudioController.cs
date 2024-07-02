using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController instance;
    private List<AudioClip> clipsPlaying;

    [SerializeField] private AudioSource sfx;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        clipsPlaying = new List<AudioClip>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        clipsPlaying.Clear();
    }

    public static void PlaySound(AudioClip sound)
    {
        if (instance.clipsPlaying.Contains(sound)) return;
        instance.clipsPlaying.Add(sound);
        instance.sfx.PlayOneShot(sound);
    }

}
