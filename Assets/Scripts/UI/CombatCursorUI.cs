using UnityEngine;
using UnityEngine.UI;

public class CombatCursorUI : MonoBehaviour
{
    [SerializeField] private Image bar;
    [SerializeField] Sprite loadSpr;
    [SerializeField] Sprite filledSpr1;
    [SerializeField] Sprite filledSpr2;

    private bool filled = true;
    private bool wasFilled = true;
    [SerializeField] int abilityID;

    public void SetBar(float amount)
    {
        bar.fillAmount = 1f - amount;
        if (amount <= 0) filled = true;
        else filled = false;

        if (wasFilled == false && filled)
        {
            if (abilityID != Player.instance.currentWeapon) AudioController.PlaySoundWithoutCooldown(AudioController.instance.sounds.abilityReadySfx);

            bar.sprite = filledSpr1;
            wasFilled = true;
        }
        
        if (wasFilled == true && filled == false)
        {
            bar.sprite = loadSpr;
            wasFilled = false;
        }
    }

    private void Update()
    {
        if (filled)
        {
            if (bar.sprite == filledSpr1) bar.sprite = filledSpr2;
            else bar.sprite = filledSpr1;
        }
    }
}