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

    [SerializeField] Image spBar;
    private RectTransform spTransform;

    [SerializeField] Image expBar;
    private RectTransform expTransform;
    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] TextMeshProUGUI MapTimeText;
    [SerializeField] GameObject damageText;

    public Image crosshair;
    RectTransform crosshair_transform;
    // Start is called before the first frame update
    void Awake()
    {
        hpTransform = hpBar.GetComponent<RectTransform>();
        spTransform = spBar.GetComponent<RectTransform>();
        expTransform = expBar.GetComponent<RectTransform>();
        crosshair_transform = crosshair.GetComponent<RectTransform>();
        Cursor.visible = false;
    }

    private void Start()
    {
        PoolManager.CreatePool(typeof(DamageText), damageText, 100);
    }

    // Update is called once per frame
    void Update()
    {
        hurtEffect.color = Color.Lerp(hurtEffect.color, new Color(1, 1, 1, 0), Time.deltaTime * 8f);
        crosshair_transform.localPosition = Mouse.current.position.value / new Vector2(Screen.width, Screen.height) * new Vector2(640, 360) - new Vector2(320, 180);
        UpdateStageTime();
    }

    public void SpawnDamageText(Vector2 position, int number)
    {
        DamageText damageText = PoolManager.Get<DamageText>();
        damageText.transform.SetParent(transform, true);
        damageText.transform.localScale = Vector3.one;
        damageText.transform.position = position;
        damageText.text.text = number.ToString();
    }

    public void DoHurtEffect()
    {
        hurtEffect.color = new Color(1, 1, 1, 0.15f);
    }

    public void UpdateHealth()
    {
        float health = (float)Player.instance.CurrentHP / (float)Player.instance.MaxHP;
        float width = (int)(190f * health);
        hpTransform.sizeDelta = new Vector2(width, hpTransform.sizeDelta.y);
        hpText.text = $"{Player.instance.CurrentHP}/{Player.instance.MaxHP}";
    }

    public void UpdateSpecial()
    {
        float special = (float)Player.instance.CurrentSP / (float)Player.instance.MaxSP;
        float width = (int)(133 * special);
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
        int totalSeconds = (int)Map.StageTime;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        MapTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
