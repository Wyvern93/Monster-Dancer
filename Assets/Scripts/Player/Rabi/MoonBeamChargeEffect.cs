using UnityEngine;

public class MoonBeamChargeEffect : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] MoonBeam beam;
    
    [SerializeField] AudioClip chargeSound;
    [SerializeField] ParticleSystem particles;

    private int beamDuration = 12;
    private int maxBeamDuration = 12;

    private bool charged = false;
    public void OnBeamChargeFinish()
    {
        charged = true;
        StartBeam();
    }
    
    public void OnAnimationEnd()
    {
        PoolManager.Return(beam.gameObject, beam.GetType());
    }

    public void OnEnable()
    {
        beam.OnInit();
        animator.Play("MoonBeam_Start");
        AudioController.PlaySound(chargeSound);
        charged = false;

        int abilityLevel = (int)Player.instance.abilityValues["ability.moonbeam.level"];

        if (abilityLevel == 1) maxBeamDuration = 12;
        else if (abilityLevel >= 4) maxBeamDuration = 18;

        maxBeamDuration = 
        beamDuration = maxBeamDuration;
        particles.Play();
    }

    public void Update()
    {
        if (!charged)
        {
            Player.TriggerCameraShake(0.2f, 0.1f);
            return;
        }
        Player.TriggerCameraShake(0.3f, 0.2f);

        if (BeatManager.isGameBeat)
        {
            if (beamDuration > 0)
            {
                beamDuration--;
                if (beamDuration == 1f)
                {
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

    private void StartBeam()
    {
        beam.OnBeamStart();
    }
}