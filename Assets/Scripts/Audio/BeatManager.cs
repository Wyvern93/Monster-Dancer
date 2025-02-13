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

    // secondsPerBeat remains the same
    public float secondsPerBeat;

    // These variables now represent times relative to “music time”
    public float nextBeat, lastBeat;
    public float currentTime;  // currentTime = (AudioSettings.dspTime - dspStartTime) + audio_offset

    public float nextMidBeat, lastMidBeat;
    public float nextQuarterBeat, lastQuarterBeat;
    public static bool isMidBeat, isQuarterBeat;
    public int midbeats, quarterBeats;

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

    public static bool menuGameBeat;
    private BeatTrigger lastFrameState, currentFrameState; // For checking beat timing of player input
    private bool canCastGameBeat;
    public static float lastBeatTime;

    private MapTrack currentMapTrack;

    public static bool isPlaying;

    public static bool compassless;
    private float targetBeatPulseAlpha;

    // NEW: Store the dsp time at which the music starts playing.
    private double dspStartTime;

    public static void UpdatePulseAnimator()
    {
        // instance.beatPulseAnimator.gameObject.SetActive(!compassless);
    }

    // SetTrack is called before starting the music.
    // We set our tempo, offsets, and also (for now) schedule the first beats relative to “music time.”
    public static void SetTrack(MapTrack track)
    {
        instance.music.Stop();
        beats = 0;
        instance.quarterBeats = 0;
        instance.midbeats = 0;
        instance.currentMapTrack = track;
        instance.secondsPerBeat = 60f / track.tempo;
        instance.music.clip = track.music;
        instance.offset = track.offset;
        // In your original code you add audio_offset in both currentTime and nextBeat.
        // Here we define “music time” as:
        //    currentTime = (AudioSettings.dspTime - dspStartTime) + audio_offset
        // and we set nextBeat relative to that music time.
        instance.nextBeat = instance.offset + audio_offset;
        instance.nextMidBeat = instance.offset + audio_offset;
        instance.nextQuarterBeat = instance.offset + audio_offset;
        instance.lastBeat = 0;
        instance.lastMidBeat = 0;
        instance.lastQuarterBeat = 0;
        instance.loopStartOffset = track.loopStart;
        instance.loopTriggerOffset = track.loopEnd;
        instance.currentTime = 0;
    }

    public static void Stop()
    {
        isPlaying = false;
        instance.beatPulseSprite.color = Color.clear;
        instance.music.Stop();
        beats = 0;
        instance.quarterBeats = 0;
        instance.midbeats = 0;
        instance.targetBeatPulseAlpha = 0;
    }

    // Instead of a simple Play(), we now schedule the music to start at a known DSP time.
    // That start time becomes our “zero” (or reference) for all beat calculations.
    public static void StartTrack()
    {
        instance.beatPulseSprite.color = Color.white;
        instance.music.volume = 1f;

        // Give the system a tiny delay so we can schedule the start.
        double startDelay = 0.1;
        instance.dspStartTime = AudioSettings.dspTime + startDelay;
        instance.music.PlayScheduled(instance.dspStartTime);

        instance.targetBeatPulseAlpha = 1;
        isPlaying = true;
        // (nextBeat, etc., were set already in SetTrack.)
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
            yield return null;
        }
        yield break;
    }

    private IEnumerator FadeInCoroutine(float speed)
    {
        while (music.volume < 1)
        {
            music.volume = Mathf.MoveTowards(music.volume, 1f, speed * Time.deltaTime);
            yield return null;
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
        return;
    }

    public static double GetNextBeatDSPTime()
    {
        // Convert the nextBeat (which is in "music time") to DSP time.
        // Using: musicTime = (AudioSettings.dspTime - dspStartTime) + audio_offset,
        // we have dspTime = dspStartTime + (musicTime - audio_offset)
        return instance.dspStartTime + (instance.nextBeat - audio_offset);
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

    // Update is now driven by DSP time.
    // We compute “music time” as the elapsed dsp time since music start plus audio_offset.
    void Update()
    {
        if (isPlaying)
        {
            currentTime = (float)(AudioSettings.dspTime - dspStartTime) + audio_offset;
        }
        else
        {
            currentTime = 0;
        }

        beatPulseSprite.color = new Color(1, 1, 1, Mathf.MoveTowards(beatPulseSprite.color.a, targetBeatPulseAlpha, Time.deltaTime * 4f));
        if (!isBeat)
        {
            beatPulseAnimator.ResetTrigger("OnBeat");
        }
        if (isBeat) isBeat = false;
        if (isQuarterBeat) isQuarterBeat = false;
        if (isMidBeat) isMidBeat = false;

        if (currentTime < 1 && beats > 20)
        {
            beats = 1;
            lastBeat = 0;
            nextBeat = offset + audio_offset;
        }

        // Looping now checks against our DSP-based currentTime.
        if (currentTime >= loopTriggerOffset)
        {
            JumpToLoop();
        }

        CheckForGameBeat();

        // Trigger the beat when our calculated “music time” passes nextBeat.
        if (currentTime >= nextBeat)
        {
            beats++;
            lastBeat = nextBeat;
            nextBeat = offset + audio_offset + (secondsPerBeat * beats);
            OnBeat();
        }

        if (currentTime >= nextMidBeat)
        {
            midbeats++;
            lastMidBeat = nextMidBeat;
            nextMidBeat = offset + audio_offset + ((secondsPerBeat / 2f) * midbeats);
            isMidBeat = true;
        }

        if (currentTime >= nextQuarterBeat)
        {
            quarterBeats++;
            lastQuarterBeat = nextQuarterBeat;
            nextQuarterBeat = offset + audio_offset + ((secondsPerBeat / 4f) * quarterBeats);
            isQuarterBeat = true;
        }

        UpdateUI();
    }

    // Recalculate beat times from a given “music time” (this is unchanged aside from the fact that “time” here is DSP‐based music time).
    private void CalculateNextBeat(float time)
    {
        beats = (int)((time - offset - audio_offset) / secondsPerBeat);
        nextBeat = offset + audio_offset + (secondsPerBeat * beats);
        lastBeat = nextBeat - secondsPerBeat;

        midbeats = (int)((time - offset - audio_offset) / (secondsPerBeat / 2f));
        nextMidBeat = offset + audio_offset + ((secondsPerBeat / 2f) * midbeats);
        lastMidBeat = nextMidBeat - (secondsPerBeat / 2f);

        quarterBeats = (int)((time - offset - audio_offset) / (secondsPerBeat / 4f));
        nextQuarterBeat = offset + audio_offset + ((secondsPerBeat / 4f) * quarterBeats);
        lastQuarterBeat = nextQuarterBeat - (secondsPerBeat / 4f);
    }

    private void CheckForGameBeat()
    {
        currentFrameState = GetBeatSuccess();
        isBeat = false;
        menuGameBeat = false;

        canCastGameBeat = true;
        lastFrameState = currentFrameState;
    }

    // When looping, we adjust dspStartTime so that our “music time” resets to loopStartOffset.
    private void JumpToLoop()
    {
        // We want: currentTime = (AudioSettings.dspTime - dspStartTime) + audio_offset == loopStartOffset.
        // Solve for dspStartTime:
        dspStartTime = AudioSettings.dspTime + audio_offset - loopStartOffset;
        currentTime = (float)(AudioSettings.dspTime - dspStartTime) + audio_offset;
        CalculateNextBeat(loopStartOffset);
    }

    public static void SetRingPosition(Vector3 position)
    {
        instance.playerRing.transform.position = position;
    }

    // When the beat occurs, trigger your UI and play the test sound.
    // (You could also use PlayScheduled here if you’d like to schedule the sound ahead—but that would mean knowing the DSP time
    // a frame in advance. In this example, we assume OnBeat is called as close as possible to the desired moment.)
    private void OnBeat()
    {
        beatPulseAnimator.SetTrigger("OnBeat");
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
        instance.currentTime = (float)(AudioSettings.dspTime - instance.dspStartTime) + audio_offset;
        float difftolast = Mathf.Abs(instance.currentTime - instance.lastBeat);
        float difftonext = Mathf.Abs(instance.nextBeat - instance.currentTime);
        return difftolast < difftonext;
    }

    public static BeatTrigger GetBeatSuccess()
    {
        if (compassless) return BeatTrigger.PERFECT;
        float successRange = instance.secondsPerBeat / 3f;
        float perfectRange = instance.secondsPerBeat / 12f;

        instance.currentTime = (float)(AudioSettings.dspTime - instance.dspStartTime) + audio_offset;

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
        instance.beatScore.transform.localPosition = new Vector3(0, 0.5f, 0f);
    }
}
