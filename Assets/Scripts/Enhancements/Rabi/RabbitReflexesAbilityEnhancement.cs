using JetBrains.Annotations;
using UnityEngine;

public class RabbitReflexesAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        int level = getLevel() + 1;
        if (level == 1)
        {
            return "Grazing bullets makes them shine, when the bullet despawns, grants a charge of <color=\"yellow\">2%</color> attack damage up to 20 charges and lose <color=\"yellow\">50%</color> charges on hit";
        }
        if (level == 2)
        {
            return "Increases graze range by <color=\"yellow\">50%</color>";
        }
        if (level == 3)
        {
            return "Gain <color=\"yellow\">2->5%</color> attack damage per charge";
        }
        if (level == 4)
        {
            return "Grazing bullets also reduce bullets' lifetime by <color=\"yellow\">50%</color>";
        }
        return "";
    }

    public override string getId()
    {
        return "rabi.rabbitreflexes";
    }

    public override int getLevel()
    {
        if (!Player.instance.abilityValues.ContainsKey("ability.rabbitreflexes.level")) return 0;
        else return (int)Player.instance.abilityValues["ability.rabbitreflexes.level"];
    }

    public override string getName()
    {
        return "Rabbit Reflexes";
    }

    public override string getType()
    {
        return "Passive";
    }

    public override int getWeight()
    {
        return 2;
    }

    public override bool isAvailable()
    {
        bool available = true;
        if (Player.instance.equippedPassiveAbilities.Count == 3) available = false;
        if (Player.instance.equippedPassiveAbilities.Find(x => x.getID() == "rabi.rabbitreflexes") != null)
        {
            if (Player.instance.abilityValues["ability.rabbitreflexes.level"] < 4) available = true;
            else available = false;
        }


        return available;
    }

    public override bool isUnique()
    {
        return false;
    }

    public override Sprite getIcon()
    {
        return IconList.instance.rabbitReflexes;
    }

    public override void OnEquip()
    {
        if (isUnique()) GameManager.runData.RemoveSkillEnhancement(this);
        Player.instance.enhancements.Add(new RabbitReflexesAbilityEnhancement());
        if (!Player.instance.abilityValues.ContainsKey("ability.rabbitreflexes.level"))
        {
            Player.instance.abilityValues.Add("ability.rabbitreflexes.level", 1);
            Player.instance.equippedPassiveAbilities.Add(new RabbitReflexesAbility());
            UIManager.Instance.PlayerUI.SetPassiveIcon(IconList.instance.rabbitReflexes, 1, false, Player.instance.getPassiveAbilityIndex(typeof(RabbitReflexesAbility)));
            Player.instance.grazeSprite.color = new Color(1, 1, 1, 0.25f);
        }
        else
        {
            Player.instance.abilityValues["ability.rabbitreflexes.level"] += 1;
            int level = (int)Player.instance.abilityValues["ability.rabbitreflexes.level"];
            UIManager.Instance.PlayerUI.SetPassiveLevel(level, level >= 4, Player.instance.getPassiveAbilityIndex(typeof(RabbitReflexesAbility)));
            if (level == 2) Player.instance.grazeSprite.transform.localScale = Vector3.one * 1.5f;
        }

        Player.instance.CalculateStats();
    }

    public override void OnStatCalculate(ref PlayerStats flatBonus, ref PlayerStats percentBonus)
    {

    }
}