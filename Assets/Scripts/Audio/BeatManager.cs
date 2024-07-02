using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatManager : MonoBehaviour
{
    [SerializeField] private AudioSource music, beatTest;
    [SerializeField] private float offset, tempo;
    [SerializeField] private float audio_offset;

    public float secondsPerBeat, nextBeat, lastBeat;
    public float currentTime;

    [Header("Loop")]
    public float loopStartOffset;
    public float loopTriggerOffset;

    public static bool isBeat;
    public static int beats;

    [Header("UI")]
    [SerializeField] Image BeatIndicator;
    [SerializeField] GameObject BeatCarrousel;
    private float CarrouselPos;

    private static BeatManager instance;

    [SerializeField] Image TestImage;

    [SerializeField] Animator beatPulseAnimator;
    [SerializeField] GameObject playerRing;
    [SerializeField] SpriteRenderer beatScore;
    private float beatScoreSpeed;
    private float beatScoreAlpha;

    [SerializeField] Sprite failSprite;
    [SerializeField] Sprite successSprite;
    [SerializeField] Sprite perfectSprite;

    public static bool isGameBeat; // If this is active, all monsters, attacks and entities not controlled by player input trigger
    private BeatTrigger lastFrameState, currentFrameState; // These are used to check when we enter the first frame of input and first frame after input
    private bool canCastGameBeat;

    public static float GetBeatDuration()
    {
        return instance.secondsPerBeat;
    }

    private void Awake()
    {
        instance = this;
    }

    public static float GetActionDuration()
    {
        return instance.secondsPerBeat / 2f;
    }
    public static void OnPlayerAction()
    {
        instance.canCastGameBeat = false;
        isGameBeat = true;
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
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        secondsPerBeat = 60f / tempo;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBeat)
        {
            beatPulseAnimator.ResetTrigger("OnBeat");
        }
        if (isBeat) isBeat = false;
        
        if (music.time < 1 && beats > 20)
        {
            beats = 1;
            lastBeat = 0;
            nextBeat = offset + (secondsPerBeat * beats);
        }
        currentTime = music.time + audio_offset;
        
        if (music.time >= loopTriggerOffset)
        {
            JumpToLoop();
        }

        if (music.time >= nextBeat)
        {
            beats++;
            lastBeat = nextBeat;
            nextBeat = offset + (secondsPerBeat * beats);
            OnBeat();
        }
        UpdateUI();
        CheckForGameBeat();
    }

    private void CalculateNextBeat(float time)
    {
        beats = (int)((time - offset) / secondsPerBeat);
        nextBeat = offset + (secondsPerBeat * beats);
    }

    private void CheckForGameBeat()
    {
        currentFrameState = GetBeatSuccess();
        isGameBeat = false;
        if (lastFrameState == BeatTrigger.FAIL &&  currentFrameState != BeatTrigger.FAIL) // First Frame of Input
        {
            canCastGameBeat = true;
        }
        if (lastFrameState != BeatTrigger.FAIL && currentFrameState == BeatTrigger.FAIL) // After last Frame of Input
        {
            if (canCastGameBeat) isGameBeat = true;

            canCastGameBeat = false;
        }
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
        beatPulseAnimator.speed = 1 / secondsPerBeat;
        
        TestImage.color = Color.red;
        isBeat = true;
        beatTest.Play();
        CarrouselPos = 0;
    }

    private void UpdateUI()
    {
        float speed = 80f; //80f per secondsPerBeat

        float carrouselOffset = offset / -80 * secondsPerBeat;
        CarrouselPos -= speed * (Time.deltaTime / secondsPerBeat);
        BeatCarrousel.GetComponent<RectTransform>().anchoredPosition = new Vector2(CarrouselPos + carrouselOffset, 0);

        if (beatScoreAlpha > 0)
        {
            beatScoreAlpha -= Time.deltaTime * 8f;
            beatScoreSpeed -= Time.deltaTime * 8f;
            beatScore.transform.localPosition = new Vector3(0, beatScore.transform.localPosition.y + beatScoreSpeed * Time.deltaTime, 0);
        }
        beatScore.color = new Color(1, 1, 1, beatScoreAlpha);
    }

    public static bool isBeatAfter()
    {
        instance.currentTime = instance.music.time + instance.audio_offset;

        float difftolast = Mathf.Abs(instance.currentTime - instance.lastBeat);
        float difftonext = Mathf.Abs(instance.nextBeat - instance.currentTime);

        return difftolast < difftonext;
    }

    public static BeatTrigger GetBeatSuccess()
    {
        float successRange = instance.secondsPerBeat / 4f;
        float perfectRange = instance.secondsPerBeat / 12f;

        instance.currentTime = instance.music.time + instance.audio_offset;

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
