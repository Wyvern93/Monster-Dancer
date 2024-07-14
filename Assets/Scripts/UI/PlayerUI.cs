using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Image hpBar;
    private RectTransform hpTransform;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] Image hurtEffect;

    [SerializeField] GameObject spHud;
    [SerializeField] Image spBar;
    private RectTransform spTransform;

    [SerializeField] Image expBar;
    private RectTransform expTransform;
    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] TextMeshProUGUI MapTimeText;
    [SerializeField] TextMeshProUGUI StageText;
    [SerializeField] GameObject damageText;
    [SerializeField] Transform damageTextParent;

    public Image crosshair;
    RectTransform crosshair_transform;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] SpriteRenderer beatIndicatorSpr;

    [SerializeField] Image bossBar;
    private RectTransform bossBarTransform;
    [SerializeField] CanvasGroup bossBarGroup;
    [SerializeField] TextMeshProUGUI bossBarName;
    [SerializeField] TextMeshProUGUI bossBarHPText;

    [SerializeField] List<PlayerUIIcon> abilityIcons;
    // Start is called before the first frame update
    void Awake()
    {
        hpTransform = hpBar.GetComponent<RectTransform>();
        spTransform = spBar.GetComponent<RectTransform>();
        expTransform = expBar.GetComponent<RectTransform>();
        crosshair_transform = crosshair.GetComponent<RectTransform>();
        Cursor.visible = false;

        bossBarTransform = bossBar.GetComponent<RectTransform>();
        bossBarGroup.alpha = 0;

        foreach (PlayerUIIcon icon in abilityIcons)
        {
            icon.Display(null, 0, false);
        }
    }

    public void CreatePools()
    {
        PoolManager.CreatePool(typeof(DamageText), damageText, 100);
    }

    public  void SetBossBarName(string name)
    {
        bossBarName.text = name;
    }

    public void HideBossBar()
    {
        bossBarGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        hurtEffect.color = Color.Lerp(hurtEffect.color, new Color(1, 1, 1, 0), Time.unscaledDeltaTime * 8f);
        if (InputManager.playerDevice == InputManager.InputDeviceType.Keyboard || Player.instance == null)
        {
            crosshair_transform.localPosition = Mouse.current.position.value / new Vector2(Screen.width, Screen.height) * new Vector2(640, 360) - new Vector2(320, 180);
        }
        else
        {
            Vector2 offset = (Vector2)Player.instance.transform.position + (InputManager.GetRightStick().normalized * 128f);
            if (InputManager.GetRightStick() == Vector2.zero) offset = crosshair_transform.localPosition;
            crosshair_transform.localPosition = Vector3.MoveTowards(crosshair_transform.localPosition, offset, Time.unscaledDeltaTime * 1280f); //Mouse.current.position.value / new Vector2(Screen.width, Screen.height) * new Vector2(640, 360) - new Vector2(320, 180);
        }
        
        if (Map.Instance != null) 
        {
            UpdateStageTime();
            bossBarGroup.alpha = Mathf.MoveTowards(bossBarGroup.alpha, Map.isBossWave() ? 1 : 0, Time.deltaTime);
        }
    }

    public void SetStageText(string text)
    {
        StageText.text = text;
    }

    public void UpdateBossBar(int current, int max)
    {
        float health = (float)current / (float)max;
        float width = (int)(190f * health);
        bossBarTransform.sizeDelta = new Vector2(width, bossBarTransform.sizeDelta.y);
        bossBarHPText.text = $"{current}/{max}";
    }

    public void SetWeaponIcon(Sprite sprite, int level)
    {
        abilityIcons[0].Display(sprite, level, false);
    }

    public void SetWeaponLevel(int level)
    {
        abilityIcons[0].SetLevel(level, false);
    }

    public void SetUltimateIcon(Sprite sprite, int level, bool maxed)
    {
        abilityIcons[1].Display(sprite, level, maxed);
    }

    public void SetUltimateLevel(int level, bool maxed)
    {
        abilityIcons[1].SetLevel(level, maxed);
    }

    public void SetActiveIcon(Sprite sprite, int level, bool maxed)
    {
        abilityIcons[2].Display(sprite, level, maxed);
    }

    public void SetActiveLevel(int level, bool maxed)
    {
        abilityIcons[2].SetLevel(level, maxed);
    }

    public void SetPassiveIcon(Sprite sprite, int level, bool maxed, int id)
    {
        abilityIcons[id + 3].Display(sprite, level, maxed);
    }

    public void SetPassiveLevel(int level, bool maxed, int id)
    {
        abilityIcons[id + 3].SetLevel(level, maxed);
    }


    public void ShowSPBar()
    {
        spHud.SetActive(true);
    }

    public void HideSPBar()
    {
        spHud.SetActive(false);
    }

    public void SpawnDamageText(Vector2 position, int number, DamageTextType textType)
    {
        DamageText damageText = PoolManager.Get<DamageText>();
        damageText.transform.SetParent(damageTextParent, true);
        damageText.transform.localScale = Vector3.one;
        damageText.transform.position = position;
        damageText.text.text = number.ToString();

        switch (textType)
        {
            default:
            case DamageTextType.Normal:
                damageText.color = Color.white;
                damageText.text.fontSize = 8f;
                break;
            case DamageTextType.Critical:
                damageText.color = Color.yellow;
                damageText.text.fontSize = 12f;
                damageText.text.text += "!";
                break;
            case DamageTextType.Heal:
                damageText.color = Color.green;
                damageText.text.fontSize = 8f;
                break;
            case DamageTextType.CriticalHeal:
                damageText.color = Color.green;
                damageText.text.fontSize = 12f;
                damageText.text.text += "!";
                break;
            case DamageTextType.Dodge:
                damageText.color = Color.white;
                damageText.text.fontSize = 12f;
                damageText.text.text = "DODGE";
                break;
        }
    }

    public void DoHurtEffect()
    {
        hurtEffect.color = new Color(1, 1, 1, 0.15f);
    }

    public void UpdateHealth()
    {
        float health = (float)Player.instance.CurrentHP / (float)Player.instance.currentStats.MaxHP;
        float width = (int)(176 * health);
        hpTransform.sizeDelta = new Vector2(width, hpTransform.sizeDelta.y);
        hpText.text = $"{Player.instance.CurrentHP}/{Player.instance.currentStats.MaxHP}";
    }

    public void UpdateSpecial()
    {
        float special = (float)Player.instance.CurrentSP / (float)Player.instance.MaxSP;
        float width = (int)(135 * special);
        spTransform.sizeDelta = new Vector2(width, spTransform.sizeDelta.y);
    }

    public void UpdateExp()
    {
        levelText.text = Player.instance.Level.ToString();

        float exp = (float)Player.instance.CurrentExp / (float)Player.instance.MaxExp;
        float width = (int)(605f * exp);
        expTransform.sizeDelta = new Vector2(width, expTransform.sizeDelta.y);
    }

    public void UpdateStageTime()
    {
        MapTimeText.text = GetStageTime();
    }

    public string GetStageTime()
    {
        int totalSeconds = (int)Map.StageTime;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public string GetStageName()
    {
        return StageText.text;
    }

    public void HideUI()
    {
        canvasGroup.alpha = 0;
        beatIndicatorSpr.color = Color.clear;
    }

    public void ShowUI()
    {
        canvasGroup.alpha = 1;
        beatIndicatorSpr.color = Color.white;
    }
}
