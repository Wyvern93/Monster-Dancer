using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class FireworksKitItem : PlayerItem
{
    public override bool CanCast()
    {
        return true;
    }

    public override List<Enhancement> getEnhancementList()
    {
        return new List<Enhancement>() { new FireworksKitItemEnhancement() };
    }

    public override string getId()
    {
        return "fireworkskit";
    }

    public override string getItemDescription()
    {
        return "";
    }

    public override string getItemName()
    {
        return "Fireworks Kit";
    }

    public override void OnCast()
    {
        throw new System.NotImplementedException();
    }

    public override void OnEquip()
    {
        
    }

    public override void OnUpdate()
    {
    }

    public override void OnHit(PlayerAbility source, float damage, Enemy target)
    {
        
    }
}
