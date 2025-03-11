using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AbilityIconUI : MonoBehaviour
{
    public Image icon;
    [SerializeField] private Image cdImage;
    public float cooldown;
    public float expPercent;
    [SerializeField] private Color frameColor;
    [SerializeField] Image Pulse;
    private Color pulseColor;
    private float pulseOpacity;
    private float PulseSize;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] int skillID;


    private void Awake()
    {
        pulseColor = Pulse.color;
        pulseOpacity = 0;
        PulseSize = 1;
        levelText.text = "";
    }

    public void SetAbilityIcon(Sprite sprite, int level)
    {
        if (sprite == null)
        {
            icon.color = Color.clear;
            cooldown = 0;
            cdImage.fillAmount = 0;
            levelText.text = "";
            expPercent = 0;
        }
        else
        {
            icon.color = Color.white;
            icon.sprite = sprite;
            levelText.text = "Lv." + level;
            expPercent = 0;
        }
    }
    
    public void UpdateExp(float level, float expPercent)
    {
        this.expPercent = expPercent;
        float min = 0.07f;
        float max = 0.86f;// total: 0.930f;

        cooldown = expPercent;
        cdImage.fillAmount = (cooldown * max) + min;
        levelText.text = "Lv." + level;
    }

    public void setCooldown(float value)
    {
       
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