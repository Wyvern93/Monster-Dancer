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

    public Image normalCursor;
    public Image crosshair;
    RectTransform crosshair_transform;

    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] SpriteRenderer beatIndicatorSpr;

    [SerializeField] Image bossBar;
    private RectTransform bossBarTransform;
    [SerializeField] CanvasGroup bossBarGroup;
    [SerializeField] TextMeshProUGUI bossBarName;
    [SerializeField] TextMeshProUGUI bossBarHPText;
    public Image activeCDImage;
    public TextMeshProUGUI coinText;

    [SerializeField] Image ultimateIcon;

    [SerializeField] Image playerIcon;
    [SerializeField] TextMeshProUGUI playerName;

    // Ammo stuff
    [SerializeField] List<Image> bullets;
    public GameObject reloadUI;
    [SerializeField] AbilityIconUI abilityIcon1, abilityIcon2, abilityIcon3;
    [SerializeField] TextMeshProUGUI ammoNumber;

    [SerializeField] CombatCursorHandler combatcursor1, combatcursor2, combatcursor3;
    [SerializeField] TextMeshProUGUI cursorAmmoText;

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

        abilityIcon1.SetAbilityIcon(null, false);
        abilityIcon2.SetAbilityIcon(null, false);
        abilityIcon3.SetAbilityIcon(null, false);

        abilityIcon1.cooldown = 0;
        abilityIcon2.cooldown = 0;
        abilityIcon3.cooldown = 0;

        abilityIcon1.SetFrame(true);
        abilityIcon2.SetFrame(false);
        abilityIcon3.SetFrame(false);

        combatcursor1.SetVisibility(true);
        combatcursor2.SetVisibility(false);
        combatcursor3.SetVisibility(false);

        combatcursor1.SetCooldown(0, 0);

        combatcursor2.SetCooldown(1, 0);
        combatcursor2.SetCooldown(2, 0);

        combatcursor3.SetCooldown(0, 0);
        combatcursor3.SetCooldown(1, 0);
        combatcursor3.SetCooldown(2, 0);
        OnOpenMenu();
    }

    public void OnOpenMenu()
    {
        normalCursor.enabled = true;
        combatcursor1.SetVisibility(false);
        combatcursor2.SetVisibility(false);
        combatcursor3.SetVisibility(false);
        cursorAmmoText.enabled = false;
    }

    public void OnCloseMenu()
    {
        normalCursor.enabled = false;
        cursorAmmoText.enabled = true;
        UpdateAbilityUI();
    }

    public void OnReset()
    {
        abilityIcon1.SetAbilityIcon(null, false);
        abilityIcon2.SetAbilityIcon(null, false);
        abilityIcon3.SetAbilityIcon(null, false);

        abilityIcon1.cooldown = 0;
        abilityIcon2.cooldown = 0;
        abilityIcon3.cooldown = 0;

        abilityIcon1.SetFrame(true);
        abilityIcon2.SetFrame(false);
        abilityIcon3.SetFrame(false);

        combatcursor1.SetVisibility(true);
        combatcursor2.SetVisibility(false);
        combatcursor3.SetVisibility(false);

        combatcursor1.SetCooldown(0, 0);

        combatcursor2.SetCooldown(1, 0);
        combatcursor2.SetCooldown(2, 0);

        combatcursor3.SetCooldown(0, 0);
        combatcursor3.SetCooldown(1, 0);
        combatcursor3.SetCooldown(2, 0);
    }

    public void SetAmmo(int current, int max)
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            if (i < max) bullets[i].gameObject.SetActive(true);
            else bullets[i].gameObject.SetActive(false);

            if (i < current) bullets[i].color = Color.white;
            else bullets[i].color = Color.black;
        }
    }

    public void SetAmmoIcons(Sprite sprite)
    {
        foreach (Image spr in bullets)
        {
            spr.sprite = sprite;
        }
    }

    public float GetAbilityPercent(int id)
    {
        PlayerAbility ability = Player.instance.equippedPassiveAbilities[id];
        if (ability.currentAmmo > 0) return 0;

        float current = ability.currentCooldown;
        float max = ability.GetMaxCooldown();
        float percent = 0;
        if (current > 0) percent = (current / max);
        return percent;
    }

    public void SetPlayerCharacter(Sprite icon, string name)
    {
        playerIcon.sprite = icon;
        playerName.text = name;
        combatcursor1.SetVisibility(true);
        combatcursor2.SetVisibility(false);
        combatcursor3.SetVisibility(false);
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
        
        if (Stage.Instance != null) 
        {
            UpdateStageTime();
            bossBarGroup.alpha = Mathf.MoveTowards(bossBarGroup.alpha, Stage.isBossWave ? 1 : 0, Time.deltaTime);
        }
        if (Player.instance == null)
        {
            // Put here a normal cursor instead
            return;
        }
        UpdateAbilityUI();
    }

    void UpdateAbilityUI()
    {
        int currentAbility = Player.instance.currentWeapon;
        if (currentAbility == 0)
        {
            abilityIcon1.SetFrame(true);
            abilityIcon2.SetFrame(false);
            abilityIcon3.SetFrame(false);
        }
        if (currentAbility == 1)
        {
            abilityIcon1.SetFrame(false);
            abilityIcon2.SetFrame(true);
            abilityIcon3.SetFrame(false);
        }
        if (currentAbility == 2)
        {
            abilityIcon1.SetFrame(false);
            abilityIcon2.SetFrame(false);
            abilityIcon3.SetFrame(true);
        }

        if (Player.instance.equippedPassiveAbilities.Count == 1)
        {
            combatcursor1.SetVisibility(true);
            combatcursor2.SetVisibility(false);
            combatcursor3.SetVisibility(false);
            combatcursor1.SetCooldown(0, GetAbilityPercent(0));
        }
        else if (Player.instance.equippedPassiveAbilities.Count == 2)
        {
            combatcursor1.SetVisibility(false);
            combatcursor2.SetVisibility(true);
            combatcursor3.SetVisibility(false);
            combatcursor2.SetCooldown(0, GetAbilityPercent(0));
            combatcursor2.SetCooldown(1, GetAbilityPercent(1));
        }
        else if (Player.instance.equippedPassiveAbilities.Count == 3)
        {
            combatcursor1.SetVisibility(false);
            combatcursor2.SetVisibility(false);
            combatcursor3.SetVisibility(true);
            combatcursor3.SetCooldown(0, GetAbilityPercent(0));
            combatcursor3.SetCooldown(1, GetAbilityPercent(1));
            combatcursor3.SetCooldown(2, GetAbilityPercent(2));
        }

        if (Player.instance.equippedPassiveAbilities.Count > 0)
        {
            abilityIcon1.setCooldown(GetAbilityPercent(0));
        }
        if (Player.instance.equippedPassiveAbilities.Count > 1)
        {
            abilityIcon2.setCooldown(GetAbilityPercent(1));  
        }
        if (Player.instance.equippedPassiveAbilities.Count > 2)
        {
            abilityIcon3.setCooldown(GetAbilityPercent(2));
        }
        ammoNumber.text = $"{Player.instance.equippedPassiveAbilities[currentAbility].currentAmmo}/{Player.instance.equippedPassiveAbilities[currentAbility].GetMaxAmmo()}";
        cursorAmmoText.text = $"{Player.instance.equippedPassiveAbilities[currentAbility].currentAmmo}/{Player.instance.equippedPassiveAbilities[currentAbility].GetMaxAmmo()}";
    }

    public void SetStageText(string text)
    {
        StageText.text = text;
    }

    public void UpdateBossBar(int current, int max)
    {
        float health = (float)current / (float)max;
        float width = (int)(182f * health);
        bossBarTransform.sizeDelta = new Vector2(width, bossBarTransform.sizeDelta.y);
        bossBarHPText.text = $"{current}/{max}";
    }

    public void SetUltimateIcon(Sprite sprite)
    {
        ultimateIcon.sprite = sprite;
    }

    public void SetPassiveIcon(Sprite sprite, int id)
    {
        //abilityIcons[id].Display(sprite, level, maxed, false);
        if (id == 0) abilityIcon1.SetAbilityIcon(sprite, false);
        if (id == 1) abilityIcon2.SetAbilityIcon(sprite, false);
        if (id == 2) abilityIcon3.SetAbilityIcon(sprite, false);

        if (Player.instance.equippedPassiveAbilities.Count == 1)
        {
            combatcursor1.SetVisibility(true);
            combatcursor2.SetVisibility(false);
            combatcursor3.SetVisibility(false);
        }
        else if (Player.instance.equippedPassiveAbilities.Count == 2)
        {
            combatcursor1.SetVisibility(false);
            combatcursor2.SetVisibility(true);
            combatcursor3.SetVisibility(false);
        }
        else if (Player.instance.equippedPassiveAbilities.Count == 3)
        {
            combatcursor1.SetVisibility(false);
            combatcursor2.SetVisibility(false);
            combatcursor3.SetVisibility(true);
        }
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
        damageText.transform.localScale = Vector3.one * 1.5f;
        damageText.transform.position = position + (Random.insideUnitCircle * 0.4f);
        damageText.text.text = number.ToString();

        switch (textType)
        {
            default:
            case DamageTextType.Normal:
                damageText.color = Color.white;
                damageText.text.fontSize = 12f;
                break;
            case DamageTextType.PlayerDamage:
                damageText.color = Color.red;
                damageText.text.fontSize = 12f;
                break;
            case DamageTextType.Critical:
                damageText.color = Color.yellow;
                damageText.text.fontSize = 20f;
                damageText.text.text += "!";
                break;
            case DamageTextType.Heal:
                damageText.color = Color.green;
                damageText.text.fontSize = 12f;
                break;
            case DamageTextType.CriticalHeal:
                damageText.color = Color.green;
                damageText.text.fontSize = 20f;
                damageText.text.text += "!";
                break;
            case DamageTextType.Dodge:
                damageText.color = Color.white;
                damageText.text.fontSize = 20f;
                damageText.text.text = "DODGE";
                break;
        }
    }

    public void DoHurtEffect()
    {
        hurtEffect.color = new Color(1, 1, 1, 0.1f);
    }

    public void UpdateHealth()
    {
        float health = (float)Player.instance.CurrentHP / (float)Player.instance.currentStats.MaxHP;
        float width = (int)(87f * health);
        hpTransform.sizeDelta = new Vector2(width, hpTransform.sizeDelta.y);
        hpText.text = $"{Player.instance.CurrentHP}/{Player.instance.currentStats.MaxHP}";
    }

    public void UpdateSpecial(float current, float max)
    {
        float special = current / max;
        spBar.fillAmount = 1 - special;
        //spTransform.sizeDelta = new Vector2(width, spTransform.sizeDelta.y);
    }

    public void UpdateExp()
    {
        levelText.text = Player.instance.Level.ToString();

        float exp = (float)Player.instance.CurrentExp / (float)Player.instance.MaxExp;
        float width = (int)(106f * exp);
        expTransform.sizeDelta = new Vector2(width, expTransform.sizeDelta.y);
    }

    public void UpdateStageTime()
    {
        MapTimeText.text = Stage.Instance.showWaveTimer ? GetRemainingStageTime() : "";
    }

    public string GetStageTime()
    {
        int totalSeconds = (int)Stage.StageTime;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private string GetRemainingStageTime()
    {
        int totalSeconds = (int)Stage.remainingWaveTime;
        totalSeconds = Mathf.Clamp(totalSeconds, 0, 60);
        return totalSeconds.ToString();
        return string.Format("{000}", totalSeconds);
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
