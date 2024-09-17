using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatManager : MonoBehaviour
{
    private static BeatManager instance;

    [SerializeField] private AudioSource music, beatTest;
    [SerializeField] private float offset, tempo;
    public static float audio_offset;

    public float secondsPerBeat, nextBeat, lastBeat;
    public float currentTime;

    [Header("Loop")]
    public float loopStartOffset;
    public float loopTriggerOffset;

    public static bool isBeat;
    public static int beats;

    
    [Header("UI")]
    [SerializeField] public Animator beatPulseAnimator;
    SpriteRenderer beatPulseSprite;
    [SerializeField] GameObject playerRing;
    [SerializeField] SpriteRenderer beatScore;
    private float beatScoreSpeed;
    private float beatScoreAlpha;

    [SerializeField] Sprite failSprite;
    [SerializeField] Sprite successSprite;
    [SerializeField] Sprite perfectSprite;

    public static bool isGameBeat; // If this is active, all monsters, attacks and entities not controlled by player input trigger
    public static bool menuGameBeat;
    private BeatTrigger lastFrameState, currentFrameState; // These are used to check when we enter the first frame of input and first frame after input
    private bool canCastGameBeat;
    public static float lastBeatTime;

    private MapTrack currentMapTrack;

    public static bool isPlaying;

    public static bool compassless;
    private float targetBeatPulseAlpha;

    public static void UpdatePulseAnimator()
    {
        //instance.beatPulseAnimator.gameObject.SetActive(!compassless);
    }

    public static void SetTrack(MapTrack track)
    {
        instance.music.Stop();
        beats = 0;
        instance.currentMapTrack = track;
        instance.secondsPerBeat = 60f / track.tempo;
        instance.music.clip = track.music;
        instance.offset = track.offset;
        instance.nextBeat = track.offset + audio_offset;
        instance.lastBeat = 0;
        instance.loopStartOffset = track.loopStart;
        instance.loopTriggerOffset = track.loopEnd;
        instance.currentTime = instance.music.time + audio_offset;
    }

    public static void Stop()
    {
        isPlaying = false;
        instance.beatPulseSprite.color = Color.clear;
        instance.music.time = 0;
        instance.music.Stop();
        beats = 0;
        instance.targetBeatPulseAlpha = 0;
    }

    public static void StartTrack()
    {
        instance.beatPulseSprite.color = Color.white;
        instance.music.volume = 1f;
        instance.music.Play();
        instance.targetBeatPulseAlpha = 1;

        isPlaying = true;
    }

    public static void FadeOut(float speed)
    {
        instance.StartCoroutine(instance.FadeOutCoroutine(speed));
    }

    public static void FadeIn(float speed)
    {
        instance.StartCoroutine(instance.FadeInCoroutine(speed));
    }

    private IEnumerator FadeOutCoroutine(float speed)
    {
        while (music.volume > 0)
        {
            music.volume = Mathf.MoveTowards(music.volume, 0f, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }

    private IEnumerator FadeInCoroutine(float speed)
    {
        while (music.volume < 1)
        {
            music.volume = Mathf.MoveTowards(music.volume, 1f, speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }

    public static float GetBeatDuration()
    {
        return instance.secondsPerBeat;
    }

    private void Awake()
    {
        instance = this;
        beatPulseSprite = beatPulseAnimator.GetComponent<SpriteRenderer>();
    }

    public static float GetActionDuration()
    {
        return instance.secondsPerBeat / 2f;
    }
    public static void OnPlayerAction()
    {
        if (!instance.canCastGameBeat) return;
        if (GameManager.isPaused)
        {
            menuGameBeat = true;
            return;
        }

        //if (compassless) return;
        return;
        instance.canCastGameBeat = false;
        isGameBeat = true;
        lastBeatTime = Time.time;
    }

    public static bool closestIsNextBeat()
    {
        float difftolast = Mathf.Abs(instance.currentTime - instance.lastBeat);
        float difftonext = Mathf.Abs(instance.nextBeat - instance.currentTime);

        return difftonext < difftolast;
    }
    public static float GetTempo()
    {
        return instance.tempo;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        beatPulseSprite.color = new Color(1, 1, 1, Mathf.MoveTowards(beatPulseSprite.color.a, targetBeatPulseAlpha, Time.deltaTime * 4f));
        if (!isBeat)
        {
            beatPulseAnimator.ResetTrigger("OnBeat");
        }
        if (isBeat) isBeat = false;
        
        if (currentTime < 1 && beats > 20)
        {
            beats = 1;
            lastBeat = 0;
            nextBeat = offset + audio_offset + (secondsPerBeat * beats);
        }
        currentTime = music.time + audio_offset;
        
        if (music.time >= loopTriggerOffset)
        {
            JumpToLoop();
        }

        CheckForGameBeat();
        if (music.time >= nextBeat)
        {
            beats++;
            lastBeat = nextBeat;
            nextBeat = offset + audio_offset + (secondsPerBeat * beats);
            OnBeat();
        }
        
        UpdateUI();
        //if (!compassless) CheckForGameBeat();
    }

    private void CalculateNextBeat(float time)
    {
        beats = (int)((time - offset - audio_offset) / secondsPerBeat);
        nextBeat = offset + audio_offset + (secondsPerBeat * beats);
        lastBeat = nextBeat - secondsPerBeat;
    }

    private void CheckForGameBeat()
    {
        currentFrameState = GetBeatSuccess();
        isGameBeat = false;
        menuGameBeat = false;
        /*
        if (!compassless)
        {
            if (lastFrameState == BeatTrigger.FAIL && currentFrameState != BeatTrigger.FAIL) // First Frame of Input
            {
                canCastGameBeat = true;
            }
            if (lastFrameState != BeatTrigger.FAIL && currentFrameState == BeatTrigger.FAIL) // After last Frame of Input
            {
                if (canCastGameBeat)
                {
                    lastBeatTime = Time.time;
                    if (!GameManager.isPaused) isGameBeat = true;
                    menuGameBeat = true;
                }
                canCastGameBeat = false;
            }
        }
        else
        {
            canCastGameBeat = true;
        }
        */
        canCastGameBeat = true;
        lastFrameState = currentFrameState;
    }

    private void JumpToLoop()
    {
        music.time = loopStartOffset;
        currentTime = music.time + audio_offset;
        CalculateNextBeat(loopStartOffset);
    }

    public static void SetRingPosition(Vector3 position)
    {
        instance.playerRing.transform.position = position;
    }

    private void OnBeat()
    {
        beatPulseAnimator.SetTrigger("OnBeat");
        //beatPulseAnimator.speed = 1 / secondsPerBeat;
        if (compassless)
        {
            
        }
        isGameBeat = true;
        menuGameBeat = true;
        isBeat = true;
        beatTest.Play();
    }

    private void UpdateUI()
    {
        if (beatScoreAlpha > 0)
        {
            beatScoreAlpha -= Time.deltaTime * 8f;
            beatScoreSpeed -= Time.deltaTime * 8f;
            beatScore.transform.localPosition = new Vector3(0, beatScore.transform.localPosition.y + beatScoreSpeed * Time.deltaTime, 0);
        }
        beatScore.color = new Color(1, 1, 1, beatScoreAlpha);
        if (!music.isPlaying) beatPulseAnimator.speed = 0f;
        else beatPulseAnimator.speed = 1 / secondsPerBeat;
    }

    public void StartMusic()
    {
        beatPulseAnimator.GetComponent<SpriteRenderer>().color = Color.white;
        music.Play();
    }

    public void StopMusic()
    {
        beatPulseAnimator.speed = 0;
        beatPulseAnimator.GetComponent<SpriteRenderer>().color = Color.clear;
    }

    public static bool isBeatAfter()
    {
        
        instance.currentTime = instance.music.time + audio_offset;

        float difftolast = Mathf.Abs(instance.currentTime - instance.lastBeat);
        float difftonext = Mathf.Abs(instance.nextBeat - instance.currentTime);

        return difftolast < difftonext;
    }

    public static BeatTrigger GetBeatSuccess()
    {
        if (compassless) return BeatTrigger.PERFECT;
        float successRange = instance.secondsPerBeat / 3f;
        float perfectRange = instance.secondsPerBeat / 12f;

        instance.currentTime = instance.music.time + audio_offset;

        float difftolast = Mathf.Abs(instance.currentTime - instance.lastBeat);
        float difftonext = Mathf.Abs(instance.nextBeat - instance.currentTime);
        
        float diff = difftolast < difftonext ? difftolast : difftonext;

        if (diff <= perfectRange) return BeatTrigger.PERFECT;
        else if (diff <= successRange) return BeatTrigger.SUCCESS;
        else return BeatTrigger.FAIL;
    }

    public static void TriggerBeatScore(BeatTrigger trigger)
    {
        instance.beatScoreSpeed = 2f;
        if (trigger == BeatTrigger.FAIL) instance.beatScore.sprite = instance.failSprite;
        else if (trigger == BeatTrigger.SUCCESS) instance.beatScore.sprite = instance.successSprite;
        else instance.beatScore.sprite = instance.perfectSprite;

        instance.beatScoreAlpha = 2.0f;
        instance.beatScore.transform.localPosition = new Vector3(0,0.5f, 0f);
    }
}
