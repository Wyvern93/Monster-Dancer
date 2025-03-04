using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class BeatManager : MonoBehaviour
{
    [SerializeField] AudioClip testClip;
    public static BeatManager instance;

    [SerializeField] private AudioSource music, beatTest;
    [SerializeField] private float offset, tempo;
    public static float audio_offset;
    public static int compassStartBeat;

    [SerializeField] SpriteRenderer skillZBeatIndicator, skillXBeatIndicator, skillCBeatIndicator;
    [SerializeField] Transform skillIndicatorTransform;
    // secondsPerBeat remains the same
    public float secondsPerBeat;

    // These variables now represent times relative to “music time”
    public float nextBeat, lastBeat;
    public float currentTime;  // currentTime = (AudioSettings.dspTime - dspStartTime) + audio_offset

    public float nextMidBeat, lastMidBeat;
    public float nextQuarterBeat, lastQuarterBeat;
    public static bool isMidBeat, isQuarterBeat;
    public int midbeats, quarterBeats;
    
    private List<BeatObject> activeTimelineObjects = new List<BeatObject>();
    private List<BeatObject> activeZBeatObjects = new List<BeatObject>();
    private List<BeatObject> activeXBeatObjects = new List<BeatObject>();
    private List<BeatObject> activeCBeatObjects = new List<BeatObject>();
    [SerializeField] Animator beatPulse;

    [SerializeField] Sprite compassSprite, beatSprite, midBeatSprite;
    [SerializeField] Sprite zSprite, xSprite, cSprite;

    [SerializeField] GameObject beatObjectPrefab;
    [SerializeField] GameObject beatSucceedEffectPrefab;
    [SerializeField] Transform beatObjectsParent;
    [SerializeField] Transform timelineObjectsParent;

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
    public MidBeat currentMidBeat;
    private List<MidBeat> midbeatList;

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
        compassStartBeat = track.compassStartBeat;
        instance.GenerateMidBeats();
    }

    private void GenerateMidBeats()
    {
        float initialBeat = instance.nextMidBeat;
        float musicLength = instance.music.clip.length;
        float midBeatLength = (secondsPerBeat / 2f);

        int midBeatsTotal = (int)(musicLength / midBeatLength) + 1;
        Debug.Log($"There is a total of {midBeatsTotal} generated");

        midbeatList = new List<MidBeat>();
        for (int i = 0; i < midBeatsTotal; i++)
        {
            midbeatList.Add(new MidBeat(initialBeat + (i * midBeatLength), GetMidBeat(i)));
        }
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
        double nextMidBeatTime = instance.dspStartTime + (instance.nextMidBeat - audio_offset);
        //instance.currentMidBeat = new MidBeat(nextMidBeatTime);
        instance.ClearBeatObjects();
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
        StartBeatObjects();
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

    public static double GetNextMidBeatDSPTime()
    {
        // Convert the nextBeat (which is in "music time") to DSP time.
        // Using: musicTime = (AudioSettings.dspTime - dspStartTime) + audio_offset,
        // we have dspTime = dspStartTime + (musicTime - audio_offset)
        return instance.dspStartTime + (instance.nextMidBeat - audio_offset);
    }

    public static double GetNextQuarterDSPTime()
    {
        // Convert the nextBeat (which is in "music time") to DSP time.
        // Using: musicTime = (AudioSettings.dspTime - dspStartTime) + audio_offset,
        // we have dspTime = dspStartTime + (musicTime - audio_offset)
        return instance.dspStartTime + (instance.nextQuarterBeat - audio_offset);
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
        if (InputManager.ActionPress(InputActionType.FIRST_SKILL))
        {
            //AudioController.PlaySoundWithoutWait(testClip);
        }
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
            OnMidBeat();
            isMidBeat = true;
            //currentMidBeat = new MidBeat(offset + audio_offset + ((secondsPerBeat / 2f) * midbeats));
        }

        if (currentTime >= nextQuarterBeat)
        {
            quarterBeats++;
            lastQuarterBeat = nextQuarterBeat;
            nextQuarterBeat = offset + audio_offset + ((secondsPerBeat / 4f) * quarterBeats);
            isQuarterBeat = true;
            OnQuarterBeat();
        }

        if (Player.instance == null)
        {
            skillZBeatIndicator.color = Color.clear;
            skillXBeatIndicator.color = Color.clear;
            skillCBeatIndicator.color = Color.clear;
        }
        UpdateTimeline();
        UpdateUI();
    }

    // Update the timeline's beat objects based on the current time
    private void UpdateTimeline()
    {
        // Lines
        while (activeTimelineObjects.Count < 16)
        {
            float nextBeatTime = nextMidBeat + ((secondsPerBeat / 2) * activeTimelineObjects.Count);
            int midbeat = midbeats + 1 + activeTimelineObjects.Count;

            Sprite targetSprite;
            string animation = "";
            if (CheckBeat(midbeat, BeatType.Compass))
            {
                targetSprite = compassSprite;
                animation = "pulsecompass";
            }
            else if (CheckBeat(midbeat, BeatType.Beat))
            {
                targetSprite = beatSprite;
                animation = "pulsebeat";
            }
            else
            {
                targetSprite = midBeatSprite;
                animation = "pulsemidbeat";
            }

            BeatObject beatObjectLeft = PoolManager.Get<BeatObject>();
            beatObjectLeft.transform.parent = timelineObjectsParent.transform;
            beatObjectLeft.transform.localScale = Vector3.one;
            beatObjectLeft.OnInit(nextBeatTime, false, midbeat, targetSprite, 0);  // Left side
            activeTimelineObjects.Add(beatObjectLeft);


            if (targetSprite)
                beatPulse.Play(animation);
        }

        // Create each beatObject 
        while (activeZBeatObjects.Count < 16)
        {
            float nextBeatTime = nextMidBeat + ((secondsPerBeat / 2) * activeZBeatObjects.Count);
            int midbeat = midbeats + 1 + activeZBeatObjects.Count;

            bool isZbeat = false;
            if (Player.instance != null)
            {
                if (Player.instance.equippedPassiveAbilities.Count > 0)
                {
                    if (CheckBeat(midbeat, Player.instance.equippedPassiveAbilities[0].getBeatTrigger())) isZbeat = true;
                }
            }

            BeatObject beatObjectLeft = PoolManager.Get<BeatObject>();
            beatObjectLeft.transform.parent = beatObjectsParent.transform;
            beatObjectLeft.transform.localScale = Vector3.one;
            beatObjectLeft.OnInit(nextBeatTime, false, midbeat, isZbeat ? zSprite : null, 10);  // Left side
            activeZBeatObjects.Add(beatObjectLeft);
        }

        while (activeXBeatObjects.Count < 16)
        {
            float nextBeatTime = nextMidBeat + ((secondsPerBeat / 2) * activeXBeatObjects.Count);
            int midbeat = midbeats + 1 + activeXBeatObjects.Count;

            bool isXbeat = false;
            if (Player.instance != null)
            {
                if (Player.instance.equippedPassiveAbilities.Count > 1)
                {
                    if (CheckBeat(midbeat, Player.instance.equippedPassiveAbilities[1].getBeatTrigger())) isXbeat = true;
                }
            }

            BeatObject beatObjectLeft = PoolManager.Get<BeatObject>();
            beatObjectLeft.transform.parent = beatObjectsParent.transform;
            beatObjectLeft.transform.localScale = Vector3.one;
            beatObjectLeft.OnInit(nextBeatTime, false, midbeat, isXbeat ? xSprite : null, 0);  // Left side
            activeXBeatObjects.Add(beatObjectLeft);
        }

        while (activeCBeatObjects.Count < 16)
        {
            float nextBeatTime = nextMidBeat + ((secondsPerBeat / 2) * activeCBeatObjects.Count);
            int midbeat = midbeats + 1 + activeCBeatObjects.Count;

            bool isCbeat = false;
            if (Player.instance != null)
            {
                if (Player.instance.equippedPassiveAbilities.Count > 2)
                {
                    if (CheckBeat(midbeat, Player.instance.equippedPassiveAbilities[2].getBeatTrigger())) isCbeat = true;
                }
            }

            BeatObject beatObjectLeft = PoolManager.Get<BeatObject>();
            beatObjectLeft.transform.parent = beatObjectsParent.transform;
            beatObjectLeft.transform.localScale = Vector3.one;
            beatObjectLeft.OnInit(nextBeatTime, false, midbeat, isCbeat ? cSprite : null, -10);  // Left side
            activeCBeatObjects.Add(beatObjectLeft);
        }
    }

    public bool CheckBeat(int midbeatNum, BeatType beatType)
    {
        int numBeat = midbeatNum - compassStartBeat;
        if (beatType == BeatType.Mid) return true;
        if (beatType == BeatType.Beat) return numBeat % 2 == 0;
        if (beatType == BeatType.MidCompass) return numBeat % 4 == 0;
        if (beatType == BeatType.Compass) return numBeat % 8 == 0;
        return false;
    }

    public BeatType GetMidBeat(int midbeatNum)
    {
        int numBeat = midbeatNum - compassStartBeat;
        if (numBeat % 8 == 0) return BeatType.Compass;
        else if (numBeat % 4 == 0) return BeatType.MidCompass;
        else if (numBeat % 2 == 0) return BeatType.Beat;
        else return BeatType.Mid;
    }

    private void OnQuarterBeat()
    {
        
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
        //currentFrameState = GetBeatSuccess(BeatType.Beat);
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
        music.time = loopStartOffset;
        dspStartTime = AudioSettings.dspTime + audio_offset - loopStartOffset;
        currentTime = (float)(AudioSettings.dspTime - dspStartTime) + audio_offset;
        CalculateNextBeat(loopStartOffset);
        ClearBeatObjects();
        foreach (MidBeat mid in midbeatList)
        {
            mid.used = false;
            // We only reset the beats ahead
            //if (mid.maximumTime < currentTime) mid.used = false;
        }
    }

    public void ClearBeatObjects()
    {
        foreach (BeatObject beat in activeTimelineObjects)
        {
            PoolManager.Return(beat.gameObject, typeof(BeatObject));
        }
        foreach (BeatObject beat in activeZBeatObjects)
        {
            PoolManager.Return(beat.gameObject, typeof(BeatObject));
        }
        foreach (BeatObject beat in activeXBeatObjects)
        {
            PoolManager.Return(beat.gameObject, typeof(BeatObject));
        }
        foreach (BeatObject beat in activeCBeatObjects)
        {
            PoolManager.Return(beat.gameObject, typeof(BeatObject));
        }
        activeTimelineObjects.Clear();
        activeZBeatObjects.Clear();
        activeXBeatObjects.Clear();
        activeCBeatObjects.Clear();
    }

    public void OnBeatObjectBeat(BeatObject beatObject)
    {
        beatObject.beatTime = nextBeat + ((nextBeat - currentTime) * 16);
    }

    private void StartBeatObjects()
    {
        PoolManager.CreatePool(typeof(BeatObject), beatObjectPrefab, 200);
        PoolManager.CreatePool(typeof(BeatSucessEffect), beatSucceedEffectPrefab, 12);
        activeTimelineObjects = new List<BeatObject>();
        activeZBeatObjects = new List<BeatObject>();
        activeXBeatObjects = new List<BeatObject>();
        activeCBeatObjects = new List<BeatObject>();
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

    private void OnMidBeat()
    {
        isMidBeat = true;
        if (activeTimelineObjects.Count > 0)
        {
            PoolManager.Return(activeTimelineObjects[0].gameObject, typeof(BeatObject));
            activeTimelineObjects.Remove(activeTimelineObjects[0]);
        }
        if (activeZBeatObjects.Count > 0)
        {
            PoolManager.Return(activeZBeatObjects[0].gameObject, typeof(BeatObject));
            activeZBeatObjects.Remove(activeZBeatObjects[0]);
        }
        if (activeXBeatObjects.Count > 0)
        {
            PoolManager.Return(activeXBeatObjects[0].gameObject, typeof(BeatObject));
            activeXBeatObjects.Remove(activeXBeatObjects[0]);
        }
        if (activeCBeatObjects.Count > 0)
        {
            PoolManager.Return(activeCBeatObjects[0].gameObject, typeof(BeatObject));
            activeCBeatObjects.Remove(activeCBeatObjects[0]);
        }

        //Debug.Log("Midbeat time: " + Time.time);
        //AudioController.PlaySoundWithoutWait(testClip);
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

    public enum BeatType
    {
        Beat,
        Mid,
        Quarter,
        MidCompass,
        Compass
    }

    public void UseMidBeat()
    {
        currentMidBeat.used = true;
    }

    public static BeatTrigger GetBeatSuccess(BeatType type, bool special = false)
    {
        //if (compassless) return BeatTrigger.PERFECT;
        float currentTime = (float)(AudioSettings.dspTime - instance.dspStartTime) + audio_offset;
        if (special)
        {
            float beatWindow = instance.secondsPerBeat / 3f; // // 3 -> Normal, 3.5 -> Hard, 4 -> Dancer
            float closestTime = -1;

            float lastMin = instance.lastBeat - beatWindow;
            float lastMax = instance.lastBeat + beatWindow;

            float nextMin = instance.nextBeat - beatWindow;
            float nextMax = instance.nextBeat + beatWindow;

            if (currentTime > lastMin && currentTime < lastMax) return BeatTrigger.PERFECT;
            if (currentTime > nextMin && currentTime < nextMax) return BeatTrigger.PERFECT;
            return BeatTrigger.FAIL;
        }

        // Find the closest MidBeat unused
        List<MidBeat> unusedBeats = instance.midbeatList.FindAll(x => x.used == false);
        if (unusedBeats.Count == 0) return BeatTrigger.FAIL;

        float range = instance.secondsPerBeat;
        for (int i = 0; i < unusedBeats.Count; i++)
        {
            Debug.Log($"Input: {instance.currentTime} | BeatTimes: {unusedBeats[i].minimumTime} ~ {unusedBeats[i].maximumTime}");
            if (unusedBeats[i].minimumTime > instance.currentTime + range)
            {
                unusedBeats[i].used = true;
                continue; // This Midbeat is too far ahead
            }
            if (unusedBeats[i].maximumTime < instance.currentTime - range)
            {
                unusedBeats[i].used = true; // If it's behind time it can't be reused
                continue; //return BeatTrigger.FAIL; // This Midbeat is too far behind
            }
            
            // Unused midbeat in the time window

            if (instance.currentTime > unusedBeats[i].maximumTime)
            {
                unusedBeats[i].used = true;
                continue;
            }
            if (instance.currentTime >= unusedBeats[i].minimumTime && instance.currentTime <= unusedBeats[i].maximumTime)
            {
                unusedBeats[i].used = true;
                int id = instance.midbeatList.IndexOf(unusedBeats[i]);
                int testingMidbeat = instance.midbeatList.IndexOf(instance.midbeatList[id]);
                BeatType beatType = instance.GetMidBeat(testingMidbeat + 9);
                Debug.Log($"Last MidBeat: {instance.midbeats} | Trying to ask {testingMidbeat}");
                Debug.Log(beatType);
                if (instance.CheckBeat(testingMidbeat + 9, type))
                {

                    return BeatTrigger.PERFECT;
                }
                else
                {
                    Debug.Log("not the beat expected");
                    return BeatTrigger.FAIL;
                }
                //return BeatTrigger.PERFECT;
            }
            else
            {
            }
            unusedBeats[i].used = false;
            return BeatTrigger.FAIL;
        }

        return BeatTrigger.FAIL; // If nothing works, there's no beat 
        /*
        // Default to full beat values.
        double beatDuration = instance.secondsPerBeat / 2f;
        double lastBeatTime = instance.currentMidBeat.minimumTime;
        double nextBeatTime = instance.currentMidBeat.maximumTime;

        float currentTime = (float)(AudioSettings.dspTime - instance.dspStartTime) + audio_offset;

        float diff = Mathf.Abs((float)(currentTime - instance.currentMidBeat.baseTime));

        if (diff <= beatDuration * 0.8f)
            return BeatTrigger.SUCCESS;
        else 
            return BeatTrigger.FAIL;*/
        /*
        if (diff <= perfectRange)
            return BeatTrigger.PERFECT;
        else if (diff <= successRange)
            return BeatTrigger.SUCCESS;
        else
            return BeatTrigger.FAIL;*/
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
