using System.Collections.Generic;
using Data;

namespace TurnBased.Scripts
{
    public enum DamageModifier {Weak, Neutral, Strong};
    public enum FighterType { Normal, Poisin, Flying, Rock }

    public static class FightingRelationships
    {
        public static Dictionary<EAdventurerTypes, Dictionary<FighterType, DamageModifier>> Relationships => new Dictionary<EAdventurerTypes, Dictionary<FighterType, DamageModifier>>
        {
            {
                EAdventurerTypes.Brawler, new Dictionary<FighterType, DamageModifier>
                {
                    {FighterType.Rock, DamageModifier.Strong},
                    {FighterType.Flying, DamageModifier.Weak}
                }
            },
            {
                EAdventurerTypes.Swordsman, new Dictionary<FighterType, DamageModifier>
                {
                    {FighterType.Poisin, DamageModifier.Strong},
                    {FighterType.Rock, DamageModifier.Weak}
                }
            },
            {
                EAdventurerTypes.Mage, new Dictionary<FighterType, DamageModifier>
                {
                    {FighterType.Flying, DamageModifier.Strong},
                    {FighterType.Poisin, DamageModifier.Weak}
                }
            }
        };

        public static DamageModifier GetDamage(EAdventurerTypes adventurerTypes, FighterType fighterType)
        {
            var toReturn = DamageModifier.Neutral;
            if (Relationships.ContainsKey(adventurerTypes))
            {
                if (Relationships[adventurerTypes].ContainsKey(fighterType))
                {
                    toReturn = Relationships[adventurerTypes][fighterType];
                }
            }

            return toReturn;
        }
    }
}
