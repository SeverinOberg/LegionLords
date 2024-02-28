using UnityEngine;

public class UIStatsButton : MonoBehaviour
{

    public enum StatsType
    {
        Attack,
        Defense,
        Ability,
    }

    [field:SerializeField]
    public StatsType Type { get; private set; }

    public TooltipContent TooltipContent { get; private set; } = TooltipContent.Vertical;

    public string DataString { get; private set; }

    private Attack _attack;
    private Defense _defense;
    private Ability _ability;

    public void SetStatsString(Entity data)
    {
        switch (Type)
        {
            case StatsType.Attack:
                SetAttackStats(ref data);
                break;
            case StatsType.Defense:
                SetDefenseStats(ref data);
                break;
            case StatsType.Ability:
               SetAbilityStats(ref data);
                break;
        }
    }

    private void SetAttackStats(ref Entity data)
    {
        _attack = data.StatsAttack.Value;

        DataString =
            $"Element: {_attack.Element} \n" +
            $"Altitude: {_attack.Altitude} \n" +
            $"Damage: {_attack.Damage} \n" +
            $"Crit Chance: {_attack.CriticalChance} \n" +
            $"Crit Damage: {_attack.CriticalDamageMultiplier} \n" +
            $"Penetration: {_attack.Penetration} \n" +
            $"Speed: {_attack.Speed} \n" +
            $"Range: {_attack.Range} \n" +
            $"Splash: {_attack.IsSplash} \n" +
            $"Splash Radius:{_attack.SplashRadius}";
    }

    private void SetDefenseStats(ref Entity data)
    {
        _defense = data.StatsDefense.Value;

        DataString =
            $"Max Health: {_defense.MaxHealth} \n" +
            $"Armor: {_defense.Armor} \n" +
            $"Resistances: \n" +
            $"Fire: {_defense.Resistances.Fire} \n" +
            $"Frost: {_defense.Resistances.Frost} \n" +
            $"Lightning: {_defense.Resistances.Lightning} \n" +
            $"Poison: {_defense.Resistances.Poison} \n" +
            $"Nature: {_defense.Resistances.Nature} \n" +
            $"Holy: {_defense.Resistances.Holy} \n" +
            $"Shadow: {_defense.Resistances.Shadow} \n" +
            $"Arcane: {_defense.Resistances.Arcane}";
    }

    private void SetAbilityStats(ref Entity data)
    {
        _ability = data.StatsAbility.Value;

        DataString = 
            $"Element: {_ability.Element} \n" +
            $"Power: {_ability.Power} \n" +
            $"Crit Chance: {_ability.CriticalChance} \n" +
            $"Crit Damage: {_ability.CriticalDamage} \n" +
            $"Penetration: {_ability.Penetration} \n" +
            $"Speed: {_ability.Speed} \n" +
            $"Cooldown: {_ability.Cooldown} \n" +
            $"Cast Duration: {_ability.CastDuration} \n" +
            $"Duration: {_ability.Duration}";
    }

}
