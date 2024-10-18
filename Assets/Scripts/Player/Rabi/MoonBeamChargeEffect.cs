using UnityEngine;

public class MoonBeamChargeEffect : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] MoonBeam beam;
    
    [SerializeField] AudioClip chargeSound;
    [SerializeField] ParticleSystem particles;

    private int beamDuration = 4;
    private int maxBeamDuration = 4;

    private bool charged = false;
    private bool hitThisBeat;
    public void OnBeamChargeFinish()
    {
        charged = true;
        StartBeam();
    }
    
    public void OnAnimationEnd()
    {
        Player.instance.despawneables.Remove(beam);
        PoolManager.Return(beam.gameObject, beam.GetType());
    }

    public void OnEnable()
    {
        beam.OnInit();
        animator.Play("MoonBeam_Start");
        AudioController.PlaySound(chargeSound);
        charged = false;

        int abilityLevel = (int)Player.instance.abilityValues["ability.moonbeam.level"];

        if (abilityLevel >= 3) maxBeamDuration = 6;
        else maxBeamDuration = 4;
 

        beamDuration = maxBeamDuration;
        particles.Play();
    }

    public void Update()
    {
        if (BeatManager.isGameBeat && !GameManager.isPaused)
        {
            hitThisBeat = false;
            if (beamDuration > 0)
            {
                beamDuration--;
                if (beamDuration == 1f)
                {
                    AudioController.FadeOut();
                    particles.Stop();
                }
            }
            else
            {
                beam.OnBeamEnd();
                charged = false;
                animator.Play("MoonBeam_End");
            }
        }
        
    }

    public void OnBeamHit()
    {
        if (hitThisBeat) return;

        MoonlightShockwave shockwave = PoolManager.Get<MoonlightShockwave>();
        shockwave.dmg = 4;
        shockwave.transform.position = transform.position;
    }

    private void StartBeam()
    {
        beam.OnBeamStart();
    }
}