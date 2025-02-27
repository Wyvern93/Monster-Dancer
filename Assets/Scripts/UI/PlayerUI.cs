using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum CursorState { Menu, Crosshair }

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Image hpBar;
    private RectTransform hpTransform;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] Image hurtEffect;

    [SerializeField] Image hpBar_mini;
    private RectTransform hpBar_mini_Transform;

    [SerializeField] GameObject spHud;
    [SerializeField] Image spBar;
    private RectTransform spTransform;
    [SerializeField] Sprite Special_Normal, Special_Blink;
    private float spBlink;

    [SerializeField] Image expBar;
    private RectTransform expTransform;
    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] TextMeshProUGUI MapTimeText;
    [SerializeField] TextMeshProUGUI StageText;
    [SerializeField] GameObject damageText;
    [SerializeField] Transform damageTextParent;

    [SerializeField] Image normalCursor;
    public Image crosshair;
    RectTransform crosshair_transform;
    public CursorState cursorState;
    public GameObject menuCursorObj, crosshairCursorObj;

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
    [SerializeField] TextMeshProUGUI comboCounter;
    private RectTransform comboCounterTransform;
    public int combo;

    private bool bossBarActive;

    // Start is called before the first frame update
    void Awake()
    {
        comboCounter.text = "";
        bossBarActive = false;
        comboCounterTransform = comboCounter.GetComponent<RectTransform>();
        hpTransform = hpBar.GetComponent<RectTransform>();
        hpBar_mini_Transform = hpBar_mini.transform.parent.GetComponent<RectTransform>();
        hpBar_mini_Transform.gameObject.SetActive(false);
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

        combatcursor1.SetVisibility(false);
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

    public void AddToComboCounter(int num)
    {
        comboCounter.color = Color.white;
        combo += num;
        comboCounter.text = "x" + combo.ToString();
        comboCounterTransform.localScale = Vector3.one * ((num * 0.1f) + 1.1f);
    }

    public void FailCombo()
    {
        combo = 0;
        comboCounter.text = "X";
        comboCounter.color = Color.red;
        comboCounterTransform.localScale = Vector3.one * 2f;
    }

    public void OnOpenMenu()
    {
        SetCursorState(CursorState.Menu);
        combatcursor1.SetVisibility(false);
        combatcursor2.SetVisibility(false);
        combatcursor3.SetVisibility(false);
        cursorAmmoText.enabled = false;
    }

    public void OnCloseMenu()
    {
        if (!UIManager.Instance.cutsceneManager.isInCutscene()) SetCursorState(CursorState.Crosshair);
        UpdateAbilityUI();
    }

    public void SetCursorState(CursorState state)
    {
        cursorState = state;
        if (state == CursorState.Menu)
        {
            crosshairCursorObj.SetActive(false);
            menuCursorObj.SetActive(true);
        }
        else
        {
            crosshairCursorObj.SetActive(true);
            menuCursorObj.SetActive(false);
        }
    }

    public void OnReset()
    {
        abilityIcon1.SetAbilityIcon(null, false);
        abilityIcon2.SetAbilityIcon(null, false);
        abilityIcon3.SetAbilityIcon(null, false);

        abilityIcon1.cooldown = 0;
        abilityIcon2.cooldown = 0;
        abilityIcon3.cooldown = 0;

        combatcursor1.SetVisibility(false);
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
        combatcursor1.SetVisibility(false);
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

    public void ShowBossBar(bool value)
    {
        bossBarActive = value;
    }

    // Update is called once per frame
    void Update()
    {
        hurtEffect.color = Color.Lerp(hurtEffect.color, new Color(1, 1, 1, 0), Time.unscaledDeltaTime * 8f);
        if (InputManager.playerDevice == InputManager.InputDeviceType.Keyboard || Player.instance == null)
        {
            Vector2 offset = Vector2.zero;
            if (Player.instance) offset = Player.instance.direction.normalized;
            if (GameManager.isPaused || Player.instance == null || Stage.Instance == null) crosshair_transform.localPosition = Mouse.current.position.value / new Vector2(Screen.width, Screen.height) * new Vector2(640, 360) - new Vector2(320, 180);
            else
            {
                crosshair_transform.position = Player.instance.transform.position + (Vector3)offset * 2;
            }
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
            bossBarGroup.alpha = Mathf.MoveTowards(bossBarGroup.alpha, bossBarActive ? 1 : 0, Time.deltaTime);
        }
        if (Player.instance == null)
        {
            // Put here a normal cursor instead
            return;
        }
        hpBar_mini_Transform.position = Player.instance.transform.position - new Vector3(0, 0.2f, 0);
        UpdateAbilityUI();
    }

    void UpdateAbilityUI()
    {
        int currentAbility = Player.instance.currentWeapon;
        /*
        if (Player.instance.equippedPassiveAbilities.Count == 1)
        {
            combatcursor1.SetVisibility(false);
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
        }*/
        combatcursor1.SetVisibility(false);
        combatcursor2.SetVisibility(false);
        combatcursor3.SetVisibility(false);

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
        if (spTransform.sizeDelta.y >= 158)
        {
            if (spBlink < 1) spBlink += Time.deltaTime * 100;
            if (spBlink >= 1) spBlink = 0;

            if (spBlink < 0.45f) spBar.sprite = Special_Normal;
            else spBar.sprite = Special_Blink;
        }
        else
        {
            spBlink = 0;
            spBar.sprite = Special_Normal;
        }
        comboCounterTransform.transform.localScale = Vector3.Lerp(comboCounterTransform.transform.localScale, Vector3.one, Time.deltaTime * 3f);
        comboCounter.color = Color.Lerp(comboCounter.color, combo == 0 ? Color.clear : Color.white, Time.deltaTime);
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
        if (UIManager.Instance.cutsceneManager.isInCutscene() || health >= 1f) hpBar_mini_Transform.gameObject.SetActive(false);
        else hpBar_mini_Transform.gameObject.SetActive(true);
        hpBar_mini.fillAmount = health;
    }

    public void UpdateSpecial(float current, float max)
    {
        float special = current / max;
        spTransform.sizeDelta = new Vector2(16, 7 + ((1 - special) * 151f));
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
