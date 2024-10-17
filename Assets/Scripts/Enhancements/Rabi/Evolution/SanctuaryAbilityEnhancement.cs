using JetBrains.Annotations;
using System;
using UnityEngine;

public class SanctuaryAbilityEnhancement : Enhancement
{
    public override string GetDescription()
    {
        return "A holy aura that protects the player, transforming projectiles inside into healing orbs and damaging enemies. Additionally executes enemies under 20% HP";
    }

    public override string getName()
    {
        return "Sanctuary";
    }

    public override string getId()
    {
        return "sanctuary";
    }

    public override string getDescriptionType()
    {
        return "Passive";
    }

    public override int getWeight()
    {
        return 4;
    }
    public override PlayerAbility getAbility()
    {
        return new SanctuaryAbilityEvolution();
    }
    public override EnhancementType GetEnhancementType()
    {
        return EnhancementType.EvolvedAbility;
    }

    public override Type getEvolutionItemType()
    {
        return typeof(BlessedFigureItem);
    }
}