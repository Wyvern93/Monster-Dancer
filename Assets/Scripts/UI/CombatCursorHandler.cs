using UnityEngine;

public class CombatCursorHandler : MonoBehaviour
{
    [Header("Ability1")]
    [SerializeField] CombatCursorUI ability1;
    [SerializeField] CombatCursorUI ability2;
    [SerializeField] CombatCursorUI ability3;

    [SerializeField] CanvasGroup group;

    public void SetCooldown(int ability, float amount)
    {
        if (group.alpha < 1) return;
        if (ability == 0 && ability1 != null) ability1.SetBar(amount);
        if (ability == 1 && ability2 != null) ability2.SetBar(amount);
        if (ability == 2 && ability3 != null) ability3.SetBar(amount);
    }
    
    public void SetVisibility(bool visible)
    {
        group.alpha = visible ? 1 : 0;
    }
}