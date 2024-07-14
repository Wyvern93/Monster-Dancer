using System;

[Serializable]
public class PlayerStats
{
    public float MaxHP, Def, Atk, Pen, CritChance, CritDmg, ExpRange, ExpMulti, Haste, Rerolls, MovRange;

    public PlayerStats Copy()
    {
        PlayerStats copy = new PlayerStats();
        copy.MaxHP = MaxHP;
        copy.Def = Def;
        copy.Atk = Atk;
        copy.Pen = Pen;
        copy.CritChance = CritChance;
        copy.CritDmg = CritDmg;
        copy.ExpRange = ExpRange;
        copy.ExpMulti = ExpMulti;
        copy.Haste = Haste;
        copy.Rerolls = Rerolls;
        copy.MovRange = MovRange;

        return copy;
    }

    public static PlayerStats operator +(PlayerStats a, PlayerStats b)
    {
        PlayerStats result = a.Copy();
        result.MaxHP += b.MaxHP;
        result.Def += b.Def;
        result.Atk += b.Atk;
        result.Pen += b.Pen;
        result.CritChance += b.CritChance;
        result.CritDmg += b.CritDmg;
        result.ExpRange += b.ExpRange;
        result.ExpMulti += b.ExpMulti;
        result.Haste += b.Haste;
        result.Rerolls += b.Rerolls;
        result.MovRange += b.MovRange;

        return result;
    }
    public static PlayerStats operator *(PlayerStats a, PlayerStats b)
    {
        PlayerStats result = a.Copy();
        result.MaxHP *= 1 + b.MaxHP;
        result.Def *= 1 + b.Def;
        result.Atk *= 1 + b.Atk;
        result.Pen *= 1 + b.Pen;
        result.CritChance *= 1 + b.CritChance;
        result.CritDmg *= 1 + b.CritDmg;
        result.ExpRange *= 1 + b.ExpRange;
        result.ExpMulti *= 1 + b.ExpMulti;
        result.Haste *= 1 + b.Haste;
        result.Rerolls *= 1 + b.Rerolls;
        result.MovRange *= 1 + b.MovRange;

        return result;
    }

    public static PlayerStats operator *(PlayerStats a, int b)
    {
        PlayerStats result = a.Copy();
        result.MaxHP *= b;
        result.Def *= b;
        result.Atk *= b;
        result.Pen *= b;
        result.CritChance *= b;
        result.CritDmg *= b;
        result.ExpRange *= b;
        result.ExpMulti *= b;
        result.Haste *= b;
        result.Rerolls *= b;
        result.MovRange *= b;

        return result;
    }
}