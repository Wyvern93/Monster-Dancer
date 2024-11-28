using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class BlessedFigureItem : PlayerItem
{
    private int beat;
    public override bool CanCast()
    {
        return true;
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new BlessedFigureItemEnhancement() };
    }

    public override string getId()
    {
        return "blessedfigure";
    }

    public override string getItemDescription()
    {
        string description = $"<color=#FFFF88>Increases healing from abilities and allows ability heals to critical strike</color>\n\n";
        description += AddStat("Healing", 25, true, "%");
        description += AddStat("Crit Chance", 15, true, "%");

        return description;
    }

    public override string getItemName()
    {
        return "Blessed Figure";
    }

    public override int getRarity()
    {
        return 2;
    }

    public override void OnCast()
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {
        if (BeatManager.isGameBeat)
        {
            beat--;
        }
    }

    public override float OnPreHeal(float heal)
    {
        return heal * 1.25f;
    }

    public override void OnHit(PlayerAbility source, float damage, Enemy target)
    {
        
    }
}
