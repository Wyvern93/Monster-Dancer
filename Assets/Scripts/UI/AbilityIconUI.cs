using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIconUI : MonoBehaviour
{
    public Image icon;
    [SerializeField] private Image cdImage;
    public float cooldown;
    [SerializeField] private Color frameColor;
    [SerializeField] Image Pulse;
    private Color pulseColor;
    private float pulseOpacity;
    private float PulseSize;

    [SerializeField] int skillID;


    private void Awake()
    {
        pulseColor = Pulse.color;
        pulseOpacity = 0;
        PulseSize = 1;
    }

    public void SetAbilityIcon(Sprite sprite, bool isItem)
    {
        if (sprite == null)
        {
            icon.color = Color.clear;
            cooldown = 0;
            cdImage.fillAmount = 0;
        }
        else
        {
            icon.color = Color.white;
            icon.sprite = sprite;
        }
    }

    public void setCooldown(float value)
    {

        float min = 0.07f;
        float max = 0.86f;// total: 0.930f;

        cooldown = value;
        cdImage.fillAmount = (cooldown * max) + min; 
    }

    private void Update()
    {
        if (Player.instance == null) return;

        if (!BeatManager.isMidBeat)
        {
            pulseOpacity = Mathf.MoveTowards(pulseOpacity, 0, Time.deltaTime * 2f);
            PulseSize = Mathf.MoveTowards(PulseSize, 1f, Time.deltaTime);
        }
        Pulse.color = new Color(pulseColor.r, pulseColor.g, pulseColor.b, pulseOpacity);
        Pulse.transform.localScale = Vector3.one * PulseSize;

        if (!BeatManager.isMidBeat) return;

        if (Player.instance.equippedPassiveAbilities.Count > skillID )
        {
            if (BeatManager.instance.CheckBeat(BeatManager.instance.midbeats, Player.instance.equippedPassiveAbilities[skillID].getBeatTrigger()))
            {
                pulseOpacity = 1f;
                PulseSize = 1.2f;
            }
        }
    }
}